namespace Restify.Auth.Application.DTOs.Auth;

/// <summary>
/// Request para iniciar sesión
/// </summary>
public record LoginRequest(
    string Email,
    string Password,
    string? TenantSlug = null
);
