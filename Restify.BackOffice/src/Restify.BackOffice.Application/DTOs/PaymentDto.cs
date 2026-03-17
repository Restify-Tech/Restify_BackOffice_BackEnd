namespace Restify.BackOffice.Application.DTOs;

public record PaymentDto(
    Guid Id, Guid OrderId, string OrderNumber, Guid? InvoiceId,
    decimal Amount, string Method, string Status,
    string? GatewayTransactionId, string? PayerName, string? PayerIdentification,
    DateTime? ProcessedAt, string? FailureReason,
    IEnumerable<PaymentItemDto> Items, DateTime CreatedAt);

public record PaymentItemDto(Guid Id, Guid OrderItemId, string ProductName, decimal Amount);

public record ProcessOrderPaymentRequest(
    Guid OrderId, decimal Amount, int Method,
    string? PayerName, string? PayerIdentification,
    string? GatewayTransactionId);

public record ProcessDirectPaymentRequest(
    string? OrderCode, string? PayerIdentification,
    decimal Amount, int Method);

public record SplitPaymentRequest(
    Guid OrderId, int Method,
    string? PayerName, string? PayerIdentification,
    IEnumerable<SplitPaymentItemRequest> Items);

public record SplitPaymentItemRequest(Guid OrderItemId, decimal Amount);

public record RefundPaymentRequest(string Reason);
