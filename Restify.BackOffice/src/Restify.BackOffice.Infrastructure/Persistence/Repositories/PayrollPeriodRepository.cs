using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class PayrollPeriodRepository : IPayrollPeriodRepository
{
    private readonly BackOfficeDbContext _context;

    public PayrollPeriodRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<PayrollPeriod?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.PayrollPeriods
            .Include(p => p.Entries)
                .ThenInclude(e => e.Employee)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<PayrollPeriod>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PayrollPeriods
            .OrderByDescending(p => p.Year)
            .ThenByDescending(p => p.Month)
            .ToListAsync(cancellationToken);
    }

    public async Task<PayrollPeriod> AddAsync(PayrollPeriod entity, CancellationToken cancellationToken = default)
    {
        await _context.PayrollPeriods.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(PayrollPeriod entity, CancellationToken cancellationToken = default)
    {
        _context.PayrollPeriods.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
