namespace Restify.BackOffice.Infrastructure.Messaging.Events;

/// <summary>
/// Event published when an invoice is created and ready for electronic processing.
/// Consumed by ElectronicInvoicingAPI and SyncService.
/// </summary>
public class InvoiceCreatedEvent
{
    public Guid InvoiceId { get; set; }
    public Guid TenantId { get; set; }
    public string IssuerRuc { get; set; } = null!;
    public string IssuerBusinessName { get; set; } = null!;
    public string IssuerTradeName { get; set; } = null!;
    public string IssuerAddress { get; set; } = null!;
    public string Establishment { get; set; } = null!;
    public string EmissionPoint { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string CustomerIdNumber { get; set; } = null!;
    public string CustomerIdType { get; set; } = null!;
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public decimal TotalDiscount { get; set; }
    public List<InvoiceItemEvent> Items { get; set; } = new();
}

public class InvoiceItemEvent
{
    public string Code { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
}
