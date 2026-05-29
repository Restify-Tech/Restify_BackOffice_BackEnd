using Restify.Auth.Domain.Entities;

namespace Restify.Auth.Application.Interfaces;

public interface IGeneralValueRepository
{
    Task<GeneralValue?> GetByKeyAsync(string key, CancellationToken ct = default);
    Task<IEnumerable<GeneralValue>> GetByCategoryAsync(string category, CancellationToken ct = default);
    Task<IEnumerable<GeneralValue>> GetRootValuesAsync(CancellationToken ct = default);
    Task<IEnumerable<GeneralValue>> GetChildrenAsync(Guid parentId, CancellationToken ct = default);
    Task<IEnumerable<GeneralValue>> GetAllActiveAsync(CancellationToken ct = default);
    Task<GeneralValue> CreateAsync(GeneralValue value, CancellationToken ct = default);
    Task UpdateAsync(GeneralValue value, CancellationToken ct = default);
    Task<string?> GetValueAsync(string key, string? defaultValue = null, CancellationToken ct = default);
}
