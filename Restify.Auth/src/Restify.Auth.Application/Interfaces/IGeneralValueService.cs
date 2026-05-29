namespace Restify.Auth.Application.Interfaces;

public interface IGeneralValueService
{
    Task<string?> GetAsync(string key, string? defaultValue = null, CancellationToken ct = default);
    Task<decimal> GetDecimalAsync(string key, decimal defaultValue = 0m, CancellationToken ct = default);
    Task<int> GetIntAsync(string key, int defaultValue = 0, CancellationToken ct = default);
    Task<bool> GetBoolAsync(string key, bool defaultValue = false, CancellationToken ct = default);
    Task<T?> GetJsonAsync<T>(string key, CancellationToken ct = default) where T : class;
    Task<IEnumerable<GeneralValueDto>> GetTreeAsync(string? category = null, CancellationToken ct = default);
    Task<GeneralValueDto> CreateAsync(CreateGeneralValueRequest request, CancellationToken ct = default);
    Task UpdateValueAsync(string key, string? newValue, CancellationToken ct = default);
}

public record GeneralValueDto(
    Guid Id,
    Guid? ParentId,
    string Key,
    string DisplayName,
    string? Value,
    string ValueType,
    string Category,
    string? Description,
    bool IsActive,
    bool IsEditable,
    int SortOrder,
    IEnumerable<GeneralValueDto> Children);

public record CreateGeneralValueRequest(
    string Key,
    string DisplayName,
    string? Value,
    string ValueType,
    string Category,
    string? Description,
    Guid? ParentId,
    bool IsEditable = true,
    int SortOrder = 0);
