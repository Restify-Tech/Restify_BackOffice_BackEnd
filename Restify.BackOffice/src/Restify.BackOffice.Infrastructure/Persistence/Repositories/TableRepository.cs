using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class TableRepository : ITableRepository
{
    private readonly BackOfficeDbContext _context;

    public TableRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<Table?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tables
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Table?> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        return await _context.Tables
            .FirstOrDefaultAsync(t => t.Number == number, cancellationToken);
    }

    public async Task<IEnumerable<Table>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tables
            .OrderBy(t => t.Zone)
            .ThenBy(t => t.Number)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Table>> GetByStatusAsync(TableStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Tables
            .Where(t => t.Status == status)
            .OrderBy(t => t.Number)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Table>> GetByZoneAsync(string zone, CancellationToken cancellationToken = default)
    {
        return await _context.Tables
            .Where(t => t.Zone == zone)
            .OrderBy(t => t.Number)
            .ToListAsync(cancellationToken);
    }

    public async Task<Table> CreateAsync(Table table, CancellationToken cancellationToken = default)
    {
        await _context.Tables.AddAsync(table, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return (await GetByIdAsync(table.Id, cancellationToken))!;
    }

    public async Task<Table> UpdateAsync(Table table, CancellationToken cancellationToken = default)
    {
        _context.Tables.Update(table);
        await _context.SaveChangesAsync(cancellationToken);
        
        return (await GetByIdAsync(table.Id, cancellationToken))!;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var table = await _context.Tables.FindAsync(new object[] { id }, cancellationToken);
        if (table != null)
        {
            _context.Tables.Remove(table);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<string>> GetZonesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tables
            .Where(t => !string.IsNullOrEmpty(t.Zone))
            .Select(t => t.Zone!)
            .Distinct()
            .OrderBy(z => z)
            .ToListAsync(cancellationToken);
    }
}
