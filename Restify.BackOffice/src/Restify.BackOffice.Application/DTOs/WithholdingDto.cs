using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.DTOs;

public class WithholdingVoucherDto
{
    public Guid Id { get; set; }
    public string VoucherNumber { get; set; } = string.Empty;
    public Guid? PurchaseOrderId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string SupplierRuc { get; set; } = string.Empty;
    public SriIdentificationType SupplierIdType { get; set; }
    public string SupportDocType { get; set; } = string.Empty;
    public string SupportDocNumber { get; set; } = string.Empty;
    public DateTime SupportDocDate { get; set; }
    public decimal TotalWithheld { get; set; }
    public List<WithholdingDetailDto> Details { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class WithholdingDetailDto
{
    public string TaxCode { get; set; } = string.Empty;
    public string RetentionCode { get; set; } = string.Empty;
    public decimal TaxBase { get; set; }
    public decimal RetentionPercentage { get; set; }
    public decimal RetentionAmount { get; set; }
}

public class CreateWithholdingRequest
{
    public Guid? PurchaseOrderId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string SupplierRuc { get; set; } = string.Empty;
    public SriIdentificationType SupplierIdType { get; set; } = SriIdentificationType.Ruc;
    public string SupportDocType { get; set; } = "01";
    public string SupportDocNumber { get; set; } = string.Empty;
    public DateTime SupportDocDate { get; set; }
    public List<WithholdingDetailRequest> Details { get; set; } = new();
}

public class WithholdingDetailRequest
{
    public string TaxCode { get; set; } = string.Empty;
    public string RetentionCode { get; set; } = string.Empty;
    public decimal TaxBase { get; set; }
    public decimal RetentionPercentage { get; set; }
}
