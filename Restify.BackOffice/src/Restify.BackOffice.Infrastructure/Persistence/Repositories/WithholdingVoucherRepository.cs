using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class WithholdingVoucherRepository : IWithholdingVoucherRepository
{
    private readonly BackOfficeDbContext _context;

    public WithholdingVoucherRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<WithholdingVoucher?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.WithholdingVouchers
            .Include(wv => wv.Details)
            .FirstOrDefaultAsync(wv => wv.Id == id, ct);
    }

    public async Task<IReadOnlyList<WithholdingVoucher>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.WithholdingVouchers
            .Include(wv => wv.Details)
            .OrderByDescending(wv => wv.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<WithholdingVoucher> CreateAsync(WithholdingVoucher voucher, CancellationToken ct = default)
    {
        _context.WithholdingVouchers.Add(voucher);
        await _context.SaveChangesAsync(ct);
        return voucher;
    }

    public async Task UpdateAsync(WithholdingVoucher voucher, CancellationToken ct = default)
    {
        _context.WithholdingVouchers.Update(voucher);
        await _context.SaveChangesAsync(ct);
    }
}
