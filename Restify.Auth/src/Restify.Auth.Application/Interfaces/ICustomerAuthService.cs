using Restify.Auth.Application.DTOs.Auth;
using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.CustomerAuth;

namespace Restify.Auth.Application.Interfaces;

/// <summary>
/// Servicio de autenticación para clientes (consumidores del restaurante)
/// </summary>
public interface ICustomerAuthService
{
    /// <summary>
    /// Registra un nuevo cliente y retorna tokens de acceso
    /// </summary>
    Task<Result<CustomerLoginResponse>> RegisterAsync(CustomerRegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Autentica un cliente con email y contraseña
    /// </summary>
    Task<Result<CustomerLoginResponse>> LoginAsync(CustomerLoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refresca el access token del cliente usando el refresh token
    /// </summary>
    Task<Result<TokenResponse>> RefreshTokenAsync(CustomerRefreshTokenRequest request, CancellationToken cancellationToken = default);
}
