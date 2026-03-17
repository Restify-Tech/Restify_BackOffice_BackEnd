using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.DTOs;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public Guid? TableId { get; set; }
    public string? TableName { get; set; }
    
    // Customer info
    public string? CustomerName { get; set; }
    public string? CustomerIdNumber { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerAddress { get; set; }
    
    // Items
    public List<InvoiceItemDto> Items { get; set; } = new();
    
    // Amounts
    public decimal Subtotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal Tax { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total { get; set; }
    
    // Payment
    public PaymentMethod PaymentMethod { get; set; }
    public string PaymentMethodName { get; set; } = string.Empty;
    public InvoiceStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    
    // Metadata
    public string? Notes { get; set; }
    public string? CancelReason { get; set; }
    public string? IssuedBy { get; set; }
    
    // Timestamps
    public DateTime? PaidAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Electronic
    public string? ElectronicAuthorizationCode { get; set; }
    public string? ElectronicAccessKey { get; set; }
}

public class InvoiceItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    public List<InvoiceItemModifierDto> Modifiers { get; set; } = new();
}

public class InvoiceItemModifierDto
{
    public string ModifierName { get; set; } = string.Empty;
    public decimal PriceAdjustment { get; set; }
}

public class InvoiceSummaryDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public string? TableName { get; set; }
    public string? CustomerName { get; set; }
    public decimal Total { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string PaymentMethodName { get; set; } = string.Empty;
    public InvoiceStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateInvoiceRequest
{
    public Guid OrderId { get; set; }
    
    // Customer (optional)
    public string? CustomerName { get; set; }
    public string? CustomerIdNumber { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerAddress { get; set; }
    
    // Discount (optional)
    public decimal DiscountPercentage { get; set; } = 0;
    
    // Payment
    public PaymentMethod PaymentMethod { get; set; }
    
    // Notes
    public string? Notes { get; set; }
}

public class UpdateInvoiceRequest
{
    public string? CustomerName { get; set; }
    public string? CustomerIdNumber { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerAddress { get; set; }
    public string? Notes { get; set; }
}

public class ProcessPaymentRequest
{
    public PaymentMethod PaymentMethod { get; set; }
    public decimal? AmountPaid { get; set; } // For validation
}

public class CancelInvoiceRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class InvoiceStatisticsDto
{
    public int TotalInvoices { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageTicket { get; set; }
    public List<PaymentMethodStats> PaymentMethodBreakdown { get; set; } = new();
}

public class PaymentMethodStats
{
    public string Method { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Total { get; set; }
}
