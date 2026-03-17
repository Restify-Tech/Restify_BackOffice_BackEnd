namespace Restify.BackOffice.Domain.Enums;

/// <summary>
/// Estado del periodo de nomina
/// </summary>
public enum PayrollPeriodStatus
{
    /// <summary>
    /// Borrador, en preparacion
    /// </summary>
    Draft = 1,

    /// <summary>
    /// Calculado, listo para revision
    /// </summary>
    Calculated = 2,

    /// <summary>
    /// Aprobado por supervisor
    /// </summary>
    Approved = 3,

    /// <summary>
    /// Pagado a los empleados
    /// </summary>
    Paid = 4,

    /// <summary>
    /// Cancelado
    /// </summary>
    Cancelled = 5
}
