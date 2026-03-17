using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Restify.Core.Application.Validators;

namespace Restify.Core.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateGridConfigurationRequestValidator>();
        return services;
    }
}
