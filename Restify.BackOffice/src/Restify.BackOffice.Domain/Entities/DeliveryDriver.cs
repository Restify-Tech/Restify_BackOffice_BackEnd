using Restify.BackOffice.Domain.Enums;
using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Motorizado de delivery (propio del tenant o pool centralizado)
/// </summary>
public class DeliveryDriver : TenantEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string IdentificationNumber { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? VehiclePlate { get; set; }
    public string? VehicleDescription { get; set; }
    public string? PhotoUrl { get; set; }
    public DriverType DriverType { get; set; }
    public Guid? CooperativeId { get; set; }
    public DeliveryCooperative? Cooperative { get; set; }
    public DriverStatus Status { get; set; } = DriverStatus.Offline;
    public bool IsVerified { get; set; }
    public bool IsActive { get; set; } = true;
    public string PasswordHash { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }
    public decimal? Rating { get; set; }
    public int TotalDeliveries { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // --- Pool & Verificación (Fase 1) ---

    /// <summary>
    /// true = repartidor del pool centralizado (TenantId = Guid.Empty)
    /// </summary>
    public bool IsPoolDriver { get; set; }

    /// <summary>
    /// Zona de delivery asignada (para pool drivers)
    /// </summary>
    public Guid? DeliveryZoneId { get; set; }

    /// <summary>
    /// Tipo de vehículo
    /// </summary>
    public VehicleType? VehicleType { get; set; }

    public string? VehicleBrand { get; set; }
    public string? VehicleModel { get; set; }
    public int? VehicleYear { get; set; }
    public string? VehicleColor { get; set; }

    /// <summary>
    /// Estado de verificación documental (para pool drivers)
    /// </summary>
    public DriverVerificationStatus VerificationStatus { get; set; } = DriverVerificationStatus.Pending;

    public DateTime? VerifiedAt { get; set; }
    public Guid? VerifiedByUserId { get; set; }
    public string? RejectionReason { get; set; }

    // --- GPS en tiempo real ---

    public double? CurrentLatitude { get; set; }
    public double? CurrentLongitude { get; set; }
    public DateTime? LastLocationUpdateAt { get; set; }

    // Navegación
    public ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
    public ICollection<DriverDocument> Documents { get; set; } = new List<DriverDocument>();
}

/// <summary>
/// Tipo de motorizado
/// </summary>
public enum DriverType
{
    Cooperative = 1,
    Individual = 2,
    Pool = 3
}

/// <summary>
/// Estado del motorizado
/// </summary>
public enum DriverStatus
{
    Available = 1,
    OnDelivery = 2,
    Offline = 3
}
