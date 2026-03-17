namespace Restify.BackOffice.Domain.Enums;

/// <summary>
/// Tipo de movimiento de caja
/// </summary>
public enum CashMovementType
{
    /// <summary>
    /// Apertura de caja (monto inicial)
    /// </summary>
    Opening = 1,
    
    /// <summary>
    /// Venta (ingreso desde factura)
    /// </summary>
    Sale = 2,
    
    /// <summary>
    /// Retiro de efectivo (para banco o seguridad)
    /// </summary>
    Withdrawal = 3,
    
    /// <summary>
    /// Ingreso manual (ej: pago de deuda, otros ingresos)
    /// </summary>
    Deposit = 4,
    
    /// <summary>
    /// Gasto (ej: compra menor, propinas)
    /// </summary>
    Expense = 5,
    
    /// <summary>
    /// Ajuste manual (corrección)
    /// </summary>
    Adjustment = 6,
    
    /// <summary>
    /// Cierre de caja
    /// </summary>
    Closing = 7
}
