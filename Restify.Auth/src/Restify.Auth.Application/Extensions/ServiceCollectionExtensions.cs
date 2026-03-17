using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Restify.Auth.Application.Extensions;

/// <summary>
/// Extensiones para registrar servicios de Application
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Agrega los servicios de Application al contenedor de DI
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registrar validadores de FluentValidation
        services.AddValidatorsFromAssemblyContaining<Validators.LoginRequestValidator>();

        return services;
    }
}
