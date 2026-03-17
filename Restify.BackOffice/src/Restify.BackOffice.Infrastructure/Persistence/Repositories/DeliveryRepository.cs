using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class DeliveryRepository : IDeliveryRepository
{
    private readonly BackOfficeDbContext _context;

    public DeliveryRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<Delivery?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Deliveries
            .Include(d => d.Order)
            .Include(d => d.Driver)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<Delivery?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _context.Deliveries
            .Include(d => d.Order)
            .Include(d => d.Driver)
            .FirstOrDefaultAsync(d => d.OrderId == orderId, cancellationToken);
    }

    public async Task<IReadOnlyList<Delivery>> GetByDriverIdAsync(Guid driverId, CancellationToken cancellationToken = default)
    {
        return await _context.Deliveries
            .Include(d => d.Order)
            .Include(d => d.Driver)
            .Where(d => d.DriverId == driverId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Delivery>> GetByStatusAsync(DeliveryStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Deliveries
            .Include(d => d.Order)
            .Include(d => d.Driver)
            .Where(d => d.Status == status)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Delivery>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Deliveries
            .Include(d => d.Order)
            .Include(d => d.Driver)
            .Where(d => d.Status == DeliveryStatus.Pending)
            .OrderBy(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Delivery?> GetActiveByDriverAsync(Guid driverId, CancellationToken cancellationToken = default)
    {
        return await _context.Deliveries
            .Include(d => d.Order)
            .Include(d => d.Driver)
            .FirstOrDefaultAsync(d => d.DriverId == driverId
                && d.Status != DeliveryStatus.Delivered
                && d.Status != DeliveryStatus.Failed
                && d.Status != DeliveryStatus.Cancelled, cancellationToken);
    }

    public async Task<IReadOnlyList<Delivery>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Deliveries
            .Include(d => d.Order)
            .Include(d => d.Driver)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Delivery> AddAsync(Delivery entity, CancellationToken cancellationToken = default)
    {
        await _context.Deliveries.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return (await GetByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task UpdateAsync(Delivery entity, CancellationToken cancellationToken = default)
    {
        _context.Deliveries.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
