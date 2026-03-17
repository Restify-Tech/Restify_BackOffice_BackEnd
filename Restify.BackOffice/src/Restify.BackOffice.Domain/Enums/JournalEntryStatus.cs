namespace Restify.BackOffice.Domain.Enums;

/// <summary>
/// Estado de un asiento contable
/// </summary>
public enum JournalEntryStatus
{
    /// <summary>
    /// Borrador, puede editarse
    /// </summary>
    Draft = 1,

    /// <summary>
    /// Contabilizado, no puede editarse
    /// </summary>
    Posted = 2,

    /// <summary>
    /// Reversado, tiene asiento inverso
    /// </summary>
    Reversed = 3
}
