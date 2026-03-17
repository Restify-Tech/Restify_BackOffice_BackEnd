using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Mappings;

public static class PaymentMappingExtensions
{
    public static PaymentDto ToDto(this Payment payment)
    {
        return new PaymentDto(
            Id: payment.Id,
            OrderId: payment.OrderId,
            OrderNumber: payment.Order?.OrderNumber ?? string.Empty,
            InvoiceId: payment.InvoiceId,
            Amount: payment.Amount,
            Method: payment.Method.ToString(),
            Status: payment.Status.ToString(),
            GatewayTransactionId: payment.GatewayTransactionId,
            PayerName: payment.PayerName,
            PayerIdentification: payment.PayerIdentification,
            ProcessedAt: payment.ProcessedAt,
            FailureReason: payment.FailureReason,
            Items: payment.Items.Select(i => i.ToDto()),
            CreatedAt: payment.CreatedAt
        );
    }

    public static PaymentItemDto ToDto(this PaymentItem item)
    {
        return new PaymentItemDto(
            Id: item.Id,
            OrderItemId: item.OrderItemId,
            ProductName: item.OrderItem?.Product?.Name ?? string.Empty,
            Amount: item.Amount
        );
    }
}
