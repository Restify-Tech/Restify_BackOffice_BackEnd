using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

public enum PromotionType
{
    PercentageDiscount = 1,
    FixedDiscount = 2,
    BuyXGetY = 3,
    HappyHour = 4
}

/// <summary>
/// Promocion aplicable a pedidos del tenant
/// </summary>
public class Promotion : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public PromotionType Type { get; set; }

    /// <summary>
    /// Porcentaje o monto fijo del descuento
    /// </summary>
    public decimal DiscountValue { get; set; }

    /// <summary>
    /// JSON flexible: { "minAmount": 20, "categoryId": "uuid", "dayOfWeek": [5,6] }
    /// </summary>
    public string? ConditionsJson { get; set; }

    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }

    public bool IsActive { get; set; } = true;
    public int? MaxUsageCount { get; set; }
    public int CurrentUsageCount { get; set; } = 0;
}
