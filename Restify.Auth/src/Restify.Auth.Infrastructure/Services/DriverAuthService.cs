using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restify.Auth.Application.DTOs.Auth;
using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.DriverAuth;
using Restify.Auth.Application.Interfaces;
using Restify.Auth.Domain.Constants;
using Restify.Auth.Infrastructure.Persistence;

namespace Restify.Auth.Infrastructure.Services;

/// <summary>
/// Servicio de autenticación para motorizados (repartidores de delivery).
/// Usa DriverDbContext para acceder a la tabla backoffice.DeliveryDrivers
/// y AppDbContext para consultar Tenants.
/// </summary>
public class DriverAuthService : IDriverAuthService
{
    private readonly DriverDbContext _driverDb;
    private readonly AppDbContext _authDb;
    private readonly JwtService _jwtService;
    private readonly PasswordService _passwordService;
    private readonly ILogger<DriverAuthService> _logger;

    public DriverAuthService(
        DriverDbContext driverDb,
        AppDbContext authDb,
        JwtService jwtService,
        PasswordService passwordService,
        ILogger<DriverAuthService> logger)
    {
        _driverDb = driverDb;
        _authDb = authDb;
        _jwtService = jwtService;
        _passwordService = passwordService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result<DriverLoginResponse>> RegisterAsync(DriverRegisterRequest request, CancellationToken cancellationToken = default)
    {
        // Buscar tenant por slug
        var tenant = await _authDb.Tenants
            .FirstOrDefaultAsync(t => t.Slug == request.TenantSlug, cancellationToken);

        if (tenant == null)
            return Result<DriverLoginResponse>.Failure("Restaurante no encontrado");

        // Verificar si ya existe un motorizado con ese email para este tenant
        var exists = await _driverDb.Drivers
            .AnyAsync(d => d.TenantId == tenant.Id && d.Email == request.Email, cancellationToken);

        if (exists)
            return Result<DriverLoginResponse>.Failure("Ya existe una cuenta con este email");

        var driver = new DriverRecord
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            IdentificationNumber = request.IdentificationNumber,
            VehiclePlate = request.VehiclePlate,
            VehicleDescription = request.VehicleDescription,
            PasswordHash = _passwordService.HashPassword(request.Password),
            IsActive = true,
            IsVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        // Generar tokens
        var accessToken = _jwtService.GenerateDriverAccessToken(
            driver.Id, driver.Email, driver.FirstName, driver.LastName, tenant.Id);
        var refreshToken = _jwtService.GenerateRefreshToken();

        driver.RefreshToken = refreshToken;
        driver.RefreshTokenExpiresAt = _jwtService.GetRefreshTokenExpiration();
        driver.LastLoginAt = DateTime.UtcNow;

        await _driverDb.Drivers.AddAsync(driver, cancellationToken);
        await _driverDb.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Motorizado registrado: {Email} para tenant {TenantSlug}", request.Email, request.TenantSlug);

        return Result<DriverLoginResponse>.Success(new DriverLoginResponse(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(60),
            new DriverInfoDto(driver.Id, driver.Email, driver.FirstName, driver.LastName, driver.Phone, tenant.Id, tenant.Name, driver.VehiclePlate, driver.IsVerified)
        ));
    }

    /// <inheritdoc />
    public async Task<Result<DriverLoginResponse>> LoginAsync(DriverLoginRequest request, CancellationToken cancellationToken = default)
    {
        // Buscar tenant por slug
        var tenant = await _authDb.Tenants
            .FirstOrDefaultAsync(t => t.Slug == request.TenantSlug, cancellationToken);

        if (tenant == null)
            return Result<DriverLoginResponse>.Failure("Restaurante no encontrado");

        // Buscar motorizado por email y tenant
        var driver = await _driverDb.Drivers
            .FirstOrDefaultAsync(d => d.TenantId == tenant.Id && d.Email == request.Email, cancellationToken);

        if (driver == null || !driver.IsActive)
            return Result<DriverLoginResponse>.Failure("Credenciales inválidas");

        if (!_passwordService.VerifyPassword(request.Password, driver.PasswordHash))
            return Result<DriverLoginResponse>.Failure("Credenciales inválidas");

        // Generar tokens
        var accessToken = _jwtService.GenerateDriverAccessToken(
            driver.Id, driver.Email, driver.FirstName, driver.LastName, tenant.Id);
        var refreshToken = _jwtService.GenerateRefreshToken();

        driver.RefreshToken = refreshToken;
        driver.RefreshTokenExpiresAt = _jwtService.GetRefreshTokenExpiration();
        driver.LastLoginAt = DateTime.UtcNow;

        await _driverDb.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Motorizado autenticado: {Email}", request.Email);

        return Result<DriverLoginResponse>.Success(new DriverLoginResponse(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(60),
            new DriverInfoDto(driver.Id, driver.Email, driver.FirstName, driver.LastName, driver.Phone, tenant.Id, tenant.Name, driver.VehiclePlate, driver.IsVerified)
        ));
    }

    /// <inheritdoc />
    public async Task<Result<TokenResponse>> RefreshTokenAsync(DriverRefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
            return Result<TokenResponse>.Failure("Token inválido");

        // Verificar que el token es de un motorizado
        var userType = principal.FindFirst(ClaimConstants.UserType)?.Value;
        if (userType != UserTypes.Driver)
            return Result<TokenResponse>.Failure("Token inválido para motorizado");

        var driverIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(driverIdClaim, out var driverId))
            return Result<TokenResponse>.Failure("Token inválido");

        var driver = await _driverDb.Drivers
            .FirstOrDefaultAsync(d => d.Id == driverId, cancellationToken);

        if (driver == null || !driver.IsActive)
            return Result<TokenResponse>.Failure("Motorizado no encontrado");

        if (driver.RefreshToken != request.RefreshToken || driver.RefreshTokenExpiresAt < DateTime.UtcNow)
            return Result<TokenResponse>.Failure("Refresh token inválido o expirado");

        // Generar nuevos tokens
        var accessToken = _jwtService.GenerateDriverAccessToken(
            driver.Id, driver.Email, driver.FirstName, driver.LastName, driver.TenantId);
        var refreshToken = _jwtService.GenerateRefreshToken();

        driver.RefreshToken = refreshToken;
        driver.RefreshTokenExpiresAt = _jwtService.GetRefreshTokenExpiration();

        await _driverDb.SaveChangesAsync(cancellationToken);

        return Result<TokenResponse>.Success(new TokenResponse(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(60)
        ));
    }
}
