namespace Restify.BackOffice.Infrastructure.Messaging.Events;

/// <summary>
/// Event published when a payment is processed.
/// Consumed by PaymentGatewayAPI and SyncService.
/// </summary>
public class PaymentProcessedEvent
{
    public Guid PaymentId { get; set; }
    public Guid TenantId { get; set; }
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Method { get; set; } = null!;
    public string? PayerName { get; set; }
    public string? PayerIdentification { get; set; }
}
