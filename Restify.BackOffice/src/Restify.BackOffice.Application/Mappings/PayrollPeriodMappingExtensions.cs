using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.Mappings;

public static class PayrollPeriodMappingExtensions
{
    public static PayrollPeriodDto ToDto(this PayrollPeriod entity)
    {
        var entries = entity.Entries?.Select(e => e.ToDto()).ToList() ?? new();

        return new PayrollPeriodDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Year = entity.Year,
            Month = entity.Month,
            PeriodType = entity.PeriodType,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            PaymentDate = entity.PaymentDate,
            Status = entity.Status,
            TotalGross = entity.TotalGross,
            TotalDeductions = entity.TotalDeductions,
            TotalNet = entity.TotalNet,
            ApprovedBy = entity.ApprovedBy,
            ApprovedAt = entity.ApprovedAt,
            PaidBy = entity.PaidBy,
            PaidAt = entity.PaidAt,
            Entries = entries,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static PayrollEntryDto ToDto(this PayrollEntry entity)
    {
        return new PayrollEntryDto
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            EmployeeName = entity.Employee != null
                ? entity.Employee.FirstName + " " + entity.Employee.LastName
                : string.Empty,
            BaseSalary = entity.BaseSalary,
            WorkedDays = entity.WorkedDays,
            OvertimeHours = entity.OvertimeHours,
            GrossPay = entity.GrossPay,
            SocialSecurityEmployee = entity.SocialSecurityEmployee,
            SocialSecurityEmployer = entity.SocialSecurityEmployer,
            IncomeTax = entity.IncomeTax,
            OtherDeductions = entity.OtherDeductions,
            OtherBenefits = entity.OtherBenefits,
            TotalDeductions = entity.TotalDeductions,
            NetPay = entity.NetPay
        };
    }

    public static PayrollPeriod ToEntity(this CreatePayrollPeriodRequest request, Guid tenantId)
    {
        var startDate = new DateTime(request.Year, request.Month, 1);
        var endDate = request.PeriodType == PayrollPeriodType.Monthly
            ? startDate.AddMonths(1).AddDays(-1)
            : startDate.AddDays(14);

        var name = request.PeriodType == PayrollPeriodType.Monthly
            ? $"Nomina {request.Year}-{request.Month:D2}"
            : $"Nomina {request.Year}-{request.Month:D2} (Quincenal)";

        return new PayrollPeriod
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = name,
            Year = request.Year,
            Month = request.Month,
            PeriodType = request.PeriodType,
            StartDate = startDate,
            EndDate = endDate,
            PaymentDate = request.PaymentDate,
            Status = PayrollPeriodStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };
    }
}
