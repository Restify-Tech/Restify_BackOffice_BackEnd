using FluentValidation;
using Restify.BackOffice.Application.DTOs;

namespace Restify.BackOffice.Application.Validators;

public class CreateCreditNoteRequestValidator : AbstractValidator<CreateCreditNoteRequest>
{
    public CreateCreditNoteRequestValidator()
    {
        RuleFor(x => x.InvoiceId)
            .NotEmpty().WithMessage("La factura es requerida");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("El motivo es requerido")
            .MaximumLength(300).WithMessage("El motivo no puede exceder 300 caracteres");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Debe incluir al menos un item");

        RuleForEach(x => x.Items).SetValidator(new CreditNoteItemRequestValidator());
    }
}

public class CreditNoteItemRequestValidator : AbstractValidator<CreditNoteItemRequest>
{
    public CreditNoteItemRequestValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("El nombre del producto es requerido")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("El precio unitario no puede ser negativo");

        RuleFor(x => x.TaxRate)
            .InclusiveBetween(0, 1).WithMessage("La tasa de impuesto debe estar entre 0 y 1");
    }
}
