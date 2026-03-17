namespace Restify.Auth.Application.DTOs.Auth;

/// <summary>
/// Response con los nuevos tokens
/// </summary>
public record TokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);
