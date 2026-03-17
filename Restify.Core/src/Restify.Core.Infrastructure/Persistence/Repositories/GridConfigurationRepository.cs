using Microsoft.EntityFrameworkCore;
using Restify.Core.Application.Interfaces;
using Restify.Core.Domain.Entities;

namespace Restify.Core.Infrastructure.Persistence.Repositories;

public class GridConfigurationRepository : IGridConfigurationRepository
{
    private readonly CoreDbContext _context;

    public GridConfigurationRepository(CoreDbContext context)
    {
        _context = context;
    }

    public async Task<GridConfiguration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.GridConfigurations
            .Include(x => x.Columns.OrderBy(c => c.GridOrder))
                .ThenInclude(c => c.Validations.OrderBy(v => v.Order))
            .Include(x => x.Columns)
                .ThenInclude(c => c.Lookup)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<GridConfiguration?> GetByEntityNameAsync(string entityName, CancellationToken cancellationToken = default)
    {
        return await _context.GridConfigurations
            .Include(x => x.Columns.OrderBy(c => c.GridOrder))
                .ThenInclude(c => c.Validations.OrderBy(v => v.Order))
            .Include(x => x.Columns)
                .ThenInclude(c => c.Lookup)
            .FirstOrDefaultAsync(x => x.EntityName == entityName, cancellationToken);
    }

    public async Task<IReadOnlyList<GridConfiguration>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.GridConfigurations
            .OrderBy(x => x.DisplayName)
            .ToListAsync(cancellationToken);
    }

    public async Task<GridConfiguration> AddAsync(GridConfiguration entity, CancellationToken cancellationToken = default)
    {
        await _context.GridConfigurations.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(GridConfiguration entity, CancellationToken cancellationToken = default)
    {
        _context.GridConfigurations.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(GridConfiguration entity, CancellationToken cancellationToken = default)
    {
        _context.GridConfigurations.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(string entityName, CancellationToken cancellationToken = default)
    {
        return await _context.GridConfigurations
            .AnyAsync(x => x.EntityName == entityName, cancellationToken);
    }
}
