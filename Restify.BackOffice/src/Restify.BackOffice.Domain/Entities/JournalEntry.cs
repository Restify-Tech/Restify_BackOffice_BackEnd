using Restify.BackOffice.Domain.Enums;
using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Asiento contable
/// </summary>
public class JournalEntry : TenantEntity
{
    public string EntryNumber { get; set; } = string.Empty;
    public Guid PeriodId { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public JournalEntryType EntryType { get; set; }
    public Guid? SourceId { get; set; }
    public JournalEntryStatus Status { get; set; } = JournalEntryStatus.Draft;
    public string? PostedBy { get; set; }
    public DateTime? PostedAt { get; set; }
    public string? ReversedBy { get; set; }
    public DateTime? ReversedAt { get; set; }

    // Navegacion
    public AccountingPeriod Period { get; set; } = null!;
    public ICollection<JournalEntryLine> Lines { get; set; } = new List<JournalEntryLine>();
}
