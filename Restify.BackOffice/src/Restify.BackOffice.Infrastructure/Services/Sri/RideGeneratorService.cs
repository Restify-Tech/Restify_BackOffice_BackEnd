using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;
using Restify.BackOffice.Infrastructure.Services.Sri.XmlBuilders;

namespace Restify.BackOffice.Infrastructure.Services.Sri;

/// <summary>
/// Genera PDF RIDE (Representacion Impresa del Documento Electronico) para comprobantes SRI
/// </summary>
public class RideGeneratorService : IRideGeneratorService
{
    static RideGeneratorService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateInvoiceRide(Invoice invoice, ElectronicDocument document, FiscalConfiguration config)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(25);
                page.DefaultTextStyle(x => x.FontSize(8));

                page.Header().Element(c => BuildHeader(c, document, config, "FACTURA"));

                page.Content().Element(c =>
                {
                    c.Column(col =>
                    {
                        col.Item().PaddingVertical(5).Element(cc => BuildBuyerInfo(cc, invoice.CustomerName ?? "CONSUMIDOR FINAL",
                            invoice.CustomerIdNumber ?? "9999999999999",
                            invoice.CustomerIdType?.ToString() ?? "Consumidor Final",
                            invoice.CustomerEmail, invoice.CustomerAddress));

                        col.Item().Element(cc =>
                        {
                            cc.Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                });

                                table.Header(header =>
                                {
                                    foreach (var h in new[] { "Cod.", "Descripción", "Cant.", "P.Unit.", "Desc.", "Subtotal" })
                                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text(h).Bold();
                                });

                                foreach (var item in invoice.Items)
                                {
                                    table.Cell().Padding(2).Text(item.ProductId.ToString()[..8]);
                                    table.Cell().Padding(2).Text(item.ProductName);
                                    table.Cell().Padding(2).AlignRight().Text(item.Quantity.ToString());
                                    table.Cell().Padding(2).AlignRight().Text(item.UnitPrice.ToString("F2"));
                                    table.Cell().Padding(2).AlignRight().Text("0.00");
                                    table.Cell().Padding(2).AlignRight().Text(item.Subtotal.ToString("F2"));
                                }
                            });
                        });

                        col.Item().PaddingTop(10).AlignRight().Width(200).Element(cc =>
                        {
                            cc.Column(totCol =>
                            {
                                BuildTotalRow(totCol, "Subtotal sin IVA:", invoice.Subtotal);
                                BuildTotalRow(totCol, "Descuento:", invoice.DiscountAmount);
                                BuildTotalRow(totCol, $"IVA {invoice.TaxRate * 100:F0}%:", invoice.Tax);
                                totCol.Item().LineHorizontal(1);
                                BuildTotalRow(totCol, "TOTAL:", invoice.Total, true);
                            });
                        });

                        col.Item().PaddingTop(10).Element(cc =>
                        {
                            cc.Column(payCol =>
                            {
                                payCol.Item().Text("Forma de Pago").Bold();
                                payCol.Item().Row(row =>
                                {
                                    row.RelativeItem().Text($"{InvoiceXmlBuilder.MapPaymentMethodToSri(invoice.PaymentMethod)} - {invoice.PaymentMethod}");
                                    row.ConstantItem(80).AlignRight().Text(invoice.Total.ToString("F2"));
                                });
                            });
                        });
                    });
                });
            });
        }).GeneratePdf();
    }

    public byte[] GenerateCreditNoteRide(CreditNote creditNote, ElectronicDocument document, FiscalConfiguration config)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(25);
                page.DefaultTextStyle(x => x.FontSize(8));

                page.Header().Element(c => BuildHeader(c, document, config, "NOTA DE CRÉDITO"));

                page.Content().Element(c =>
                {
                    c.Column(col =>
                    {
                        col.Item().PaddingVertical(5).Element(cc => BuildBuyerInfo(cc,
                            creditNote.CustomerName, creditNote.CustomerIdNumber,
                            creditNote.CustomerIdType.ToString(), null, null));

                        col.Item().PaddingBottom(5).Text($"Motivo: {creditNote.Reason}");

                        col.Item().Element(cc =>
                        {
                            cc.Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                });

                                table.Header(header =>
                                {
                                    foreach (var h in new[] { "Descripción", "Cant.", "P.Unit.", "Subtotal" })
                                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text(h).Bold();
                                });

                                foreach (var item in creditNote.Items)
                                {
                                    table.Cell().Padding(2).Text(item.ProductName);
                                    table.Cell().Padding(2).AlignRight().Text(item.Quantity.ToString());
                                    table.Cell().Padding(2).AlignRight().Text(item.UnitPrice.ToString("F2"));
                                    table.Cell().Padding(2).AlignRight().Text(item.Subtotal.ToString("F2"));
                                }
                            });
                        });

                        col.Item().PaddingTop(10).AlignRight().Width(200).Element(cc =>
                        {
                            cc.Column(totCol =>
                            {
                                BuildTotalRow(totCol, "Subtotal:", creditNote.Subtotal);
                                BuildTotalRow(totCol, $"IVA {creditNote.TaxRate * 100:F0}%:", creditNote.Tax);
                                totCol.Item().LineHorizontal(1);
                                BuildTotalRow(totCol, "TOTAL:", creditNote.Total, true);
                            });
                        });
                    });
                });
            });
        }).GeneratePdf();
    }

    public byte[] GenerateWithholdingRide(WithholdingVoucher voucher, ElectronicDocument document, FiscalConfiguration config)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(25);
                page.DefaultTextStyle(x => x.FontSize(8));

                page.Header().Element(c => BuildHeader(c, document, config, "COMPROBANTE DE RETENCIÓN"));

                page.Content().Element(c =>
                {
                    c.Column(col =>
                    {
                        col.Item().PaddingVertical(5).Row(row =>
                        {
                            row.RelativeItem().Column(inner =>
                            {
                                inner.Item().Text($"Sujeto Retenido: {voucher.SupplierName}");
                                inner.Item().Text($"RUC: {voucher.SupplierRuc}");
                                inner.Item().Text($"Doc. Sustento: {voucher.SupportDocType} - {voucher.SupportDocNumber}");
                                inner.Item().Text($"Fecha Doc. Sustento: {voucher.SupportDocDate:dd/MM/yyyy}");
                            });
                        });

                        col.Item().PaddingTop(5).Element(cc =>
                        {
                            cc.Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                });

                                table.Header(header =>
                                {
                                    foreach (var h in new[] { "Impuesto", "Cod.Ret.", "Base Imp.", "% Ret.", "Valor Ret." })
                                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text(h).Bold();
                                });

                                foreach (var d in voucher.Details)
                                {
                                    table.Cell().Padding(2).Text(d.TaxCode);
                                    table.Cell().Padding(2).Text(d.RetentionCode);
                                    table.Cell().Padding(2).AlignRight().Text(d.TaxBase.ToString("F2"));
                                    table.Cell().Padding(2).AlignRight().Text(d.RetentionPercentage.ToString("F2") + "%");
                                    table.Cell().Padding(2).AlignRight().Text(d.RetentionAmount.ToString("F2"));
                                }
                            });
                        });

                        col.Item().PaddingTop(10).AlignRight().Width(200).Element(cc =>
                        {
                            cc.Column(totCol =>
                            {
                                totCol.Item().LineHorizontal(1);
                                BuildTotalRow(totCol, "TOTAL RETENIDO:", voucher.TotalWithheld, true);
                            });
                        });
                    });
                });
            });
        }).GeneratePdf();
    }

    private static void BuildHeader(IContainer container, ElectronicDocument document, FiscalConfiguration config, string docTitle)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text(config.BusinessName).Bold().FontSize(10);
                if (config.TradeName != null)
                    col.Item().Text(config.TradeName);
                col.Item().Text($"RUC: {config.Ruc}");
                col.Item().Text($"Dir. Matriz: {config.MainAddress}");
                col.Item().Text($"Dir. Establecimiento: {config.EstablishmentAddress}");
                if (config.ObligadoContabilidad)
                    col.Item().Text("OBLIGADO A LLEVAR CONTABILIDAD: SÍ");
                if (config.ContribuyenteEspecial != null)
                    col.Item().Text($"Contribuyente Especial: {config.ContribuyenteEspecial}");
            });

            row.ConstantItem(220).Border(1).Padding(5).Column(col =>
            {
                col.Item().Text($"R.U.C.: {config.Ruc}").Bold();
                col.Item().Text(docTitle).Bold().FontSize(10);
                col.Item().Text($"No. {document.Establishment}-{document.EmissionPoint}-{document.Sequential:D9}");
                col.Item().Text("NÚMERO DE AUTORIZACIÓN");
                col.Item().Text(document.AuthorizationCode ?? document.AccessKey).FontSize(6);
                if (document.AuthorizationDate.HasValue)
                    col.Item().Text($"FECHA AUTORIZACIÓN: {document.AuthorizationDate:dd/MM/yyyy HH:mm:ss}");
                col.Item().Text($"AMBIENTE: {(config.Environment == SriEnvironment.Production ? "PRODUCCIÓN" : "PRUEBAS")}");
                col.Item().Text("EMISIÓN: NORMAL");

                // Clave de acceso como texto (sin barcode externo)
                col.Item().PaddingTop(5).Border(0.5f).Padding(2).Column(barcodeCol =>
                {
                    barcodeCol.Item().Text("CLAVE DE ACCESO").FontSize(5).Bold();
                    barcodeCol.Item().Text(document.AccessKey).FontSize(6).FontFamily("Courier New");
                });
            });
        });
    }

    private static void BuildBuyerInfo(IContainer container, string name, string idNumber, string idType, string? email, string? address)
    {
        container.Border(0.5f).Padding(5).Column(col =>
        {
            col.Item().Row(r =>
            {
                r.RelativeItem().Text($"Razón Social/Nombres: {name}");
                r.ConstantItem(150).Text($"Identificación: {idNumber}");
            });
            if (email != null)
                col.Item().Text($"Email: {email}");
            if (address != null)
                col.Item().Text($"Dirección: {address}");
        });
    }

    private static void BuildTotalRow(ColumnDescriptor col, string label, decimal value, bool bold = false)
    {
        col.Item().Row(row =>
        {
            var labelText = row.RelativeItem().AlignRight().Text(label);
            var valueText = row.ConstantItem(60).AlignRight().Text(value.ToString("F2"));
            if (bold)
            {
                labelText.Bold();
                valueText.Bold();
            }
        });
    }
}
