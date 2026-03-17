using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services.Gateways;

public class ManualPaymentGateway : IPaymentGateway
{
    public string GatewayName => "Manual";

    public Task<PaymentGatewayResult> ProcessPaymentAsync(PaymentGatewayRequest request, CancellationToken cancellationToken = default)
    {
        // Manual payments (cash, transfer) are always approved immediately
        return Task.FromResult(new PaymentGatewayResult(
            IsSuccess: true,
            TransactionId: $"MANUAL-{Guid.NewGuid():N}",
            Status: "Completed",
            ErrorMessage: null,
            RawResponse: null));
    }

    public Task<PaymentGatewayResult> RefundPaymentAsync(string transactionId, decimal amount, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new PaymentGatewayResult(
            IsSuccess: true,
            TransactionId: transactionId,
            Status: "Refunded",
            ErrorMessage: null,
            RawResponse: null));
    }

    public Task<PaymentGatewayResult> GetTransactionStatusAsync(string transactionId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new PaymentGatewayResult(
            IsSuccess: true,
            TransactionId: transactionId,
            Status: "Completed",
            ErrorMessage: null,
            RawResponse: null));
    }
}
