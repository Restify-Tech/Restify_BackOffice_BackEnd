using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Payment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Payment?> GetByGatewayTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);
    Task<Payment> AddAsync(Payment entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Payment entity, CancellationToken cancellationToken = default);
}
