using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Mappings;

public static class DispatchMappingExtensions
{
    public static DispatchApprovalDto ToDto(this DispatchApproval approval)
    {
        return new DispatchApprovalDto(
            Id: approval.Id,
            OrderId: approval.OrderId,
            OrderNumber: approval.Order?.OrderNumber ?? string.Empty,
            Status: approval.Status.ToString(),
            ApprovedBy: approval.ApprovedBy,
            ApprovedAt: approval.ApprovedAt,
            RejectionReason: approval.RejectionReason,
            Notes: approval.Notes,
            CreatedAt: approval.CreatedAt
        );
    }

    public static DispatchQueueItemDto ToDispatchQueueItemDto(this Order order)
    {
        return new DispatchQueueItemDto(
            OrderId: order.Id,
            OrderNumber: order.OrderNumber,
            OrderType: order.Type.ToString(),
            CustomerName: order.CustomerName ?? string.Empty,
            TableNumber: order.Table?.Number,
            ItemCount: order.Items.Count,
            Total: order.Total,
            PaymentStatus: order.PaymentStatus.ToString(),
            ReadyAt: order.UpdatedAt ?? order.CreatedAt,
            Notes: order.Notes
        );
    }
}
