namespace Restify.Auth.Domain.Exceptions;

/// <summary>
/// Excepción cuando no se encuentra una entidad
/// </summary>
public class EntityNotFoundException : DomainException
{
    public string EntityName { get; }
    public object EntityId { get; }

    public EntityNotFoundException(string entityName, object entityId)
        : base($"{entityName} con ID '{entityId}' no fue encontrado.", "ENTITY_NOT_FOUND")
    {
        EntityName = entityName;
        EntityId = entityId;
    }
}
