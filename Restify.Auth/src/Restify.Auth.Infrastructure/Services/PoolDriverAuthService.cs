using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restify.Auth.Application.DTOs.Auth;
using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.PoolDriverAuth;
using Restify.Auth.Application.Interfaces;
using Restify.Auth.Domain.Constants;
using Restify.Auth.Infrastructure.Persistence;

namespace Restify.Auth.Infrastructure.Services;

public class PoolDriverAuthService : IPoolDriverAuthService
{
    private readonly DriverDbContext _driverDb;
    private readonly AppDbContext _authDb;
    private readonly JwtService _jwtService;
    private readonly PasswordService _passwordService;
    private readonly ILogger<PoolDriverAuthService> _logger;

    public PoolDriverAuthService(
        DriverDbContext driverDb,
        AppDbContext authDb,
        JwtService jwtService,
        PasswordService passwordService,
        ILogger<PoolDriverAuthService> logger)
    {
        _driverDb = driverDb;
        _authDb = authDb;
        _jwtService = jwtService;
        _passwordService = passwordService;
        _logger = logger;
    }

    public async Task<Result<PoolDriverLoginResponse>> RegisterAsync(PoolDriverRegisterRequest request, CancellationToken cancellationToken = default)
    {
        // Verificar que la zona de delivery existe
        var zoneExists = await _authDb.DeliveryZones
            .AnyAsync(z => z.Id == request.DeliveryZoneId && z.IsActive, cancellationToken);

        if (!zoneExists)
            return Result<PoolDriverLoginResponse>.Failure("Zona de delivery no encontrada o inactiva");

        // Verificar si ya existe un pool driver con ese email
        var exists = await _driverDb.Drivers
            .AnyAsync(d => d.Email == request.Email && d.IsPoolDriver, cancellationToken);

        if (exists)
            return Result<PoolDriverLoginResponse>.Failure("Ya existe una cuenta pool con este email");

        var driver = new DriverRecord
        {
            Id = Guid.NewGuid(),
            TenantId = SuperAdminConstants.TenantId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            IdentificationNumber = request.IdentificationNumber,
            VehiclePlate = request.VehiclePlate,
            PasswordHash = _passwordService.HashPassword(request.Password),
            IsActive = true,
            IsVerified = false,
            IsPoolDriver = true,
            DeliveryZoneId = request.DeliveryZoneId,
            VehicleType = request.VehicleType,
            VerificationStatus = 1, // Pending
            CreatedAt = DateTime.UtcNow
        };

        // Generar JWT con user_type=pool_driver y sin tenant_id
        var accessToken = _jwtService.GeneratePoolDriverAccessToken(
            driver.Id, driver.Email, driver.FirstName, driver.LastName, request.DeliveryZoneId);
        var refreshToken = _jwtService.GenerateRefreshToken();

        driver.RefreshToken = refreshToken;
        driver.RefreshTokenExpiresAt = _jwtService.GetRefreshTokenExpiration();
        driver.LastLoginAt = DateTime.UtcNow;

        await _driverDb.Drivers.AddAsync(driver, cancellationToken);
        await _driverDb.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Pool driver registrado: {Email} en zona {ZoneId}", request.Email, request.DeliveryZoneId);

        return Result<PoolDriverLoginResponse>.Success(new PoolDriverLoginResponse(
            accessToken, refreshToken, DateTime.UtcNow.AddMinutes(60),
            new PoolDriverInfoDto(driver.Id, driver.Email, driver.FirstName, driver.LastName,
                driver.Phone, driver.DeliveryZoneId, driver.IsVerified, driver.VerificationStatus)
        ));
    }

    public async Task<Result<PoolDriverLoginResponse>> LoginAsync(PoolDriverLoginRequest request, CancellationToken cancellationToken = default)
    {
        var driver = await _driverDb.Drivers
            .FirstOrDefaultAsync(d => d.Email == request.Email && d.IsPoolDriver, cancellationToken);

        if (driver == null || !driver.IsActive)
            return Result<PoolDriverLoginResponse>.Failure("Credenciales inválidas");

        if (!_passwordService.VerifyPassword(request.Password, driver.PasswordHash))
            return Result<PoolDriverLoginResponse>.Failure("Credenciales inválidas");

        var accessToken = _jwtService.GeneratePoolDriverAccessToken(
            driver.Id, driver.Email, driver.FirstName, driver.LastName, driver.DeliveryZoneId);
        var refreshToken = _jwtService.GenerateRefreshToken();

        driver.RefreshToken = refreshToken;
        driver.RefreshTokenExpiresAt = _jwtService.GetRefreshTokenExpiration();
        driver.LastLoginAt = DateTime.UtcNow;

        await _driverDb.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Pool driver autenticado: {Email}", request.Email);

        return Result<PoolDriverLoginResponse>.Success(new PoolDriverLoginResponse(
            accessToken, refreshToken, DateTime.UtcNow.AddMinutes(60),
            new PoolDriverInfoDto(driver.Id, driver.Email, driver.FirstName, driver.LastName,
                driver.Phone, driver.DeliveryZoneId, driver.IsVerified, driver.VerificationStatus)
        ));
    }

    public async Task<Result<TokenResponse>> RefreshTokenAsync(PoolDriverRefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
            return Result<TokenResponse>.Failure("Token inválido");

        var userType = principal.FindFirst(ClaimConstants.UserType)?.Value;
        if (userType != UserTypes.PoolDriver)
            return Result<TokenResponse>.Failure("Token inválido para pool driver");

        var driverIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(driverIdClaim, out var driverId))
            return Result<TokenResponse>.Failure("Token inválido");

        var driver = await _driverDb.Drivers
            .FirstOrDefaultAsync(d => d.Id == driverId && d.IsPoolDriver, cancellationToken);

        if (driver == null || !driver.IsActive)
            return Result<TokenResponse>.Failure("Pool driver no encontrado");

        if (driver.RefreshToken != request.RefreshToken || driver.RefreshTokenExpiresAt < DateTime.UtcNow)
            return Result<TokenResponse>.Failure("Refresh token inválido o expirado");

        var accessToken = _jwtService.GeneratePoolDriverAccessToken(
            driver.Id, driver.Email, driver.FirstName, driver.LastName, driver.DeliveryZoneId);
        var refreshToken = _jwtService.GenerateRefreshToken();

        driver.RefreshToken = refreshToken;
        driver.RefreshTokenExpiresAt = _jwtService.GetRefreshTokenExpiration();

        await _driverDb.SaveChangesAsync(cancellationToken);

        return Result<TokenResponse>.Success(new TokenResponse(
            accessToken, refreshToken, DateTime.UtcNow.AddMinutes(60)
        ));
    }
}
