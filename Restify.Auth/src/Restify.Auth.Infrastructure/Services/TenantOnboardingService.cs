using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.Tenant;
using Restify.Auth.Application.Interfaces;
using Restify.Auth.Infrastructure.Persistence;

namespace Restify.Auth.Infrastructure.Services;

/// <summary>
/// Servicio para el proceso de onboarding del tenant
/// </summary>
public class TenantOnboardingService : ITenantOnboardingService
{
    private readonly AppDbContext _context;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<TenantOnboardingService> _logger;

    public TenantOnboardingService(
        AppDbContext context,
        IFileStorageService fileStorageService,
        ILogger<TenantOnboardingService> logger)
    {
        _context = context;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<Result<TenantDto>> UpdateOnboardingAsync(
        Guid tenantId,
        TenantOnboardingRequest request,
        CancellationToken cancellationToken = default)
    {
        var tenant = await _context.Tenants
            .Include(t => t.DeliveryZone)
            .Include(t => t.Users)
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);

        if (tenant == null)
            return Result<TenantDto>.Failure("Tenant no encontrado");

        // Actualizar solo los campos que vienen en el request (partial update)
        if (request.Name != null)
            tenant.Name = request.Name;

        if (request.LogoUrl != null)
            tenant.LogoUrl = request.LogoUrl;

        if (request.SignatureUrl != null)
            tenant.SignatureUrl = request.SignatureUrl;

        if (request.FullAddress != null)
            tenant.FullAddress = request.FullAddress;

        if (request.Latitude.HasValue)
            tenant.Latitude = request.Latitude.Value;

        if (request.Longitude.HasValue)
            tenant.Longitude = request.Longitude.Value;

        if (request.Currency != null)
            tenant.Currency = request.Currency;

        if (request.TaxPercentage.HasValue)
            tenant.TaxPercentage = request.TaxPercentage.Value;

        if (request.TimeZone != null)
            tenant.TimeZone = request.TimeZone;

        // Verificar si el onboarding está completo
        // Campos requeridos: Name, FullAddress, Currency, TaxPercentage, TimeZone
        tenant.OnboardingCompleted = !string.IsNullOrWhiteSpace(tenant.Name)
            && !string.IsNullOrWhiteSpace(tenant.FullAddress)
            && !string.IsNullOrWhiteSpace(tenant.Currency)
            && tenant.TaxPercentage > 0
            && !string.IsNullOrWhiteSpace(tenant.TimeZone);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Onboarding actualizado para tenant {TenantId}. Completado: {Completed}",
            tenantId, tenant.OnboardingCompleted);

        var dto = new TenantDto(
            tenant.Id, tenant.Name, tenant.Slug, tenant.Ruc, tenant.BusinessName,
            tenant.Address, tenant.Phone, tenant.Email, tenant.LogoUrl,
            tenant.SignatureUrl, tenant.FullAddress, tenant.Latitude, tenant.Longitude,
            tenant.IdentificationType, tenant.IdentificationNumber,
            tenant.Currency, tenant.TaxPercentage, tenant.TimeZone,
            tenant.Status, tenant.DeliveryOperationMode, tenant.DeliveryZoneId,
            tenant.DeliveryZone?.Name, tenant.OnboardingCompleted,
            tenant.TrialExpiresAt, tenant.CreatedAt, tenant.Users.Count
        );

        return Result<TenantDto>.Success(dto);
    }

    public async Task<Result<string>> UploadLogoAsync(
        Guid tenantId,
        Stream file,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var tenant = await _context.Tenants.FindAsync([tenantId], cancellationToken);

        if (tenant == null)
            return Result<string>.Failure("Tenant no encontrado");

        try
        {
            // Eliminar logo anterior si existe
            if (!string.IsNullOrEmpty(tenant.LogoUrl))
                await _fileStorageService.DeleteFileAsync(tenant.LogoUrl, cancellationToken);

            var url = await _fileStorageService.SaveFileAsync(file, fileName, "tenant-logos", cancellationToken);

            tenant.LogoUrl = url;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Logo subido para tenant {TenantId}: {Url}", tenantId, url);

            return Result<string>.Success(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al subir logo para tenant {TenantId}", tenantId);
            return Result<string>.Failure("Error al subir el logo. Intente nuevamente.");
        }
    }

    public async Task<Result<string>> UploadSignatureAsync(
        Guid tenantId,
        Stream file,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var tenant = await _context.Tenants.FindAsync([tenantId], cancellationToken);

        if (tenant == null)
            return Result<string>.Failure("Tenant no encontrado");

        try
        {
            // Eliminar firma anterior si existe
            if (!string.IsNullOrEmpty(tenant.SignatureUrl))
                await _fileStorageService.DeleteFileAsync(tenant.SignatureUrl, cancellationToken);

            var url = await _fileStorageService.SaveFileAsync(file, fileName, "tenant-signatures", cancellationToken);

            tenant.SignatureUrl = url;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Firma subida para tenant {TenantId}: {Url}", tenantId, url);

            return Result<string>.Success(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al subir firma para tenant {TenantId}", tenantId);
            return Result<string>.Failure("Error al subir la firma. Intente nuevamente.");
        }
    }
}
