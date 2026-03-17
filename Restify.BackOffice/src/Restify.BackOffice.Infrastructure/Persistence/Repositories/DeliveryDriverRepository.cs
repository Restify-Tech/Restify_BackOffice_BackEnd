using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class DeliveryDriverRepository : IDeliveryDriverRepository
{
    private readonly BackOfficeDbContext _context;

    public DeliveryDriverRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<DeliveryDriver?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.DeliveryDrivers
            .Include(d => d.Cooperative)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<DeliveryDriver>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DeliveryDrivers
            .Include(d => d.Cooperative)
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<DeliveryDriver?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.DeliveryDrivers
            .Include(d => d.Cooperative)
            .FirstOrDefaultAsync(d => d.Email == email, cancellationToken);
    }

    public async Task<IReadOnlyList<DeliveryDriver>> GetByCooperativeIdAsync(Guid cooperativeId, CancellationToken cancellationToken = default)
    {
        return await _context.DeliveryDrivers
            .Include(d => d.Cooperative)
            .Where(d => d.CooperativeId == cooperativeId)
            .OrderBy(d => d.LastName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DeliveryDriver>> GetAvailableDriversAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DeliveryDrivers
            .Include(d => d.Cooperative)
            .Where(d => d.Status == DriverStatus.Available && d.IsActive && d.IsVerified)
            .OrderBy(d => d.LastName)
            .ToListAsync(cancellationToken);
    }

    public async Task<DeliveryDriver> AddAsync(DeliveryDriver entity, CancellationToken cancellationToken = default)
    {
        await _context.DeliveryDrivers.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return (await GetByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task UpdateAsync(DeliveryDriver entity, CancellationToken cancellationToken = default)
    {
        _context.DeliveryDrivers.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(DeliveryDriver entity, CancellationToken cancellationToken = default)
    {
        _context.DeliveryDrivers.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
