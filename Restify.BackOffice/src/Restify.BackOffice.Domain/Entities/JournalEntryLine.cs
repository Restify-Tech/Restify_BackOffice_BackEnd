using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Linea de asiento contable (debito o credito)
/// </summary>
public class JournalEntryLine : TenantEntity
{
    public Guid JournalEntryId { get; set; }
    public Guid AccountId { get; set; }
    public string? Description { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }

    // Navegacion
    public JournalEntry JournalEntry { get; set; } = null!;
    public AccountingAccount Account { get; set; } = null!;
}
