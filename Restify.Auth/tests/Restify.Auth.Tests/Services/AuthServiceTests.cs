using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Restify.Auth.Application.DTOs.Auth;
using Restify.Auth.Domain.Entities;
using Restify.Auth.Domain.Enums;
using Restify.Auth.Infrastructure.Services;
using Restify.Auth.Tests.Helpers;

namespace Restify.Auth.Tests.Services;

/// <summary>
/// Tests para el servicio de autenticacion (login, refresh, logout)
/// </summary>
public class AuthServiceTests : IDisposable
{
    private readonly Infrastructure.Persistence.AppDbContext _context;
    private readonly JwtService _jwtService;
    private readonly PasswordService _passwordService;
    private readonly AuthService _authService;
    private readonly Guid _tenantId = Guid.NewGuid();
    private readonly Guid _adminUserId = Guid.NewGuid();

    public AuthServiceTests()
    {
        // Crear contexto sin filtro de tenant (AuthService usa IgnoreQueryFilters)
        _context = TestDbContextFactory.Create();

        _passwordService = new PasswordService();

        var jwtSettings = Options.Create(new JwtSettings
        {
            SecretKey = "TuClaveSecretaMuyLargaYSeguraDeAlMenos32Caracteres!",
            Issuer = "Restify.Auth",
            Audience = "Restify.Client",
            AccessTokenExpirationMinutes = 60,
            RefreshTokenExpirationDays = 7
        });
        _jwtService = new JwtService(jwtSettings);

        var loggerMock = new Mock<ILogger<AuthService>>();
        _authService = new AuthService(_context, _jwtService, _passwordService, loggerMock.Object);

        // Seed datos base
        SeedData();
    }

    private void SeedData()
    {
        var tenant = new Tenant
        {
            Id = _tenantId,
            Name = "Test Restaurant",
            Slug = "test-restaurant",
            Status = TenantStatus.Active,
            Currency = "USD",
            TaxPercentage = 12.00m
        };
        _context.Tenants.Add(tenant);

        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Code = "users.view",
            Name = "Ver usuarios",
            Module = "users"
        };
        _context.Permissions.Add(permission);

        var role = new Role
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantId,
            Name = "Admin",
            NormalizedName = "ADMIN",
            IsSystem = true
        };
        _context.Roles.Add(role);

        var rolePermission = new RolePermission
        {
            RoleId = role.Id,
            PermissionId = permission.Id
        };
        _context.RolePermissions.Add(rolePermission);

        var user = new User
        {
            Id = _adminUserId,
            TenantId = _tenantId,
            Email = "admin@test.com",
            PasswordHash = _passwordService.HashPassword("Admin123!"),
            FirstName = "Admin",
            LastName = "Test",
            Status = UserStatus.Active,
            EmailVerified = true
        };
        _context.Users.Add(user);

        var userRole = new UserRole
        {
            UserId = _adminUserId,
            RoleId = role.Id
        };
        _context.UserRoles.Add(userRole);

        // Usuario inactivo
        var inactiveUser = new User
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantId,
            Email = "inactive@test.com",
            PasswordHash = _passwordService.HashPassword("Admin123!"),
            FirstName = "Inactive",
            LastName = "User",
            Status = UserStatus.Inactive,
            EmailVerified = true
        };
        _context.Users.Add(inactiveUser);

        _context.SaveChangesAsync().GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Login_ConCredencialesValidas_RetornaExitoConTokens()
    {
        // Arrange
        var request = new LoginRequest("admin@test.com", "Admin123!", "test-restaurant");

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().NotBeNullOrEmpty();
        result.Data.RefreshToken.Should().NotBeNullOrEmpty();
        result.Data.User.Email.Should().Be("admin@test.com");
        result.Data.User.TenantId.Should().Be(_tenantId);
        result.Data.User.Roles.Should().Contain("Admin");
        result.Data.User.Permissions.Should().Contain("users.view");
    }

    [Fact]
    public async Task Login_ConEmailInexistente_RetornaFallo()
    {
        // Arrange
        var request = new LoginRequest("noexiste@test.com", "Admin123!", "test-restaurant");

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Credenciales inválidas");
    }

    [Fact]
    public async Task Login_ConPasswordIncorrecto_RetornaFallo()
    {
        // Arrange
        var request = new LoginRequest("admin@test.com", "WrongPassword!", "test-restaurant");

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Credenciales inválidas");
    }

    [Fact]
    public async Task Login_ConUsuarioInactivo_RetornaFallo()
    {
        // Arrange
        var request = new LoginRequest("inactive@test.com", "Admin123!", "test-restaurant");

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Usuario inactivo o bloqueado");
    }

    [Fact]
    public async Task Login_ConTenantInexistente_RetornaFallo()
    {
        // Arrange
        var request = new LoginRequest("admin@test.com", "Admin123!", "slug-no-existe");

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Tenant no encontrado o inactivo");
    }

    [Fact]
    public async Task RefreshToken_ConTokenValido_RetornaNuevosTokens()
    {
        // Arrange: primero hacer login para obtener tokens
        var loginResult = await _authService.LoginAsync(
            new LoginRequest("admin@test.com", "Admin123!", "test-restaurant"));
        loginResult.IsSuccess.Should().BeTrue();

        var refreshRequest = new RefreshTokenRequest(
            loginResult.Data!.AccessToken,
            loginResult.Data.RefreshToken);

        // Act
        var result = await _authService.RefreshTokenAsync(refreshRequest);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().NotBeNullOrEmpty();
        result.Data.RefreshToken.Should().NotBeNullOrEmpty();
        // El nuevo refresh token debe ser diferente al anterior
        result.Data.RefreshToken.Should().NotBe(loginResult.Data.RefreshToken);
    }

    [Fact]
    public async Task RefreshToken_ConRefreshTokenIncorrecto_RetornaFallo()
    {
        // Arrange: primero hacer login
        var loginResult = await _authService.LoginAsync(
            new LoginRequest("admin@test.com", "Admin123!", "test-restaurant"));
        loginResult.IsSuccess.Should().BeTrue();

        var refreshRequest = new RefreshTokenRequest(
            loginResult.Data!.AccessToken,
            "refresh-token-falso");

        // Act
        var result = await _authService.RefreshTokenAsync(refreshRequest);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Refresh token inválido");
    }

    [Fact]
    public async Task RefreshToken_ConRefreshTokenExpirado_RetornaFallo()
    {
        // Arrange: hacer login y luego expirar manualmente el refresh token
        var loginResult = await _authService.LoginAsync(
            new LoginRequest("admin@test.com", "Admin123!", "test-restaurant"));
        loginResult.IsSuccess.Should().BeTrue();

        var user = _context.Users.IgnoreQueryFilters().First(u => u.Id == _adminUserId);
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(-1); // Expirado
        await _context.SaveChangesAsync();

        var refreshRequest = new RefreshTokenRequest(
            loginResult.Data!.AccessToken,
            loginResult.Data.RefreshToken);

        // Act
        var result = await _authService.RefreshTokenAsync(refreshRequest);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Refresh token expirado");
    }

    [Fact]
    public async Task Logout_InvalidaRefreshToken()
    {
        // Arrange: hacer login primero
        var loginResult = await _authService.LoginAsync(
            new LoginRequest("admin@test.com", "Admin123!", "test-restaurant"));
        loginResult.IsSuccess.Should().BeTrue();

        var user = _context.Users.IgnoreQueryFilters().First(u => u.Id == _adminUserId);
        user.RefreshToken.Should().NotBeNull();

        // Act
        var result = await _authService.LogoutAsync(user.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var updatedUser = _context.Users.IgnoreQueryFilters().First(u => u.Id == _adminUserId);
        updatedUser.RefreshToken.Should().BeNull();
        updatedUser.RefreshTokenExpiresAt.Should().BeNull();
    }

    [Fact]
    public async Task Logout_ConUsuarioInexistente_RetornaFallo()
    {
        // Act
        var result = await _authService.LogoutAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Usuario no encontrado");
    }

    [Fact]
    public async Task ChangePassword_ConDatosValidos_RetornaExito()
    {
        // Arrange
        var user = _context.Users.IgnoreQueryFilters().First(u => u.Id == _adminUserId);
        var request = new ChangePasswordRequest("Admin123!", "NuevoPass123!", "NuevoPass123!");

        // Act
        var result = await _authService.ChangePasswordAsync(user.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        // Verificar que se invalido el refresh token
        var updatedUser = _context.Users.IgnoreQueryFilters().First(u => u.Id == _adminUserId);
        updatedUser.RefreshToken.Should().BeNull();
    }

    [Fact]
    public async Task ChangePassword_ConPasswordActualIncorrecto_RetornaFallo()
    {
        // Arrange
        var user = _context.Users.IgnoreQueryFilters().First(u => u.Id == _adminUserId);
        var request = new ChangePasswordRequest("WrongPassword!", "NuevoPass123!", "NuevoPass123!");

        // Act
        var result = await _authService.ChangePasswordAsync(user.Id, request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Contraseña actual incorrecta");
    }

    [Fact]
    public async Task ChangePassword_ConPasswordsNoCoinciden_RetornaFallo()
    {
        // Arrange
        var user = _context.Users.IgnoreQueryFilters().First(u => u.Id == _adminUserId);
        var request = new ChangePasswordRequest("Admin123!", "NuevoPass123!", "OtraPass456!");

        // Act
        var result = await _authService.ChangePasswordAsync(user.Id, request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Las contraseñas no coinciden");
    }

    [Fact]
    public async Task ChangePassword_ConUsuarioInexistente_RetornaFallo()
    {
        // Arrange
        var request = new ChangePasswordRequest("Admin123!", "NuevoPass123!", "NuevoPass123!");

        // Act
        var result = await _authService.ChangePasswordAsync(Guid.NewGuid(), request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Usuario no encontrado");
    }

    [Fact]
    public async Task GetCurrentUser_ConUsuarioValido_RetornaInfo()
    {
        // Arrange
        var user = _context.Users.IgnoreQueryFilters().First(u => u.Id == _adminUserId);

        // Act
        var result = await _authService.GetCurrentUserAsync(user.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Email.Should().Be("admin@test.com");
        result.Data.FirstName.Should().Be("Admin");
        result.Data.TenantName.Should().Be("Test Restaurant");
        result.Data.Roles.Should().Contain("Admin");
        result.Data.Permissions.Should().Contain("users.view");
        result.Data.TaxPercentage.Should().Be(12.00m);
        result.Data.Currency.Should().Be("USD");
    }

    [Fact]
    public async Task GetCurrentUser_ConUsuarioInexistente_RetornaFallo()
    {
        // Act
        var result = await _authService.GetCurrentUserAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Usuario no encontrado");
    }

    [Fact]
    public async Task Login_SinTenantSlug_BuscaEnTodos()
    {
        // Arrange: login sin especificar tenant slug
        var request = new LoginRequest("admin@test.com", "Admin123!");

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.User.Email.Should().Be("admin@test.com");
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
