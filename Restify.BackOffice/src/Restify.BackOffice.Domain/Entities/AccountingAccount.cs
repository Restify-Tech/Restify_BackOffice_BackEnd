using Restify.BackOffice.Domain.Enums;
using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Cuenta contable del plan de cuentas (NIIF)
/// </summary>
public class AccountingAccount : TenantEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AccountType AccountType { get; set; }
    public Guid? ParentId { get; set; }
    public int Level { get; set; }
    public bool AcceptsEntries { get; set; }
    public bool IsActive { get; set; } = true;

    // Navegacion
    public AccountingAccount? Parent { get; set; }
    public ICollection<AccountingAccount> Children { get; set; } = new List<AccountingAccount>();
    public ICollection<JournalEntryLine> JournalEntryLines { get; set; } = new List<JournalEntryLine>();
}
