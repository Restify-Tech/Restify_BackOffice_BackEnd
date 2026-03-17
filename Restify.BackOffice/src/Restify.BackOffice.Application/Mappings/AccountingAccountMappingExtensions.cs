using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Mappings;

public static class AccountingAccountMappingExtensions
{
    public static AccountingAccountDto ToDto(this AccountingAccount entity, bool includeChildren = false)
    {
        return new AccountingAccountDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Description = entity.Description,
            AccountType = entity.AccountType,
            ParentId = entity.ParentId,
            ParentName = entity.Parent?.Name,
            Level = entity.Level,
            AcceptsEntries = entity.AcceptsEntries,
            IsActive = entity.IsActive,
            Children = includeChildren && entity.Children?.Count > 0
                ? entity.Children.Select(c => c.ToDto(true)).ToList()
                : new(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static AccountingAccount ToEntity(this CreateAccountingAccountRequest request, Guid tenantId)
    {
        return new AccountingAccount
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            AccountType = request.AccountType,
            ParentId = request.ParentId,
            AcceptsEntries = request.AcceptsEntries,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateFrom(this AccountingAccount entity, UpdateAccountingAccountRequest request)
    {
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
    }
}
