using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Restify.Auth.Domain.Constants;
using Restify.Auth.Domain.Entities;

namespace Restify.Auth.Infrastructure.Services;

/// <summary>
/// Servicio para generación y validación de JWT
/// </summary>
public class JwtService
{
    private readonly JwtSettings _settings;

    public JwtService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    /// <summary>
    /// Genera un access token JWT para el usuario
    /// </summary>
    public string GenerateAccessToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var isSuperAdmin = user.TenantId == SuperAdminConstants.TenantId;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimConstants.FirstName, user.FirstName),
            new(ClaimConstants.LastName, user.LastName)
        };

        if (isSuperAdmin)
        {
            claims.Add(new Claim(ClaimConstants.UserType, UserTypes.SuperAdmin));
            claims.Add(new Claim(ClaimConstants.IsSuperAdmin, "true"));
        }
        else
        {
            claims.Add(new Claim(ClaimConstants.TenantId, user.TenantId.ToString()));
            claims.Add(new Claim(ClaimConstants.UserType, UserTypes.Staff));
        }

        // Agregar roles
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Agregar permisos
        foreach (var permission in permissions)
        {
            claims.Add(new Claim(ClaimConstants.Permission, permission));
        }

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Genera un access token JWT para un cliente (consumidor del restaurante)
    /// </summary>
    public string GenerateCustomerAccessToken(Guid customerId, string email, string firstName, string lastName, Guid tenantId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, customerId.ToString()),
            new(ClaimTypes.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimConstants.TenantId, tenantId.ToString()),
            new(ClaimConstants.FirstName, firstName),
            new(ClaimConstants.LastName, lastName),
            new(ClaimConstants.UserType, UserTypes.Customer)
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Genera un access token JWT para un motorizado (repartidor de delivery)
    /// </summary>
    public string GenerateDriverAccessToken(Guid driverId, string email, string firstName, string lastName, Guid tenantId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, driverId.ToString()),
            new(ClaimTypes.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimConstants.TenantId, tenantId.ToString()),
            new(ClaimConstants.FirstName, firstName),
            new(ClaimConstants.LastName, lastName),
            new(ClaimConstants.UserType, UserTypes.Driver)
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Genera un access token JWT para un repartidor del pool centralizado (sin tenant)
    /// </summary>
    public string GeneratePoolDriverAccessToken(Guid driverId, string email, string firstName, string lastName, Guid? deliveryZoneId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, driverId.ToString()),
            new(ClaimTypes.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimConstants.FirstName, firstName),
            new(ClaimConstants.LastName, lastName),
            new(ClaimConstants.UserType, UserTypes.PoolDriver),
            new(ClaimConstants.IsPoolDriver, "true")
        };

        if (deliveryZoneId.HasValue)
            claims.Add(new Claim(ClaimConstants.DeliveryZoneId, deliveryZoneId.Value.ToString()));

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Genera un refresh token aleatorio
    /// </summary>
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    /// <summary>
    /// Obtiene el ClaimsPrincipal de un token expirado
    /// </summary>
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey)),
            ValidIssuer = _settings.Issuer,
            ValidAudience = _settings.Audience,
            ValidateLifetime = false // No validar expiración
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Obtiene la fecha de expiración del refresh token
    /// </summary>
    public DateTime GetRefreshTokenExpiration()
    {
        return DateTime.UtcNow.AddDays(_settings.RefreshTokenExpirationDays);
    }
}
