using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.Tenant;
using Restify.Auth.Application.Interfaces;
using Restify.Auth.Domain.Entities;
using Restify.Auth.Infrastructure.Persistence;

namespace Restify.Auth.Infrastructure.Services;

public class TenantManagementService : ITenantManagementService
{
    private readonly AppDbContext _context;
    private readonly IFileStorageService _fileStorage;
    private readonly ILogger<TenantManagementService> _logger;

    public TenantManagementService(AppDbContext context, IFileStorageService fileStorage, ILogger<TenantManagementService> logger)
    {
        _context = context;
        _fileStorage = fileStorage;
        _logger = logger;
    }

    public async Task<Result<PagedResponse<TenantListDto>>> GetAllAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var query = _context.Tenants.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(t => t.Name.ToLower().Contains(search)
                || t.Slug.ToLower().Contains(search)
                || (t.Email != null && t.Email.ToLower().Contains(search)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = request.OrderBy?.ToLower() switch
        {
            "name" => request.Descending ? query.OrderByDescending(t => t.Name) : query.OrderBy(t => t.Name),
            "status" => request.Descending ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),
            "createdat" => request.Descending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt),
            _ => query.OrderByDescending(t => t.CreatedAt)
        };

        var tenants = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TenantListDto(
                t.Id, t.Name, t.Slug, t.Email, t.Status,
                t.DeliveryOperationMode, t.OnboardingCompleted, t.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);
        var response = new PagedResponse<TenantListDto>(
            tenants, request.Page, request.PageSize, totalCount, totalPages
        );

        return Result<PagedResponse<TenantListDto>>.Success(response);
    }

    public async Task<Result<TenantDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tenant = await _context.Tenants
            .AsNoTracking()
            .Include(t => t.DeliveryZone)
            .Include(t => t.Users)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (tenant == null)
            return Result<TenantDto>.Failure("Tenant no encontrado");

        return Result<TenantDto>.Success(MapToDto(tenant));
    }

    public async Task<Result<TenantDto>> UpdateAsync(Guid id, UpdateTenantRequest request, CancellationToken cancellationToken = default)
    {
        var tenant = await _context.Tenants
            .Include(t => t.DeliveryZone)
            .Include(t => t.Users)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (tenant == null)
            return Result<TenantDto>.Failure("Tenant no encontrado");

        tenant.Name = request.Name;
        tenant.BusinessName = request.BusinessName;
        tenant.Address = request.Address;
        tenant.Phone = request.Phone;
        tenant.Email = request.Email;
        tenant.Currency = request.Currency;
        tenant.TaxPercentage = request.TaxPercentage;
        tenant.TimeZone = request.TimeZone;

        if (request.DeliveryOperationMode.HasValue)
            tenant.DeliveryOperationMode = request.DeliveryOperationMode.Value;

        tenant.DeliveryZoneId = request.DeliveryZoneId;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<TenantDto>.Success(MapToDto(tenant));
    }

    public async Task<Result> UpdateStatusAsync(Guid id, UpdateTenantStatusRequest request, CancellationToken cancellationToken = default)
    {
        var tenant = await _context.Tenants.FindAsync([id], cancellationToken);

        if (tenant == null)
            return Result.Failure("Tenant no encontrado");

        tenant.Status = request.Status;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> UpdateDeliveryModeAsync(Guid id, UpdateTenantDeliveryModeRequest request, CancellationToken cancellationToken = default)
    {
        var tenant = await _context.Tenants.FindAsync([id], cancellationToken);

        if (tenant == null)
            return Result.Failure("Tenant no encontrado");

        if (request.DeliveryOperationMode != Domain.Enums.DeliveryOperationMode.Standalone && request.DeliveryZoneId == null)
            return Result.Failure("Debe asignar una zona de delivery para los modos Networked o Hybrid");

        if (request.DeliveryZoneId.HasValue)
        {
            var zoneExists = await _context.DeliveryZones.AnyAsync(z => z.Id == request.DeliveryZoneId.Value, cancellationToken);
            if (!zoneExists)
                return Result.Failure("La zona de delivery especificada no existe");
        }

        tenant.DeliveryOperationMode = request.DeliveryOperationMode;
        tenant.DeliveryZoneId = request.DeliveryZoneId;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<TenantBrandingDto>> GetBrandingByIdentificationAsync(string identificationNumber, CancellationToken cancellationToken = default)
    {
        var tenant = await _context.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t =>
                t.IdentificationNumber == identificationNumber || t.Ruc == identificationNumber,
                cancellationToken);

        if (tenant == null)
            return Result<TenantBrandingDto>.Failure("No se encontró un restaurante con esa identificación");

        if (tenant.Status != Domain.Enums.TenantStatus.Active)
            return Result<TenantBrandingDto>.Failure("El restaurante no está activo");

        return Result<TenantBrandingDto>.Success(MapToBrandingDto(tenant));
    }

    public async Task<Result<TenantBrandingDto>> GetBrandingBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var tenant = await _context.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Slug == slug, cancellationToken);

        if (tenant == null)
            return Result<TenantBrandingDto>.Failure("Restaurante no encontrado");

        return Result<TenantBrandingDto>.Success(MapToBrandingDto(tenant));
    }

    public async Task<Result<TenantBrandingDto>> GetBrandingByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tenant = await _context.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (tenant == null)
            return Result<TenantBrandingDto>.Failure("Tenant no encontrado");

        return Result<TenantBrandingDto>.Success(MapToBrandingDto(tenant));
    }

    public async Task<Result<TenantBrandingDto>> UpdateBrandingAsync(Guid id, UpdateTenantBrandingRequest request, CancellationToken cancellationToken = default)
    {
        var tenant = await _context.Tenants.FindAsync([id], cancellationToken);

        if (tenant == null)
            return Result<TenantBrandingDto>.Failure("Tenant no encontrado");

        if (request.PrimaryColor != null) tenant.PrimaryColor = request.PrimaryColor;
        if (request.SecondaryColor != null) tenant.SecondaryColor = request.SecondaryColor;
        if (request.AccentColor != null) tenant.AccentColor = request.AccentColor;
        if (request.TemplateName != null) tenant.TemplateName = request.TemplateName;
        if (request.FontHeading != null) tenant.FontHeading = request.FontHeading;
        if (request.FontBody != null) tenant.FontBody = request.FontBody;
        if (request.CustomCss != null) tenant.CustomCss = request.CustomCss;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Branding actualizado para tenant {TenantId}", id);

        return Result<TenantBrandingDto>.Success(MapToBrandingDto(tenant));
    }

    public async Task<Result<string>> UploadLogoAsync(Guid id, Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        var tenant = await _context.Tenants.FindAsync([id], cancellationToken);

        if (tenant == null)
            return Result<string>.Failure("Tenant no encontrado");

        var url = await _fileStorage.SaveFileAsync(fileStream, fileName, "tenant-logos", cancellationToken);
        tenant.LogoUrl = url;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Logo actualizado para tenant {TenantId}: {Url}", id, url);

        return Result<string>.Success(url);
    }

    public async Task<Result<string>> UploadCoverImageAsync(Guid id, Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        var tenant = await _context.Tenants.FindAsync([id], cancellationToken);

        if (tenant == null)
            return Result<string>.Failure("Tenant no encontrado");

        var url = await _fileStorage.SaveFileAsync(fileStream, fileName, "tenant-covers", cancellationToken);
        tenant.CoverImageUrl = url;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cover image actualizado para tenant {TenantId}: {Url}", id, url);

        return Result<string>.Success(url);
    }

    public async Task<Result<IEnumerable<PublicTenantDto>>> GetActivePublicAsync(
        CancellationToken cancellationToken = default)
    {
        // Obtener tenants activos — sin filtro de TenantId (es endpoint publico)
        var tenants = await _context.Tenants
            .AsNoTracking()
            .Where(t => t.Status == Domain.Enums.TenantStatus.Active)
            .OrderBy(t => t.Name)
            .Select(t => new PublicTenantDto(
                t.Id,
                t.Name,
                t.Slug,
                t.LogoUrl,
                t.Address
            ))
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<PublicTenantDto>>.Success(tenants);
    }

    private static TenantDto MapToDto(Tenant tenant)
    {
        return new TenantDto(
            tenant.Id, tenant.Name, tenant.Slug, tenant.Ruc, tenant.BusinessName,
            tenant.Address, tenant.Phone, tenant.Email, tenant.LogoUrl,
            tenant.SignatureUrl, tenant.FullAddress, tenant.Latitude, tenant.Longitude,
            tenant.IdentificationType, tenant.IdentificationNumber,
            tenant.PrimaryColor, tenant.SecondaryColor, tenant.AccentColor,
            tenant.TemplateName, tenant.FaviconUrl, tenant.CoverImageUrl,
            tenant.FontHeading, tenant.FontBody, tenant.CustomCss,
            tenant.Currency, tenant.TaxPercentage, tenant.TimeZone,
            tenant.Status, tenant.DeliveryOperationMode, tenant.DeliveryZoneId,
            tenant.DeliveryZone?.Name, tenant.OnboardingCompleted,
            tenant.TrialExpiresAt, tenant.CreatedAt, tenant.Users.Count
        );
    }

    private static TenantBrandingDto MapToBrandingDto(Tenant tenant)
    {
        return new TenantBrandingDto(
            tenant.Name, tenant.Slug, tenant.LogoUrl, tenant.CoverImageUrl,
            tenant.FaviconUrl, tenant.PrimaryColor, tenant.SecondaryColor,
            tenant.AccentColor, tenant.TemplateName, tenant.FontHeading,
            tenant.FontBody, tenant.CustomCss, tenant.Currency, tenant.TaxPercentage
        );
    }
}
