using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.DTOs;

public class GlobalModifierDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal DefaultPriceAdjustment { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public ModifierType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateGlobalModifierRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal DefaultPriceAdjustment { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
    public ModifierType Type { get; set; } = ModifierType.Other;
}

public class UpdateGlobalModifierRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal DefaultPriceAdjustment { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public ModifierType Type { get; set; }
}

public class GlobalModifierProductDto
{
    public Guid GlobalModifierId { get; set; }
    public string ModifierName { get; set; } = string.Empty;
    public decimal? CustomPriceAdjustment { get; set; }
    public decimal EffectivePriceAdjustment { get; set; }
    public bool IsRequired { get; set; }
    public bool IsActive { get; set; }
    public ModifierType Type { get; set; }
}

public class AssignGlobalModifierRequest
{
    public Guid GlobalModifierId { get; set; }
    public decimal? CustomPriceAdjustment { get; set; }
    public bool IsRequired { get; set; } = false;
    public bool IsActive { get; set; } = true;
}
