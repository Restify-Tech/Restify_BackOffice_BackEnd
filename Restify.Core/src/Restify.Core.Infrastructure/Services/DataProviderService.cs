using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.DTOs.DataProvider;
using Restify.Core.Application.DTOs.Grid;
using Restify.Core.Application.Interfaces;

namespace Restify.Core.Infrastructure.Services;

/// <summary>
/// Implementación del servicio DataProvider para queries genéricos
/// </summary>
public class DataProviderService : IDataProviderService
{
    private readonly IEntityRegistry _entityRegistry;
    private readonly IGridConfigurationService _gridConfigService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DataProviderService> _logger;

    public DataProviderService(
        IEntityRegistry entityRegistry,
        IGridConfigurationService gridConfigService,
        IServiceProvider serviceProvider,
        ILogger<DataProviderService> logger)
    {
        _entityRegistry = entityRegistry;
        _gridConfigService = gridConfigService;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<Result<DataProviderQueryResponse>> QueryAsync(
        DataProviderQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var registration = _entityRegistry.GetRegistration(request.EntityName);
            if (registration == null)
            {
                return Result<DataProviderQueryResponse>.Failure($"Entidad '{request.EntityName}' no está registrada");
            }

            var dbContext = (DbContext)_serviceProvider.GetRequiredService(registration.DbContextType);
            var entityType = registration.EntityType;

            // Obtener DbSet usando reflection
            var dbSetMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), Type.EmptyTypes)!
                .MakeGenericMethod(entityType);
            var dbSet = dbSetMethod.Invoke(dbContext, null);

            // Crear query base
            var queryable = dbSet as IQueryable<object>;
            if (queryable == null)
            {
                return Result<DataProviderQueryResponse>.Failure("No se pudo crear el query");
            }

            // Aplicar filtros
            queryable = ApplyFilters(queryable, entityType, request.Filters, request.GlobalSearch);

            // Obtener total antes de paginar
            var totalCount = await queryable.CountAsync(cancellationToken);

            // Aplicar ordenamiento
            queryable = ApplyOrdering(queryable, entityType, request.SortField, request.SortDirection);

            // Aplicar paginación
            var skip = (request.Page - 1) * request.PageSize;
            queryable = queryable.Skip(skip).Take(request.PageSize);

            // Ejecutar query y convertir a diccionarios
            var entities = await queryable.ToListAsync(cancellationToken);
            var data = entities.Select(e => EntityToDictionary(e, entityType)).ToList();

            return Result<DataProviderQueryResponse>.Success(new DataProviderQueryResponse
            {
                Data = data,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar query para entidad {EntityName}", request.EntityName);
            return Result<DataProviderQueryResponse>.Failure($"Error al ejecutar query: {ex.Message}");
        }
    }

    public async Task<Result<DataProviderMetadataResponse>> GetMetadataAsync(
        DataProviderMetadataRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var registration = _entityRegistry.GetRegistration(request.EntityName);
            if (registration == null)
            {
                return Result<DataProviderMetadataResponse>.Failure($"Entidad '{request.EntityName}' no está registrada");
            }

            // Obtener configuración del grid
            var gridResult = await _gridConfigService.GetByEntityNameAsync(request.EntityName, cancellationToken);

            var response = new DataProviderMetadataResponse
            {
                EntityName = request.EntityName,
                ViewName = request.ViewName,
                Title = request.EntityName,
                GridConfiguration = gridResult.IsSuccess ? gridResult.Data : null,
                FilterFields = GetFilterFields(registration.EntityType)
            };

            return Result<DataProviderMetadataResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener metadata para entidad {EntityName}", request.EntityName);
            return Result<DataProviderMetadataResponse>.Failure($"Error al obtener metadata: {ex.Message}");
        }
    }

    public async Task<Result<Dictionary<string, object?>>> GetByIdAsync(
        string entityName,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var registration = _entityRegistry.GetRegistration(entityName);
            if (registration == null)
            {
                return Result<Dictionary<string, object?>>.Failure($"Entidad '{entityName}' no está registrada");
            }

            var dbContext = (DbContext)_serviceProvider.GetRequiredService(registration.DbContextType);
            var entity = await dbContext.FindAsync(registration.EntityType, [id], cancellationToken);

            if (entity == null)
            {
                return Result<Dictionary<string, object?>>.Failure($"Registro no encontrado");
            }

            return Result<Dictionary<string, object?>>.Success(EntityToDictionary(entity, registration.EntityType));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener registro {Id} de {EntityName}", id, entityName);
            return Result<Dictionary<string, object?>>.Failure($"Error: {ex.Message}");
        }
    }

    public async Task<Result<Dictionary<string, object?>>> CreateAsync(
        string entityName,
        Dictionary<string, object?> data,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var registration = _entityRegistry.GetRegistration(entityName);
            if (registration == null)
            {
                return Result<Dictionary<string, object?>>.Failure($"Entidad '{entityName}' no está registrada");
            }

            var dbContext = (DbContext)_serviceProvider.GetRequiredService(registration.DbContextType);
            var entity = Activator.CreateInstance(registration.EntityType)!;

            // Asignar valores desde el diccionario
            DictionaryToEntity(data, entity, registration.EntityType);

            // Asignar ID si no tiene
            var idProp = registration.EntityType.GetProperty("Id");
            if (idProp != null)
            {
                var currentId = (Guid?)idProp.GetValue(entity);
                if (currentId == null || currentId == Guid.Empty)
                {
                    idProp.SetValue(entity, Guid.NewGuid());
                }
            }

            dbContext.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Result<Dictionary<string, object?>>.Success(EntityToDictionary(entity, registration.EntityType));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear registro en {EntityName}", entityName);
            return Result<Dictionary<string, object?>>.Failure($"Error: {ex.Message}");
        }
    }

    public async Task<Result<Dictionary<string, object?>>> UpdateAsync(
        string entityName,
        Guid id,
        Dictionary<string, object?> data,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var registration = _entityRegistry.GetRegistration(entityName);
            if (registration == null)
            {
                return Result<Dictionary<string, object?>>.Failure($"Entidad '{entityName}' no está registrada");
            }

            var dbContext = (DbContext)_serviceProvider.GetRequiredService(registration.DbContextType);
            var entity = await dbContext.FindAsync(registration.EntityType, [id], cancellationToken);

            if (entity == null)
            {
                return Result<Dictionary<string, object?>>.Failure("Registro no encontrado");
            }

            // Actualizar valores
            DictionaryToEntity(data, entity, registration.EntityType);

            await dbContext.SaveChangesAsync(cancellationToken);

            return Result<Dictionary<string, object?>>.Success(EntityToDictionary(entity, registration.EntityType));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar registro {Id} en {EntityName}", id, entityName);
            return Result<Dictionary<string, object?>>.Failure($"Error: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteAsync(
        string entityName,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var registration = _entityRegistry.GetRegistration(entityName);
            if (registration == null)
            {
                return Result<bool>.Failure($"Entidad '{entityName}' no está registrada");
            }

            var dbContext = (DbContext)_serviceProvider.GetRequiredService(registration.DbContextType);
            var entity = await dbContext.FindAsync(registration.EntityType, [id], cancellationToken);

            if (entity == null)
            {
                return Result<bool>.Failure("Registro no encontrado");
            }

            dbContext.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar registro {Id} de {EntityName}", id, entityName);
            return Result<bool>.Failure($"Error: {ex.Message}");
        }
    }

    public IEnumerable<string> GetRegisteredEntities()
    {
        return _entityRegistry.GetRegisteredEntities();
    }

    #region Private Methods

    private static IQueryable<object> ApplyFilters(
        IQueryable<object> query,
        Type entityType,
        Dictionary<string, object?>? filters,
        string? globalSearch)
    {
        if (filters != null && filters.Count > 0)
        {
            foreach (var filter in filters)
            {
                var prop = entityType.GetProperty(filter.Key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop != null && filter.Value != null)
                {
                    var parameter = Expression.Parameter(typeof(object), "x");
                    var converted = Expression.Convert(parameter, entityType);
                    var property = Expression.Property(converted, prop);

                    Expression comparison;
                    var filterValue = ConvertValue(filter.Value, prop.PropertyType);

                    if (prop.PropertyType == typeof(string))
                    {
                        // Para strings, usar Contains
                        var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)])!;
                        var valueExpr = Expression.Constant(filterValue?.ToString() ?? "", typeof(string));
                        comparison = Expression.Call(property, containsMethod, valueExpr);
                    }
                    else
                    {
                        // Para otros tipos, usar Equal
                        var valueExpr = Expression.Constant(filterValue, prop.PropertyType);
                        comparison = Expression.Equal(property, valueExpr);
                    }

                    var lambda = Expression.Lambda<Func<object, bool>>(comparison, parameter);
                    query = query.Where(lambda);
                }
            }
        }

        // Global search en campos string
        if (!string.IsNullOrWhiteSpace(globalSearch))
        {
            var stringProperties = entityType.GetProperties()
                .Where(p => p.PropertyType == typeof(string) && p.CanRead)
                .ToList();

            if (stringProperties.Count > 0)
            {
                var parameter = Expression.Parameter(typeof(object), "x");
                var converted = Expression.Convert(parameter, entityType);
                Expression? orExpression = null;

                foreach (var prop in stringProperties)
                {
                    var property = Expression.Property(converted, prop);
                    var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)])!;
                    var searchValue = Expression.Constant(globalSearch, typeof(string));

                    // Manejar nulls
                    var notNull = Expression.NotEqual(property, Expression.Constant(null, typeof(string)));
                    var contains = Expression.Call(property, containsMethod, searchValue);
                    var condition = Expression.AndAlso(notNull, contains);

                    orExpression = orExpression == null
                        ? condition
                        : Expression.OrElse(orExpression, condition);
                }

                if (orExpression != null)
                {
                    var lambda = Expression.Lambda<Func<object, bool>>(orExpression, parameter);
                    query = query.Where(lambda);
                }
            }
        }

        return query;
    }

    private static IQueryable<object> ApplyOrdering(
        IQueryable<object> query,
        Type entityType,
        string? sortField,
        string sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortField))
        {
            // Ordenar por CreatedAt o Id por defecto
            sortField = entityType.GetProperty("CreatedAt") != null ? "CreatedAt" : "Id";
            sortDirection = "desc";
        }

        var prop = entityType.GetProperty(sortField, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (prop == null)
        {
            return query;
        }

        var parameter = Expression.Parameter(typeof(object), "x");
        var converted = Expression.Convert(parameter, entityType);
        var property = Expression.Property(converted, prop);
        var convertedProperty = Expression.Convert(property, typeof(object));
        var lambda = Expression.Lambda<Func<object, object>>(convertedProperty, parameter);

        return sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
            ? query.OrderByDescending(lambda)
            : query.OrderBy(lambda);
    }

    private static Dictionary<string, object?> EntityToDictionary(object entity, Type entityType)
    {
        var result = new Dictionary<string, object?>();
        var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && !IsNavigationProperty(p));

        foreach (var prop in properties)
        {
            var value = prop.GetValue(entity);
            result[ToCamelCase(prop.Name)] = value;
        }

        return result;
    }

    private static void DictionaryToEntity(Dictionary<string, object?> data, object entity, Type entityType)
    {
        foreach (var kvp in data)
        {
            // Buscar propiedad (case insensitive)
            var prop = entityType.GetProperty(kvp.Key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            // También intentar con PascalCase
            if (prop == null)
            {
                var pascalKey = ToPascalCase(kvp.Key);
                prop = entityType.GetProperty(pascalKey, BindingFlags.Public | BindingFlags.Instance);
            }

            if (prop != null && prop.CanWrite && !IsReadOnlyProperty(prop.Name))
            {
                var value = ConvertValue(kvp.Value, prop.PropertyType);
                prop.SetValue(entity, value);
            }
        }
    }

    private static object? ConvertValue(object? value, Type targetType)
    {
        if (value == null)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }

        // Manejar tipos nullable
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        // Si el valor ya es del tipo correcto
        if (underlyingType.IsAssignableFrom(value.GetType()))
        {
            return value;
        }

        // Manejar JsonElement
        if (value is JsonElement jsonElement)
        {
            return ConvertJsonElement(jsonElement, underlyingType);
        }

        // Conversión estándar
        try
        {
            if (underlyingType == typeof(Guid))
            {
                return Guid.Parse(value.ToString()!);
            }
            if (underlyingType == typeof(DateTime))
            {
                return DateTime.Parse(value.ToString()!);
            }
            if (underlyingType.IsEnum)
            {
                return Enum.Parse(underlyingType, value.ToString()!);
            }

            return Convert.ChangeType(value, underlyingType);
        }
        catch
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }
    }

    private static object? ConvertJsonElement(JsonElement element, Type targetType)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String when targetType == typeof(Guid) => Guid.Parse(element.GetString()!),
            JsonValueKind.String when targetType == typeof(DateTime) => element.GetDateTime(),
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number when targetType == typeof(int) => element.GetInt32(),
            JsonValueKind.Number when targetType == typeof(long) => element.GetInt64(),
            JsonValueKind.Number when targetType == typeof(decimal) => element.GetDecimal(),
            JsonValueKind.Number when targetType == typeof(double) => element.GetDouble(),
            JsonValueKind.Number when targetType == typeof(float) => element.GetSingle(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => element.ToString()
        };
    }

    private static List<FilterFieldInfo> GetFilterFields(Type entityType)
    {
        var fields = new List<FilterFieldInfo>();
        var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && !IsNavigationProperty(p) && !IsReadOnlyProperty(p.Name));

        foreach (var prop in properties)
        {
            var fieldType = GetFieldType(prop.PropertyType);
            if (fieldType != null)
            {
                fields.Add(new FilterFieldInfo
                {
                    Field = ToCamelCase(prop.Name),
                    Label = SplitCamelCase(prop.Name),
                    Type = fieldType
                });
            }
        }

        return fields;
    }

    private static string? GetFieldType(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        if (underlyingType == typeof(string)) return "text";
        if (underlyingType == typeof(int) || underlyingType == typeof(long) ||
            underlyingType == typeof(decimal) || underlyingType == typeof(double) ||
            underlyingType == typeof(float)) return "number";
        if (underlyingType == typeof(DateTime)) return "date";
        if (underlyingType == typeof(bool)) return "boolean";
        if (underlyingType == typeof(Guid)) return "text";
        if (underlyingType.IsEnum) return "select";

        return null;
    }

    private static bool IsNavigationProperty(PropertyInfo prop)
    {
        // Excluir propiedades de navegación (colecciones y referencias)
        if (prop.PropertyType.IsGenericType)
        {
            var genericType = prop.PropertyType.GetGenericTypeDefinition();
            if (genericType == typeof(ICollection<>) || genericType == typeof(List<>) ||
                genericType == typeof(IEnumerable<>) || genericType == typeof(IList<>))
            {
                return true;
            }
        }

        // Excluir referencias a otras entidades (clases que no sean tipos primitivos o conocidos)
        if (prop.PropertyType.IsClass && prop.PropertyType != typeof(string))
        {
            return true;
        }

        return false;
    }

    private static bool IsReadOnlyProperty(string propertyName)
    {
        var readOnlyProps = new[] { "Id", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "TenantId" };
        return readOnlyProps.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
    }

    private static string ToCamelCase(string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        return char.ToLowerInvariant(str[0]) + str[1..];
    }

    private static string ToPascalCase(string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        return char.ToUpperInvariant(str[0]) + str[1..];
    }

    private static string SplitCamelCase(string str)
    {
        return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? " " + x : x.ToString()));
    }

    #endregion
}
