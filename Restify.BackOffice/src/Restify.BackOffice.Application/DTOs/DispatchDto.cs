namespace Restify.BackOffice.Application.DTOs;

public record DispatchQueueItemDto(
    Guid OrderId, string OrderNumber, string OrderType,
    string CustomerName, string? TableNumber,
    int ItemCount, decimal Total, string PaymentStatus,
    DateTime ReadyAt, string? Notes);

public record DispatchApprovalDto(
    Guid Id, Guid OrderId, string OrderNumber,
    string Status, string? ApprovedBy, DateTime? ApprovedAt,
    string? RejectionReason, string? Notes, DateTime CreatedAt);

public record ApproveDispatchRequest(string? Notes);

public record RejectDispatchRequest(string Reason, string? Notes);
