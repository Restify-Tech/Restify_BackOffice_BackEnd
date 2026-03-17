namespace Restify.BackOffice.Domain.Enums;

/// <summary>
/// Tipo de cuenta contable (Plan de cuentas NIIF)
/// </summary>
public enum AccountType
{
    /// <summary>
    /// Activos
    /// </summary>
    Asset = 1,

    /// <summary>
    /// Pasivos
    /// </summary>
    Liability = 2,

    /// <summary>
    /// Patrimonio
    /// </summary>
    Equity = 3,

    /// <summary>
    /// Ingresos
    /// </summary>
    Revenue = 4,

    /// <summary>
    /// Gastos
    /// </summary>
    Expense = 5
}
