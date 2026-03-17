using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class FiscalConfigurationService : IFiscalConfigurationService
{
    private readonly IFiscalConfigurationRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<FiscalConfigurationService> _logger;

    public FiscalConfigurationService(
        IFiscalConfigurationRepository repository,
        ICurrentUserService currentUserService,
        ILogger<FiscalConfigurationService> logger)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result<FiscalConfigurationDto>> GetAsync(CancellationToken ct = default)
    {
        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var config = await _repository.GetByTenantIdAsync(tenantId, ct);

        if (config == null)
            return Result<FiscalConfigurationDto>.Success(new FiscalConfigurationDto());

        return Result<FiscalConfigurationDto>.Success(MapToDto(config));
    }

    public async Task<Result<FiscalConfigurationDto>> SaveAsync(SaveFiscalConfigurationRequest request, CancellationToken ct = default)
    {
        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var config = await _repository.GetByTenantIdAsync(tenantId, ct);

        if (config == null)
        {
            config = new FiscalConfiguration
            {
                TenantId = tenantId
            };
            UpdateFromRequest(config, request);
            await _repository.CreateAsync(config, ct);
        }
        else
        {
            UpdateFromRequest(config, request);
            await _repository.UpdateAsync(config, ct);
        }

        _logger.LogInformation("Configuración fiscal guardada para tenant {TenantId}", tenantId);
        return Result<FiscalConfigurationDto>.Success(MapToDto(config));
    }

    public async Task<Result<FiscalConfigurationDto>> UploadCertificateAsync(Stream certificateStream, string password, CancellationToken ct = default)
    {
        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var config = await _repository.GetByTenantIdAsync(tenantId, ct);

        if (config == null)
            return Result<FiscalConfigurationDto>.Failure("Debe guardar la configuración fiscal primero");

        using var ms = new MemoryStream();
        await certificateStream.CopyToAsync(ms, ct);
        var certData = ms.ToArray();

        // Validar certificado
        try
        {
            var cert = new X509Certificate2(certData, password);
            config.CertificateData = certData;
            config.CertificatePassword = password; // TODO: Encriptar con Data Protection
            config.CertificateExpiration = cert.NotAfter;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar certificado digital");
            return Result<FiscalConfigurationDto>.Failure("El certificado .p12 no es válido o la contraseña es incorrecta");
        }

        await _repository.UpdateAsync(config, ct);
        return Result<FiscalConfigurationDto>.Success(MapToDto(config));
    }

    public async Task<Result<bool>> ValidateCertificateAsync(CancellationToken ct = default)
    {
        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var config = await _repository.GetByTenantIdAsync(tenantId, ct);

        if (config?.CertificateData == null)
            return Result<bool>.Failure("No hay certificado digital configurado");

        try
        {
            var cert = new X509Certificate2(config.CertificateData, config.CertificatePassword);

            if (cert.NotAfter < DateTime.UtcNow)
                return Result<bool>.Failure($"El certificado expiró el {cert.NotAfter:dd/MM/yyyy}");

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error al validar certificado: {ex.Message}");
        }
    }

    private static void UpdateFromRequest(FiscalConfiguration config, SaveFiscalConfigurationRequest request)
    {
        config.Environment = request.Environment;
        config.Ruc = request.Ruc;
        config.BusinessName = request.BusinessName;
        config.TradeName = request.TradeName;
        config.MainAddress = request.MainAddress;
        config.EstablishmentAddress = request.EstablishmentAddress;
        config.ObligadoContabilidad = request.ObligadoContabilidad;
        config.ContribuyenteEspecial = request.ContribuyenteEspecial;
        config.Establishment = request.Establishment;
        config.EmissionPoint = request.EmissionPoint;
        config.IsActive = request.IsActive;
        config.AutoSendOnPayment = request.AutoSendOnPayment;
        config.AutoSendEmailOnAuth = request.AutoSendEmailOnAuth;
    }

    private static FiscalConfigurationDto MapToDto(FiscalConfiguration config)
    {
        return new FiscalConfigurationDto
        {
            Id = config.Id,
            Environment = config.Environment,
            Ruc = config.Ruc,
            BusinessName = config.BusinessName,
            TradeName = config.TradeName,
            MainAddress = config.MainAddress,
            EstablishmentAddress = config.EstablishmentAddress,
            ObligadoContabilidad = config.ObligadoContabilidad,
            ContribuyenteEspecial = config.ContribuyenteEspecial,
            Establishment = config.Establishment,
            EmissionPoint = config.EmissionPoint,
            HasCertificate = config.CertificateData != null,
            CertificateExpiration = config.CertificateExpiration,
            NextInvoiceSequential = config.NextInvoiceSequential,
            NextCreditNoteSequential = config.NextCreditNoteSequential,
            NextWithholdingSequential = config.NextWithholdingSequential,
            IsActive = config.IsActive,
            AutoSendOnPayment = config.AutoSendOnPayment,
            AutoSendEmailOnAuth = config.AutoSendEmailOnAuth
        };
    }
}
