using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class DispatchRepository : IDispatchRepository
{
    private readonly BackOfficeDbContext _context;

    public DispatchRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<DispatchApproval?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.DispatchApprovals
            .Include(d => d.Order)
                .ThenInclude(o => o.Table)
            .Include(d => d.Order)
                .ThenInclude(o => o.Items)
                    .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<DispatchApproval?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _context.DispatchApprovals
            .Include(d => d.Order)
                .ThenInclude(o => o.Table)
            .FirstOrDefaultAsync(d => d.OrderId == orderId, cancellationToken);
    }

    public async Task<IReadOnlyList<DispatchApproval>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DispatchApprovals
            .Include(d => d.Order)
                .ThenInclude(o => o.Table)
            .Include(d => d.Order)
                .ThenInclude(o => o.Items)
            .Where(d => d.Status == DispatchApprovalStatus.Pending)
            .OrderBy(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<DispatchApproval> AddAsync(DispatchApproval entity, CancellationToken cancellationToken = default)
    {
        await _context.DispatchApprovals.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return (await GetByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task UpdateAsync(DispatchApproval entity, CancellationToken cancellationToken = default)
    {
        _context.DispatchApprovals.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
