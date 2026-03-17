using Restify.BackOffice.Domain.Enums;
using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Documento electronico enviado al SRI Ecuador
/// </summary>
public class ElectronicDocument : TenantEntity
{
    // Referencias al comprobante origen
    public Guid? InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }

    public Guid? CreditNoteId { get; set; }
    public CreditNote? CreditNote { get; set; }

    public Guid? WithholdingVoucherId { get; set; }
    public WithholdingVoucher? WithholdingVoucher { get; set; }

    // Datos SRI
    public SriDocumentType DocumentType { get; set; }
    public string AccessKey { get; set; } = string.Empty; // 49 digitos
    public string Establishment { get; set; } = "001"; // 3 digitos
    public string EmissionPoint { get; set; } = "001"; // 3 digitos
    public int Sequential { get; set; }
    public ElectronicDocumentStatus Status { get; set; } = ElectronicDocumentStatus.Draft;
    public SriEnvironment Environment { get; set; } = SriEnvironment.Testing;

    // XML
    public string? XmlContent { get; set; }
    public string? SignedXmlContent { get; set; }

    // Respuesta SRI
    public string? AuthorizationCode { get; set; }
    public DateTime? AuthorizationDate { get; set; }
    public string? SriResponse { get; set; } // JSON
    public string? SriErrors { get; set; } // JSON

    // Control de envio
    public int SendAttempts { get; set; }
    public DateTime? LastSendAttempt { get; set; }
    public string? RidePdfUrl { get; set; }
    public bool EmailSent { get; set; }
}
