using Microsoft.EntityFrameworkCore;
using Restify.Core.Application.Interfaces;
using Restify.Core.Domain.Entities;

namespace Restify.Core.Infrastructure.Persistence.Repositories;

public class GridColumnValidationRepository : IGridColumnValidationRepository
{
    private readonly CoreDbContext _context;

    public GridColumnValidationRepository(CoreDbContext context)
    {
        _context = context;
    }

    public async Task<GridColumnValidation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.GridColumnValidations
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<GridColumnValidation>> GetByColumnIdAsync(Guid columnId, CancellationToken cancellationToken = default)
    {
        return await _context.GridColumnValidations
            .Where(x => x.GridColumnId == columnId)
            .OrderBy(x => x.Order)
            .ToListAsync(cancellationToken);
    }

    public async Task<GridColumnValidation> AddAsync(GridColumnValidation entity, CancellationToken cancellationToken = default)
    {
        await _context.GridColumnValidations.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task AddRangeAsync(IEnumerable<GridColumnValidation> entities, CancellationToken cancellationToken = default)
    {
        await _context.GridColumnValidations.AddRangeAsync(entities, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(GridColumnValidation entity, CancellationToken cancellationToken = default)
    {
        _context.GridColumnValidations.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(GridColumnValidation entity, CancellationToken cancellationToken = default)
    {
        _context.GridColumnValidations.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteByColumnIdAsync(Guid columnId, CancellationToken cancellationToken = default)
    {
        var validations = await _context.GridColumnValidations
            .Where(x => x.GridColumnId == columnId)
            .ToListAsync(cancellationToken);

        _context.GridColumnValidations.RemoveRange(validations);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
