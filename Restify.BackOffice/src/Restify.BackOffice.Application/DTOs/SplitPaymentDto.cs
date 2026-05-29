using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.DTOs;

public record SplitPaymentDto(
    Guid Id,
    Guid OrderId,
    decimal TotalAmount,
    int SplitCount,
    string Status,
    string? CreatedByUserId,
    IEnumerable<SplitPaymentItemDto> Items,
    DateTime CreatedAt);

public record SplitPaymentItemDto(
    Guid Id,
    int Index,
    decimal Amount,
    string PaymentMethod,
    string? PaidBy,
    DateTime? PaidAt,
    string Status);

public record CreateSplitPaymentRequest(
    Guid OrderId,
    int SplitCount,
    IEnumerable<CreateSplitPaymentItemRequest> Items);

public record CreateSplitPaymentItemRequest(
    decimal Amount,
    PaymentMethodType PaymentMethod,
    string? PaidBy);

public record ProcessSplitItemRequest(
    Guid SplitPaymentId,
    int ItemIndex,
    PaymentMethodType PaymentMethod,
    decimal Amount);
