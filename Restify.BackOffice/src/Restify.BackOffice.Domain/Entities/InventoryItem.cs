using Restify.BackOffice.Domain.Enums;
using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Item de inventario - control de stock por producto
/// </summary>
public class InventoryItem : TenantEntity
{
    /// <summary>
    /// Producto asociado
    /// </summary>
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    /// <summary>
    /// Cantidad actual en stock
    /// </summary>
    public decimal CurrentStock { get; set; }
    
    /// <summary>
    /// Unidad de medida (kg, lt, unidades, etc)
    /// </summary>
    public string Unit { get; set; } = "unidades";
    
    /// <summary>
    /// Stock mínimo (alerta de bajo stock)
    /// </summary>
    public decimal MinimumStock { get; set; }
    
    /// <summary>
    /// Stock máximo (para reorden)
    /// </summary>
    public decimal? MaximumStock { get; set; }
    
    /// <summary>
    /// Costo promedio actual
    /// </summary>
    public decimal AverageCost { get; set; }
    
    /// <summary>
    /// Último costo de compra
    /// </summary>
    public decimal? LastPurchaseCost { get; set; }
    
    /// <summary>
    /// Fecha de última compra
    /// </summary>
    public DateTime? LastPurchaseDate { get; set; }
    
    /// <summary>
    /// Fecha de última venta/salida
    /// </summary>
    public DateTime? LastSaleDate { get; set; }
    
    /// <summary>
    /// Ubicación en almacén
    /// </summary>
    public string? StorageLocation { get; set; }
    
    /// <summary>
    /// Si se controla stock para este producto
    /// </summary>
    public bool TrackStock { get; set; } = true;
    
    /// <summary>
    /// Método de cálculo de costo
    /// </summary>
    public StockCostMethod CostMethod { get; set; } = StockCostMethod.WeightedAverage;
    
    /// <summary>
    /// Movimientos de inventario
    /// </summary>
    public ICollection<InventoryMovement> Movements { get; set; } = new List<InventoryMovement>();
    
    // Propiedades calculadas
    
    /// <summary>
    /// Si está por debajo del stock mínimo
    /// </summary>
    public bool IsLowStock => CurrentStock <= MinimumStock;
    
    /// <summary>
    /// Si está agotado
    /// </summary>
    public bool IsOutOfStock => CurrentStock <= 0;
    
    /// <summary>
    /// Valor total del inventario (cantidad * costo promedio)
    /// </summary>
    public decimal TotalValue => CurrentStock * AverageCost;
}
