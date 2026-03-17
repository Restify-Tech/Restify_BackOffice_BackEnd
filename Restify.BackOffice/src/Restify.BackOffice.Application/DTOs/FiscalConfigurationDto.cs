using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.DTOs;

public class FiscalConfigurationDto
{
    public Guid Id { get; set; }
    public SriEnvironment Environment { get; set; }
    public string Ruc { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string? TradeName { get; set; }
    public string MainAddress { get; set; } = string.Empty;
    public string EstablishmentAddress { get; set; } = string.Empty;
    public bool ObligadoContabilidad { get; set; }
    public string? ContribuyenteEspecial { get; set; }
    public string Establishment { get; set; } = "001";
    public string EmissionPoint { get; set; } = "001";
    public bool HasCertificate { get; set; }
    public DateTime? CertificateExpiration { get; set; }
    public int NextInvoiceSequential { get; set; }
    public int NextCreditNoteSequential { get; set; }
    public int NextWithholdingSequential { get; set; }
    public bool IsActive { get; set; }
    public bool AutoSendOnPayment { get; set; }
    public bool AutoSendEmailOnAuth { get; set; }
}

public class SaveFiscalConfigurationRequest
{
    public SriEnvironment Environment { get; set; } = SriEnvironment.Testing;
    public string Ruc { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string? TradeName { get; set; }
    public string MainAddress { get; set; } = string.Empty;
    public string EstablishmentAddress { get; set; } = string.Empty;
    public bool ObligadoContabilidad { get; set; }
    public string? ContribuyenteEspecial { get; set; }
    public string Establishment { get; set; } = "001";
    public string EmissionPoint { get; set; } = "001";
    public bool IsActive { get; set; }
    public bool AutoSendOnPayment { get; set; }
    public bool AutoSendEmailOnAuth { get; set; }
}
