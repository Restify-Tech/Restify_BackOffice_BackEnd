using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.DTOs;

/// <summary>
/// DTO de tipo de deduccion
/// </summary>
public class DeductionTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DeductionCalculationType CalculationType { get; set; }
    public decimal DefaultValue { get; set; }
    public bool IsRequired { get; set; }
    public DeductionAppliesTo AppliesTo { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Request para crear tipo de deduccion
/// </summary>
public class CreateDeductionTypeRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DeductionCalculationType CalculationType { get; set; }
    public decimal DefaultValue { get; set; }
    public bool IsRequired { get; set; }
    public DeductionAppliesTo AppliesTo { get; set; }
}

/// <summary>
/// Request para actualizar tipo de deduccion
/// </summary>
public class UpdateDeductionTypeRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DeductionCalculationType CalculationType { get; set; }
    public decimal DefaultValue { get; set; }
    public bool IsRequired { get; set; }
    public DeductionAppliesTo AppliesTo { get; set; }
    public bool IsActive { get; set; }
}
