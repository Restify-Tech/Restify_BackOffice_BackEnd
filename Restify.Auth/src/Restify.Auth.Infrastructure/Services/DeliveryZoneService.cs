using Microsoft.EntityFrameworkCore;
using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.DeliveryZones;
using Restify.Auth.Application.Interfaces;
using Restify.Auth.Domain.Entities;
using Restify.Auth.Infrastructure.Persistence;

namespace Restify.Auth.Infrastructure.Services;

public class DeliveryZoneService : IDeliveryZoneService
{
    private readonly AppDbContext _context;

    public DeliveryZoneService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<DeliveryZoneListDto>>> GetAllAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var query = _context.DeliveryZones.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(z => z.Name.ToLower().Contains(search)
                || z.City.ToLower().Contains(search)
                || (z.Region != null && z.Region.ToLower().Contains(search)));
        }

        query = request.OrderBy?.ToLower() switch
        {
            "name" => request.Descending ? query.OrderByDescending(z => z.Name) : query.OrderBy(z => z.Name),
            "city" => request.Descending ? query.OrderByDescending(z => z.City) : query.OrderBy(z => z.City),
            _ => query.OrderBy(z => z.Name)
        };

        var zones = await query.Select(z => new DeliveryZoneListDto(
            z.Id, z.Name, z.City, z.Region, z.Country,
            z.DefaultCommissionPercentage, z.IsActive
        )).ToListAsync(cancellationToken);

        return Result<IEnumerable<DeliveryZoneListDto>>.Success(zones);
    }

    public async Task<Result<DeliveryZoneDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var zone = await _context.DeliveryZones
            .AsNoTracking()
            .FirstOrDefaultAsync(z => z.Id == id, cancellationToken);

        if (zone == null)
            return Result<DeliveryZoneDto>.Failure("Zona de delivery no encontrada");

        return Result<DeliveryZoneDto>.Success(MapToDto(zone));
    }

    public async Task<Result<DeliveryZoneDto>> CreateAsync(CreateDeliveryZoneRequest request, CancellationToken cancellationToken = default)
    {
        var exists = await _context.DeliveryZones
            .AnyAsync(z => z.Name.ToLower() == request.Name.ToLower() && z.City.ToLower() == request.City.ToLower(), cancellationToken);

        if (exists)
            return Result<DeliveryZoneDto>.Failure("Ya existe una zona con ese nombre en esa ciudad");

        var zone = new DeliveryZone
        {
            Name = request.Name,
            City = request.City,
            Region = request.Region,
            Country = request.Country,
            DefaultCommissionPercentage = request.DefaultCommissionPercentage,
            MaxDeliveryRadiusKm = request.MaxDeliveryRadiusKm,
            MinDriverRating = request.MinDriverRating
        };

        _context.DeliveryZones.Add(zone);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<DeliveryZoneDto>.Success(MapToDto(zone));
    }

    public async Task<Result<DeliveryZoneDto>> UpdateAsync(Guid id, UpdateDeliveryZoneRequest request, CancellationToken cancellationToken = default)
    {
        var zone = await _context.DeliveryZones.FindAsync([id], cancellationToken);

        if (zone == null)
            return Result<DeliveryZoneDto>.Failure("Zona de delivery no encontrada");

        zone.Name = request.Name;
        zone.City = request.City;
        zone.Region = request.Region;
        zone.Country = request.Country;
        zone.DefaultCommissionPercentage = request.DefaultCommissionPercentage;
        zone.MaxDeliveryRadiusKm = request.MaxDeliveryRadiusKm;
        zone.MinDriverRating = request.MinDriverRating;
        zone.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<DeliveryZoneDto>.Success(MapToDto(zone));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var zone = await _context.DeliveryZones.FindAsync([id], cancellationToken);

        if (zone == null)
            return Result.Failure("Zona de delivery no encontrada");

        _context.DeliveryZones.Remove(zone);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static DeliveryZoneDto MapToDto(DeliveryZone zone) => new(
        zone.Id, zone.Name, zone.City, zone.Region, zone.Country,
        zone.DefaultCommissionPercentage, zone.MaxDeliveryRadiusKm,
        zone.MinDriverRating, zone.IsActive, zone.CreatedAt
    );
}
