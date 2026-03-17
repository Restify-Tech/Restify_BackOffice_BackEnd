namespace Restify.Auth.Application.DTOs.Auth;

/// <summary>
/// Response del login exitoso
/// </summary>
public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserInfoDto User
);

/// <summary>
/// Información básica del usuario autenticado
/// </summary>
public record UserInfoDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    Guid TenantId,
    string TenantName,
    IEnumerable<string> Roles,
    IEnumerable<string> Permissions,
    decimal TaxPercentage = 15.00m,
    string Currency = "USD"
);
