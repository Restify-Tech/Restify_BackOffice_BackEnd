namespace Restify.BackOffice.Domain.Enums;

/// <summary>
/// Estado de un periodo contable
/// </summary>
public enum AccountingPeriodStatus
{
    /// <summary>
    /// Periodo abierto, acepta asientos
    /// </summary>
    Open = 1,

    /// <summary>
    /// Periodo cerrado, no acepta nuevos asientos
    /// </summary>
    Closed = 2,

    /// <summary>
    /// Periodo bloqueado, no puede reabrirse
    /// </summary>
    Locked = 3
}
