using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.Tenant;

namespace Restify.Auth.Application.Interfaces;

/// <summary>
/// Servicio para registro público de nuevos tenants (restaurantes)
/// </summary>
public interface ITenantRegistrationService
{
    /// <summary>
    /// Registra un nuevo tenant con su usuario admin, rol y permisos base.
    /// Retorna JWT para auto-login.
    /// </summary>
    Task<Result<TenantRegisterResponse>> RegisterAsync(TenantRegisterRequest request, CancellationToken cancellationToken = default);
}
