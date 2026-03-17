using Restify.Auth.Domain.Common;

namespace Restify.Auth.Domain.Entities;

/// <summary>
/// Zona de delivery gestionada por SuperAdmin (entidad global, sin TenantId)
/// </summary>
public class DeliveryZone : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Region { get; set; }
    public string Country { get; set; } = "Ecuador";
    public decimal DefaultCommissionPercentage { get; set; } = 10.00m;
    public double MaxDeliveryRadiusKm { get; set; } = 10.0;
    public decimal MinDriverRating { get; set; } = 3.0m;
    public bool IsActive { get; set; } = true;
}
