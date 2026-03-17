using FluentValidation;
using Restify.BackOffice.Application.DTOs;

namespace Restify.BackOffice.Application.Validators;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("El tipo de orden no es válido");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("La orden debe tener al menos un item");

        RuleForEach(x => x.Items)
            .SetValidator(new CreateOrderItemRequestValidator());

        RuleFor(x => x.Discount)
            .GreaterThanOrEqualTo(0).WithMessage("El descuento no puede ser negativo");

        RuleFor(x => x.CustomerName)
            .MaximumLength(200).WithMessage("El nombre del cliente no puede exceder 200 caracteres")
            .When(x => x.CustomerName != null);

        RuleFor(x => x.CustomerPhone)
            .MaximumLength(20).WithMessage("El teléfono no puede exceder 20 caracteres")
            .When(x => x.CustomerPhone != null);
    }
}

public class CreateOrderItemRequestValidator : AbstractValidator<CreateOrderItemRequest>
{
    public CreateOrderItemRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("El producto es requerido");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Las notas no pueden exceder 500 caracteres")
            .When(x => x.Notes != null);
    }
}

public class UpdateOrderStatusRequestValidator : AbstractValidator<UpdateOrderStatusRequest>
{
    public UpdateOrderStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("El estado no es válido");

        RuleFor(x => x.CancelReason)
            .NotEmpty().WithMessage("El motivo de cancelación es requerido")
            .When(x => x.Status == Domain.Entities.OrderStatus.Cancelled);

        RuleFor(x => x.CancelReason)
            .MaximumLength(500).WithMessage("El motivo no puede exceder 500 caracteres")
            .When(x => x.CancelReason != null);
    }
}

public class UpdateOrderItemStatusRequestValidator : AbstractValidator<UpdateOrderItemStatusRequest>
{
    public UpdateOrderItemStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("El estado del item no es válido");
    }
}
