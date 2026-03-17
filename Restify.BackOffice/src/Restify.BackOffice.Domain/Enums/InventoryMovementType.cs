namespace Restify.BackOffice.Domain.Enums;

/// <summary>
/// Tipo de movimiento de inventario
/// </summary>
public enum InventoryMovementType
{
    /// <summary>
    /// Entrada por compra (purchase order)
    /// </summary>
    Purchase = 1,
    
    /// <summary>
    /// Salida por venta (consumo en orden)
    /// </summary>
    Sale = 2,
    
    /// <summary>
    /// Ajuste positivo (corrección, inventario físico)
    /// </summary>
    AdjustmentIn = 3,
    
    /// <summary>
    /// Ajuste negativo (merma, robo, vencimiento)
    /// </summary>
    AdjustmentOut = 4,
    
    /// <summary>
    /// Transferencia entre ubicaciones
    /// </summary>
    Transfer = 5,
    
    /// <summary>
    /// Devolución a proveedor
    /// </summary>
    Return = 6,
    
    /// <summary>
    /// Inventario inicial (apertura)
    /// </summary>
    InitialStock = 7
}
