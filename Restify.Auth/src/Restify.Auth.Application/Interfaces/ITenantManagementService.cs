using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.Tenant;

namespace Restify.Auth.Application.Interfaces;

public interface ITenantManagementService
{
    Task<Result<PagedResponse<TenantListDto>>> GetAllAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<TenantDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<TenantDto>> UpdateAsync(Guid id, UpdateTenantRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateStatusAsync(Guid id, UpdateTenantStatusRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateDeliveryModeAsync(Guid id, UpdateTenantDeliveryModeRequest request, CancellationToken cancellationToken = default);
    Task<Result<TenantBrandingDto>> GetBrandingByIdentificationAsync(string identificationNumber, CancellationToken cancellationToken = default);
    Task<Result<TenantBrandingDto>> GetBrandingBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Result<TenantBrandingDto>> GetBrandingByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<TenantBrandingDto>> UpdateBrandingAsync(Guid id, UpdateTenantBrandingRequest request, CancellationToken cancellationToken = default);
    Task<Result<string>> UploadLogoAsync(Guid id, Stream fileStream, string fileName, CancellationToken cancellationToken = default);
    Task<Result<string>> UploadCoverImageAsync(Guid id, Stream fileStream, string fileName, CancellationToken cancellationToken = default);
}
