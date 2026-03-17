using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Comisión generada por una entrega del pool centralizado
/// </summary>
public class PoolDeliveryCommission : TenantEntity
{
    public Guid DeliveryId { get; set; }
    public Delivery Delivery { get; set; } = null!;

    public Guid DriverId { get; set; }
    public DeliveryDriver Driver { get; set; } = null!;

    public decimal OrderAmount { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal CommissionPercentage { get; set; }
    public decimal CommissionAmount { get; set; }

    public PoolCommissionStatus Status { get; set; } = PoolCommissionStatus.Pending;
    public DateTime? PaidAt { get; set; }
}

/// <summary>
/// Estado de la comisión del pool
/// </summary>
public enum PoolCommissionStatus
{
    Pending = 1,
    Paid = 2,
    Cancelled = 3
}
