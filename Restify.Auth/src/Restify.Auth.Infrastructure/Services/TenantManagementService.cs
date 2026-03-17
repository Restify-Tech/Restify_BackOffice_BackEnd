using Microsoft.EntityFrameworkCore;
using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.Tenant;
using Restify.Auth.Application.Interfaces;
using Restify.Auth.Infrastructure.Persistence;

namespace Restify.Auth.Infrastructure.Services;

public class TenantManagementService : ITenantManagementService
{
    private readonly AppDbContext _context;

    public TenantManagementService(AppDbContext context)
    {
        _context = context;
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
}
