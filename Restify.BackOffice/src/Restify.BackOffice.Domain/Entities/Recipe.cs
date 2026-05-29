using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Receta de un producto del menu (ingredientes y costos)
/// </summary>
public class Recipe : TenantEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string? Instructions { get; set; }
    public int PreparationMinutes { get; set; } = 15;

    /// <summary>
    /// 0 = calculado automatico desde ingredientes
    /// </summary>
    public decimal EstimatedCostOverride { get; set; } = 0;

    public ICollection<RecipeIngredient> Ingredients { get; set; } = new List<RecipeIngredient>();
}

/// <summary>
/// Ingrediente de una receta
/// </summary>
public class RecipeIngredient : TenantEntity
{
    public Guid RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    public Guid InventoryItemId { get; set; }
    public InventoryItem InventoryItem { get; set; } = null!;

    public decimal Quantity { get; set; }

    /// <summary>
    /// "gr", "ml", "unidad"
    /// </summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>
    /// Factor de merma (0.05 = 5%)
    /// </summary>
    public decimal WasteFactor { get; set; } = 0.05m;
}
