using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface ITransferApprovalService
{
    Task<Result<TransferPaymentRequestDto>> CreateAsync(CreateTransferPaymentRequest request, CancellationToken ct = default);
    Task<Result<TransferPaymentRequestDto>> ReviewAsync(Guid id, ReviewTransferPaymentRequest request, CancellationToken ct = default);
    Task<Result<TransferPaymentRequestDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<IEnumerable<TransferPaymentRequestDto>>> GetPendingAsync(CancellationToken ct = default);
    Task<Result<IEnumerable<TransferPaymentRequestDto>>> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);
}
