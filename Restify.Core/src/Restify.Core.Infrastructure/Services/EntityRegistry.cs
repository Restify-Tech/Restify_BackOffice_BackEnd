using Restify.Core.Application.Interfaces;

namespace Restify.Core.Infrastructure.Services;

/// <summary>
/// Implementación del registro de entidades para el DataProvider
/// </summary>
public class EntityRegistry : IEntityRegistry
{
    private readonly Dictionary<string, EntityRegistration> _registrations = new(StringComparer.OrdinalIgnoreCase);

    public void Register(string entityName, Type entityType, Type dbContextType)
    {
        _registrations[entityName] = new EntityRegistration
        {
            EntityName = entityName,
            EntityType = entityType,
            DbContextType = dbContextType
        };
    }

    public void Register<TEntity, TDbContext>(string entityName)
        where TEntity : class
    {
        Register(entityName, typeof(TEntity), typeof(TDbContext));
    }

    public EntityRegistration? GetRegistration(string entityName)
    {
        return _registrations.TryGetValue(entityName, out var registration) ? registration : null;
    }

    public IEnumerable<string> GetRegisteredEntities()
    {
        return _registrations.Keys;
    }

    public bool IsRegistered(string entityName)
    {
        return _registrations.ContainsKey(entityName);
    }
}
