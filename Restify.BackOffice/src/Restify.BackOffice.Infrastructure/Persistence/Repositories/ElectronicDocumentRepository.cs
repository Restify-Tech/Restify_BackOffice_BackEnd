using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class ElectronicDocumentRepository : IElectronicDocumentRepository
{
    private readonly BackOfficeDbContext _context;

    public ElectronicDocumentRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<ElectronicDocument?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.ElectronicDocuments
            .Include(d => d.Invoice)
            .Include(d => d.CreditNote)
            .Include(d => d.WithholdingVoucher)
            .FirstOrDefaultAsync(d => d.Id == id, ct);
    }

    public async Task<ElectronicDocument?> GetByAccessKeyAsync(string accessKey, CancellationToken ct = default)
    {
        return await _context.ElectronicDocuments
            .FirstOrDefaultAsync(d => d.AccessKey == accessKey, ct);
    }

    public async Task<ElectronicDocument?> GetByInvoiceIdAsync(Guid invoiceId, CancellationToken ct = default)
    {
        return await _context.ElectronicDocuments
            .FirstOrDefaultAsync(d => d.InvoiceId == invoiceId, ct);
    }

    public async Task<IReadOnlyList<ElectronicDocument>> GetPendingAuthorizationAsync(CancellationToken ct = default)
    {
        return await _context.ElectronicDocuments
            .Where(d => d.Status == ElectronicDocumentStatus.Sent || d.Status == ElectronicDocumentStatus.Received)
            .OrderBy(d => d.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ElectronicDocument>> GetByFilterAsync(ElectronicDocumentFilter filter, CancellationToken ct = default)
    {
        var query = _context.ElectronicDocuments.AsQueryable();

        if (filter.DocumentType.HasValue)
            query = query.Where(d => d.DocumentType == filter.DocumentType.Value);

        if (filter.Status.HasValue)
            query = query.Where(d => d.Status == filter.Status.Value);

        if (filter.From.HasValue)
            query = query.Where(d => d.CreatedAt >= filter.From.Value);

        if (filter.To.HasValue)
            query = query.Where(d => d.CreatedAt <= filter.To.Value);

        if (!string.IsNullOrEmpty(filter.AccessKey))
            query = query.Where(d => d.AccessKey.Contains(filter.AccessKey));

        return await query.OrderByDescending(d => d.CreatedAt).ToListAsync(ct);
    }

    public async Task<ElectronicDocument> CreateAsync(ElectronicDocument document, CancellationToken ct = default)
    {
        _context.ElectronicDocuments.Add(document);
        await _context.SaveChangesAsync(ct);
        return document;
    }

    public async Task UpdateAsync(ElectronicDocument document, CancellationToken ct = default)
    {
        _context.ElectronicDocuments.Update(document);
        await _context.SaveChangesAsync(ct);
    }
}
