using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class DeliveryCooperativeRepository : IDeliveryCooperativeRepository
{
    private readonly BackOfficeDbContext _context;

    public DeliveryCooperativeRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<DeliveryCooperative?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.DeliveryCooperatives
            .Include(c => c.Drivers)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<DeliveryCooperative>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DeliveryCooperatives
            .Include(c => c.Drivers)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<DeliveryCooperative?> GetByRucAsync(string ruc, CancellationToken cancellationToken = default)
    {
        return await _context.DeliveryCooperatives
            .FirstOrDefaultAsync(c => c.Ruc == ruc, cancellationToken);
    }

    public async Task<DeliveryCooperative> AddAsync(DeliveryCooperative entity, CancellationToken cancellationToken = default)
    {
        await _context.DeliveryCooperatives.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return (await GetByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task UpdateAsync(DeliveryCooperative entity, CancellationToken cancellationToken = default)
    {
        _context.DeliveryCooperatives.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(DeliveryCooperative entity, CancellationToken cancellationToken = default)
    {
        _context.DeliveryCooperatives.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
