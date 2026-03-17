using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class DeductionTypeRepository : IDeductionTypeRepository
{
    private readonly BackOfficeDbContext _context;

    public DeductionTypeRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<DeductionType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.DeductionTypes
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<DeductionType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DeductionTypes
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DeductionType>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DeductionTypes
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<DeductionType> AddAsync(DeductionType entity, CancellationToken cancellationToken = default)
    {
        await _context.DeductionTypes.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(DeductionType entity, CancellationToken cancellationToken = default)
    {
        _context.DeductionTypes.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(DeductionType entity, CancellationToken cancellationToken = default)
    {
        _context.DeductionTypes.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
