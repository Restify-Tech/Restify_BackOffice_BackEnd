using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface IWithholdingVoucherRepository
{
    Task<WithholdingVoucher?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<WithholdingVoucher>> GetAllAsync(CancellationToken ct = default);
    Task<WithholdingVoucher> CreateAsync(WithholdingVoucher voucher, CancellationToken ct = default);
    Task UpdateAsync(WithholdingVoucher voucher, CancellationToken ct = default);
}
