using Restify.BackOffice.Domain.Enums;
using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Orden de compra a proveedor
/// </summary>
public class PurchaseOrder : TenantEntity
{
    /// <summary>
    /// Número de orden (auto-generado)
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Proveedor
    /// </summary>
    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    
    /// <summary>
    /// Fecha de la orden
    /// </summary>
    public DateTime OrderDate { get; set; }
    
    /// <summary>
    /// Fecha esperada de entrega
    /// </summary>
    public DateTime? ExpectedDeliveryDate { get; set; }
    
    /// <summary>
    /// Fecha real de entrega
    /// </summary>
    public DateTime? ActualDeliveryDate { get; set; }
    
    /// <summary>
    /// Estado de la orden
    /// </summary>
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
    
    /// <summary>
    /// Items de la orden
    /// </summary>
    public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
    
    /// <summary>
    /// Subtotal
    /// </summary>
    public decimal Subtotal { get; set; }
    
    /// <summary>
    /// IVA
    /// </summary>
    public decimal Tax { get; set; }
    
    /// <summary>
    /// Descuento
    /// </summary>
    public decimal Discount { get; set; }
    
    /// <summary>
    /// Total
    /// </summary>
    public decimal Total { get; set; }
    
    /// <summary>
    /// Notas de la orden
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Usuario que creó la orden
    /// </summary>
    public string? OrderedBy { get; set; }
    
    /// <summary>
    /// Usuario que recibió la orden
    /// </summary>
    public string? ReceivedBy { get; set; }
    
    /// <summary>
    /// Número de factura del proveedor
    /// </summary>
    public string? SupplierInvoiceNumber { get; set; }
}

/// <summary>
/// Item individual de una orden de compra
/// </summary>
public class PurchaseOrderItem : TenantEntity
{
    /// <summary>
    /// Orden de compra
    /// </summary>
    public Guid PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    
    /// <summary>
    /// Producto
    /// </summary>
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    /// <summary>
    /// Cantidad ordenada
    /// </summary>
    public decimal QuantityOrdered { get; set; }
    
    /// <summary>
    /// Cantidad recibida (puede ser menor si entrega parcial)
    /// </summary>
    public decimal QuantityReceived { get; set; }
    
    /// <summary>
    /// Unidad de medida
    /// </summary>
    public string Unit { get; set; } = "unidades";
    
    /// <summary>
    /// Costo unitario
    /// </summary>
    public decimal UnitCost { get; set; }
    
    /// <summary>
    /// Subtotal del item
    /// </summary>
    public decimal Subtotal { get; set; }
    
    /// <summary>
    /// Notas del item
    /// </summary>
    public string? Notes { get; set; }
    
    // Propiedades calculadas
    
    /// <summary>
    /// Cantidad pendiente de recibir
    /// </summary>
    public decimal QuantityPending => QuantityOrdered - QuantityReceived;
    
    /// <summary>
    /// Si el item está completamente recibido
    /// </summary>
    public bool IsFullyReceived => QuantityReceived >= QuantityOrdered;
}
