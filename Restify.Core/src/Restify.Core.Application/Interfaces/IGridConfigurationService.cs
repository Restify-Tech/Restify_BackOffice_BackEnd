using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.DTOs.Grid;

namespace Restify.Core.Application.Interfaces;

/// <summary>
/// Servicio para gestión de configuraciones de grid
/// </summary>
public interface IGridConfigurationService
{
    /// <summary>
    /// Obtiene la configuración completa de un grid por nombre de entidad
    /// </summary>
    Task<Result<GridConfigurationDto>> GetByEntityNameAsync(string entityName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene la configuración completa de un grid por ID
    /// </summary>
    Task<Result<GridConfigurationDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista todas las configuraciones de grid
    /// </summary>
    Task<Result<List<GridConfigurationDto>>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Crea una nueva configuración de grid
    /// </summary>
    Task<Result<GridConfigurationDto>> CreateAsync(CreateGridConfigurationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza una configuración de grid existente
    /// </summary>
    Task<Result<GridConfigurationDto>> UpdateAsync(UpdateGridConfigurationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina una configuración de grid
    /// </summary>
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega una columna a un grid
    /// </summary>
    Task<Result<GridColumnDto>> AddColumnAsync(CreateGridColumnRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza una columna
    /// </summary>
    Task<Result<GridColumnDto>> UpdateColumnAsync(Guid columnId, CreateGridColumnRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina una columna
    /// </summary>
    Task<Result<bool>> DeleteColumnAsync(Guid columnId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega una validación a una columna
    /// </summary>
    Task<Result<GridColumnValidationDto>> AddValidationAsync(CreateGridColumnValidationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina una validación
    /// </summary>
    Task<Result<bool>> DeleteValidationAsync(Guid validationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Configura el lookup de una columna
    /// </summary>
    Task<Result<GridColumnLookupDto>> SetLookupAsync(CreateGridColumnLookupRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina el lookup de una columna
    /// </summary>
    Task<Result<bool>> RemoveLookupAsync(Guid columnId, CancellationToken cancellationToken = default);
}
