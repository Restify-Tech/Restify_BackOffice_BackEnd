using FluentValidation;
using Restify.BackOffice.Application.DTOs;

namespace Restify.BackOffice.Application.Validators;

public class CreatePurchaseOrderRequestValidator : AbstractValidator<CreatePurchaseOrderRequest>
{
    public CreatePurchaseOrderRequestValidator()
    {
        RuleFor(x => x.SupplierId)
            .NotEmpty().WithMessage("El proveedor es requerido");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("La orden debe tener al menos un item");

        RuleForEach(x => x.Items)
            .SetValidator(new CreatePurchaseOrderItemRequestValidator());

        RuleFor(x => x.Discount)
            .GreaterThanOrEqualTo(0).WithMessage("El descuento no puede ser negativo");
    }
}

public class CreatePurchaseOrderItemRequestValidator : AbstractValidator<CreatePurchaseOrderItemRequest>
{
    public CreatePurchaseOrderItemRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("El producto es requerido");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0");

        RuleFor(x => x.UnitCost)
            .GreaterThan(0).WithMessage("El costo unitario debe ser mayor a 0");

        RuleFor(x => x.Unit)
            .NotEmpty().WithMessage("La unidad es requerida")
            .MaximumLength(20).WithMessage("La unidad no puede exceder 20 caracteres");
    }
}

public class UpdatePurchaseOrderRequestValidator : AbstractValidator<UpdatePurchaseOrderRequest>
{
    public UpdatePurchaseOrderRequestValidator()
    {
        RuleFor(x => x.Discount)
            .GreaterThanOrEqualTo(0).WithMessage("El descuento no puede ser negativo");

        RuleForEach(x => x.Items)
            .SetValidator(new CreatePurchaseOrderItemRequestValidator());
    }
}

public class ReceivePurchaseOrderRequestValidator : AbstractValidator<ReceivePurchaseOrderRequest>
{
    public ReceivePurchaseOrderRequestValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Debe indicar al menos un item recibido");

        RuleForEach(x => x.Items)
            .SetValidator(new ReceiveItemRequestValidator());
    }
}

public class ReceiveItemRequestValidator : AbstractValidator<ReceiveItemRequest>
{
    public ReceiveItemRequestValidator()
    {
        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("El item es requerido");

        RuleFor(x => x.QuantityReceived)
            .GreaterThan(0).WithMessage("La cantidad recibida debe ser mayor a 0");
    }
}
