using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IPaymentService
{
    Task<Result<PaymentDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<PaymentDto>>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<PaymentDto>> ProcessPaymentAsync(ProcessOrderPaymentRequest request, CancellationToken cancellationToken = default);
    Task<Result<PaymentDto>> ProcessDirectPaymentAsync(ProcessDirectPaymentRequest request, CancellationToken cancellationToken = default);
    Task<Result<PaymentDto>> ProcessSplitPaymentAsync(SplitPaymentRequest request, CancellationToken cancellationToken = default);
    Task<Result<PaymentDto>> RefundPaymentAsync(Guid paymentId, RefundPaymentRequest request, CancellationToken cancellationToken = default);
}
