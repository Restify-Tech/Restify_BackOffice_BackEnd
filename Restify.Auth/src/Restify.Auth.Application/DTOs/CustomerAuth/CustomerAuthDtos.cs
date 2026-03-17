namespace Restify.Auth.Application.DTOs.CustomerAuth;

/// <summary>
/// Request para registro de cliente
/// </summary>
public record CustomerRegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string? Phone,
    string TenantSlug);

/// <summary>
/// Request para login de cliente
/// </summary>
public record CustomerLoginRequest(
    string Email,
    string Password,
    string TenantSlug);

/// <summary>
/// Response de login/registro de cliente
/// </summary>
public record CustomerLoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    CustomerInfoDto Customer);

/// <summary>
/// Información del cliente autenticado
/// </summary>
public record CustomerInfoDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? Phone,
    Guid TenantId,
    string TenantName);

/// <summary>
/// Request para refrescar token de cliente
/// </summary>
public record CustomerRefreshTokenRequest(
    string AccessToken,
    string RefreshToken);
