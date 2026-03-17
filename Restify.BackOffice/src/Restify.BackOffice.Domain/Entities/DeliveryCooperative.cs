using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Cooperativa de delivery que agrupa motorizados
/// </summary>
public class DeliveryCooperative : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string Ruc { get; set; } = string.Empty;
    public string? ContactName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    public decimal? CommissionPercentage { get; set; }

    // Navegación
    public ICollection<DeliveryDriver> Drivers { get; set; } = new List<DeliveryDriver>();
}
