using FluentValidation;
using Restify.Auth.Application.DTOs.CustomerAuth;

namespace Restify.Auth.Application.Validators;

public class CustomerRegisterRequestValidator : AbstractValidator<CustomerRegisterRequest>
{
    public CustomerRegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es requerido")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El formato del email no es válido");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres");

        RuleFor(x => x.TenantSlug)
            .NotEmpty().WithMessage("El restaurante es requerido");
    }
}

public class CustomerLoginRequestValidator : AbstractValidator<CustomerLoginRequest>
{
    public CustomerLoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El formato del email no es válido");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida");

        RuleFor(x => x.TenantSlug)
            .NotEmpty().WithMessage("El restaurante es requerido");
    }
}
