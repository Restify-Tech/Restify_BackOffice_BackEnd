using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Configuracion de franquicia — este tenant es el franquiciante (marca maestra)
/// </summary>
public class FranchiseConfig : TenantEntity
{
    public string FranchiseName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool AllowLocalMenuOverrides { get; set; } = true;
    public bool AllowLocalPromotions { get; set; } = true;
    public bool SyncMenuAutomatically { get; set; } = false;
    public string? ContactEmail { get; set; }
    public string? LogoUrl { get; set; }

    public ICollection<FranchiseeRelation> Franchisees { get; set; } = new List<FranchiseeRelation>();
}

/// <summary>
/// Relacion entre franquiciante y un franquiciado
/// </summary>
public class FranchiseeRelation : TenantEntity
{
    public Guid FranchiseConfigId { get; set; }
    public FranchiseConfig FranchiseConfig { get; set; } = null!;

    public Guid FranchiseeTenantId { get; set; }
    public string FranchiseeName { get; set; } = string.Empty;
    public string? FranchiseeCity { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public string? RoyaltyPercentage { get; set; }
}
