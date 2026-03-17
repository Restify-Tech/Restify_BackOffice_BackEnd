using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Persistence;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class QRCodeRepository : IQRCodeRepository
{
    private readonly BackOfficeDbContext _context;

    public QRCodeRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<QRCode?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.QRCodes
            .Include(q => q.Table)
            .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
    }

    public async Task<QRCode?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.QRCodes
            .IgnoreQueryFilters()
            .Include(q => q.Table)
            .FirstOrDefaultAsync(q => q.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<QRCode>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.QRCodes
            .Include(q => q.Table)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<QRCode>> GetByTableIdAsync(Guid tableId, CancellationToken cancellationToken = default)
    {
        return await _context.QRCodes
            .Include(q => q.Table)
            .Where(q => q.TableId == tableId)
            .ToListAsync(cancellationToken);
    }

    public async Task<QRCode> CreateAsync(QRCode qrCode, CancellationToken cancellationToken = default)
    {
        _context.QRCodes.Add(qrCode);
        await _context.SaveChangesAsync(cancellationToken);
        return qrCode;
    }

    public async Task<QRCode> UpdateAsync(QRCode qrCode, CancellationToken cancellationToken = default)
    {
        _context.QRCodes.Update(qrCode);
        await _context.SaveChangesAsync(cancellationToken);
        return qrCode;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var qrCode = await _context.QRCodes.FindAsync(new object[] { id }, cancellationToken);
        if (qrCode != null)
        {
            _context.QRCodes.Remove(qrCode);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.QRCodes
            .IgnoreQueryFilters()
            .AnyAsync(q => q.Code == code, cancellationToken);
    }
}
