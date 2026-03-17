using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.Mappings;

public static class AccountingPeriodMappingExtensions
{
    public static AccountingPeriodDto ToDto(this AccountingPeriod entity)
    {
        return new AccountingPeriodDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Year = entity.Year,
            Month = entity.Month,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Status = entity.Status,
            ClosedBy = entity.ClosedBy,
            ClosedAt = entity.ClosedAt,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static AccountingPeriod ToEntity(this CreateAccountingPeriodRequest request, Guid tenantId)
    {
        var startDate = new DateTime(request.Year, request.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        var name = $"{startDate:MMMM yyyy}";

        return new AccountingPeriod
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = name,
            Year = request.Year,
            Month = request.Month,
            StartDate = startDate,
            EndDate = endDate,
            Status = AccountingPeriodStatus.Open,
            CreatedAt = DateTime.UtcNow
        };
    }
}
