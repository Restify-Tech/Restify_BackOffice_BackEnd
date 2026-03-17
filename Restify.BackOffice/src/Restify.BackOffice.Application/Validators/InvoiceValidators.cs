using FluentValidation;
using Restify.BackOffice.Application.DTOs;

namespace Restify.BackOffice.Application.Validators;

public class CreateInvoiceRequestValidator : AbstractValidator<CreateInvoiceRequest>
{
    public CreateInvoiceRequestValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("El pedido es requerido");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum().WithMessage("El método de pago no es válido");

        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(0, 100).WithMessage("El descuento debe estar entre 0 y 100%");

        RuleFor(x => x.CustomerEmail)
            .EmailAddress().WithMessage("El formato del email no es válido")
            .When(x => !string.IsNullOrEmpty(x.CustomerEmail));

        RuleFor(x => x.CustomerName)
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres")
            .When(x => x.CustomerName != null);

        RuleFor(x => x.CustomerIdNumber)
            .MaximumLength(20).WithMessage("La identificación no puede exceder 20 caracteres")
            .When(x => x.CustomerIdNumber != null);
    }
}

public class UpdateInvoiceRequestValidator : AbstractValidator<UpdateInvoiceRequest>
{
    public UpdateInvoiceRequestValidator()
    {
        RuleFor(x => x.CustomerEmail)
            .EmailAddress().WithMessage("El formato del email no es válido")
            .When(x => !string.IsNullOrEmpty(x.CustomerEmail));

        RuleFor(x => x.CustomerName)
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres")
            .When(x => x.CustomerName != null);
    }
}

public class ProcessPaymentRequestValidator : AbstractValidator<ProcessPaymentRequest>
{
    public ProcessPaymentRequestValidator()
    {
        RuleFor(x => x.PaymentMethod)
            .IsInEnum().WithMessage("El método de pago no es válido");

        RuleFor(x => x.AmountPaid)
            .GreaterThan(0).WithMessage("El monto debe ser mayor a 0")
            .When(x => x.AmountPaid.HasValue);
    }
}

public class CancelInvoiceRequestValidator : AbstractValidator<CancelInvoiceRequest>
{
    public CancelInvoiceRequestValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("El motivo de anulación es requerido")
            .MaximumLength(500).WithMessage("El motivo no puede exceder 500 caracteres");
    }
}
