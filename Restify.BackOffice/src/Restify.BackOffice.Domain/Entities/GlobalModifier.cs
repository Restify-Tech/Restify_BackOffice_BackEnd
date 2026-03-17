using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Modificador global reutilizable entre productos
/// </summary>
public class GlobalModifier : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal DefaultPriceAdjustment { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
    
    // Tipo de modificador (para agruparlos)
    public ModifierType Type { get; set; }
    
    // Relación con productos
    public ICollection<GlobalModifierProduct> Products { get; set; } = new List<GlobalModifierProduct>();
}

/// <summary>
/// Relación many-to-many entre GlobalModifier y Product
/// Permite customizar el precio del modificador por producto
/// </summary>
public class GlobalModifierProduct : TenantEntity
{
    public Guid GlobalModifierId { get; set; }
    public GlobalModifier GlobalModifier { get; set; } = null!;
    
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    // Permite override del precio por producto (si es null, usa DefaultPriceAdjustment)
    public decimal? CustomPriceAdjustment { get; set; }
    public bool IsRequired { get; set; } = false;
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Tipos de modificadores para agrupación
/// </summary>
public enum ModifierType
{
    Size = 1,           // Tamaño (Grande, Mediano, Chico)
    AddOn = 2,          // Extras (Extra queso, Aguacate, etc)
    Removal = 3,        // Remover ingredientes (Sin cebolla, Sin tomate)
    Preparation = 4,    // Preparación (Al grill, Frito, Al horno)
    Temperature = 5,    // Temperatura (Caliente, Frío, Tibio)
    Other = 99          // Otros
}
