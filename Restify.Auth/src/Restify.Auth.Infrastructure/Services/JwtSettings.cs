namespace Restify.Auth.Infrastructure.Services;

/// <summary>
/// Configuración de JWT
/// </summary>
public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    /// <summary>
    /// Clave secreta para firmar tokens
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Emisor del token
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Audiencia del token
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Tiempo de expiración del access token en minutos
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Tiempo de expiración del refresh token en días
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; } = 7;
}
