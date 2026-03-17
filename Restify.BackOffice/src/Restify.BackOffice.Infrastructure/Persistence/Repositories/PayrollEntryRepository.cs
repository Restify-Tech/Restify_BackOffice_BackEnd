using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class PayrollEntryRepository : IPayrollEntryRepository
{
    private readonly BackOfficeDbContext _context;

    public PayrollEntryRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<PayrollEntry>> GetByPeriodIdAsync(Guid periodId, CancellationToken cancellationToken = default)
    {
        return await _context.PayrollEntries
            .Include(e => e.Employee)
            .Where(e => e.PayrollPeriodId == periodId)
            .ToListAsync(cancellationToken);
    }

    public async Task CreateRangeAsync(IEnumerable<PayrollEntry> entries, CancellationToken cancellationToken = default)
    {
        await _context.PayrollEntries.AddRangeAsync(entries, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteByPeriodIdAsync(Guid periodId, CancellationToken cancellationToken = default)
    {
        var entries = await _context.PayrollEntries
            .Where(e => e.PayrollPeriodId == periodId)
            .ToListAsync(cancellationToken);

        _context.PayrollEntries.RemoveRange(entries);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
