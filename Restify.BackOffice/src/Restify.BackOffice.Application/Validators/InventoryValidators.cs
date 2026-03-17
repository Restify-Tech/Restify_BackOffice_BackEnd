using FluentValidation;
using Restify.BackOffice.Application.DTOs;

namespace Restify.BackOffice.Application.Validators;

public class UpsertInventoryItemRequestValidator : AbstractValidator<UpsertInventoryItemRequest>
{
    public UpsertInventoryItemRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("El producto es requerido");

        RuleFor(x => x.CurrentStock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo");

        RuleFor(x => x.Unit)
            .NotEmpty().WithMessage("La unidad es requerida")
            .MaximumLength(20).WithMessage("La unidad no puede exceder 20 caracteres");

        RuleFor(x => x.MinimumStock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock mínimo no puede ser negativo");

        RuleFor(x => x.MaximumStock)
            .GreaterThan(x => x.MinimumStock).WithMessage("El stock máximo debe ser mayor al mínimo")
            .When(x => x.MaximumStock.HasValue);

        RuleFor(x => x.AverageCost)
            .GreaterThanOrEqualTo(0).WithMessage("El costo promedio no puede ser negativo");

        RuleFor(x => x.CostMethod)
            .IsInEnum().WithMessage("El método de costeo no es válido");
    }
}

public class AdjustInventoryRequestValidator : AbstractValidator<AdjustInventoryRequest>
{
    public AdjustInventoryRequestValidator()
    {
        RuleFor(x => x.InventoryItemId)
            .NotEmpty().WithMessage("El item de inventario es requerido");

        RuleFor(x => x.Quantity)
            .NotEqual(0).WithMessage("La cantidad no puede ser 0");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("El motivo es requerido")
            .MaximumLength(200).WithMessage("El motivo no puede exceder 200 caracteres");

        RuleFor(x => x.UnitCost)
            .GreaterThan(0).WithMessage("El costo unitario debe ser mayor a 0")
            .When(x => x.UnitCost.HasValue);
    }
}
