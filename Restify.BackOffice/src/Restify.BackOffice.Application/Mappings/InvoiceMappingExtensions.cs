using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Mappings;

public static class InvoiceMappingExtensions
{
    public static InvoiceDto ToDto(this Invoice invoice)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            OrderId = invoice.OrderId,
            OrderNumber = invoice.Order?.OrderNumber ?? string.Empty,
            TableId = invoice.Order?.TableId,
            TableName = invoice.Order?.Table?.Number,
            CustomerName = invoice.CustomerName,
            CustomerIdNumber = invoice.CustomerIdNumber,
            CustomerEmail = invoice.CustomerEmail,
            CustomerPhone = invoice.CustomerPhone,
            CustomerAddress = invoice.CustomerAddress,
            Items = invoice.Items.Select(i => i.ToDto()).ToList(),
            Subtotal = invoice.Subtotal,
            TaxRate = invoice.TaxRate,
            Tax = invoice.Tax,
            DiscountPercentage = invoice.DiscountPercentage,
            DiscountAmount = invoice.DiscountAmount,
            Total = invoice.Total,
            PaymentMethod = invoice.PaymentMethod,
            PaymentMethodName = invoice.PaymentMethod.ToString(),
            Status = invoice.Status,
            StatusName = invoice.Status.ToString(),
            Notes = invoice.Notes,
            CancelReason = invoice.CancelReason,
            IssuedBy = invoice.IssuedBy,
            PaidAt = invoice.PaidAt,
            CancelledAt = invoice.CancelledAt,
            CreatedAt = invoice.CreatedAt,
            UpdatedAt = invoice.UpdatedAt,
            ElectronicAuthorizationCode = invoice.ElectronicAuthorizationCode,
            ElectronicAccessKey = invoice.ElectronicAccessKey
        };
    }

    public static InvoiceSummaryDto ToSummaryDto(this Invoice invoice)
    {
        return new InvoiceSummaryDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            OrderNumber = invoice.Order?.OrderNumber ?? string.Empty,
            TableName = invoice.Order?.Table?.Number,
            CustomerName = invoice.CustomerName,
            Total = invoice.Total,
            PaymentMethod = invoice.PaymentMethod,
            PaymentMethodName = invoice.PaymentMethod.ToString(),
            Status = invoice.Status,
            StatusName = invoice.Status.ToString(),
            CreatedAt = invoice.CreatedAt
        };
    }

    public static InvoiceItemDto ToDto(this InvoiceItem item)
    {
        return new InvoiceItemDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            Subtotal = item.Subtotal,
            Modifiers = item.Modifiers.Select(m => m.ToDto()).ToList()
        };
    }

    public static InvoiceItemModifierDto ToDto(this InvoiceItemModifier modifier)
    {
        return new InvoiceItemModifierDto
        {
            ModifierName = modifier.ModifierName,
            PriceAdjustment = modifier.PriceAdjustment
        };
    }

    public static Invoice ToEntity(this CreateInvoiceRequest request, Order order, string invoiceNumber, Guid tenantId, string issuedBy)
    {
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            InvoiceNumber = invoiceNumber,
            OrderId = request.OrderId,
            CustomerName = request.CustomerName ?? order.CustomerName,
            CustomerIdNumber = request.CustomerIdNumber,
            CustomerEmail = request.CustomerEmail,
            CustomerPhone = request.CustomerPhone ?? order.CustomerPhone,
            CustomerAddress = request.CustomerAddress,
            DiscountPercentage = request.DiscountPercentage,
            PaymentMethod = request.PaymentMethod,
            Status = InvoiceStatus.Draft,
            Notes = request.Notes,
            IssuedBy = issuedBy,
            CreatedAt = DateTime.UtcNow
        };

        // Copy items from order
        foreach (var orderItem in order.Items)
        {
            var invoiceItem = new InvoiceItem
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                InvoiceId = invoice.Id,
                ProductId = orderItem.ProductId,
                ProductName = orderItem.Product?.Name ?? string.Empty,
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice,
                Subtotal = orderItem.Subtotal,
                CreatedAt = DateTime.UtcNow
            };

            // Copy modifiers
            foreach (var orderModifier in orderItem.Modifiers)
            {
                var invoiceModifier = new InvoiceItemModifier
                {
                    Id = Guid.NewGuid(),
                    InvoiceItemId = invoiceItem.Id,
                    ModifierName = orderModifier.ModifierName,
                    PriceAdjustment = orderModifier.PriceAdjustment,
                    CreatedAt = DateTime.UtcNow
                };

                invoiceItem.Modifiers.Add(invoiceModifier);
            }

            invoice.Items.Add(invoiceItem);
        }

        return invoice;
    }

    public static void Update(this Invoice invoice, UpdateInvoiceRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.CustomerName))
            invoice.CustomerName = request.CustomerName;

        if (!string.IsNullOrWhiteSpace(request.CustomerIdNumber))
            invoice.CustomerIdNumber = request.CustomerIdNumber;

        if (!string.IsNullOrWhiteSpace(request.CustomerEmail))
            invoice.CustomerEmail = request.CustomerEmail;

        if (!string.IsNullOrWhiteSpace(request.CustomerPhone))
            invoice.CustomerPhone = request.CustomerPhone;

        if (!string.IsNullOrWhiteSpace(request.CustomerAddress))
            invoice.CustomerAddress = request.CustomerAddress;

        if (!string.IsNullOrWhiteSpace(request.Notes))
            invoice.Notes = request.Notes;

        invoice.UpdatedAt = DateTime.UtcNow;
    }

    public static void ProcessPayment(this Invoice invoice, ProcessPaymentRequest request)
    {
        invoice.PaymentMethod = request.PaymentMethod;
        invoice.Status = InvoiceStatus.Paid;
        invoice.PaidAt = DateTime.UtcNow;
        invoice.UpdatedAt = DateTime.UtcNow;
    }

    public static void Cancel(this Invoice invoice, CancelInvoiceRequest request)
    {
        invoice.Status = InvoiceStatus.Cancelled;
        invoice.CancelReason = request.Reason;
        invoice.CancelledAt = DateTime.UtcNow;
        invoice.UpdatedAt = DateTime.UtcNow;
    }

    public static void RecalculateTotals(this Invoice invoice)
    {
        // Subtotal from items
        invoice.Subtotal = invoice.Items.Sum(i => i.Subtotal);

        // Apply discount
        invoice.DiscountAmount = invoice.Subtotal * (invoice.DiscountPercentage / 100);
        var subtotalAfterDiscount = invoice.Subtotal - invoice.DiscountAmount;

        // Calculate tax
        invoice.Tax = subtotalAfterDiscount * invoice.TaxRate;

        // Total
        invoice.Total = subtotalAfterDiscount + invoice.Tax;
    }
}
