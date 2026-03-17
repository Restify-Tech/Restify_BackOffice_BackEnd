using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.DTOs;

/// <summary>
/// DTO de periodo contable
/// </summary>
public class AccountingPeriodDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Month { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public AccountingPeriodStatus Status { get; set; }
    public string? ClosedBy { get; set; }
    public DateTime? ClosedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Request para crear periodo contable
/// </summary>
public class CreateAccountingPeriodRequest
{
    public int Year { get; set; }
    public int Month { get; set; }
}
