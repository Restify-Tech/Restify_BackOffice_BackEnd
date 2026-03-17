using Microsoft.EntityFrameworkCore;
using Restify.Core.Application.Interfaces;
using Restify.Core.Domain.Entities;

namespace Restify.Core.Infrastructure.Persistence.Repositories;

public class GeneralTableRepository : IGeneralTableRepository
{
    private readonly CoreDbContext _context;

    public GeneralTableRepository(CoreDbContext context)
    {
        _context = context;
    }

    public async Task<GeneralTable?> GetByIdAsync(Guid id, bool includeValues = false, CancellationToken cancellationToken = default)
    {
        var query = _context.GeneralTables.AsQueryable();

        if (includeValues)
        {
            query = query.Include(x => x.Values.OrderBy(v => v.DisplayOrder));
        }

        return await query
            .Include(x => x.Children.OrderBy(c => c.DisplayOrder))
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<GeneralTable?> GetByCodeAsync(string code, bool includeValues = false, CancellationToken cancellationToken = default)
    {
        var query = _context.GeneralTables.AsQueryable();

        if (includeValues)
        {
            query = query.Include(x => x.Values.OrderBy(v => v.DisplayOrder));
        }

        return await query
            .Include(x => x.Children.OrderBy(c => c.DisplayOrder))
            .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    public async Task<IReadOnlyList<GeneralTable>> GetAllAsync(bool? activeOnly = true, CancellationToken cancellationToken = default)
    {
        var query = _context.GeneralTables.AsQueryable();

        if (activeOnly == true)
        {
            query = query.Where(x => x.IsActive);
        }

        return await query
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GeneralTable>> GetByApplicationCodeAsync(string applicationCode, CancellationToken cancellationToken = default)
    {
        return await _context.GeneralTables
            .Where(x => x.ApplicationCode == applicationCode && x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GeneralTable>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        return await _context.GeneralTables
            .Where(x => x.ParentId == parentId && x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GeneralTable>> GetRootTablesAsync(string? applicationCode = null, CancellationToken cancellationToken = default)
    {
        var query = _context.GeneralTables
            .Include(x => x.Children.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder))
            .Include(x => x.Values.Where(v => v.IsActive).OrderBy(v => v.DisplayOrder))
            .Where(x => x.ParentId == null && x.IsActive);

        if (!string.IsNullOrEmpty(applicationCode))
        {
            query = query.Where(x => x.ApplicationCode == applicationCode);
        }

        return await query
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<GeneralTable> AddAsync(GeneralTable entity, CancellationToken cancellationToken = default)
    {
        await _context.GeneralTables.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(GeneralTable entity, CancellationToken cancellationToken = default)
    {
        _context.GeneralTables.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(GeneralTable entity, CancellationToken cancellationToken = default)
    {
        _context.GeneralTables.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.GeneralTables
            .AnyAsync(x => x.Code == code, cancellationToken);
    }

    public async Task<bool> HasChildrenAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.GeneralTables
            .AnyAsync(x => x.ParentId == id, cancellationToken);
    }

    public async Task<bool> HasValuesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.GeneralValues
            .AnyAsync(x => x.GeneralTableId == id, cancellationToken);
    }
}
