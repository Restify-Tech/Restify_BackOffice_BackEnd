using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restify.Auth.Application.DTOs.Auth;
using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.Interfaces;
using Restify.Auth.Domain.Entities;
using Restify.Auth.Domain.Enums;
using Restify.Auth.Infrastructure.Persistence;

namespace Restify.Auth.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de autenticación
/// </summary>
public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;
    private readonly PasswordService _passwordService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        AppDbContext context,
        JwtService jwtService,
        PasswordService passwordService,
        ILogger<AuthService> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _passwordService = passwordService;
        _logger = logger;
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        // Resolver tenant por slug o por identificacion (RUC/cedula)
        Tenant? tenant = null;
        if (!string.IsNullOrEmpty(request.TenantSlug))
        {
            tenant = await _context.Tenants
                .FirstOrDefaultAsync(t => t.Slug == request.TenantSlug && t.Status == TenantStatus.Active, cancellationToken);

            if (tenant == null)
                return Result<LoginResponse>.Failure("Tenant no encontrado o inactivo");
        }
        else if (!string.IsNullOrEmpty(request.IdentificationNumber))
        {
            tenant = await _context.Tenants
                .FirstOrDefaultAsync(t =>
                    (t.IdentificationNumber == request.IdentificationNumber || t.Ruc == request.IdentificationNumber)
                    && t.Status == TenantStatus.Active, cancellationToken);

            if (tenant == null)
                return Result<LoginResponse>.Failure("No se encontró un restaurante con esa identificación");
        }

        // Buscar usuario por email o username
        var query = _context.Users
            .IgnoreQueryFilters()
            .Include(u => u.Tenant)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Email))
            query = query.Where(u => u.Email == request.Email);
        else if (!string.IsNullOrEmpty(request.Username))
            query = query.Where(u => u.Username == request.Username);
        else
            return Result<LoginResponse>.Failure("Debe proporcionar un email o nombre de usuario");

        if (tenant != null)
            query = query.Where(u => u.TenantId == tenant.Id);

        var user = await query.FirstOrDefaultAsync(cancellationToken);

        if (user == null)
            return Result<LoginResponse>.Failure("Credenciales inválidas");

        if (user.Status != UserStatus.Active)
            return Result<LoginResponse>.Failure("Usuario inactivo o bloqueado");

        if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
        {
            var credential = request.Email ?? request.Username;
            _logger.LogWarning("Intento de login fallido para {Credential}", credential);
            return Result<LoginResponse>.Failure("Credenciales inválidas");
        }

        // Obtener roles y permisos
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToList();

        // Generar tokens
        var accessToken = _jwtService.GenerateAccessToken(user, roles, permissions);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Guardar refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = _jwtService.GetRefreshTokenExpiration();
        user.LastLoginAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var logCredential = request.Email ?? request.Username;
        _logger.LogInformation("Login exitoso para {Credential}", logCredential);

        return Result<LoginResponse>.Success(new LoginResponse(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(60),
            new UserInfoDto(
                user.Id,
                user.Email,
                user.Username,
                user.FirstName,
                user.LastName,
                user.TenantId,
                user.Tenant.Name,
                roles,
                permissions,
                user.Tenant.TaxPercentage,
                user.Tenant.Currency
            )
        ));
    }

    public async Task<Result<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
            return Result<TokenResponse>.Failure("Token inválido");

        var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Result<TokenResponse>.Failure("Token inválido");

        var user = await _context.Users
            .IgnoreQueryFilters()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            return Result<TokenResponse>.Failure("Usuario no encontrado");

        if (user.RefreshToken != request.RefreshToken)
            return Result<TokenResponse>.Failure("Refresh token inválido");

        if (user.RefreshTokenExpiresAt <= DateTime.UtcNow)
            return Result<TokenResponse>.Failure("Refresh token expirado");

        // Obtener roles y permisos
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToList();

        // Generar nuevos tokens
        var newAccessToken = _jwtService.GenerateAccessToken(user, roles, permissions);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiresAt = _jwtService.GetRefreshTokenExpiration();

        await _context.SaveChangesAsync(cancellationToken);

        return Result<TokenResponse>.Success(new TokenResponse(
            newAccessToken,
            newRefreshToken,
            DateTime.UtcNow.AddMinutes(60)
        ));
    }

    public async Task<Result> LogoutAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            return Result.Failure("Usuario no encontrado");

        user.RefreshToken = null;
        user.RefreshTokenExpiresAt = null;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Logout para usuario {UserId}", userId);

        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        if (request.NewPassword != request.ConfirmPassword)
            return Result.Failure("Las contraseñas no coinciden");

        var user = await _context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            return Result.Failure("Usuario no encontrado");

        if (!_passwordService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            return Result.Failure("Contraseña actual incorrecta");

        user.PasswordHash = _passwordService.HashPassword(request.NewPassword);
        user.RefreshToken = null;
        user.RefreshTokenExpiresAt = null;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Contraseña cambiada para usuario {UserId}", userId);

        return Result.Success();
    }

    public async Task<Result<UserInfoDto>> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .IgnoreQueryFilters()
            .Include(u => u.Tenant)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            return Result<UserInfoDto>.Failure("Usuario no encontrado");

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToList();

        return Result<UserInfoDto>.Success(new UserInfoDto(
            user.Id,
            user.Email,
            user.Username,
            user.FirstName,
            user.LastName,
            user.TenantId,
            user.Tenant.Name,
            roles,
            permissions,
            user.Tenant.TaxPercentage,
            user.Tenant.Currency
        ));
    }
}
