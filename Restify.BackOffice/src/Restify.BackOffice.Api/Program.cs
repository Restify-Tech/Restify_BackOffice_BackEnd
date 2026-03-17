using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Restify.BackOffice.Api.Hubs;
using Restify.BackOffice.Api.Services;
using Restify.BackOffice.Application.Extensions;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Extensions;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.Core.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddBackOfficeApplication();
builder.Services.AddBackOfficeInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

// Agregar seeder
builder.Services.AddScoped<BackOfficeDbSeeder>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

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

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Registrar entidades para DataProvider genérico
var entityRegistry = app.Services.GetRequiredService<IEntityRegistry>();
entityRegistry.Register<Category, BackOfficeDbContext>("Category");
// Agregar más entidades aquí según se vayan creando:
// entityRegistry.Register<Product, BackOfficeDbContext>("Product");
// entityRegistry.Register<Table, BackOfficeDbContext>("Table");

// Ejecutar seeder en desarrollo
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<BackOfficeDbSeeder>();
    await seeder.SeedAsync();
}

// Configure pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restify BackOffice API v1");
    c.RoutePrefix = string.Empty;
});

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<OrderHub>("/hubs/orders");
app.MapHub<DeliveryTrackingHub>("/hubs/delivery-tracking");

// Health Check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "backoffice", timestamp = DateTime.UtcNow }));

app.Run();
