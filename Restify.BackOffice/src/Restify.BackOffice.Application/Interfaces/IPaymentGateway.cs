namespace Restify.BackOffice.Application.Interfaces;

public interface IPaymentGateway
{
    string GatewayName { get; }
    Task<PaymentGatewayResult> ProcessPaymentAsync(PaymentGatewayRequest request, CancellationToken cancellationToken = default);
    Task<PaymentGatewayResult> RefundPaymentAsync(string transactionId, decimal amount, CancellationToken cancellationToken = default);
    Task<PaymentGatewayResult> GetTransactionStatusAsync(string transactionId, CancellationToken cancellationToken = default);
}

public record PaymentGatewayRequest(
    decimal Amount,
    string Currency,
    string? Description,
    string? PayerName,
    string? PayerEmail,
    string? PayerIdentification,
    Dictionary<string, string>? Metadata);

public record PaymentGatewayResult(
    bool IsSuccess,
    string? TransactionId,
    string? Status,
    string? ErrorMessage,
    string? RawResponse);

public interface IPaymentGatewayFactory
{
    IPaymentGateway GetGateway(string gatewayName);
    IPaymentGateway GetDefaultGateway();
}
