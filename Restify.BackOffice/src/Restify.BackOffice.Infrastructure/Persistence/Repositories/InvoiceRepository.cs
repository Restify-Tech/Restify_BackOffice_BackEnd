using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly BackOfficeDbContext _context;

    public InvoiceRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .Include(i => i.Order)
                .ThenInclude(o => o.Table)
            .Include(i => i.Items)
                .ThenInclude(it => it.Modifiers)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .Include(i => i.Order)
                .ThenInclude(o => o.Table)
            .Include(i => i.Items)
                .ThenInclude(it => it.Modifiers)
            .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber, cancellationToken);
    }

    public async Task<Invoice?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .Include(i => i.Order)
                .ThenInclude(o => o.Table)
            .Include(i => i.Items)
                .ThenInclude(it => it.Modifiers)
            .FirstOrDefaultAsync(i => i.OrderId == orderId, cancellationToken);
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .Include(i => i.Order)
                .ThenInclude(o => o.Table)
            .Include(i => i.Items)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Invoice>> GetByStatusAsync(InvoiceStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .Include(i => i.Order)
                .ThenInclude(o => o.Table)
            .Include(i => i.Items)
            .Where(i => i.Status == status)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Invoice>> GetByPaymentMethodAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .Include(i => i.Order)
                .ThenInclude(o => o.Table)
            .Include(i => i.Items)
            .Where(i => i.PaymentMethod == paymentMethod)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .Include(i => i.Order)
                .ThenInclude(o => o.Table)
            .Include(i => i.Items)
            .Where(i => i.CreatedAt >= from && i.CreatedAt <= to)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Invoice> CreateAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        await _context.Invoices.AddAsync(invoice, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(invoice.Id, cancellationToken))!;
    }

    public async Task<Invoice> UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        _context.Invoices.Update(invoice);
        await _context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(invoice.Id, cancellationToken))!;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var invoice = await _context.Invoices.FindAsync(new object[] { id }, cancellationToken);
        if (invoice == null)
            return false;

        _context.Invoices.Remove(invoice);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<string> GenerateInvoiceNumberAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var todayInvoicesCount = await _context.Invoices
            .Where(i => i.CreatedAt >= today && i.CreatedAt < tomorrow)
            .CountAsync(cancellationToken);

        var invoiceNumber = $"INV-{today:yyyyMMdd}-{(todayInvoicesCount + 1):D4}";

        return invoiceNumber;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Invoices.AnyAsync(i => i.Id == id, cancellationToken);
    }
}
