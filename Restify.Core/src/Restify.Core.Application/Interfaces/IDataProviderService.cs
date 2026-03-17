using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.DTOs.DataProvider;

namespace Restify.Core.Application.Interfaces;

/// <summary>
/// Servicio genérico de proveedor de datos para entidades
/// </summary>
public interface IDataProviderService
{
    /// <summary>
    /// Ejecuta una consulta genérica sobre una entidad
    /// </summary>
    Task<Result<DataProviderQueryResponse>> QueryAsync(
        DataProviderQueryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene la metadata de una entidad (configuración del grid, campos, etc.)
    /// </summary>
    Task<Result<DataProviderMetadataResponse>> GetMetadataAsync(
        DataProviderMetadataRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un registro por ID
    /// </summary>
    Task<Result<Dictionary<string, object?>>> GetByIdAsync(
        string entityName,
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Crea un nuevo registro
    /// </summary>
    Task<Result<Dictionary<string, object?>>> CreateAsync(
        string entityName,
        Dictionary<string, object?> data,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza un registro existente
    /// </summary>
    Task<Result<Dictionary<string, object?>>> UpdateAsync(
        string entityName,
        Guid id,
        Dictionary<string, object?> data,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina un registro
    /// </summary>
    Task<Result<bool>> DeleteAsync(
        string entityName,
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene las entidades registradas disponibles
    /// </summary>
    IEnumerable<string> GetRegisteredEntities();
}
