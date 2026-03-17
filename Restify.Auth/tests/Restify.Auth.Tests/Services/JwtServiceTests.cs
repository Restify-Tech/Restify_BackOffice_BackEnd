using FluentAssertions;
using Microsoft.Extensions.Options;
using Restify.Auth.Domain.Entities;
using Restify.Auth.Domain.Enums;
using Restify.Auth.Infrastructure.Services;

namespace Restify.Auth.Tests.Services;

public class JwtServiceTests
{
    private readonly JwtService _jwtService;
    private readonly JwtSettings _jwtSettings;

    public JwtServiceTests()
    {
        _jwtSettings = new JwtSettings
        {
            SecretKey = "TuClaveSecretaMuyLargaYSeguraDeAlMenos32Caracteres!",
            Issuer = "Restify.Auth",
            Audience = "Restify.Client",
            AccessTokenExpirationMinutes = 60,
            RefreshTokenExpirationDays = 7
        };
        _jwtService = new JwtService(Options.Create(_jwtSettings));
    }

    [Fact]
    public void GenerateAccessToken_ConDatosValidos_GeneraToken()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            Status = UserStatus.Active
        };
        var roles = new[] { "Admin", "User" };
        var permissions = new[] { "users.view", "orders.create" };

        // Act
        var token = _jwtService.GenerateAccessToken(user, roles, permissions);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Split('.').Should().HaveCount(3); // JWT format: header.payload.signature
    }

    [Fact]
    public void GenerateRefreshToken_GeneraTokenValido()
    {
        // Act
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Assert
        refreshToken.Should().NotBeNullOrEmpty();
        refreshToken.Length.Should().BeGreaterThan(20);
    }

    [Fact]
    public void GenerateCustomerAccessToken_ConDatosValidos_GeneraToken()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        // Act
        var token = _jwtService.GenerateCustomerAccessToken(customerId, "customer@test.com", "John", "Doe", tenantId);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Split('.').Should().HaveCount(3);
    }

    [Fact]
    public void GenerateDriverAccessToken_ConDatosValidos_GeneraToken()
    {
        // Arrange
        var driverId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        // Act
        var token = _jwtService.GenerateDriverAccessToken(driverId, "driver@test.com", "John", "Doe", tenantId);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Split('.').Should().HaveCount(3);
    }

    [Fact]
    public void GeneratePoolDriverAccessToken_ConDatosValidos_GeneraToken()
    {
        // Arrange
        var driverId = Guid.NewGuid();
        var deliveryZoneId = Guid.NewGuid();

        // Act
        var token = _jwtService.GeneratePoolDriverAccessToken(driverId, "pool@test.com", "John", "Doe", deliveryZoneId);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Split('.').Should().HaveCount(3);
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_ConTokenValido_RetornaPrincipal()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            Status = UserStatus.Active
        };
        var token = _jwtService.GenerateAccessToken(user, Array.Empty<string>(), Array.Empty<string>());

        // Act
        var principal = _jwtService.GetPrincipalFromExpiredToken(token);

        // Assert
        principal.Should().NotBeNull();
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_ConTokenInvalido_RetornaNull()
    {
        // Arrange
        var invalidToken = "token.invalido";

        // Act
        var principal = _jwtService.GetPrincipalFromExpiredToken(invalidToken);

        // Assert
        principal.Should().BeNull();
    }

    [Fact]
    public void GetRefreshTokenExpiration_RetornaFechaFutura()
    {
        // Act
        var expiration = _jwtService.GetRefreshTokenExpiration();

        // Assert
        expiration.Should().BeAfter(DateTime.UtcNow);
        expiration.Should().BeWithin(TimeSpan.FromDays(7)).Before(DateTime.UtcNow.AddDays(7));
    }
}
