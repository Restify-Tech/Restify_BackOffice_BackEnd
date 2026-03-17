using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Restify.BackOffice.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackOfficeApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<Validators.CreateCategoryRequestValidator>();

        return services;
    }
}
