using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Mappings;

public static class GlobalModifierMappingExtensions
{
    public static GlobalModifierDto ToDto(this GlobalModifier modifier)
    {
        return new GlobalModifierDto
        {
            Id = modifier.Id,
            Name = modifier.Name,
            Description = modifier.Description,
            DefaultPriceAdjustment = modifier.DefaultPriceAdjustment,
            IsActive = modifier.IsActive,
            DisplayOrder = modifier.DisplayOrder,
            Type = modifier.Type,
            TypeName = modifier.Type.ToString(),
            ProductCount = modifier.Products?.Count ?? 0,
            CreatedAt = modifier.CreatedAt,
            UpdatedAt = modifier.UpdatedAt
        };
    }

    public static GlobalModifierProductDto ToDto(this GlobalModifierProduct modifierProduct)
    {
        return new GlobalModifierProductDto
        {
            GlobalModifierId = modifierProduct.GlobalModifierId,
            ModifierName = modifierProduct.GlobalModifier?.Name ?? string.Empty,
            CustomPriceAdjustment = modifierProduct.CustomPriceAdjustment,
            EffectivePriceAdjustment = modifierProduct.CustomPriceAdjustment 
                ?? modifierProduct.GlobalModifier?.DefaultPriceAdjustment 
                ?? 0,
            IsRequired = modifierProduct.IsRequired,
            IsActive = modifierProduct.IsActive,
            Type = modifierProduct.GlobalModifier?.Type ?? ModifierType.Other
        };
    }

    public static GlobalModifier ToEntity(this CreateGlobalModifierRequest request, Guid tenantId)
    {
        return new GlobalModifier
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = request.Name,
            Description = request.Description,
            DefaultPriceAdjustment = request.DefaultPriceAdjustment,
            IsActive = request.IsActive,
            DisplayOrder = request.DisplayOrder,
            Type = request.Type,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateFromRequest(this GlobalModifier modifier, UpdateGlobalModifierRequest request)
    {
        modifier.Name = request.Name;
        modifier.Description = request.Description;
        modifier.DefaultPriceAdjustment = request.DefaultPriceAdjustment;
        modifier.IsActive = request.IsActive;
        modifier.DisplayOrder = request.DisplayOrder;
        modifier.Type = request.Type;
        modifier.UpdatedAt = DateTime.UtcNow;
    }
}
