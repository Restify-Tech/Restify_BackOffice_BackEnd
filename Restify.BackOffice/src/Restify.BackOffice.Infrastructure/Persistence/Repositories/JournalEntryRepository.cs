using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class JournalEntryRepository : IJournalEntryRepository
{
    private readonly BackOfficeDbContext _context;

    public JournalEntryRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<JournalEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.JournalEntries
            .Include(x => x.Period)
            .Include(x => x.Lines)
                .ThenInclude(l => l.Account)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<JournalEntry>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.JournalEntries
            .Include(x => x.Period)
            .Include(x => x.Lines)
                .ThenInclude(l => l.Account)
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.EntryNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<JournalEntry>> GetByPeriodIdAsync(Guid periodId, CancellationToken cancellationToken = default)
    {
        return await _context.JournalEntries
            .Include(x => x.Period)
            .Include(x => x.Lines)
                .ThenInclude(l => l.Account)
            .Where(x => x.PeriodId == periodId)
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.EntryNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<string> GetNextEntryNumberAsync(CancellationToken cancellationToken = default)
    {
        var count = await _context.JournalEntries
            .CountAsync(cancellationToken);

        return $"AST-{(count + 1):D6}";
    }

    public async Task<JournalEntry> AddAsync(JournalEntry entity, CancellationToken cancellationToken = default)
    {
        await _context.JournalEntries.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(JournalEntry entity, CancellationToken cancellationToken = default)
    {
        _context.JournalEntries.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(JournalEntry entity, CancellationToken cancellationToken = default)
    {
        _context.JournalEntries.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
