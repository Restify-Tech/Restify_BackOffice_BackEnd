using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class AccountingAccountRepository : IAccountingAccountRepository
{
    private readonly BackOfficeDbContext _context;

    public AccountingAccountRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<AccountingAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AccountingAccounts
            .Include(x => x.Parent)
            .Include(x => x.Children)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<AccountingAccount>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AccountingAccounts
            .Include(x => x.Parent)
            .Include(x => x.Children)
            .OrderBy(x => x.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task<AccountingAccount?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.AccountingAccounts
            .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    public async Task<AccountingAccount> AddAsync(AccountingAccount entity, CancellationToken cancellationToken = default)
    {
        await _context.AccountingAccounts.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(AccountingAccount entity, CancellationToken cancellationToken = default)
    {
        _context.AccountingAccounts.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(AccountingAccount entity, CancellationToken cancellationToken = default)
    {
        _context.AccountingAccounts.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
