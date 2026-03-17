using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restify.Auth.Application.Interfaces;
using Restify.Auth.Domain.Interfaces;
using Restify.Auth.Infrastructure.Persistence;
using Restify.Auth.Infrastructure.Persistence.Repositories;
using Restify.Auth.Infrastructure.Services;

namespace Restify.Auth.Infrastructure.Extensions;

/// <summary>
/// Extensiones para registrar servicios de Infrastructure
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Agrega los servicios de Infrastructure al contenedor de DI
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Registrar ICurrentUserService ANTES del DbContext
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Configurar DbContext con factory para inyectar ICurrentUserService
        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                    npgsqlOptions.EnableRetryOnFailure(3);
                });
        });

        // CustomerDbContext para autenticación de clientes (lee del schema backoffice)
        services.AddDbContext<CustomerDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(3));
        });

        // DriverDbContext para autenticación de motorizados (lee del schema backoffice)
        services.AddDbContext<DriverDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(3));
        });

        // Registrar UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Registrar Repository genérico
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Configurar JWT
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        // Registrar servicios internos
        services.AddScoped<JwtService>();
        services.AddScoped<PasswordService>();

        // Registrar servicios de aplicación
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IScreenPermissionService, ScreenPermissionService>();
        services.AddScoped<ICustomerAuthService, CustomerAuthService>();
        services.AddScoped<IDriverAuthService, DriverAuthService>();
        services.AddScoped<IDeliveryZoneService, DeliveryZoneService>();
        services.AddScoped<ITenantManagementService, TenantManagementService>();
        services.AddScoped<IPoolDriverAuthService, PoolDriverAuthService>();

        // Registrar servicios de registro y onboarding de tenants
        services.AddScoped<ITenantRegistrationService, TenantRegistrationService>();
        services.AddScoped<ITenantOnboardingService, TenantOnboardingService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        return services;
    }
}
