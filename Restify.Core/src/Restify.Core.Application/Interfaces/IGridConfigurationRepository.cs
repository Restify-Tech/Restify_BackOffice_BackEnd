using Restify.Core.Domain.Entities;

namespace Restify.Core.Application.Interfaces;

/// <summary>
/// Repositorio para configuraciones de grid
/// </summary>
public interface IGridConfigurationRepository
{
    Task<GridConfiguration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GridConfiguration?> GetByEntityNameAsync(string entityName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GridConfiguration>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<GridConfiguration> AddAsync(GridConfiguration entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(GridConfiguration entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(GridConfiguration entity, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string entityName, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repositorio para columnas de grid
/// </summary>
public interface IGridColumnRepository
{
    Task<GridColumn?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GridColumn>> GetByGridConfigurationIdAsync(Guid gridConfigurationId, CancellationToken cancellationToken = default);
    Task<GridColumn> AddAsync(GridColumn entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<GridColumn> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(GridColumn entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(GridColumn entity, CancellationToken cancellationToken = default);
    Task DeleteByGridConfigurationIdAsync(Guid gridConfigurationId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repositorio para validaciones de columna
/// </summary>
public interface IGridColumnValidationRepository
{
    Task<GridColumnValidation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GridColumnValidation>> GetByColumnIdAsync(Guid columnId, CancellationToken cancellationToken = default);
    Task<GridColumnValidation> AddAsync(GridColumnValidation entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<GridColumnValidation> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(GridColumnValidation entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(GridColumnValidation entity, CancellationToken cancellationToken = default);
    Task DeleteByColumnIdAsync(Guid columnId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repositorio para lookups de columna
/// </summary>
public interface IGridColumnLookupRepository
{
    Task<GridColumnLookup?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GridColumnLookup?> GetByColumnIdAsync(Guid columnId, CancellationToken cancellationToken = default);
    Task<GridColumnLookup> AddAsync(GridColumnLookup entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(GridColumnLookup entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(GridColumnLookup entity, CancellationToken cancellationToken = default);
    Task DeleteByColumnIdAsync(Guid columnId, CancellationToken cancellationToken = default);
}
