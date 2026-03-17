using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Entrega de un pedido a domicilio
/// </summary>
public class Delivery : TenantEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public Guid? DriverId { get; set; }
    public DeliveryDriver? Driver { get; set; }
    public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;
    public string DeliveryAddress { get; set; } = string.Empty;
    public string? DeliveryNotes { get; set; }
    public decimal DeliveryFee { get; set; }
    public int? EstimatedDeliveryMinutes { get; set; }
    public DateTime? AssignedAt { get; set; }
    public DateTime? PickedUpAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? DeliveryProofUrl { get; set; }
    public int? CustomerRating { get; set; }
    public string? CustomerFeedback { get; set; }
    public decimal? DriverLatitude { get; set; }
    public decimal? DriverLongitude { get; set; }
    public DateTime? LastLocationUpdate { get; set; }
    public string? FailureReason { get; set; }

    // --- Pool Delivery (Fase 1) ---

    /// <summary>
    /// true = entrega realizada por repartidor del pool centralizado
    /// </summary>
    public bool IsPoolDelivery { get; set; }

    public decimal? PoolCommissionAmount { get; set; }
    public decimal? PoolCommissionPercentage { get; set; }
    public Guid? DeliveryZoneId { get; set; }

    /// <summary>
    /// Ubicación del restaurante al momento de la entrega
    /// </summary>
    public double? RestaurantLatitude { get; set; }
    public double? RestaurantLongitude { get; set; }
}

/// <summary>
/// Estado de la entrega
/// </summary>
public enum DeliveryStatus
{
    Pending = 1,
    Assigned = 2,
    PickedUp = 3,
    InTransit = 4,
    Delivered = 5,
    Failed = 6,
    Cancelled = 7,
    AwaitingPoolAssignment = 8
}
