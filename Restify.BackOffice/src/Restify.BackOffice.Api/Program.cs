using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Prometheus;
using Microsoft.IdentityModel.Tokens;
using Restify.BackOffice.Api.Hubs;
using Restify.BackOffice.Api.Services;
using Restify.BackOffice.Application.Extensions;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Extensions;
using Restify.BackOffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Restify.Core.Application.Interfaces;
using TakuSoft.Observability.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddBackOfficeApplication();
builder.Services.AddBackOfficeInfrastructure(builder.Configuration);

// TakuSoft Observability
builder.Services.AddTakuObservability(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

// Agregar seeder
builder.Services.AddScoped<BackOfficeDbSeeder>();

// Validar JWT SecretKey al iniciar
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
    throw new InvalidOperationException("JwtSettings:SecretKey debe tener al menos 32 caracteres. Proveer via variable de entorno JwtSettings__SecretKey");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };

    // Allow JWT token via query string for SignalR WebSocket connections
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// SignalR
builder.Services.AddSignalR();
builder.Services.AddScoped<IOrderNotificationService, OrderNotificationService>();
builder.Services.AddScoped<INotificationsService, NotificationsService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar CORS con whitelist desde configuracion
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(
            builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? ["http://localhost:3000"])
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()));

var app = builder.Build();

// Registrar entidades para DataProvider genérico
var entityRegistry = app.Services.GetRequiredService<IEntityRegistry>();
entityRegistry.Register<Category, BackOfficeDbContext>("Category");
// Agregar más entidades aquí según se vayan creando:
// entityRegistry.Register<Product, BackOfficeDbContext>("Product");
// entityRegistry.Register<Table, BackOfficeDbContext>("Table");

// Auto-migrate database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BackOfficeDbContext>();
    await db.Database.MigrateAsync();
}

// Ejecutar seeder en desarrollo
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<BackOfficeDbSeeder>();
    await seeder.SeedAsync();
}

// Swagger solo en Development/Staging
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Staging"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restify BackOffice API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseCorrelationId();
app.UseGlobalExceptionHandler();
app.UseAuditMiddleware();
app.UseCors();
app.UseHttpMetrics();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapMetrics();
app.MapHub<OrderHub>("/hubs/orders");
app.MapHub<DeliveryTrackingHub>("/hubs/delivery-tracking");
app.MapHub<NotificationsHub>("/hubs/notifications");

// Health Check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "backoffice", timestamp = DateTime.UtcNow }));

app.Run();
