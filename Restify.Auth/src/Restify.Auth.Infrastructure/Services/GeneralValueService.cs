using Restify.Auth.Application.Interfaces;
using Restify.Auth.Domain.Entities;

namespace Restify.Auth.Infrastructure.Services;

public class GeneralValueService : IGeneralValueService
{
    private readonly IGeneralValueRepository _repo;

    public GeneralValueService(IGeneralValueRepository repo) => _repo = repo;

    public async Task<string?> GetAsync(string key, string? defaultValue = null, CancellationToken ct = default)
        => await _repo.GetValueAsync(key, defaultValue, ct);

    public async Task<decimal> GetDecimalAsync(string key, decimal defaultValue = 0m, CancellationToken ct = default)
    {
        var val = await _repo.GetValueAsync(key, null, ct);
        return decimal.TryParse(val, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var d) ? d : defaultValue;
    }

    public async Task<int> GetIntAsync(string key, int defaultValue = 0, CancellationToken ct = default)
    {
        var val = await _repo.GetValueAsync(key, null, ct);
        return int.TryParse(val, out var i) ? i : defaultValue;
    }

    public async Task<bool> GetBoolAsync(string key, bool defaultValue = false, CancellationToken ct = default)
    {
        var val = await _repo.GetValueAsync(key, null, ct);
        return bool.TryParse(val, out var b) ? b : defaultValue;
    }

    public async Task<T?> GetJsonAsync<T>(string key, CancellationToken ct = default) where T : class
    {
        var val = await _repo.GetValueAsync(key, null, ct);
        if (string.IsNullOrWhiteSpace(val)) return null;
        return System.Text.Json.JsonSerializer.Deserialize<T>(val);
    }

    public async Task<IEnumerable<GeneralValueDto>> GetTreeAsync(string? category = null, CancellationToken ct = default)
    {
        var roots = category is null
            ? await _repo.GetRootValuesAsync(ct)
            : await _repo.GetByCategoryAsync(category, ct);
        return roots.Select(MapToDto);
    }

    public async Task<GeneralValueDto> CreateAsync(CreateGeneralValueRequest request, CancellationToken ct = default)
    {
        var entity = GeneralValue.Create(
            request.Key, request.DisplayName, request.Value,
            Enum.Parse<GeneralValueType>(request.ValueType, true),
            request.Category, request.Description, request.ParentId,
            request.IsEditable, request.SortOrder);
        await _repo.CreateAsync(entity, ct);
        return MapToDto(entity);
    }

    public async Task UpdateValueAsync(string key, string? newValue, CancellationToken ct = default)
    {
        var entity = await _repo.GetByKeyAsync(key, ct)
            ?? throw new KeyNotFoundException($"GeneralValue '{key}' no encontrado.");
        entity.UpdateValue(newValue);
        await _repo.UpdateAsync(entity, ct);
    }

    private static GeneralValueDto MapToDto(GeneralValue v) => new(
        v.Id, v.ParentId, v.Key, v.DisplayName, v.Value,
        v.ValueType.ToString(), v.Category, v.Description,
        v.IsActive, v.IsEditable, v.SortOrder,
        v.Children.Where(c => c.IsActive).Select(MapToDto));
}
