using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Prometheus;
using Microsoft.IdentityModel.Tokens;
using Restify.Core.Application.Extensions;
using Restify.Core.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Restify.Core.Infrastructure.Persistence.Seeders;
using TakuSoft.Observability.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddCoreApplication();
builder.Services.AddCoreInfrastructure(builder.Configuration);

// TakuSoft Observability
builder.Services.AddTakuObservability(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

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
});

builder.Services.AddAuthorization();

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

// Swagger solo en Development/Staging
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Staging"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restify Core API v1");
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

// Health Check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "core", timestamp = DateTime.UtcNow }));

// Auto-migrate database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Restify.Core.Infrastructure.Persistence.CoreDbContext>();
    await db.Database.MigrateAsync();
}

// Seed data (el seeder verifica internamente si ya existe data)
await GeneralTableSeeder.SeedAsync(app.Services);

app.Run();
