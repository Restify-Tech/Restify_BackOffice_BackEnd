using Restify.BackOffice.Domain.Enums;
using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Periodo de nomina
/// </summary>
public class PayrollPeriod : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Month { get; set; }
    public PayrollPeriodType PeriodType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime PaymentDate { get; set; }
    public PayrollPeriodStatus Status { get; set; } = PayrollPeriodStatus.Draft;
    public decimal TotalGross { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TotalNet { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? PaidBy { get; set; }
    public DateTime? PaidAt { get; set; }

    // Navegacion
    public ICollection<PayrollEntry> Entries { get; set; } = new List<PayrollEntry>();
}
