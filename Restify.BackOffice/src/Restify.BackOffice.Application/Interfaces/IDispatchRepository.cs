using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface IDispatchRepository
{
    Task<DispatchApproval?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DispatchApproval?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DispatchApproval>> GetPendingAsync(CancellationToken cancellationToken = default);
    Task<DispatchApproval> AddAsync(DispatchApproval entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(DispatchApproval entity, CancellationToken cancellationToken = default);
}
