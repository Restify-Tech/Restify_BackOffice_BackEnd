using Restify.BackOffice.Domain.Enums;
using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Factura generada desde un pedido
/// </summary>
public class Invoice : TenantEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    
    // Relación con el pedido
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
    
    // Información del cliente (opcional, puede venir del Order)
    public string? CustomerName { get; set; }
    public string? CustomerIdNumber { get; set; } // RUC/CI
    public SriIdentificationType? CustomerIdType { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerAddress { get; set; }
    
    // Items de la factura (copiados del pedido en el momento de facturar)
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    
    // Montos
    public decimal Subtotal { get; set; }
    public decimal TaxRate { get; set; } = 0.12m; // 12% IVA por defecto
    public decimal Tax { get; set; }
    public decimal DiscountPercentage { get; set; } = 0;
    public decimal DiscountAmount { get; set; } = 0;
    public decimal Total { get; set; }
    
    // Pago
    public PaymentMethod PaymentMethod { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    
    // Metadata
    public string? Notes { get; set; }
    public string? CancelReason { get; set; }
    
    // Usuario que emitió la factura
    public string? IssuedBy { get; set; }
    
    // Timestamps
    public DateTime? PaidAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    // Facturación electrónica (futuro)
    public string? ElectronicAuthorizationCode { get; set; }
    public string? ElectronicAccessKey { get; set; }
}

/// <summary>
/// Item individual de una factura
/// </summary>
public class InvoiceItem : TenantEntity
{
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;
    
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    
    // Modificadores aplicados (guardados como snapshot)
    public ICollection<InvoiceItemModifier> Modifiers { get; set; } = new List<InvoiceItemModifier>();
}

/// <summary>
/// Modificador aplicado a un item de factura
/// </summary>
public class InvoiceItemModifier : BaseEntity
{
    public Guid InvoiceItemId { get; set; }
    public InvoiceItem InvoiceItem { get; set; } = null!;
    
    public string ModifierName { get; set; } = string.Empty;
    public decimal PriceAdjustment { get; set; }
}

/// <summary>
/// Método de pago
/// </summary>
public enum PaymentMethod
{
    Cash = 1,           // Efectivo
    CreditCard = 2,     // Tarjeta de crédito
    DebitCard = 3,      // Tarjeta de débito
    Transfer = 4,       // Transferencia bancaria
    Other = 5           // Otro
}

/// <summary>
/// Estado de la factura
/// </summary>
public enum InvoiceStatus
{
    Draft = 1,      // Borrador (pre-cuenta)
    Paid = 2,       // Pagada
    Cancelled = 3,  // Anulada
    Refunded = 4    // Reembolsada
}
