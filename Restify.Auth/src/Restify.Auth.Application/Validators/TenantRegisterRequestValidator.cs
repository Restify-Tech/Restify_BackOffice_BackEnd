using FluentValidation;
using Restify.Auth.Application.DTOs.Tenant;
using Restify.Auth.Domain.Enums;

namespace Restify.Auth.Application.Validators;

public class TenantRegisterRequestValidator : AbstractValidator<TenantRegisterRequest>
{
    public TenantRegisterRequestValidator()
    {
        RuleFor(x => x.IdentificationType)
            .IsInEnum().WithMessage("El tipo de identificación no es válido");

        RuleFor(x => x.IdentificationNumber)
            .NotEmpty().WithMessage("El número de identificación es requerido")
            .Must((req, number) => BeValidIdentification(req.IdentificationType, number))
            .WithMessage("El número de identificación no es válido para el tipo seleccionado");

        RuleFor(x => x.BusinessName)
            .NotEmpty().WithMessage("La razón social es requerida")
            .MaximumLength(200).WithMessage("La razón social no puede exceder 200 caracteres");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("El slug es requerido")
            .MinimumLength(3).WithMessage("El slug debe tener al menos 3 caracteres")
            .MaximumLength(50).WithMessage("El slug no puede exceder 50 caracteres")
            .Matches(@"^[a-z0-9]+(-[a-z0-9]+)*$").WithMessage("El slug solo puede contener letras minúsculas, números y guiones");

        RuleFor(x => x.AdminEmail)
            .NotEmpty().WithMessage("El email del administrador es requerido")
            .EmailAddress().WithMessage("El formato del email no es válido");

        RuleFor(x => x.AdminPassword)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres")
            .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula")
            .Matches(@"[a-z]").WithMessage("La contraseña debe contener al menos una letra minúscula")
            .Matches(@"[0-9]").WithMessage("La contraseña debe contener al menos un número")
            .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("La contraseña debe contener al menos un carácter especial");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("El teléfono es requerido")
            .MaximumLength(20).WithMessage("El teléfono no puede exceder 20 caracteres");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("La ciudad es requerida")
            .MaximumLength(100).WithMessage("La ciudad no puede exceder 100 caracteres");
    }

    private static bool BeValidIdentification(IdentificationType type, string? number)
    {
        if (string.IsNullOrWhiteSpace(number))
            return false;

        // Solo dígitos
        if (!number.All(char.IsDigit))
            return false;

        return type switch
        {
            IdentificationType.Cedula => number.Length == 10,
            IdentificationType.Ruc => number.Length == 13,
            IdentificationType.Passport => number.Length >= 5 && number.Length <= 20,
            _ => false
        };
    }
}
