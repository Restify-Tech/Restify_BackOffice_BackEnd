using System.Xml.Linq;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Infrastructure.Services.Sri.XmlBuilders;

/// <summary>
/// Genera XML de comprobante de retencion electronico segun esquema SRI v2.0.0
/// </summary>
public static class WithholdingXmlBuilder
{
    public static string Build(WithholdingVoucher voucher, ElectronicDocument document, FiscalConfiguration config)
    {
        var xml = new XElement("comprobanteRetencion",
            new XAttribute("id", "comprobante"),
            new XAttribute("version", "2.0.0"),
            BuildInfoTributaria(document, config),
            BuildInfoCompRetencion(voucher, config),
            BuildDocsSustento(voucher)
        );

        return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + xml.ToString(SaveOptions.DisableFormatting);
    }

    private static XElement BuildInfoTributaria(ElectronicDocument document, FiscalConfiguration config)
    {
        return new XElement("infoTributaria",
            new XElement("ambiente", ((int)config.Environment).ToString()),
            new XElement("tipoEmision", "1"),
            new XElement("razonSocial", config.BusinessName),
            config.TradeName != null ? new XElement("nombreComercial", config.TradeName) : null!,
            new XElement("ruc", config.Ruc),
            new XElement("claveAcceso", document.AccessKey),
            new XElement("codDoc", ((int)SriDocumentType.WithholdingVoucher).ToString("D2")),
            new XElement("estab", document.Establishment),
            new XElement("ptoEmi", document.EmissionPoint),
            new XElement("secuencial", document.Sequential.ToString("D9")),
            new XElement("dirMatriz", config.MainAddress)
        );
    }

    private static XElement BuildInfoCompRetencion(WithholdingVoucher voucher, FiscalConfiguration config)
    {
        return new XElement("infoCompRetencion",
            new XElement("fechaEmision", DateTime.UtcNow.ToString("dd/MM/yyyy")),
            new XElement("dirEstablecimiento", config.EstablishmentAddress),
            config.ContribuyenteEspecial != null ? new XElement("contribuyenteEspecial", config.ContribuyenteEspecial) : null!,
            new XElement("obligadoContabilidad", config.ObligadoContabilidad ? "SI" : "NO"),
            new XElement("tipoIdentificacionSujetoRetenido", ((int)voucher.SupplierIdType).ToString("D2")),
            new XElement("razonSocialSujetoRetenido", voucher.SupplierName),
            new XElement("identificacionSujetoRetenido", voucher.SupplierRuc),
            new XElement("periodoFiscal", voucher.SupportDocDate.ToString("MM/yyyy"))
        );
    }

    private static XElement BuildDocsSustento(WithholdingVoucher voucher)
    {
        var docSustento = new XElement("docSustento",
            new XElement("codSustento", voucher.SupportDocType),
            new XElement("codDocSustento", voucher.SupportDocType),
            new XElement("numDocSustento", voucher.SupportDocNumber),
            new XElement("fechaEmisionDocSustento", voucher.SupportDocDate.ToString("dd/MM/yyyy")),
            BuildRetenciones(voucher)
        );

        return new XElement("docsSustento", docSustento);
    }

    private static XElement BuildRetenciones(WithholdingVoucher voucher)
    {
        var retenciones = new XElement("retenciones");

        foreach (var detail in voucher.Details)
        {
            retenciones.Add(new XElement("retencion",
                new XElement("codigo", detail.TaxCode),
                new XElement("codigoRetencion", detail.RetentionCode),
                new XElement("baseImponible", detail.TaxBase.ToString("F2")),
                new XElement("porcentajeRetener", detail.RetentionPercentage.ToString("F2")),
                new XElement("valorRetenido", detail.RetentionAmount.ToString("F2"))
            ));
        }

        return retenciones;
    }
}
