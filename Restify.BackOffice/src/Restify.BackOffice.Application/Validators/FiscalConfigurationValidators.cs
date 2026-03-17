using FluentValidation;
using Restify.BackOffice.Application.DTOs;

namespace Restify.BackOffice.Application.Validators;

public class SaveFiscalConfigurationRequestValidator : AbstractValidator<SaveFiscalConfigurationRequest>
{
    public SaveFiscalConfigurationRequestValidator()
    {
        RuleFor(x => x.Ruc)
            .NotEmpty().WithMessage("El RUC es requerido")
            .Length(13).WithMessage("El RUC debe tener 13 dígitos")
            .Matches(@"^\d{13}$").WithMessage("El RUC solo debe contener dígitos");

        RuleFor(x => x.BusinessName)
            .NotEmpty().WithMessage("La razón social es requerida")
            .MaximumLength(300).WithMessage("La razón social no puede exceder 300 caracteres");

        RuleFor(x => x.TradeName)
            .MaximumLength(300).WithMessage("El nombre comercial no puede exceder 300 caracteres")
            .When(x => x.TradeName != null);

        RuleFor(x => x.MainAddress)
            .NotEmpty().WithMessage("La dirección matriz es requerida")
            .MaximumLength(300).WithMessage("La dirección no puede exceder 300 caracteres");

        RuleFor(x => x.EstablishmentAddress)
            .NotEmpty().WithMessage("La dirección del establecimiento es requerida")
            .MaximumLength(300).WithMessage("La dirección no puede exceder 300 caracteres");

        RuleFor(x => x.Establishment)
            .NotEmpty().WithMessage("El establecimiento es requerido")
            .Length(3).WithMessage("El establecimiento debe tener 3 dígitos")
            .Matches(@"^\d{3}$").WithMessage("El establecimiento solo debe contener dígitos");

        RuleFor(x => x.EmissionPoint)
            .NotEmpty().WithMessage("El punto de emisión es requerido")
            .Length(3).WithMessage("El punto de emisión debe tener 3 dígitos")
            .Matches(@"^\d{3}$").WithMessage("El punto de emisión solo debe contener dígitos");

        RuleFor(x => x.Environment)
            .IsInEnum().WithMessage("El ambiente SRI no es válido");
    }
}
