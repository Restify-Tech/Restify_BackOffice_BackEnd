using FluentValidation;
using Restify.Auth.Application.DTOs.Tenant;
using Restify.Auth.Domain.Constants;

namespace Restify.Auth.Application.Validators;

public class UpdateTenantBrandingRequestValidator : AbstractValidator<UpdateTenantBrandingRequest>
{
    private const string HexColorPattern = @"^#([0-9A-Fa-f]{3}|[0-9A-Fa-f]{6})$";

    public UpdateTenantBrandingRequestValidator()
    {
        When(x => x.PrimaryColor != null, () =>
        {
            RuleFor(x => x.PrimaryColor)
                .Matches(HexColorPattern)
                .WithMessage("El color primario debe ser un valor hex valido (ej: #C8963E)");
        });

        When(x => x.SecondaryColor != null, () =>
        {
            RuleFor(x => x.SecondaryColor)
                .Matches(HexColorPattern)
                .WithMessage("El color secundario debe ser un valor hex valido (ej: #FF9800)");
        });

        When(x => x.AccentColor != null, () =>
        {
            RuleFor(x => x.AccentColor)
                .Matches(HexColorPattern)
                .WithMessage("El color de acento debe ser un valor hex valido (ej: #F59E0B)");
        });

        When(x => x.TemplateName != null, () =>
        {
            RuleFor(x => x.TemplateName)
                .Must(t => TemplateNames.IsValid(t!))
                .WithMessage($"El template debe ser uno de: {string.Join(", ", TemplateNames.All)}");
        });

        When(x => x.FontHeading != null, () =>
        {
            RuleFor(x => x.FontHeading)
                .MaximumLength(100)
                .WithMessage("La fuente de encabezados no puede exceder 100 caracteres");
        });

        When(x => x.FontBody != null, () =>
        {
            RuleFor(x => x.FontBody)
                .MaximumLength(100)
                .WithMessage("La fuente del cuerpo no puede exceder 100 caracteres");
        });

        When(x => x.CustomCss != null, () =>
        {
            RuleFor(x => x.CustomCss)
                .MaximumLength(5000)
                .WithMessage("El CSS personalizado no puede exceder 5000 caracteres");
        });
    }
}
