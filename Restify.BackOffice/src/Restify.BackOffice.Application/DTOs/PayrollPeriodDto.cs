using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.DTOs;

/// <summary>
/// DTO de periodo de nomina
/// </summary>
public class PayrollPeriodDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Month { get; set; }
    public PayrollPeriodType PeriodType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime PaymentDate { get; set; }
    public PayrollPeriodStatus Status { get; set; }
    public decimal TotalGross { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TotalNet { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? PaidBy { get; set; }
    public DateTime? PaidAt { get; set; }
    public List<PayrollEntryDto> Entries { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO de entrada de nomina por empleado
/// </summary>
public class PayrollEntryDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
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
}

/// <summary>
/// Request para crear periodo de nomina
/// </summary>
public class CreatePayrollPeriodRequest
{
    public int Year { get; set; }
    public int Month { get; set; }
    public PayrollPeriodType PeriodType { get; set; }
    public DateTime PaymentDate { get; set; }
}
