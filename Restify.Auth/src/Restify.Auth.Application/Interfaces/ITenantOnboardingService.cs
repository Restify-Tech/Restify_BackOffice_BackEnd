using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.Tenant;

namespace Restify.Auth.Application.Interfaces;

/// <summary>
/// Servicio para el proceso de onboarding del tenant
/// </summary>
public interface ITenantOnboardingService
{
    /// <summary>
    /// Actualiza los datos de onboarding del tenant.
    /// Marca OnboardingCompleted = true cuando todos los campos requeridos están presentes.
    /// </summary>
    Task<Result<TenantDto>> UpdateOnboardingAsync(Guid tenantId, TenantOnboardingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sube el logo del tenant y retorna la URL
    /// </summary>
    Task<Result<string>> UploadLogoAsync(Guid tenantId, Stream file, string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sube la firma del tenant y retorna la URL
    /// </summary>
    Task<Result<string>> UploadSignatureAsync(Guid tenantId, Stream file, string fileName, CancellationToken cancellationToken = default);
}
