using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.DTOs.General;

namespace Restify.Core.Application.Interfaces;

/// <summary>
/// Servicio para gestión de GeneralTables (catálogo de grupos)
/// </summary>
public interface IGeneralTableService
{
    /// <summary>
    /// Obtiene una tabla por su código
    /// </summary>
    Task<Result<GeneralTableDto>> GetByCodeAsync(string code, bool includeValues = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una tabla por ID
    /// </summary>
    Task<Result<GeneralTableDto>> GetByIdAsync(Guid id, bool includeValues = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista todas las tablas
    /// </summary>
    Task<Result<List<GeneralTableListDto>>> GetAllAsync(bool? activeOnly = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista tablas por código de aplicación
    /// </summary>
    Task<Result<List<GeneralTableListDto>>> GetByApplicationCodeAsync(string applicationCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista tablas hijas de una tabla padre
    /// </summary>
    Task<Result<List<GeneralTableListDto>>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el árbol jerárquico de tablas
    /// </summary>
    Task<Result<List<GeneralTableDto>>> GetTreeAsync(string? applicationCode = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crea una nueva tabla
    /// </summary>
    Task<Result<GeneralTableDto>> CreateAsync(CreateGeneralTableRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza una tabla existente
    /// </summary>
    Task<Result<GeneralTableDto>> UpdateAsync(UpdateGeneralTableRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina una tabla (solo si no tiene valores ni hijos)
    /// </summary>
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista para lookup (id, name)
    /// </summary>
    Task<Result<List<LookupItem>>> GetLookupAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Item simple para lookups
/// </summary>
public class LookupItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
}
