using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface ITransferPaymentRequestRepository
{
    Task<TransferPaymentRequest?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<TransferPaymentRequest>> GetPendingAsync(CancellationToken ct = default);
    Task<IEnumerable<TransferPaymentRequest>> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);
    Task<TransferPaymentRequest> CreateAsync(TransferPaymentRequest entity, CancellationToken ct = default);
    Task UpdateAsync(TransferPaymentRequest entity, CancellationToken ct = default);
}
