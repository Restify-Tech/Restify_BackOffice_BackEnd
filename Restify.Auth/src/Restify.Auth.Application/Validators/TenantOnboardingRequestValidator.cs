using FluentValidation;
using Restify.Auth.Application.DTOs.Tenant;

namespace Restify.Auth.Application.Validators;

public class TenantOnboardingRequestValidator : AbstractValidator<TenantOnboardingRequest>
{
    public TenantOnboardingRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres")
            .When(x => x.Name != null);

        RuleFor(x => x.FullAddress)
            .MaximumLength(500).WithMessage("La dirección no puede exceder 500 caracteres")
            .When(x => x.FullAddress != null);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("La latitud debe estar entre -90 y 90")
            .When(x => x.Latitude.HasValue);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("La longitud debe estar entre -180 y 180")
            .When(x => x.Longitude.HasValue);

        RuleFor(x => x.Currency)
            .MaximumLength(3).WithMessage("La moneda debe ser un código ISO 4217 de 3 caracteres")
            .MinimumLength(3).WithMessage("La moneda debe ser un código ISO 4217 de 3 caracteres")
            .When(x => x.Currency != null);

        RuleFor(x => x.TaxPercentage)
            .InclusiveBetween(0, 100).WithMessage("El porcentaje de impuesto debe estar entre 0 y 100")
            .When(x => x.TaxPercentage.HasValue);

        RuleFor(x => x.TimeZone)
            .MaximumLength(50).WithMessage("La zona horaria no puede exceder 50 caracteres")
            .When(x => x.TimeZone != null);
    }
}
