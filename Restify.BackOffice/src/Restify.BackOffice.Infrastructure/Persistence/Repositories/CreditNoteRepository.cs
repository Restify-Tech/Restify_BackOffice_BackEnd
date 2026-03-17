using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class CreditNoteRepository : ICreditNoteRepository
{
    private readonly BackOfficeDbContext _context;

    public CreditNoteRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<CreditNote?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.CreditNotes
            .Include(cn => cn.Items)
            .Include(cn => cn.Invoice)
            .FirstOrDefaultAsync(cn => cn.Id == id, ct);
    }

    public async Task<IReadOnlyList<CreditNote>> GetByInvoiceIdAsync(Guid invoiceId, CancellationToken ct = default)
    {
        return await _context.CreditNotes
            .Include(cn => cn.Items)
            .Where(cn => cn.InvoiceId == invoiceId)
            .OrderByDescending(cn => cn.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CreditNote>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.CreditNotes
            .Include(cn => cn.Items)
            .OrderByDescending(cn => cn.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<CreditNote> CreateAsync(CreditNote creditNote, CancellationToken ct = default)
    {
        _context.CreditNotes.Add(creditNote);
        await _context.SaveChangesAsync(ct);
        return creditNote;
    }

    public async Task UpdateAsync(CreditNote creditNote, CancellationToken ct = default)
    {
        _context.CreditNotes.Update(creditNote);
        await _context.SaveChangesAsync(ct);
    }
}
