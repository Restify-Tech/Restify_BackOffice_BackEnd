using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Restify.Auth.Application.DTOs.Auth;
using Restify.Auth.Application.DTOs.Tenant;
using Restify.Auth.Domain.Entities;
using Restify.Auth.Domain.Enums;
using Restify.Auth.Infrastructure.Services;
using Restify.Auth.Tests.Helpers;

namespace Restify.Auth.Tests.Services;

public class TenantRegistrationServiceTests : IDisposable
{
    private readonly Infrastructure.Persistence.AppDbContext _context;
    private readonly TenantRegistrationService _service;
    private readonly JwtService _jwtService;
    private readonly PasswordService _passwordService;

    public TenantRegistrationServiceTests()
    {
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

        var loggerMock = new Mock<ILogger<TenantRegistrationService>>();
        _service = new TenantRegistrationService(_context, _jwtService, _passwordService, loggerMock.Object);

        SeedPermissions();
    }

    private void SeedPermissions()
    {
        var permissions = new[]
        {
            new Permission { Id = Guid.NewGuid(), Code = "dashboard.view", Name = "Ver Dashboard", Module = "dashboard", IsTenantDefault = true, IsActive = true },
            new Permission { Id = Guid.NewGuid(), Code = "users.view", Name = "Ver Usuarios", Module = "users", IsTenantDefault = true, IsActive = true },
            new Permission { Id = Guid.NewGuid(), Code = "orders.view", Name = "Ver Pedidos", Module = "orders", IsTenantDefault = true, IsActive = true },
        };
        _context.Permissions.AddRange(permissions);
        _context.SaveChangesAsync().GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Register_ConDatosValidos_RetornaExito()
    {
        // Arrange
        var request = new TenantRegisterRequest(
            IdentificationType.Cedula,
            "1234567890",
            "Nuevo Restaurante",
            "nuevo-restaurante",
            "admin@nuevo.com",
            "Admin123!",
            "0991234567",
            "Quito"
        );

        // Act
        var result = await _service.RegisterAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().NotBeNullOrEmpty();
        result.Data.RefreshToken.Should().NotBeNullOrEmpty();
        result.Data.TenantId.Should().NotBeEmpty();
        result.Data.AdminUserId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Register_ConSlugDuplicado_RetornaFallo()
    {
        // Arrange: crear tenant existente
        var existingTenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = "Restaurante Existente",
            Slug = "existente",
            Status = TenantStatus.Active
        };
        _context.Tenants.Add(existingTenant);
        await _context.SaveChangesAsync();

        var request = new TenantRegisterRequest(
            IdentificationType.Cedula,
            "1234567890",
            "Nuevo Restaurante",
            "existente",
            "admin@nuevo.com",
            "Admin123!",
            "0991234567",
            "Quito"
        );

        // Act
        var result = await _service.RegisterAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("slug");
    }

    [Fact]
    public async Task Register_ConEmailDuplicado_RetornaFallo()
    {
        // Arrange: crear usuario existente
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "existente@test.com",
            PasswordHash = _passwordService.HashPassword("Admin123!"),
            FirstName = "Test",
            LastName = "User"
        };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var request = new TenantRegisterRequest(
            IdentificationType.Cedula,
            "1234567890",
            "Nuevo Restaurante",
            "nuevo-restaurante",
            "existente@test.com",
            "Admin123!",
            "0991234567",
            "Quito"
        );

        // Act
        var result = await _service.RegisterAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("email");
    }

    [Fact]
    public async Task Register_ConPasswordInvalido_RetornaFallo()
    {
        // Arrange
        var request = new TenantRegisterRequest(
            IdentificationType.Cedula,
            "1234567890",
            "Nuevo Restaurante",
            "nuevo-restaurante",
            "admin@nuevo.com",
            "weak",
            "0991234567",
            "Quito"
        );

        // Act
        var result = await _service.RegisterAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("contraseña");
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
