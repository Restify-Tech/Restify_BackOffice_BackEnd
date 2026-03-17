using FluentValidation;
using Restify.Auth.Application.DTOs.Auth;

namespace Restify.Auth.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El formato del email no es válido");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida");
    }
}
