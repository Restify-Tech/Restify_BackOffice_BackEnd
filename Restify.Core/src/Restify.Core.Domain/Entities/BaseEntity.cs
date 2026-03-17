namespace Restify.Core.Domain.Entities;

/// <summary>
/// Entidad base con campos comunes de auditoría
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

/// <summary>
/// Entidad base con soporte multi-tenant
/// </summary>
public abstract class TenantEntity : BaseEntity
{
    public Guid TenantId { get; set; }
}
