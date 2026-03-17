namespace Restify.Core.Application.Interfaces;

/// <summary>
/// Registro de entidades disponibles para el DataProvider
/// </summary>
public interface IEntityRegistry
{
    /// <summary>
    /// Registra una entidad en el registry
    /// </summary>
    void Register(string entityName, Type entityType, Type dbContextType);

    /// <summary>
    /// Registra una entidad en el registry usando genéricos
    /// </summary>
    void Register<TEntity, TDbContext>(string entityName)
        where TEntity : class;

    /// <summary>
    /// Obtiene la información de una entidad registrada
    /// </summary>
    EntityRegistration? GetRegistration(string entityName);

    /// <summary>
    /// Obtiene todas las entidades registradas
    /// </summary>
    IEnumerable<string> GetRegisteredEntities();

    /// <summary>
    /// Verifica si una entidad está registrada
    /// </summary>
    bool IsRegistered(string entityName);
}

/// <summary>
/// Información de registro de una entidad
/// </summary>
public class EntityRegistration
{
    public string EntityName { get; set; } = string.Empty;
    public Type EntityType { get; set; } = null!;
    public Type DbContextType { get; set; } = null!;
}
