namespace Restify.Auth.Application.DTOs.Auth;

/// <summary>
/// Request para refrescar el token
/// </summary>
public record RefreshTokenRequest(
    string AccessToken,
    string RefreshToken
);
