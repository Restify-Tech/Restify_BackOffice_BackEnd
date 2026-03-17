using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Codigo QR para acceso al menu digital
/// </summary>
public class QRCode : TenantEntity
{
    public Guid? TableId { get; set; }
    public Table? Table { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public QRCodeType Type { get; set; } = QRCodeType.Table;
    public bool IsActive { get; set; } = true;
    public DateTime? LastScannedAt { get; set; }
    public int ScanCount { get; set; } = 0;
}

/// <summary>
/// Tipo de codigo QR
/// </summary>
public enum QRCodeType
{
    Table = 1,
    Takeaway = 2,
    General = 3
}
