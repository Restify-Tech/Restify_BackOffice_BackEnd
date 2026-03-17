using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.DTOs;

/// <summary>
/// DTO de asiento contable
/// </summary>
public class JournalEntryDto
{
    public Guid Id { get; set; }
    public string EntryNumber { get; set; } = string.Empty;
    public Guid PeriodId { get; set; }
    public string? PeriodName { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public JournalEntryType EntryType { get; set; }
    public Guid? SourceId { get; set; }
    public JournalEntryStatus Status { get; set; }
    public string? PostedBy { get; set; }
    public DateTime? PostedAt { get; set; }
    public string? ReversedBy { get; set; }
    public DateTime? ReversedAt { get; set; }
    public List<JournalEntryLineDto> Lines { get; set; } = new();
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO de linea de asiento contable
/// </summary>
public class JournalEntryLineDto
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public string? AccountCode { get; set; }
    public string? AccountName { get; set; }
    public string? Description { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
}

/// <summary>
/// Request para crear asiento contable
/// </summary>
public class CreateJournalEntryRequest
{
    public Guid PeriodId { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public JournalEntryType EntryType { get; set; }
    public Guid? SourceId { get; set; }
    public List<CreateJournalEntryLineRequest> Lines { get; set; } = new();
}

/// <summary>
/// Request para crear linea de asiento contable
/// </summary>
public class CreateJournalEntryLineRequest
{
    public Guid AccountId { get; set; }
    public string? Description { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
}
