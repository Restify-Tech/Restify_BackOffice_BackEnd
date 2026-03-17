using System.Xml.Linq;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Infrastructure.Services.Sri.XmlBuilders;

/// <summary>
/// Genera XML de nota de credito electronica segun esquema SRI v1.1.0
/// </summary>
public static class CreditNoteXmlBuilder
{
    public static string Build(CreditNote creditNote, ElectronicDocument document, FiscalConfiguration config, Invoice originalInvoice)
    {
        var xml = new XElement("notaCredito",
            new XAttribute("id", "comprobante"),
            new XAttribute("version", "1.1.0"),
            BuildInfoTributaria(document, config),
            BuildInfoNotaCredito(creditNote, config, originalInvoice),
            BuildDetalles(creditNote)
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
            new XElement("codDoc", ((int)SriDocumentType.CreditNote).ToString("D2")),
            new XElement("estab", document.Establishment),
            new XElement("ptoEmi", document.EmissionPoint),
            new XElement("secuencial", document.Sequential.ToString("D9")),
            new XElement("dirMatriz", config.MainAddress)
        );
    }

    private static XElement BuildInfoNotaCredito(CreditNote creditNote, FiscalConfiguration config, Invoice originalInvoice)
    {
        return new XElement("infoNotaCredito",
            new XElement("fechaEmision", DateTime.UtcNow.ToString("dd/MM/yyyy")),
            new XElement("dirEstablecimiento", config.EstablishmentAddress),
            new XElement("tipoIdentificacionComprador", ((int)creditNote.CustomerIdType).ToString("D2")),
            new XElement("razonSocialComprador", creditNote.CustomerName),
            new XElement("identificacionComprador", creditNote.CustomerIdNumber),
            config.ContribuyenteEspecial != null ? new XElement("contribuyenteEspecial", config.ContribuyenteEspecial) : null!,
            new XElement("obligadoContabilidad", config.ObligadoContabilidad ? "SI" : "NO"),
            new XElement("codDocModificado", "01"),
            new XElement("numDocModificado", originalInvoice.InvoiceNumber),
            new XElement("fechaEmisionDocSustento", originalInvoice.CreatedAt.ToString("dd/MM/yyyy")),
            new XElement("totalSinImpuestos", creditNote.Subtotal.ToString("F2")),
            new XElement("valorModificacion", creditNote.Total.ToString("F2")),
            new XElement("moneda", "DOLAR"),
            BuildTotalConImpuestos(creditNote),
            new XElement("motivo", creditNote.Reason)
        );
    }

    private static XElement BuildTotalConImpuestos(CreditNote creditNote)
    {
        var taxCode = creditNote.TaxRate > 0 ? "2" : "0";
        var taxPercentageCode = InvoiceXmlBuilder.MapTaxRateToSriCode(creditNote.TaxRate);

        return new XElement("totalConImpuestos",
            new XElement("totalImpuesto",
                new XElement("codigo", taxCode),
                new XElement("codigoPorcentaje", taxPercentageCode),
                new XElement("baseImponible", creditNote.Subtotal.ToString("F2")),
                new XElement("valor", creditNote.Tax.ToString("F2"))
            )
        );
    }

    private static XElement BuildDetalles(CreditNote creditNote)
    {
        var detalles = new XElement("detalles");

        foreach (var item in creditNote.Items)
        {
            var taxCode = item.TaxRate > 0 ? "2" : "0";
            var taxPercentageCode = InvoiceXmlBuilder.MapTaxRateToSriCode(item.TaxRate);

            detalles.Add(new XElement("detalle",
                new XElement("descripcion", item.ProductName),
                new XElement("cantidad", item.Quantity.ToString("F2")),
                new XElement("precioUnitario", item.UnitPrice.ToString("F2")),
                new XElement("descuento", "0.00"),
                new XElement("precioTotalSinImpuesto", item.Subtotal.ToString("F2")),
                new XElement("impuestos",
                    new XElement("impuesto",
                        new XElement("codigo", taxCode),
                        new XElement("codigoPorcentaje", taxPercentageCode),
                        new XElement("tarifa", (item.TaxRate * 100).ToString("F2")),
                        new XElement("baseImponible", item.Subtotal.ToString("F2")),
                        new XElement("valor", item.TaxAmount.ToString("F2"))
                    )
                )
            ));
        }

        return detalles;
    }
}
