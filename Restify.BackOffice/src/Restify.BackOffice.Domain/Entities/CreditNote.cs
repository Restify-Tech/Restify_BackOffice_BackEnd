using Restify.BackOffice.Domain.Enums;
using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Nota de credito electronica
/// </summary>
public class CreditNote : TenantEntity
{
    public string CreditNoteNumber { get; set; } = string.Empty;

    // Referencia a la factura modificada
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;

    public string Reason { get; set; } = string.Empty;

    // Datos del comprador
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerIdNumber { get; set; } = string.Empty;
    public SriIdentificationType CustomerIdType { get; set; } = SriIdentificationType.FinalConsumer;

    // Montos
    public decimal Subtotal { get; set; }
    public decimal TaxRate { get; set; } = 0.15m;
    public decimal Tax { get; set; }
    public decimal Total { get; set; }

    // Items
    public ICollection<CreditNoteItem> Items { get; set; } = new List<CreditNoteItem>();
}

/// <summary>
/// Item de nota de credito
/// </summary>
public class CreditNoteItem : TenantEntity
{
    public Guid CreditNoteId { get; set; }
    public CreditNote CreditNote { get; set; } = null!;

    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
}
