using Microsoft.EntityFrameworkCore;
using Restify.Core.Application.Interfaces;
using Restify.Core.Domain.Entities;

namespace Restify.Core.Infrastructure.Persistence.Repositories;

public class GridColumnLookupRepository : IGridColumnLookupRepository
{
    private readonly CoreDbContext _context;

    public GridColumnLookupRepository(CoreDbContext context)
    {
        _context = context;
    }

    public async Task<GridColumnLookup?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.GridColumnLookups
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<GridColumnLookup?> GetByColumnIdAsync(Guid columnId, CancellationToken cancellationToken = default)
    {
        return await _context.GridColumnLookups
            .FirstOrDefaultAsync(x => x.GridColumnId == columnId, cancellationToken);
    }

    public async Task<GridColumnLookup> AddAsync(GridColumnLookup entity, CancellationToken cancellationToken = default)
    {
        await _context.GridColumnLookups.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(GridColumnLookup entity, CancellationToken cancellationToken = default)
    {
        _context.GridColumnLookups.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(GridColumnLookup entity, CancellationToken cancellationToken = default)
    {
        _context.GridColumnLookups.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteByColumnIdAsync(Guid columnId, CancellationToken cancellationToken = default)
    {
        var lookup = await _context.GridColumnLookups
            .FirstOrDefaultAsync(x => x.GridColumnId == columnId, cancellationToken);

        if (lookup != null)
        {
            _context.GridColumnLookups.Remove(lookup);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
