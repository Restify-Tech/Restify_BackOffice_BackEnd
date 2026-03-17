namespace Restify.BackOffice.Domain.Enums;

/// <summary>
/// Tipo de asiento contable
/// </summary>
public enum JournalEntryType
{
    /// <summary>
    /// Asiento manual
    /// </summary>
    Manual = 1,

    /// <summary>
    /// Generado automaticamente por factura
    /// </summary>
    AutoInvoice = 2,

    /// <summary>
    /// Generado automaticamente por pago
    /// </summary>
    AutoPayment = 3,

    /// <summary>
    /// Generado automaticamente por nomina
    /// </summary>
    AutoPayroll = 4,

    /// <summary>
    /// Ajuste contable
    /// </summary>
    Adjustment = 5,

    /// <summary>
    /// Asiento de cierre
    /// </summary>
    Closing = 6
}
