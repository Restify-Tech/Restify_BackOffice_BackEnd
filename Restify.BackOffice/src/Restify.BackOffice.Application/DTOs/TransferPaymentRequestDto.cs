using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.DTOs;

public record TransferPaymentRequestDto(
    Guid Id,
    Guid OrderId,
    string OrderNumber,
    decimal Amount,
    string? ImageUrl,
    string? BankReference,
    string? PayerName,
    string? PayerIdentification,
    string Status,
    string? ReviewedBy,
    DateTime? ReviewedAt,
    string? RejectionReason,
    DateTime CreatedAt
);

public record CreateTransferPaymentRequest(
    Guid OrderId,
    decimal Amount,
    string? ImageUrl,
    string? BankReference,
    string? PayerName,
    string? PayerIdentification
);

public record ReviewTransferPaymentRequest(
    bool Approved,
    string? RejectionReason
);
