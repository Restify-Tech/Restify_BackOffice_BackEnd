using Restify.Core.Domain.Entities;

namespace Restify.Core.Application.Interfaces;

/// <summary>
/// Repositorio para GeneralTable
/// </summary>
public interface IGeneralTableRepository
{
    Task<GeneralTable?> GetByIdAsync(Guid id, bool includeValues = false, CancellationToken cancellationToken = default);
    Task<GeneralTable?> GetByCodeAsync(string code, bool includeValues = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GeneralTable>> GetAllAsync(bool? activeOnly = true, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GeneralTable>> GetByApplicationCodeAsync(string applicationCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GeneralTable>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GeneralTable>> GetRootTablesAsync(string? applicationCode = null, CancellationToken cancellationToken = default);
    Task<GeneralTable> AddAsync(GeneralTable entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(GeneralTable entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(GeneralTable entity, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> HasChildrenAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> HasValuesAsync(Guid id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repositorio para GeneralValue
/// </summary>
public interface IGeneralValueRepository
{
    Task<GeneralValue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GeneralValue?> GetByCodeAsync(Guid tableId, string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GeneralValue>> GetByTableIdAsync(Guid tableId, bool? activeOnly = true, CancellationToken cancellationToken = default);
    Task<GeneralValue?> GetDefaultAsync(Guid tableId, CancellationToken cancellationToken = default);
    Task<GeneralValue> AddAsync(GeneralValue entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(GeneralValue entity, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<GeneralValue> entities, CancellationToken cancellationToken = default);
    Task DeleteAsync(GeneralValue entity, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid tableId, string code, CancellationToken cancellationToken = default);
    Task ClearDefaultAsync(Guid tableId, CancellationToken cancellationToken = default);
}
