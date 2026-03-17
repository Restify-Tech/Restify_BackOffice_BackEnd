using Restify.BackOffice.Domain.Enums;
using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Periodo contable (mensual)
/// </summary>
public class AccountingPeriod : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Month { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public AccountingPeriodStatus Status { get; set; } = AccountingPeriodStatus.Open;
    public string? ClosedBy { get; set; }
    public DateTime? ClosedAt { get; set; }

    // Navegacion
    public ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
}
