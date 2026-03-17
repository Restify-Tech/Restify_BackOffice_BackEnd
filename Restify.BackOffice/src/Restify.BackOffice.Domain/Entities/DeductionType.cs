using Restify.BackOffice.Domain.Enums;
using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Tipo de deduccion configurable para nomina
/// </summary>
public class DeductionType : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DeductionCalculationType CalculationType { get; set; }
    public decimal DefaultValue { get; set; }
    public bool IsRequired { get; set; }
    public DeductionAppliesTo AppliesTo { get; set; }
    public bool IsActive { get; set; } = true;
}
