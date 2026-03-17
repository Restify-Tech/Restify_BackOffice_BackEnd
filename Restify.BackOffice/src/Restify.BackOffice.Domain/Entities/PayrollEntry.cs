using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Entrada de nomina por empleado en un periodo
/// </summary>
public class PayrollEntry : TenantEntity
{
    public Guid PayrollPeriodId { get; set; }
    public Guid EmployeeId { get; set; }
    public decimal BaseSalary { get; set; }
    public int WorkedDays { get; set; }
    public decimal OvertimeHours { get; set; }
    public decimal GrossPay { get; set; }
    public decimal SocialSecurityEmployee { get; set; }
    public decimal SocialSecurityEmployer { get; set; }
    public decimal IncomeTax { get; set; }
    public decimal OtherDeductions { get; set; }
    public decimal OtherBenefits { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetPay { get; set; }

    // Navegacion
    public PayrollPeriod PayrollPeriod { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
}
