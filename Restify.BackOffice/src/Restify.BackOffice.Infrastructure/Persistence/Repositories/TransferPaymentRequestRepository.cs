using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class TransferPaymentRequestRepository : ITransferPaymentRequestRepository
{
    private readonly BackOfficeDbContext _context;

    public TransferPaymentRequestRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<TransferPaymentRequest?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.TransferPaymentRequests
            .Include(t => t.Order)
            .FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public async Task<IEnumerable<TransferPaymentRequest>> GetPendingAsync(CancellationToken ct = default)
    {
        return await _context.TransferPaymentRequests
            .Include(t => t.Order)
            .Where(t => t.Status == TransferApprovalStatus.Pending)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TransferPaymentRequest>> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default)
    {
        return await _context.TransferPaymentRequests
            .Include(t => t.Order)
            .Where(t => t.OrderId == orderId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<TransferPaymentRequest> CreateAsync(TransferPaymentRequest entity, CancellationToken ct = default)
    {
        entity.CreatedAt = DateTime.UtcNow;
        await _context.TransferPaymentRequests.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task UpdateAsync(TransferPaymentRequest entity, CancellationToken ct = default)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.TransferPaymentRequests.Update(entity);
        await _context.SaveChangesAsync(ct);
    }
}
