using Restify.BackOffice.Domain.Enums;
using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Comprobante de retencion electronico
/// </summary>
public class WithholdingVoucher : TenantEntity
{
    public string VoucherNumber { get; set; } = string.Empty;

    // Referencia opcional a orden de compra
    public Guid? PurchaseOrderId { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }

    // Datos del proveedor
    public string SupplierName { get; set; } = string.Empty;
    public string SupplierRuc { get; set; } = string.Empty;
    public SriIdentificationType SupplierIdType { get; set; } = SriIdentificationType.Ruc;

    // Documento sustento
    public string SupportDocType { get; set; } = "01"; // 01=Factura
    public string SupportDocNumber { get; set; } = string.Empty;
    public DateTime SupportDocDate { get; set; }

    // Totales
    public decimal TotalWithheld { get; set; }

    // Detalles
    public ICollection<WithholdingDetail> Details { get; set; } = new List<WithholdingDetail>();
}

/// <summary>
/// Detalle de retencion (impuesto retenido)
/// </summary>
public class WithholdingDetail : TenantEntity
{
    public Guid WithholdingVoucherId { get; set; }
    public WithholdingVoucher WithholdingVoucher { get; set; } = null!;

    /// <summary>
    /// Codigo del impuesto: 1=Renta, 2=IVA, 6=ISD
    /// </summary>
    public string TaxCode { get; set; } = string.Empty;

    /// <summary>
    /// Codigo de retencion (ej: 303, 312 para renta; 10, 20, 30 para IVA)
    /// </summary>
    public string RetentionCode { get; set; } = string.Empty;

    public decimal TaxBase { get; set; }
    public decimal RetentionPercentage { get; set; }
    public decimal RetentionAmount { get; set; }
}
