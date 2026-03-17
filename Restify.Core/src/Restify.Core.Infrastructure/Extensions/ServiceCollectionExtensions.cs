using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restify.Core.Application.Interfaces;
using Restify.Core.Infrastructure.Persistence;
using Restify.Core.Infrastructure.Persistence.Repositories;
using Restify.Core.Infrastructure.Services;

namespace Restify.Core.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<CoreDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(CoreDbContext).Assembly.FullName)
            ));

        // Repositories - Grid
        services.AddScoped<IGridConfigurationRepository, GridConfigurationRepository>();
        services.AddScoped<IGridColumnRepository, GridColumnRepository>();
        services.AddScoped<IGridColumnValidationRepository, GridColumnValidationRepository>();
        services.AddScoped<IGridColumnLookupRepository, GridColumnLookupRepository>();

        // Repositories - General Tables/Values
        services.AddScoped<IGeneralTableRepository, GeneralTableRepository>();
        services.AddScoped<IGeneralValueRepository, GeneralValueRepository>();

        // Services - Grid
        services.AddScoped<IGridConfigurationService, GridConfigurationService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Services - General Tables/Values
        services.AddScoped<IGeneralTableService, GeneralTableService>();
        services.AddScoped<IGeneralValueService, GeneralValueService>();

        // DataProvider - Registro de entidades (Singleton para mantener estado)
        services.AddSingleton<IEntityRegistry, EntityRegistry>();

        // DataProvider Service
        services.AddScoped<IDataProviderService, DataProviderService>();

        return services;
    }
}
