using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly BackOfficeDbContext _context;

    public PaymentRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Include(p => p.Order)
            .Include(p => p.Invoice)
            .Include(p => p.Items)
                .ThenInclude(i => i.OrderItem)
                    .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Payment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Include(p => p.Order)
            .Include(p => p.Items)
                .ThenInclude(i => i.OrderItem)
                    .ThenInclude(oi => oi.Product)
            .Where(p => p.OrderId == orderId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Payment?> GetByGatewayTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Include(p => p.Order)
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.GatewayTransactionId == transactionId, cancellationToken);
    }

    public async Task<Payment> AddAsync(Payment entity, CancellationToken cancellationToken = default)
    {
        await _context.Payments.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return (await GetByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task UpdateAsync(Payment entity, CancellationToken cancellationToken = default)
    {
        _context.Payments.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
