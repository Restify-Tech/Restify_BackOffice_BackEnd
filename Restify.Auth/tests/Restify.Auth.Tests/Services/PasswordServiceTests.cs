using FluentAssertions;
using Restify.Auth.Infrastructure.Services;

namespace Restify.Auth.Tests.Services;

public class PasswordServiceTests
{
    private readonly PasswordService _passwordService;

    public PasswordServiceTests()
    {
        _passwordService = new PasswordService();
    }

    [Fact]
    public void HashPassword_ConPasswordValido_GeneraHash()
    {
        // Arrange
        var password = "MySecurePassword123!";

        // Act
        var hash = _passwordService.HashPassword(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(password);
        hash.StartsWith("$2").Should().BeTrue(); // BCrypt prefix
    }

    [Fact]
    public void HashPassword_PasswordsIguales_GeneranHashesDistintos()
    {
        // Arrange
        var password = "MySecurePassword123!";

        // Act
        var hash1 = _passwordService.HashPassword(password);
        var hash2 = _passwordService.HashPassword(password);

        // Assert
        hash1.Should().NotBe(hash2); // BCrypt genera salts aleatorios
    }

    [Fact]
    public void VerifyPassword_ConPasswordCorrecto_RetornaTrue()
    {
        // Arrange
        var password = "MySecurePassword123!";
        var hash = _passwordService.HashPassword(password);

        // Act
        var result = _passwordService.VerifyPassword(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_ConPasswordIncorrecto_RetornaFalse()
    {
        // Arrange
        var password = "MySecurePassword123!";
        var wrongPassword = "WrongPassword456!";
        var hash = _passwordService.HashPassword(password);

        // Act
        var result = _passwordService.VerifyPassword(wrongPassword, hash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_ConHashInvalido_RetornaFalse()
    {
        // Arrange
        var password = "MySecurePassword123!";
        var invalidHash = "invalid-hash";

        // Act
        var result = _passwordService.VerifyPassword(password, invalidHash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_ConPasswordVacio_RetornaFalse()
    {
        // Arrange
        var password = "MySecurePassword123!";
        var hash = _passwordService.HashPassword(password);

        // Act
        var result = _passwordService.VerifyPassword("", hash);

        // Assert
        result.Should().BeFalse();
    }
}
