using Microsoft.EntityFrameworkCore;
using Restify.Auth.Application.Interfaces;
using Restify.Auth.Domain.Entities;

namespace Restify.Auth.Infrastructure.Persistence.Repositories;

public class GeneralValueRepository : IGeneralValueRepository
{
    private readonly AppDbContext _context;

    public GeneralValueRepository(AppDbContext context) => _context = context;

    public async Task<GeneralValue?> GetByKeyAsync(string key, CancellationToken ct = default)
        => await _context.GeneralValues
            .Include(v => v.Children)
            .FirstOrDefaultAsync(v => v.Key == key.ToUpperInvariant() && v.IsActive, ct);

    public async Task<IEnumerable<GeneralValue>> GetByCategoryAsync(string category, CancellationToken ct = default)
        => await _context.GeneralValues
            .Include(v => v.Children)
            .Where(v => v.Category == category.ToUpperInvariant() && v.IsActive)
            .OrderBy(v => v.SortOrder).ThenBy(v => v.Key)
            .ToListAsync(ct);

    public async Task<IEnumerable<GeneralValue>> GetRootValuesAsync(CancellationToken ct = default)
        => await _context.GeneralValues
            .Include(v => v.Children)
            .Where(v => v.ParentId == null && v.IsActive)
            .OrderBy(v => v.Category).ThenBy(v => v.SortOrder)
            .ToListAsync(ct);

    public async Task<IEnumerable<GeneralValue>> GetChildrenAsync(Guid parentId, CancellationToken ct = default)
        => await _context.GeneralValues
            .Where(v => v.ParentId == parentId && v.IsActive)
            .OrderBy(v => v.SortOrder)
            .ToListAsync(ct);

    public async Task<IEnumerable<GeneralValue>> GetAllActiveAsync(CancellationToken ct = default)
        => await _context.GeneralValues
            .Where(v => v.IsActive)
            .OrderBy(v => v.Category).ThenBy(v => v.SortOrder)
            .ToListAsync(ct);

    public async Task<string?> GetValueAsync(string key, string? defaultValue = null, CancellationToken ct = default)
    {
        var entity = await _context.GeneralValues
            .Where(v => v.Key == key.ToUpperInvariant() && v.IsActive)
            .Select(v => v.Value)
            .FirstOrDefaultAsync(ct);
        return entity ?? defaultValue;
    }

    public async Task<GeneralValue> CreateAsync(GeneralValue value, CancellationToken ct = default)
    {
        _context.GeneralValues.Add(value);
        await _context.SaveChangesAsync(ct);
        return value;
    }

    public async Task UpdateAsync(GeneralValue value, CancellationToken ct = default)
    {
        _context.GeneralValues.Update(value);
        await _context.SaveChangesAsync(ct);
    }
}
