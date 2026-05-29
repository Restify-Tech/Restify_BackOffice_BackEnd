using FluentValidation;
using Restify.Auth.Application.DTOs.Auth;

namespace Restify.Auth.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.Email) || !string.IsNullOrEmpty(x.Username))
            .WithMessage("Debe proporcionar un email o nombre de usuario");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("El formato del email no es válido")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Username)
            .MaximumLength(50).WithMessage("El nombre de usuario no puede exceder 50 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Username));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida");
    }
}
