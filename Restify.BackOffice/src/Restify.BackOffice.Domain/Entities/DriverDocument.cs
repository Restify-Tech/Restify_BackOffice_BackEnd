using Restify.BackOffice.Domain.Enums;
using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Documento subido por un repartidor para verificación
/// </summary>
public class DriverDocument : BaseEntity
{
    public Guid DriverId { get; set; }
    public DeliveryDriver Driver { get; set; } = null!;

    public DocumentType DocumentType { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public DocumentStatus Status { get; set; } = DocumentStatus.Pending;

    public DateTime? ReviewedAt { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public string? RejectionReason { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
