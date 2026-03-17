using Microsoft.EntityFrameworkCore;
using Restify.Core.Application.Interfaces;
using Restify.Core.Domain.Entities;

namespace Restify.Core.Infrastructure.Persistence.Repositories;

public class GridColumnRepository : IGridColumnRepository
{
    private readonly CoreDbContext _context;

    public GridColumnRepository(CoreDbContext context)
    {
        _context = context;
    }

    public async Task<GridColumn?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.GridColumns
            .Include(x => x.Validations)
            .Include(x => x.Lookup)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<GridColumn>> GetByGridConfigurationIdAsync(Guid gridConfigurationId, CancellationToken cancellationToken = default)
    {
        return await _context.GridColumns
            .Where(x => x.GridConfigurationId == gridConfigurationId)
            .OrderBy(x => x.GridOrder)
            .Include(x => x.Validations.OrderBy(v => v.Order))
            .Include(x => x.Lookup)
            .ToListAsync(cancellationToken);
    }

    public async Task<GridColumn> AddAsync(GridColumn entity, CancellationToken cancellationToken = default)
    {
        await _context.GridColumns.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task AddRangeAsync(IEnumerable<GridColumn> entities, CancellationToken cancellationToken = default)
    {
        await _context.GridColumns.AddRangeAsync(entities, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(GridColumn entity, CancellationToken cancellationToken = default)
    {
        _context.GridColumns.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(GridColumn entity, CancellationToken cancellationToken = default)
    {
        _context.GridColumns.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteByGridConfigurationIdAsync(Guid gridConfigurationId, CancellationToken cancellationToken = default)
    {
        var columns = await _context.GridColumns
            .Where(x => x.GridConfigurationId == gridConfigurationId)
            .ToListAsync(cancellationToken);

        _context.GridColumns.RemoveRange(columns);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
