using System.Text.Json;
using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;
using Restify.BackOffice.Infrastructure.Messaging.Events;
using Restify.BackOffice.Infrastructure.Services.Sri.XmlBuilders;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class ElectronicInvoiceService : IElectronicInvoiceService
{
    private readonly IElectronicDocumentRepository _documentRepo;
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly ICreditNoteRepository _creditNoteRepo;
    private readonly IWithholdingVoucherRepository _withholdingRepo;
    private readonly IFiscalConfigurationRepository _configRepo;
    private readonly IAccessKeyGenerator _accessKeyGenerator;
    private readonly IXmlSigningService _signingService;
    private readonly ISriSoapClient _sriClient;
    private readonly IRideGeneratorService _rideGenerator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<ElectronicInvoiceService> _logger;

    public ElectronicInvoiceService(
        IElectronicDocumentRepository documentRepo,
        IInvoiceRepository invoiceRepo,
        ICreditNoteRepository creditNoteRepo,
        IWithholdingVoucherRepository withholdingRepo,
        IFiscalConfigurationRepository configRepo,
        IAccessKeyGenerator accessKeyGenerator,
        IXmlSigningService signingService,
        ISriSoapClient sriClient,
        IRideGeneratorService rideGenerator,
        ICurrentUserService currentUserService,
        IEventPublisher eventPublisher,
        ILogger<ElectronicInvoiceService> logger)
    {
        _documentRepo = documentRepo;
        _invoiceRepo = invoiceRepo;
        _creditNoteRepo = creditNoteRepo;
        _withholdingRepo = withholdingRepo;
        _configRepo = configRepo;
        _accessKeyGenerator = accessKeyGenerator;
        _signingService = signingService;
        _sriClient = sriClient;
        _rideGenerator = rideGenerator;
        _currentUserService = currentUserService;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<Result<ElectronicDocumentDto>> EmitElectronicInvoiceAsync(Guid invoiceId, CancellationToken ct = default)
    {
        // 1. Obtener factura
        var invoice = await _invoiceRepo.GetByIdAsync(invoiceId, ct);
        if (invoice == null)
            return Result<ElectronicDocumentDto>.Failure("Factura no encontrada");

        if (invoice.Status != InvoiceStatus.Paid)
            return Result<ElectronicDocumentDto>.Failure("La factura debe estar pagada para emitir comprobante electrónico");

        // Verificar si ya existe documento electronico
        var existing = await _documentRepo.GetByInvoiceIdAsync(invoiceId, ct);
        if (existing != null && existing.Status == ElectronicDocumentStatus.Authorized)
            return Result<ElectronicDocumentDto>.Failure("La factura ya tiene un documento electrónico autorizado");

        // 2. Obtener configuracion fiscal
        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var config = await _configRepo.GetByTenantIdAsync(tenantId, ct);
        if (config == null || !config.IsActive)
            return Result<ElectronicDocumentDto>.Failure("La configuración fiscal no está activa");

        if (config.CertificateData == null)
            return Result<ElectronicDocumentDto>.Failure("No hay certificado digital configurado");

        // 3. Obtener secuencial atomico
        var sequential = await _configRepo.GetAndIncrementSequentialAsync(tenantId, SriDocumentType.Invoice, ct);

        // 4. Generar clave de acceso
        var accessKey = _accessKeyGenerator.Generate(
            DateTime.UtcNow, SriDocumentType.Invoice, config.Ruc, config.Environment,
            config.Establishment, config.EmissionPoint, sequential);

        // 5. Crear documento electronico
        var document = new ElectronicDocument
        {
            InvoiceId = invoiceId,
            DocumentType = SriDocumentType.Invoice,
            AccessKey = accessKey,
            Establishment = config.Establishment,
            EmissionPoint = config.EmissionPoint,
            Sequential = sequential,
            Status = ElectronicDocumentStatus.Draft,
            Environment = config.Environment
        };

        // 6. Generar XML
        var xml = InvoiceXmlBuilder.Build(invoice, document, config);
        document.XmlContent = xml;

        // 7. Firmar XML
        try
        {
            var signedXml = await _signingService.SignXmlAsync(xml, config.CertificateData, config.CertificatePassword!, ct);
            document.SignedXmlContent = signedXml;
            document.Status = ElectronicDocumentStatus.Signed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al firmar XML de factura {InvoiceId}", invoiceId);
            document.SriErrors = JsonSerializer.Serialize(new[] { ex.Message });
            await _documentRepo.CreateAsync(document, ct);
            return Result<ElectronicDocumentDto>.Failure($"Error al firmar el documento: {ex.Message}");
        }

        // 8. Guardar documento
        await _documentRepo.CreateAsync(document, ct);

        // 9. Publicar evento para ElectronicInvoicingAPI (async processing)
        await _eventPublisher.PublishAsync(new InvoiceCreatedEvent
        {
            InvoiceId = invoiceId,
            TenantId = tenantId,
            IssuerRuc = config.Ruc,
            IssuerBusinessName = config.BusinessName,
            IssuerTradeName = config.TradeName ?? config.BusinessName,
            IssuerAddress = config.MainAddress,
            Establishment = config.Establishment,
            EmissionPoint = config.EmissionPoint,
            CustomerName = invoice.CustomerName ?? "CONSUMIDOR FINAL",
            CustomerIdNumber = invoice.CustomerIdNumber ?? "9999999999999",
            CustomerIdType = invoice.CustomerIdType?.ToString() ?? "07",
            Subtotal = invoice.Subtotal,
            Tax = invoice.Tax,
            Total = invoice.Total,
            TotalDiscount = invoice.DiscountAmount,
            Items = invoice.Items.Select(i => new InvoiceItemEvent
            {
                Code = i.ProductId.ToString(),
                Description = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Discount = 0,
                Subtotal = i.Subtotal,
                TaxRate = invoice.TaxRate * 100,
                TaxAmount = i.Subtotal * invoice.TaxRate
            }).ToList()
        }, ct);

        // 10. Enviar al SRI
        return await SendAndAuthorizeAsync(document, config, invoice, ct);
    }

    public async Task<Result<ElectronicDocumentDto>> EmitCreditNoteAsync(CreateCreditNoteRequest request, CancellationToken ct = default)
    {
        // Obtener factura original
        var invoice = await _invoiceRepo.GetByIdAsync(request.InvoiceId, ct);
        if (invoice == null)
            return Result<ElectronicDocumentDto>.Failure("Factura no encontrada");

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var config = await _configRepo.GetByTenantIdAsync(tenantId, ct);
        if (config == null || !config.IsActive)
            return Result<ElectronicDocumentDto>.Failure("La configuración fiscal no está activa");

        if (config.CertificateData == null)
            return Result<ElectronicDocumentDto>.Failure("No hay certificado digital configurado");

        // Crear nota de credito
        var creditNote = new CreditNote
        {
            CreditNoteNumber = $"NC-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4]}",
            InvoiceId = request.InvoiceId,
            Reason = request.Reason,
            CustomerName = invoice.CustomerName ?? "CONSUMIDOR FINAL",
            CustomerIdNumber = invoice.CustomerIdNumber ?? "9999999999999",
            CustomerIdType = invoice.CustomerIdType ?? SriIdentificationType.FinalConsumer
        };

        foreach (var item in request.Items)
        {
            var subtotal = item.Quantity * item.UnitPrice;
            var taxAmount = subtotal * item.TaxRate;
            creditNote.Items.Add(new CreditNoteItem
            {
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Subtotal = subtotal,
                TaxRate = item.TaxRate,
                TaxAmount = taxAmount
            });
        }

        creditNote.Subtotal = creditNote.Items.Sum(i => i.Subtotal);
        creditNote.TaxRate = invoice.TaxRate;
        creditNote.Tax = creditNote.Items.Sum(i => i.TaxAmount);
        creditNote.Total = creditNote.Subtotal + creditNote.Tax;

        await _creditNoteRepo.CreateAsync(creditNote, ct);

        // Generar documento electronico
        var sequential = await _configRepo.GetAndIncrementSequentialAsync(tenantId, SriDocumentType.CreditNote, ct);
        var accessKey = _accessKeyGenerator.Generate(
            DateTime.UtcNow, SriDocumentType.CreditNote, config.Ruc, config.Environment,
            config.Establishment, config.EmissionPoint, sequential);

        var document = new ElectronicDocument
        {
            CreditNoteId = creditNote.Id,
            DocumentType = SriDocumentType.CreditNote,
            AccessKey = accessKey,
            Establishment = config.Establishment,
            EmissionPoint = config.EmissionPoint,
            Sequential = sequential,
            Environment = config.Environment
        };

        var xml = CreditNoteXmlBuilder.Build(creditNote, document, config, invoice);
        document.XmlContent = xml;

        try
        {
            document.SignedXmlContent = await _signingService.SignXmlAsync(xml, config.CertificateData, config.CertificatePassword!, ct);
            document.Status = ElectronicDocumentStatus.Signed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al firmar XML de nota de crédito");
            document.SriErrors = JsonSerializer.Serialize(new[] { ex.Message });
            await _documentRepo.CreateAsync(document, ct);
            return Result<ElectronicDocumentDto>.Failure($"Error al firmar: {ex.Message}");
        }

        await _documentRepo.CreateAsync(document, ct);
        return await SendAndAuthorizeAsync(document, config, null, ct);
    }

    public async Task<Result<ElectronicDocumentDto>> EmitWithholdingVoucherAsync(CreateWithholdingRequest request, CancellationToken ct = default)
    {
        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var config = await _configRepo.GetByTenantIdAsync(tenantId, ct);
        if (config == null || !config.IsActive)
            return Result<ElectronicDocumentDto>.Failure("La configuración fiscal no está activa");

        if (config.CertificateData == null)
            return Result<ElectronicDocumentDto>.Failure("No hay certificado digital configurado");

        var voucher = new WithholdingVoucher
        {
            VoucherNumber = $"RET-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4]}",
            PurchaseOrderId = request.PurchaseOrderId,
            SupplierName = request.SupplierName,
            SupplierRuc = request.SupplierRuc,
            SupplierIdType = request.SupplierIdType,
            SupportDocType = request.SupportDocType,
            SupportDocNumber = request.SupportDocNumber,
            SupportDocDate = request.SupportDocDate
        };

        foreach (var d in request.Details)
        {
            voucher.Details.Add(new WithholdingDetail
            {
                TaxCode = d.TaxCode,
                RetentionCode = d.RetentionCode,
                TaxBase = d.TaxBase,
                RetentionPercentage = d.RetentionPercentage,
                RetentionAmount = Math.Round(d.TaxBase * d.RetentionPercentage / 100, 2)
            });
        }

        voucher.TotalWithheld = voucher.Details.Sum(d => d.RetentionAmount);
        await _withholdingRepo.CreateAsync(voucher, ct);

        var sequential = await _configRepo.GetAndIncrementSequentialAsync(tenantId, SriDocumentType.WithholdingVoucher, ct);
        var accessKey = _accessKeyGenerator.Generate(
            DateTime.UtcNow, SriDocumentType.WithholdingVoucher, config.Ruc, config.Environment,
            config.Establishment, config.EmissionPoint, sequential);

        var document = new ElectronicDocument
        {
            WithholdingVoucherId = voucher.Id,
            DocumentType = SriDocumentType.WithholdingVoucher,
            AccessKey = accessKey,
            Establishment = config.Establishment,
            EmissionPoint = config.EmissionPoint,
            Sequential = sequential,
            Environment = config.Environment
        };

        var xml = WithholdingXmlBuilder.Build(voucher, document, config);
        document.XmlContent = xml;

        try
        {
            document.SignedXmlContent = await _signingService.SignXmlAsync(xml, config.CertificateData, config.CertificatePassword!, ct);
            document.Status = ElectronicDocumentStatus.Signed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al firmar XML de retención");
            document.SriErrors = JsonSerializer.Serialize(new[] { ex.Message });
            await _documentRepo.CreateAsync(document, ct);
            return Result<ElectronicDocumentDto>.Failure($"Error al firmar: {ex.Message}");
        }

        await _documentRepo.CreateAsync(document, ct);
        return await SendAndAuthorizeAsync(document, config, null, ct);
    }

    public async Task<Result<ElectronicDocumentDto>> CheckAuthorizationAsync(Guid documentId, CancellationToken ct = default)
    {
        var document = await _documentRepo.GetByIdAsync(documentId, ct);
        if (document == null)
            return Result<ElectronicDocumentDto>.Failure("Documento electrónico no encontrado");

        if (document.Status == ElectronicDocumentStatus.Authorized)
            return Result<ElectronicDocumentDto>.Success(MapToDto(document));

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var config = await _configRepo.GetByTenantIdAsync(tenantId, ct);
        if (config == null)
            return Result<ElectronicDocumentDto>.Failure("Configuración fiscal no encontrada");

        var authResponse = await _sriClient.CheckAuthorizationAsync(document.AccessKey, config.Environment, ct);

        if (authResponse.Status == "AUTORIZADO")
        {
            document.Status = ElectronicDocumentStatus.Authorized;
            document.AuthorizationCode = authResponse.AuthorizationNumber;
            document.AuthorizationDate = authResponse.AuthorizationDate;

            // Actualizar factura si aplica
            if (document.InvoiceId.HasValue)
            {
                var invoice = await _invoiceRepo.GetByIdAsync(document.InvoiceId.Value, ct);
                if (invoice != null)
                {
                    invoice.ElectronicAuthorizationCode = authResponse.AuthorizationNumber;
                    invoice.ElectronicAccessKey = document.AccessKey;
                }
            }
        }
        else
        {
            document.Status = ElectronicDocumentStatus.Rejected;
            document.SriResponse = JsonSerializer.Serialize(authResponse);
            document.SriErrors = JsonSerializer.Serialize(authResponse.Messages);
        }

        await _documentRepo.UpdateAsync(document, ct);
        return Result<ElectronicDocumentDto>.Success(MapToDto(document));
    }

    public async Task<Result<ElectronicDocumentDto>> ResendDocumentAsync(Guid documentId, CancellationToken ct = default)
    {
        var document = await _documentRepo.GetByIdAsync(documentId, ct);
        if (document == null)
            return Result<ElectronicDocumentDto>.Failure("Documento electrónico no encontrado");

        if (document.Status == ElectronicDocumentStatus.Authorized)
            return Result<ElectronicDocumentDto>.Failure("El documento ya está autorizado");

        if (string.IsNullOrEmpty(document.SignedXmlContent))
            return Result<ElectronicDocumentDto>.Failure("El documento no tiene XML firmado");

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var config = await _configRepo.GetByTenantIdAsync(tenantId, ct);
        if (config == null)
            return Result<ElectronicDocumentDto>.Failure("Configuración fiscal no encontrada");

        return await SendAndAuthorizeAsync(document, config, null, ct);
    }

    public async Task<Result<byte[]>> GetRidePdfAsync(Guid documentId, CancellationToken ct = default)
    {
        var document = await _documentRepo.GetByIdAsync(documentId, ct);
        if (document == null)
            return Result<byte[]>.Failure("Documento electrónico no encontrado");

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var config = await _configRepo.GetByTenantIdAsync(tenantId, ct);
        if (config == null)
            return Result<byte[]>.Failure("Configuración fiscal no encontrada");

        byte[] pdf;
        switch (document.DocumentType)
        {
            case SriDocumentType.Invoice:
                var invoice = await _invoiceRepo.GetByIdAsync(document.InvoiceId!.Value, ct);
                if (invoice == null) return Result<byte[]>.Failure("Factura no encontrada");
                pdf = _rideGenerator.GenerateInvoiceRide(invoice, document, config);
                break;
            case SriDocumentType.CreditNote:
                var cn = await _creditNoteRepo.GetByIdAsync(document.CreditNoteId!.Value, ct);
                if (cn == null) return Result<byte[]>.Failure("Nota de crédito no encontrada");
                pdf = _rideGenerator.GenerateCreditNoteRide(cn, document, config);
                break;
            case SriDocumentType.WithholdingVoucher:
                var wv = await _withholdingRepo.GetByIdAsync(document.WithholdingVoucherId!.Value, ct);
                if (wv == null) return Result<byte[]>.Failure("Retención no encontrada");
                pdf = _rideGenerator.GenerateWithholdingRide(wv, document, config);
                break;
            default:
                return Result<byte[]>.Failure("Tipo de documento no soportado");
        }

        return Result<byte[]>.Success(pdf);
    }

    public async Task<Result<IEnumerable<ElectronicDocumentSummaryDto>>> GetAllAsync(ElectronicDocumentFilter filter, CancellationToken ct = default)
    {
        var documents = await _documentRepo.GetByFilterAsync(filter, ct);
        var dtos = documents.Select(d => new ElectronicDocumentSummaryDto
        {
            Id = d.Id,
            DocumentType = d.DocumentType,
            DocumentTypeName = GetDocumentTypeName(d.DocumentType),
            FullNumber = $"{d.Establishment}-{d.EmissionPoint}-{d.Sequential:D9}",
            AccessKey = d.AccessKey,
            Status = d.Status,
            StatusName = GetStatusName(d.Status),
            CreatedAt = d.CreatedAt,
            AuthorizationCode = d.AuthorizationCode
        });

        return Result<IEnumerable<ElectronicDocumentSummaryDto>>.Success(dtos);
    }

    public async Task<Result<ElectronicDocumentDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var document = await _documentRepo.GetByIdAsync(id, ct);
        if (document == null)
            return Result<ElectronicDocumentDto>.Failure("Documento electrónico no encontrado");

        return Result<ElectronicDocumentDto>.Success(MapToDto(document));
    }

    private async Task<Result<ElectronicDocumentDto>> SendAndAuthorizeAsync(
        ElectronicDocument document, FiscalConfiguration config, Invoice? invoice, CancellationToken ct)
    {
        try
        {
            document.SendAttempts++;
            document.LastSendAttempt = DateTime.UtcNow;

            var receptionResponse = await _sriClient.SendDocumentAsync(document.SignedXmlContent!, config.Environment, ct);
            document.SriResponse = JsonSerializer.Serialize(receptionResponse);

            if (receptionResponse.Status == "RECIBIDA")
            {
                document.Status = ElectronicDocumentStatus.Received;
                await _documentRepo.UpdateAsync(document, ct);

                // Consultar autorizacion
                await Task.Delay(2000, ct); // Esperar un poco para que el SRI procese
                var authResponse = await _sriClient.CheckAuthorizationAsync(document.AccessKey, config.Environment, ct);

                if (authResponse.Status == "AUTORIZADO")
                {
                    document.Status = ElectronicDocumentStatus.Authorized;
                    document.AuthorizationCode = authResponse.AuthorizationNumber;
                    document.AuthorizationDate = authResponse.AuthorizationDate;

                    if (invoice != null)
                    {
                        invoice.ElectronicAuthorizationCode = authResponse.AuthorizationNumber;
                        invoice.ElectronicAccessKey = document.AccessKey;
                    }
                }
                else
                {
                    document.SriErrors = JsonSerializer.Serialize(authResponse.Messages);
                }
            }
            else
            {
                document.Status = ElectronicDocumentStatus.Rejected;
                document.SriErrors = JsonSerializer.Serialize(receptionResponse.Messages);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar documento al SRI. AccessKey: {AccessKey}", document.AccessKey);
            document.Status = ElectronicDocumentStatus.Sent;
            document.SriErrors = JsonSerializer.Serialize(new[] { ex.Message });
        }

        await _documentRepo.UpdateAsync(document, ct);
        return Result<ElectronicDocumentDto>.Success(MapToDto(document));
    }

    private static ElectronicDocumentDto MapToDto(ElectronicDocument document)
    {
        return new ElectronicDocumentDto
        {
            Id = document.Id,
            InvoiceId = document.InvoiceId,
            CreditNoteId = document.CreditNoteId,
            WithholdingVoucherId = document.WithholdingVoucherId,
            DocumentType = document.DocumentType,
            DocumentTypeName = GetDocumentTypeName(document.DocumentType),
            AccessKey = document.AccessKey,
            Establishment = document.Establishment,
            EmissionPoint = document.EmissionPoint,
            Sequential = document.Sequential,
            FullNumber = $"{document.Establishment}-{document.EmissionPoint}-{document.Sequential:D9}",
            Status = document.Status,
            StatusName = GetStatusName(document.Status),
            Environment = document.Environment,
            AuthorizationCode = document.AuthorizationCode,
            AuthorizationDate = document.AuthorizationDate,
            SriErrors = document.SriErrors,
            SendAttempts = document.SendAttempts,
            LastSendAttempt = document.LastSendAttempt,
            RidePdfUrl = document.RidePdfUrl,
            EmailSent = document.EmailSent,
            CreatedAt = document.CreatedAt
        };
    }

    private static string GetDocumentTypeName(SriDocumentType type) => type switch
    {
        SriDocumentType.Invoice => "Factura",
        SriDocumentType.CreditNote => "Nota de Crédito",
        SriDocumentType.WithholdingVoucher => "Retención",
        _ => "Desconocido"
    };

    private static string GetStatusName(ElectronicDocumentStatus status) => status switch
    {
        ElectronicDocumentStatus.Draft => "Borrador",
        ElectronicDocumentStatus.Signed => "Firmado",
        ElectronicDocumentStatus.Sent => "Enviado",
        ElectronicDocumentStatus.Received => "Recibido",
        ElectronicDocumentStatus.Authorized => "Autorizado",
        ElectronicDocumentStatus.Rejected => "Rechazado",
        _ => "Desconocido"
    };
}
