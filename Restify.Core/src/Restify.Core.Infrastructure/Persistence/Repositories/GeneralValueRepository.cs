using Microsoft.EntityFrameworkCore;
using Restify.Core.Application.Interfaces;
using Restify.Core.Domain.Entities;

namespace Restify.Core.Infrastructure.Persistence.Repositories;

public class GeneralValueRepository : IGeneralValueRepository
{
    private readonly CoreDbContext _context;

    public GeneralValueRepository(CoreDbContext context)
    {
        _context = context;
    }

    public async Task<GeneralValue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.GeneralValues
            .Include(x => x.GeneralTable)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<GeneralValue?> GetByCodeAsync(Guid tableId, string code, CancellationToken cancellationToken = default)
    {
        return await _context.GeneralValues
            .Include(x => x.GeneralTable)
            .FirstOrDefaultAsync(x => x.GeneralTableId == tableId && x.Code == code, cancellationToken);
    }

    public async Task<IReadOnlyList<GeneralValue>> GetByTableIdAsync(Guid tableId, bool? activeOnly = true, CancellationToken cancellationToken = default)
    {
        var query = _context.GeneralValues
            .Include(x => x.GeneralTable)
            .Where(x => x.GeneralTableId == tableId);

        if (activeOnly == true)
        {
            query = query.Where(x => x.IsActive);
        }

        return await query
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Content)
            .ToListAsync(cancellationToken);
    }

    public async Task<GeneralValue?> GetDefaultAsync(Guid tableId, CancellationToken cancellationToken = default)
    {
        return await _context.GeneralValues
            .FirstOrDefaultAsync(x => x.GeneralTableId == tableId && x.IsDefault && x.IsActive, cancellationToken);
    }

    public async Task<GeneralValue> AddAsync(GeneralValue entity, CancellationToken cancellationToken = default)
    {
        await _context.GeneralValues.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(GeneralValue entity, CancellationToken cancellationToken = default)
    {
        _context.GeneralValues.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRangeAsync(IEnumerable<GeneralValue> entities, CancellationToken cancellationToken = default)
    {
        _context.GeneralValues.UpdateRange(entities);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(GeneralValue entity, CancellationToken cancellationToken = default)
    {
        _context.GeneralValues.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid tableId, string code, CancellationToken cancellationToken = default)
    {
        return await _context.GeneralValues
            .AnyAsync(x => x.GeneralTableId == tableId && x.Code == code, cancellationToken);
    }

    public async Task ClearDefaultAsync(Guid tableId, CancellationToken cancellationToken = default)
    {
        var defaults = await _context.GeneralValues
            .Where(x => x.GeneralTableId == tableId && x.IsDefault)
            .ToListAsync(cancellationToken);

        foreach (var item in defaults)
        {
            item.IsDefault = false;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
