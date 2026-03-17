using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restify.Auth.Application.DTOs.Auth;
using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.CustomerAuth;
using Restify.Auth.Application.Interfaces;
using Restify.Auth.Domain.Constants;
using Restify.Auth.Infrastructure.Persistence;

namespace Restify.Auth.Infrastructure.Services;

/// <summary>
/// Servicio de autenticación para clientes (consumidores del restaurante).
/// Usa CustomerDbContext para acceder a la tabla backoffice.Customers
/// y AppDbContext para consultar Tenants.
/// </summary>
public class CustomerAuthService : ICustomerAuthService
{
    private readonly CustomerDbContext _customerDb;
    private readonly AppDbContext _authDb;
    private readonly JwtService _jwtService;
    private readonly PasswordService _passwordService;
    private readonly ILogger<CustomerAuthService> _logger;

    public CustomerAuthService(
        CustomerDbContext customerDb,
        AppDbContext authDb,
        JwtService jwtService,
        PasswordService passwordService,
        ILogger<CustomerAuthService> logger)
    {
        _customerDb = customerDb;
        _authDb = authDb;
        _jwtService = jwtService;
        _passwordService = passwordService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result<CustomerLoginResponse>> RegisterAsync(CustomerRegisterRequest request, CancellationToken cancellationToken = default)
    {
        // Buscar tenant por slug
        var tenant = await _authDb.Tenants
            .FirstOrDefaultAsync(t => t.Slug == request.TenantSlug, cancellationToken);

        if (tenant == null)
            return Result<CustomerLoginResponse>.Failure("Restaurante no encontrado");

        // Verificar si ya existe un cliente con ese email para este tenant
        var exists = await _customerDb.Customers
            .AnyAsync(c => c.TenantId == tenant.Id && c.Email == request.Email, cancellationToken);

        if (exists)
            return Result<CustomerLoginResponse>.Failure("Ya existe una cuenta con este email");

        var customer = new CustomerRecord
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            PasswordHash = _passwordService.HashPassword(request.Password),
            IsActive = true,
            EmailVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        // Generar tokens
        var accessToken = _jwtService.GenerateCustomerAccessToken(
            customer.Id, customer.Email, customer.FirstName, customer.LastName, tenant.Id);
        var refreshToken = _jwtService.GenerateRefreshToken();

        customer.RefreshToken = refreshToken;
        customer.RefreshTokenExpiresAt = _jwtService.GetRefreshTokenExpiration();
        customer.LastLoginAt = DateTime.UtcNow;

        await _customerDb.Customers.AddAsync(customer, cancellationToken);
        await _customerDb.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cliente registrado: {Email} para tenant {TenantSlug}", request.Email, request.TenantSlug);

        return Result<CustomerLoginResponse>.Success(new CustomerLoginResponse(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(60),
            new CustomerInfoDto(customer.Id, customer.Email, customer.FirstName, customer.LastName, customer.Phone, tenant.Id, tenant.Name)
        ));
    }

    /// <inheritdoc />
    public async Task<Result<CustomerLoginResponse>> LoginAsync(CustomerLoginRequest request, CancellationToken cancellationToken = default)
    {
        // Buscar tenant por slug
        var tenant = await _authDb.Tenants
            .FirstOrDefaultAsync(t => t.Slug == request.TenantSlug, cancellationToken);

        if (tenant == null)
            return Result<CustomerLoginResponse>.Failure("Restaurante no encontrado");

        // Buscar cliente por email y tenant
        var customer = await _customerDb.Customers
            .FirstOrDefaultAsync(c => c.TenantId == tenant.Id && c.Email == request.Email, cancellationToken);

        if (customer == null || !customer.IsActive)
            return Result<CustomerLoginResponse>.Failure("Credenciales inválidas");

        if (!_passwordService.VerifyPassword(request.Password, customer.PasswordHash))
            return Result<CustomerLoginResponse>.Failure("Credenciales inválidas");

        // Generar tokens
        var accessToken = _jwtService.GenerateCustomerAccessToken(
            customer.Id, customer.Email, customer.FirstName, customer.LastName, tenant.Id);
        var refreshToken = _jwtService.GenerateRefreshToken();

        customer.RefreshToken = refreshToken;
        customer.RefreshTokenExpiresAt = _jwtService.GetRefreshTokenExpiration();
        customer.LastLoginAt = DateTime.UtcNow;

        await _customerDb.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cliente autenticado: {Email}", request.Email);

        return Result<CustomerLoginResponse>.Success(new CustomerLoginResponse(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(60),
            new CustomerInfoDto(customer.Id, customer.Email, customer.FirstName, customer.LastName, customer.Phone, tenant.Id, tenant.Name)
        ));
    }

    /// <inheritdoc />
    public async Task<Result<TokenResponse>> RefreshTokenAsync(CustomerRefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
            return Result<TokenResponse>.Failure("Token inválido");

        // Verificar que el token es de un cliente
        var userType = principal.FindFirst(ClaimConstants.UserType)?.Value;
        if (userType != UserTypes.Customer)
            return Result<TokenResponse>.Failure("Token inválido para cliente");

        var customerIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(customerIdClaim, out var customerId))
            return Result<TokenResponse>.Failure("Token inválido");

        var customer = await _customerDb.Customers
            .FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);

        if (customer == null || !customer.IsActive)
            return Result<TokenResponse>.Failure("Cliente no encontrado");

        if (customer.RefreshToken != request.RefreshToken || customer.RefreshTokenExpiresAt < DateTime.UtcNow)
            return Result<TokenResponse>.Failure("Refresh token inválido o expirado");

        // Generar nuevos tokens
        var accessToken = _jwtService.GenerateCustomerAccessToken(
            customer.Id, customer.Email, customer.FirstName, customer.LastName, customer.TenantId);
        var refreshToken = _jwtService.GenerateRefreshToken();

        customer.RefreshToken = refreshToken;
        customer.RefreshTokenExpiresAt = _jwtService.GetRefreshTokenExpiration();

        await _customerDb.SaveChangesAsync(cancellationToken);

        return Result<TokenResponse>.Success(new TokenResponse(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(60)
        ));
    }
}
