using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.DTOs.General;

namespace Restify.Core.Application.Interfaces;

/// <summary>
/// Servicio para gestión de GeneralValues (valores de cada grupo)
/// </summary>
public interface IGeneralValueService
{
    /// <summary>
    /// Obtiene un valor por ID
    /// </summary>
    Task<Result<GeneralValueDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un valor por código de tabla y código de valor
    /// </summary>
    Task<Result<GeneralValueDto>> GetByCodeAsync(string tableCode, string valueCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista valores de una tabla por ID de tabla
    /// </summary>
    Task<Result<List<GeneralValueListDto>>> GetByTableIdAsync(Guid tableId, bool? activeOnly = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista valores de una tabla por código de tabla
    /// </summary>
    Task<Result<List<GeneralValueListDto>>> GetByTableCodeAsync(string tableCode, bool? activeOnly = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crea un nuevo valor
    /// </summary>
    Task<Result<GeneralValueDto>> CreateAsync(CreateGeneralValueRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza un valor existente
    /// </summary>
    Task<Result<GeneralValueDto>> UpdateAsync(UpdateGeneralValueRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina un valor (solo si no está bloqueado)
    /// </summary>
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Establece un valor como default (y quita el default de los demás)
    /// </summary>
    Task<Result<bool>> SetDefaultAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reordena los valores de una tabla
    /// </summary>
    Task<Result<bool>> ReorderAsync(Guid tableId, List<Guid> orderedIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista para lookup (id, name)
    /// </summary>
    Task<Result<List<LookupItem>>> GetLookupAsync(Guid tableId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista para lookup por código de tabla
    /// </summary>
    Task<Result<List<LookupItem>>> GetLookupByTableCodeAsync(string tableCode, CancellationToken cancellationToken = default);
}
