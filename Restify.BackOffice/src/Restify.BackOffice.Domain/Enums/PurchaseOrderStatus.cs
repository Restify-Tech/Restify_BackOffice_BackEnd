namespace Restify.BackOffice.Domain.Enums;

/// <summary>
/// Estado de una orden de compra
/// </summary>
public enum PurchaseOrderStatus
{
    /// <summary>
    /// Borrador (sin enviar)
    /// </summary>
    Draft = 1,
    
    /// <summary>
    /// Enviada al proveedor
    /// </summary>
    Sent = 2,
    
    /// <summary>
    /// Confirmada por el proveedor
    /// </summary>
    Confirmed = 3,
    
    /// <summary>
    /// Recibida parcialmente
    /// </summary>
    PartiallyReceived = 4,
    
    /// <summary>
    /// Recibida completamente
    /// </summary>
    Received = 5,
    
    /// <summary>
    /// Cancelada
    /// </summary>
    Cancelled = 6
}
