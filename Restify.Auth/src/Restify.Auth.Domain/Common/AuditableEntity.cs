namespace Restify.Auth.Domain.Common;

/// <summary>
/// Entidad con campos de auditoría
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    protected AuditableEntity() : base()
    {
        CreatedAt = DateTime.UtcNow;
    }
}
