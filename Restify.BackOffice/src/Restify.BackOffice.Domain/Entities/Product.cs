using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Producto del menú
/// </summary>
public class Product : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public string? Sku { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsAvailable { get; set; } = true;
    public int DisplayOrder { get; set; }
    
    // Relación con categoría
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    
    // Modificadores específicos del producto
    public ICollection<ProductModifier> Modifiers { get; set; } = new List<ProductModifier>();
    
    // Modificadores globales asociados
    public ICollection<GlobalModifierProduct> GlobalModifiers { get; set; } = new List<GlobalModifierProduct>();
}

/// <summary>
/// Modificador de producto (ej: tamaño, ingredientes extra, etc.)
/// </summary>
public class ProductModifier : TenantEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PriceAdjustment { get; set; } = 0; // Puede ser positivo (extra) o negativo (descuento)
    public bool IsRequired { get; set; } = false;
    public bool IsActive { get; set; } = true;
}
