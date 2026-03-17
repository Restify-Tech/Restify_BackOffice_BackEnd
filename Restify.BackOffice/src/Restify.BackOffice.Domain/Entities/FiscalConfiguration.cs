using Restify.BackOffice.Domain.Enums;
using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Configuracion fiscal del tenant para facturacion electronica SRI
/// </summary>
public class FiscalConfiguration : TenantEntity
{
    // Ambiente SRI
    public SriEnvironment Environment { get; set; } = SriEnvironment.Testing;

    // Datos del emisor
    public string Ruc { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty; // Razon social
    public string? TradeName { get; set; } // Nombre comercial
    public string MainAddress { get; set; } = string.Empty; // Direccion matriz
    public string EstablishmentAddress { get; set; } = string.Empty; // Direccion establecimiento
    public bool ObligadoContabilidad { get; set; }
    public string? ContribuyenteEspecial { get; set; } // Numero de resolucion

    // Serie
    public string Establishment { get; set; } = "001"; // 3 digitos
    public string EmissionPoint { get; set; } = "001"; // 3 digitos

    // Certificado digital (.p12)
    public byte[]? CertificateData { get; set; }
    public string? CertificatePassword { get; set; } // Encriptado
    public DateTime? CertificateExpiration { get; set; }

    // Secuenciales
    public int NextInvoiceSequential { get; set; } = 1;
    public int NextCreditNoteSequential { get; set; } = 1;
    public int NextWithholdingSequential { get; set; } = 1;

    // Control
    public bool IsActive { get; set; }
    public bool AutoSendOnPayment { get; set; }
    public bool AutoSendEmailOnAuth { get; set; }
}
