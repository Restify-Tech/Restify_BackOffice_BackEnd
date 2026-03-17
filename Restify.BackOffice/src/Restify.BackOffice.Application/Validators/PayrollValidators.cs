using FluentValidation;
using Restify.BackOffice.Application.DTOs;

namespace Restify.BackOffice.Application.Validators;

public class CreatePayrollPeriodRequestValidator : AbstractValidator<CreatePayrollPeriodRequest>
{
    public CreatePayrollPeriodRequestValidator()
    {
        RuleFor(x => x.Year)
            .InclusiveBetween(2020, 2099).WithMessage("El año debe estar entre 2020 y 2099");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12).WithMessage("El mes debe estar entre 1 y 12");

        RuleFor(x => x.PeriodType)
            .IsInEnum().WithMessage("El tipo de periodo no es válido");

        RuleFor(x => x.PaymentDate)
            .NotEmpty().WithMessage("La fecha de pago es requerida");
    }
}

public class CreateDeductionTypeRequestValidator : AbstractValidator<CreateDeductionTypeRequest>
{
    public CreateDeductionTypeRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.CalculationType)
            .IsInEnum().WithMessage("El tipo de cálculo no es válido");

        RuleFor(x => x.DefaultValue)
            .GreaterThanOrEqualTo(0).WithMessage("El valor por defecto no puede ser negativo");

        RuleFor(x => x.AppliesTo)
            .IsInEnum().WithMessage("El campo 'aplica a' no es válido");
    }
}

public class UpdateDeductionTypeRequestValidator : AbstractValidator<UpdateDeductionTypeRequest>
{
    public UpdateDeductionTypeRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.CalculationType)
            .IsInEnum().WithMessage("El tipo de cálculo no es válido");

        RuleFor(x => x.DefaultValue)
            .GreaterThanOrEqualTo(0).WithMessage("El valor por defecto no puede ser negativo");

        RuleFor(x => x.AppliesTo)
            .IsInEnum().WithMessage("El campo 'aplica a' no es válido");
    }
}
