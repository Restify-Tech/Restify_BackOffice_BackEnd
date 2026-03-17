using Restify.BackOffice.Domain.Enums;
using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Movimiento de inventario (entrada/salida)
/// </summary>
public class InventoryMovement : TenantEntity
{
    /// <summary>
    /// Item de inventario afectado
    /// </summary>
    public Guid InventoryItemId { get; set; }
    public InventoryItem InventoryItem { get; set; } = null!;
    
    /// <summary>
    /// Tipo de movimiento
    /// </summary>
    public InventoryMovementType Type { get; set; }
    
    /// <summary>
    /// Cantidad (positiva para entradas, negativa para salidas)
    /// </summary>
    public decimal Quantity { get; set; }
    
    /// <summary>
    /// Costo unitario del movimiento
    /// </summary>
    public decimal UnitCost { get; set; }
    
    /// <summary>
    /// Costo total del movimiento
    /// </summary>
    public decimal TotalCost { get; set; }
    
    /// <summary>
    /// Stock anterior al movimiento
    /// </summary>
    public decimal PreviousStock { get; set; }
    
    /// <summary>
    /// Stock posterior al movimiento
    /// </summary>
    public decimal NewStock { get; set; }
    
    /// <summary>
    /// Referencia externa (ej: PurchaseOrderId, OrderId)
    /// </summary>
    public Guid? ReferenceId { get; set; }
    
    /// <summary>
    /// Tipo de referencia (ej: "PurchaseOrder", "Order", "Adjustment")
    /// </summary>
    public string? ReferenceType { get; set; }
    
    /// <summary>
    /// Descripción del movimiento
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Notas adicionales
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Fecha del movimiento
    /// </summary>
    public DateTime MovementDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Usuario que registró el movimiento
    /// </summary>
    public string? RegisteredBy { get; set; }
    
    /// <summary>
    /// Si es un movimiento de entrada (aumenta stock)
    /// </summary>
    public bool IsInbound => Type is InventoryMovementType.Purchase 
        or InventoryMovementType.AdjustmentIn 
        or InventoryMovementType.InitialStock;
    
    /// <summary>
    /// Si es un movimiento de salida (disminuye stock)
    /// </summary>
    public bool IsOutbound => Type is InventoryMovementType.Sale 
        or InventoryMovementType.AdjustmentOut 
        or InventoryMovementType.Return;
}
