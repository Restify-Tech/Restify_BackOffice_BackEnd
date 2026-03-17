using FluentValidation;
using Restify.BackOffice.Application.DTOs;

namespace Restify.BackOffice.Application.Validators;

public class CreateWithholdingRequestValidator : AbstractValidator<CreateWithholdingRequest>
{
    public CreateWithholdingRequestValidator()
    {
        RuleFor(x => x.SupplierName)
            .NotEmpty().WithMessage("El nombre del proveedor es requerido")
            .MaximumLength(300).WithMessage("El nombre no puede exceder 300 caracteres");

        RuleFor(x => x.SupplierRuc)
            .NotEmpty().WithMessage("El RUC del proveedor es requerido")
            .Length(13).WithMessage("El RUC debe tener 13 dígitos")
            .Matches(@"^\d{13}$").WithMessage("El RUC solo debe contener dígitos");

        RuleFor(x => x.SupplierIdType)
            .IsInEnum().WithMessage("El tipo de identificación no es válido");

        RuleFor(x => x.SupportDocType)
            .NotEmpty().WithMessage("El tipo de documento sustento es requerido")
            .MaximumLength(2).WithMessage("El tipo no puede exceder 2 caracteres");

        RuleFor(x => x.SupportDocNumber)
            .NotEmpty().WithMessage("El número del documento sustento es requerido")
            .MaximumLength(49).WithMessage("El número no puede exceder 49 caracteres");

        RuleFor(x => x.SupportDocDate)
            .NotEmpty().WithMessage("La fecha del documento sustento es requerida");

        RuleFor(x => x.Details)
            .NotEmpty().WithMessage("Debe incluir al menos un detalle de retención");

        RuleForEach(x => x.Details).SetValidator(new WithholdingDetailRequestValidator());
    }
}

public class WithholdingDetailRequestValidator : AbstractValidator<WithholdingDetailRequest>
{
    public WithholdingDetailRequestValidator()
    {
        RuleFor(x => x.TaxCode)
            .NotEmpty().WithMessage("El código del impuesto es requerido");

        RuleFor(x => x.RetentionCode)
            .NotEmpty().WithMessage("El código de retención es requerido");

        RuleFor(x => x.TaxBase)
            .GreaterThan(0).WithMessage("La base imponible debe ser mayor a 0");

        RuleFor(x => x.RetentionPercentage)
            .InclusiveBetween(0, 100).WithMessage("El porcentaje de retención debe estar entre 0 y 100");
    }
}
