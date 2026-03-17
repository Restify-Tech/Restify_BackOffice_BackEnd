using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class AccountingPeriodRepository : IAccountingPeriodRepository
{
    private readonly BackOfficeDbContext _context;

    public AccountingPeriodRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<AccountingPeriod?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AccountingPeriods
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<AccountingPeriod>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AccountingPeriods
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Month)
            .ToListAsync(cancellationToken);
    }

    public async Task<AccountingPeriod?> GetByYearMonthAsync(int year, int month, CancellationToken cancellationToken = default)
    {
        return await _context.AccountingPeriods
            .FirstOrDefaultAsync(x => x.Year == year && x.Month == month, cancellationToken);
    }

    public async Task<AccountingPeriod> AddAsync(AccountingPeriod entity, CancellationToken cancellationToken = default)
    {
        await _context.AccountingPeriods.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(AccountingPeriod entity, CancellationToken cancellationToken = default)
    {
        _context.AccountingPeriods.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
