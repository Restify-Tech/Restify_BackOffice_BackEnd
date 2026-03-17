using System.Xml.Linq;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Infrastructure.Services.Sri.XmlBuilders;

/// <summary>
/// Genera XML de factura electronica segun esquema SRI v2.1.0
/// </summary>
public static class InvoiceXmlBuilder
{
    public static string Build(Invoice invoice, ElectronicDocument document, FiscalConfiguration config)
    {
        var xml = new XElement("factura",
            new XAttribute("id", "comprobante"),
            new XAttribute("version", "2.1.0"),
            BuildInfoTributaria(document, config),
            BuildInfoFactura(invoice, config),
            BuildDetalles(invoice),
            BuildInfoAdicional(invoice)
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
            new XElement("codDoc", ((int)SriDocumentType.Invoice).ToString("D2")),
            new XElement("estab", document.Establishment),
            new XElement("ptoEmi", document.EmissionPoint),
            new XElement("secuencial", document.Sequential.ToString("D9")),
            new XElement("dirMatriz", config.MainAddress)
        );
    }

    private static XElement BuildInfoFactura(Invoice invoice, FiscalConfiguration config)
    {
        var idType = invoice.CustomerIdType ?? SriIdentificationType.FinalConsumer;
        var idNumber = invoice.CustomerIdNumber ?? "9999999999999";
        var customerName = invoice.CustomerName ?? "CONSUMIDOR FINAL";

        var infoFactura = new XElement("infoFactura",
            new XElement("fechaEmision", invoice.CreatedAt.ToString("dd/MM/yyyy")),
            new XElement("dirEstablecimiento", config.EstablishmentAddress),
            config.ContribuyenteEspecial != null ? new XElement("contribuyenteEspecial", config.ContribuyenteEspecial) : null!,
            new XElement("obligadoContabilidad", config.ObligadoContabilidad ? "SI" : "NO"),
            new XElement("tipoIdentificacionComprador", ((int)idType).ToString("D2")),
            new XElement("razonSocialComprador", customerName),
            new XElement("identificacionComprador", idNumber),
            new XElement("totalSinImpuestos", invoice.Subtotal.ToString("F2")),
            new XElement("totalDescuento", invoice.DiscountAmount.ToString("F2")),
            BuildTotalConImpuestos(invoice),
            new XElement("propina", "0.00"),
            new XElement("importeTotal", invoice.Total.ToString("F2")),
            new XElement("moneda", "DOLAR"),
            BuildPagos(invoice)
        );

        return infoFactura;
    }

    private static XElement BuildTotalConImpuestos(Invoice invoice)
    {
        var taxCode = invoice.TaxRate > 0 ? "2" : "0"; // 2=IVA, 0=Sin impuesto
        var taxPercentageCode = MapTaxRateToSriCode(invoice.TaxRate);

        return new XElement("totalConImpuestos",
            new XElement("totalImpuesto",
                new XElement("codigo", taxCode),
                new XElement("codigoPorcentaje", taxPercentageCode),
                new XElement("baseImponible", invoice.Subtotal.ToString("F2")),
                new XElement("valor", invoice.Tax.ToString("F2"))
            )
        );
    }

    private static XElement BuildDetalles(Invoice invoice)
    {
        var detalles = new XElement("detalles");

        foreach (var item in invoice.Items)
        {
            var taxPercentageCode = MapTaxRateToSriCode(invoice.TaxRate);
            var taxCode = invoice.TaxRate > 0 ? "2" : "0";

            detalles.Add(new XElement("detalle",
                new XElement("codigoPrincipal", item.ProductId.ToString()[..8]),
                new XElement("descripcion", item.ProductName),
                new XElement("cantidad", item.Quantity.ToString("F2")),
                new XElement("precioUnitario", item.UnitPrice.ToString("F2")),
                new XElement("descuento", "0.00"),
                new XElement("precioTotalSinImpuesto", item.Subtotal.ToString("F2")),
                new XElement("impuestos",
                    new XElement("impuesto",
                        new XElement("codigo", taxCode),
                        new XElement("codigoPorcentaje", taxPercentageCode),
                        new XElement("tarifa", (invoice.TaxRate * 100).ToString("F2")),
                        new XElement("baseImponible", item.Subtotal.ToString("F2")),
                        new XElement("valor", (item.Subtotal * invoice.TaxRate).ToString("F2"))
                    )
                )
            ));
        }

        return detalles;
    }

    private static XElement BuildPagos(Invoice invoice)
    {
        var sriPaymentCode = MapPaymentMethodToSri(invoice.PaymentMethod);

        return new XElement("pagos",
            new XElement("pago",
                new XElement("formaPago", sriPaymentCode),
                new XElement("total", invoice.Total.ToString("F2"))
            )
        );
    }

    private static XElement BuildInfoAdicional(Invoice invoice)
    {
        var info = new XElement("infoAdicional");

        if (!string.IsNullOrEmpty(invoice.CustomerEmail))
            info.Add(new XElement("campoAdicional", new XAttribute("nombre", "Email"), invoice.CustomerEmail));

        if (!string.IsNullOrEmpty(invoice.CustomerPhone))
            info.Add(new XElement("campoAdicional", new XAttribute("nombre", "Teléfono"), invoice.CustomerPhone));

        if (!string.IsNullOrEmpty(invoice.CustomerAddress))
            info.Add(new XElement("campoAdicional", new XAttribute("nombre", "Dirección"), invoice.CustomerAddress));

        return info;
    }

    /// <summary>
    /// Mapea PaymentMethod del sistema a codigo SRI formaPago
    /// </summary>
    public static string MapPaymentMethodToSri(PaymentMethod method) => method switch
    {
        PaymentMethod.Cash => "01",
        PaymentMethod.CreditCard => "19",
        PaymentMethod.DebitCard => "16",
        PaymentMethod.Transfer => "20",
        PaymentMethod.Other => "20",
        _ => "20"
    };

    /// <summary>
    /// Mapea tasa de impuesto a codigo SRI codigoPorcentaje
    /// 0% → "0", 12% → "2", 15% → "4" (Ecuador 2026)
    /// </summary>
    public static string MapTaxRateToSriCode(decimal taxRate) => taxRate switch
    {
        0m => "0",
        0.12m => "2",
        0.15m => "4",
        _ when taxRate > 0.14m => "4", // 15% vigente 2026
        _ when taxRate > 0 => "2",     // 12% legacy
        _ => "0"
    };
}
