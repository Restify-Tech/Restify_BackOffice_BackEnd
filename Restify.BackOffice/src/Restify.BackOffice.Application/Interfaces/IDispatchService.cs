using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IDispatchService
{
    Task<Result<IEnumerable<DispatchQueueItemDto>>> GetDispatchQueueAsync(CancellationToken cancellationToken = default);
    Task<Result<DispatchApprovalDto>> ApproveDispatchAsync(Guid orderId, ApproveDispatchRequest request, CancellationToken cancellationToken = default);
    Task<Result<DispatchApprovalDto>> RejectDispatchAsync(Guid orderId, RejectDispatchRequest request, CancellationToken cancellationToken = default);
    Task<Result<DispatchApprovalDto>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
}
