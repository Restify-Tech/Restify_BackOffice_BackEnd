using FluentValidation;
using Restify.BackOffice.Application.DTOs;

namespace Restify.BackOffice.Application.Validators;

public class CreateCashRegisterRequestValidator : AbstractValidator<CreateCashRegisterRequest>
{
    public CreateCashRegisterRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");
    }
}

public class UpdateCashRegisterRequestValidator : AbstractValidator<UpdateCashRegisterRequest>
{
    public UpdateCashRegisterRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");
    }
}

public class OpenCashRegisterRequestValidator : AbstractValidator<OpenCashRegisterRequest>
{
    public OpenCashRegisterRequestValidator()
    {
        RuleFor(x => x.CashRegisterId)
            .NotEmpty().WithMessage("La caja es requerida");

        RuleFor(x => x.OpeningBalance)
            .GreaterThanOrEqualTo(0).WithMessage("El saldo inicial no puede ser negativo");
    }
}

public class CloseCashRegisterRequestValidator : AbstractValidator<CloseCashRegisterRequest>
{
    public CloseCashRegisterRequestValidator()
    {
        RuleFor(x => x.ActualClosingBalance)
            .GreaterThanOrEqualTo(0).WithMessage("El saldo de cierre no puede ser negativo");
    }
}

public class RegisterCashMovementRequestValidator : AbstractValidator<RegisterCashMovementRequest>
{
    public RegisterCashMovementRequestValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("El tipo de movimiento no es válido");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("El monto debe ser mayor a 0");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es requerida")
            .MaximumLength(200).WithMessage("La descripción no puede exceder 200 caracteres");
    }
}
