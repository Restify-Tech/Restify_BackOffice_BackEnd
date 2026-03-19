namespace Restify.Auth.Application.DTOs.Auth;

/// <summary>
/// Request para iniciar sesión
/// </summary>
public record LoginRequest(
    string? Email,
    string? Username,
    string Password,
    string? TenantSlug = null,
    string? IdentificationNumber = null
);
