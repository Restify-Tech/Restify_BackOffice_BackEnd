using System.Text;
using System.Threading.RateLimiting;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Restify.Auth.Application.Extensions;
using Restify.Auth.Infrastructure.Extensions;
using Restify.Auth.Infrastructure.Persistence;
using Restify.Auth.Infrastructure.Services;
using Prometheus;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TakuSoft.Observability.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Agregar servicios al contenedor
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();

// Agregar HttpContextAccessor para CurrentUserService
builder.Services.AddHttpContextAccessor();

// Agregar capas de la aplicación
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// TakuSoft Observability
builder.Services.AddTakuObservability(builder.Configuration);

// Agregar DbSeeder
builder.Services.AddScoped<DbSeeder>();

// Rate Limiting — proteccion contra fuerza bruta en endpoints de autenticacion
builder.Services.AddRateLimiter(options =>
{
    // Politica estricta para login: max 5 intentos por IP en 15 minutos
    options.AddFixedWindowLimiter("auth-login", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(15);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });

    // Politica para refresh token: max 20 refreshes en ventana deslizante de 15 minutos
    options.AddSlidingWindowLimiter("auth-refresh", limiterOptions =>
    {
        limiterOptions.Window = TimeSpan.FromMinutes(15);
        limiterOptions.SegmentsPerWindow = 3;
        limiterOptions.PermitLimit = 20;
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    // Politica general para otros endpoints: 60 peticiones por minuto por IP
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 60,
                QueueLimit = 5,
                Window = TimeSpan.FromMinutes(1)
            }));

    // Respuesta JSON para 429 con cabecera Retry-After
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";

        var retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retry)
            ? (int)retry.TotalSeconds
            : 60;

        context.HttpContext.Response.Headers.RetryAfter = retryAfter.ToString();
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            error = "Demasiadas solicitudes. Intente nuevamente en unos minutos.",
            retryAfterSeconds = retryAfter
        }, cancellationToken);
    };
});

// Validar JWT SecretKey al iniciar
var jwtSecret = builder.Configuration["JwtSettings:SecretKey"];
if (string.IsNullOrEmpty(jwtSecret) || jwtSecret.Length < 32)
    throw new InvalidOperationException("JwtSettings:SecretKey debe tener al menos 32 caracteres. Proveer via variable de entorno JwtSettings__SecretKey");

// Configurar JWT Authentication
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;

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
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Configurar CORS con whitelist desde configuracion
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(
            builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? ["http://localhost:3000"])
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()));

// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Auto-migrate database
using (var scope = app.Services.CreateScope())
{
    var appDb = scope.ServiceProvider.GetRequiredService<Restify.Auth.Infrastructure.Persistence.AppDbContext>();
    await appDb.Database.MigrateAsync();
}

// Ejecutar seeder en desarrollo
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    await seeder.SeedAsync();
}

// Swagger solo en Development/Staging
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Staging"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restify Auth API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseCorrelationId();
app.UseGlobalExceptionHandler();
app.UseAuditMiddleware();
app.UseSerilogRequestLogging();

app.UseCors();
app.UseRateLimiter();

// Prometheus metrics
app.UseHttpMetrics();

// Servir archivos estáticos (uploads de logos, firmas, etc.)
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapMetrics();

// Health Check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "auth", timestamp = DateTime.UtcNow }));

app.Run();
