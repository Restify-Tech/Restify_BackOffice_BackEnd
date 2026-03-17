using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Mappings;

public static class DeductionTypeMappingExtensions
{
    public static DeductionTypeDto ToDto(this DeductionType entity)
    {
        return new DeductionTypeDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            CalculationType = entity.CalculationType,
            DefaultValue = entity.DefaultValue,
            IsRequired = entity.IsRequired,
            AppliesTo = entity.AppliesTo,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static DeductionType ToEntity(this CreateDeductionTypeRequest request, Guid tenantId)
    {
        return new DeductionType
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = request.Name,
            Description = request.Description,
            CalculationType = request.CalculationType,
            DefaultValue = request.DefaultValue,
            IsRequired = request.IsRequired,
            AppliesTo = request.AppliesTo,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateFrom(this DeductionType entity, UpdateDeductionTypeRequest request)
    {
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.CalculationType = request.CalculationType;
        entity.DefaultValue = request.DefaultValue;
        entity.IsRequired = request.IsRequired;
        entity.AppliesTo = request.AppliesTo;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
    }
}
