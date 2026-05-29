using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Pago dividido entre N personas o N metodos de pago para un mismo pedido
/// </summary>
public class SplitPayment : TenantEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public decimal TotalAmount { get; set; }
    public int SplitCount { get; set; }

    public SplitPaymentStatus Status { get; set; } = SplitPaymentStatus.Pending;

    /// <summary>
    /// Usuario que inicio el pago dividido
    /// </summary>
    public string? CreatedByUserId { get; set; }

    public ICollection<SplitPaymentItem> Items { get; set; } = new List<SplitPaymentItem>();
}

/// <summary>
/// Item individual dentro de un pago dividido
/// </summary>
public class SplitPaymentItem : BaseEntity
{
    public Guid SplitPaymentId { get; set; }
    public SplitPayment SplitPayment { get; set; } = null!;

    public decimal Amount { get; set; }
    public PaymentMethodType PaymentMethod { get; set; }

    /// <summary>
    /// Nombre de la persona que paga este item (opcional)
    /// </summary>
    public string? PaidBy { get; set; }

    public DateTime? PaidAt { get; set; }
    public SplitPaymentItemStatus Status { get; set; } = SplitPaymentItemStatus.Pending;
}

/// <summary>
/// Estado del pago dividido
/// </summary>
public enum SplitPaymentStatus
{
    Pending = 1,
    PartiallyPaid = 2,
    Completed = 3,
    Cancelled = 4
}

/// <summary>
/// Estado de un item individual del pago dividido
/// </summary>
public enum SplitPaymentItemStatus
{
    Pending = 1,
    Paid = 2,
    Cancelled = 3
}
