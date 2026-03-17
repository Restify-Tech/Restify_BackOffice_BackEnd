namespace Restify.BackOffice.Domain.Enums;

/// <summary>
/// Estado de una caja registradora
/// </summary>
public enum CashRegisterStatus
{
    /// <summary>
    /// Caja cerrada (sin operaciones)
    /// </summary>
    Closed = 0,
    
    /// <summary>
    /// Caja abierta (operando)
    /// </summary>
    Open = 1,
    
    /// <summary>
    /// Caja en proceso de cierre (contando)
    /// </summary>
    Closing = 2
}
