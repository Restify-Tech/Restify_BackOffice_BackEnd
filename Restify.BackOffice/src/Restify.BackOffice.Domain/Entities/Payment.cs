using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Pago asociado a un pedido
/// </summary>
public class Payment : TenantEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public Guid? InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethodType Method { get; set; }
    public PaymentTransactionStatus Status { get; set; } = PaymentTransactionStatus.Pending;
    public string? GatewayTransactionId { get; set; }
    public string? GatewayResponse { get; set; }
    public string? PayerName { get; set; }
    public string? PayerIdentification { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? FailureReason { get; set; }

    // Items para pagos divididos
    public ICollection<PaymentItem> Items { get; set; } = new List<PaymentItem>();
}

/// <summary>
/// Item individual de un pago dividido
/// </summary>
public class PaymentItem : BaseEntity
{
    public Guid PaymentId { get; set; }
    public Payment Payment { get; set; } = null!;
    public Guid OrderItemId { get; set; }
    public OrderItem OrderItem { get; set; } = null!;
    public decimal Amount { get; set; }
}

/// <summary>
/// Método de pago para transacciones
/// </summary>
public enum PaymentMethodType
{
    Cash = 1,
    CreditCard = 2,
    DebitCard = 3,
    Transfer = 4,
    MercadoPago = 5,
    Stripe = 6,
    Other = 99
}

/// <summary>
/// Estado de la transacción de pago
/// </summary>
public enum PaymentTransactionStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Refunded = 5
}
