using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Enums;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class PoolDriverService : IPoolDriverService
{
    private readonly BackOfficeDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public PoolDriverService(BackOfficeDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResponse<PoolDriverListDto>>> GetAllPoolDriversAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var query = _context.DeliveryDrivers
            .IgnoreQueryFilters()
            .Where(d => d.IsPoolDriver)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(d =>
                d.FirstName.ToLower().Contains(s) ||
                d.LastName.ToLower().Contains(s) ||
                d.Email.ToLower().Contains(s) ||
                d.IdentificationNumber.Contains(s));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var drivers = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new PoolDriverListDto(
                d.Id, d.FirstName, d.LastName, d.Email, d.Phone,
                d.IdentificationNumber, d.VehicleType, d.VehiclePlate,
                d.VerificationStatus, d.Rating, d.TotalDeliveries,
                d.IsActive, d.DeliveryZoneId, d.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        var response = new PagedResponse<PoolDriverListDto>
        {
            Items = drivers,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return Result<PagedResponse<PoolDriverListDto>>.Success(response);
    }

    public async Task<Result<PoolDriverDetailDto>> GetPoolDriverByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var driver = await _context.DeliveryDrivers
            .IgnoreQueryFilters()
            .Include(d => d.Documents)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id && d.IsPoolDriver, cancellationToken);

        if (driver == null)
            return Result<PoolDriverDetailDto>.Failure("Repartidor pool no encontrado");

        var dto = new PoolDriverDetailDto(
            driver.Id, driver.FirstName, driver.LastName, driver.Email, driver.Phone,
            driver.IdentificationNumber, driver.VehicleType, driver.VehiclePlate,
            driver.VehicleBrand, driver.VehicleModel, driver.VehicleYear, driver.VehicleColor,
            driver.PhotoUrl, driver.VerificationStatus, driver.VerifiedAt, driver.RejectionReason,
            driver.Rating, driver.TotalDeliveries, driver.IsActive, driver.DeliveryZoneId,
            driver.CreatedAt,
            driver.Documents.Select(doc => new DriverDocumentDto(
                doc.Id, doc.DocumentType, doc.FileUrl, doc.FileName,
                doc.Status, doc.ReviewedAt, doc.RejectionReason, doc.UploadedAt
            ))
        );

        return Result<PoolDriverDetailDto>.Success(dto);
    }

    public async Task<Result> ApproveDriverAsync(Guid id, ApproveDriverRequest request, CancellationToken cancellationToken = default)
    {
        var driver = await _context.DeliveryDrivers
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(d => d.Id == id && d.IsPoolDriver, cancellationToken);

        if (driver == null)
            return Result.Failure("Repartidor pool no encontrado");

        driver.VerificationStatus = DriverVerificationStatus.Approved;
        driver.IsVerified = true;
        driver.VerifiedAt = DateTime.UtcNow;
        driver.VerifiedByUserId = _currentUser.UserId;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> RejectDriverAsync(Guid id, RejectDriverRequest request, CancellationToken cancellationToken = default)
    {
        var driver = await _context.DeliveryDrivers
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(d => d.Id == id && d.IsPoolDriver, cancellationToken);

        if (driver == null)
            return Result.Failure("Repartidor pool no encontrado");

        driver.VerificationStatus = DriverVerificationStatus.Rejected;
        driver.RejectionReason = request.RejectionReason;
        driver.VerifiedAt = DateTime.UtcNow;
        driver.VerifiedByUserId = _currentUser.UserId;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ReviewDocumentAsync(Guid driverId, Guid documentId, ReviewDocumentRequest request, CancellationToken cancellationToken = default)
    {
        var document = await _context.DriverDocuments
            .FirstOrDefaultAsync(d => d.Id == documentId && d.DriverId == driverId, cancellationToken);

        if (document == null)
            return Result.Failure("Documento no encontrado");

        document.Status = request.Status;
        document.RejectionReason = request.RejectionReason;
        document.ReviewedAt = DateTime.UtcNow;
        document.ReviewedByUserId = _currentUser.UserId;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
