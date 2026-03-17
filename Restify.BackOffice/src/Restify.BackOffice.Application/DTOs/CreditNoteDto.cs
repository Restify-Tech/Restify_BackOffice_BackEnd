using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.DTOs;

public class CreditNoteDto
{
    public Guid Id { get; set; }
    public string CreditNoteNumber { get; set; } = string.Empty;
    public Guid InvoiceId { get; set; }
    public string? InvoiceNumber { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerIdNumber { get; set; } = string.Empty;
    public SriIdentificationType CustomerIdType { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public List<CreditNoteItemDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class CreditNoteItemDto
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
}

public class CreateCreditNoteRequest
{
    public Guid InvoiceId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public List<CreditNoteItemRequest> Items { get; set; } = new();
}

public class CreditNoteItemRequest
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
}
