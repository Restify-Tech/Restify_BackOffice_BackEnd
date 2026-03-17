using Restify.Auth.Application.DTOs.Auth;
using Restify.Auth.Application.DTOs.Common;

namespace Restify.Auth.Application.Interfaces;

/// <summary>
/// Servicio de autenticación
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Inicia sesión con email y contraseña
    /// </summary>
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refresca el access token usando el refresh token
    /// </summary>
    Task<Result<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cierra sesión e invalida el refresh token
    /// </summary>
    Task<Result> LogoutAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cambia la contraseña del usuario actual
    /// </summary>
    Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene información del usuario actual
    /// </summary>
    Task<Result<UserInfoDto>> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
