using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.DTOs;

public class ElectronicDocumentDto
{
    public Guid Id { get; set; }
    public Guid? InvoiceId { get; set; }
    public Guid? CreditNoteId { get; set; }
    public Guid? WithholdingVoucherId { get; set; }
    public SriDocumentType DocumentType { get; set; }
    public string DocumentTypeName { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string Establishment { get; set; } = string.Empty;
    public string EmissionPoint { get; set; } = string.Empty;
    public int Sequential { get; set; }
    public string FullNumber { get; set; } = string.Empty; // 001-001-000000001
    public ElectronicDocumentStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public SriEnvironment Environment { get; set; }
    public string? AuthorizationCode { get; set; }
    public DateTime? AuthorizationDate { get; set; }
    public string? SriErrors { get; set; }
    public int SendAttempts { get; set; }
    public DateTime? LastSendAttempt { get; set; }
    public string? RidePdfUrl { get; set; }
    public bool EmailSent { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ElectronicDocumentSummaryDto
{
    public Guid Id { get; set; }
    public SriDocumentType DocumentType { get; set; }
    public string DocumentTypeName { get; set; } = string.Empty;
    public string FullNumber { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public ElectronicDocumentStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? AuthorizationCode { get; set; }
}

public class ElectronicDocumentFilter
{
    public SriDocumentType? DocumentType { get; set; }
    public ElectronicDocumentStatus? Status { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? AccessKey { get; set; }
}
