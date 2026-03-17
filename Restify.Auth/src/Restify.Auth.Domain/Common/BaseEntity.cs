namespace Restify.Auth.Domain.Common;

/// <summary>
/// Entidad base para todas las entidades del dominio
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
    }
}
