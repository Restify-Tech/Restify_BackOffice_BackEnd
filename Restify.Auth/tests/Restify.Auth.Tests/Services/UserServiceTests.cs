using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.Users;
using Restify.Auth.Application.Interfaces;
using Restify.Auth.Domain.Entities;
using Restify.Auth.Domain.Enums;
using Restify.Auth.Infrastructure.Persistence;
using Restify.Auth.Infrastructure.Services;
using Restify.Auth.Tests.Helpers;

namespace Restify.Auth.Tests.Services;

/// <summary>
/// Tests para el servicio de gestion de usuarios
/// </summary>
public class UserServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly UserService _userService;
    private readonly PasswordService _passwordService;
    private readonly Guid _tenantId = Guid.NewGuid();
    private readonly string _dbName;

    // IDs para datos seed
    private readonly Guid _existingUserId = Guid.NewGuid();
    private readonly Guid _roleId = Guid.NewGuid();

    public UserServiceTests()
    {
        _dbName = Guid.NewGuid().ToString();
        _context = TestDbContextFactory.CreateWithTenant(_tenantId, _dbName);
        _passwordService = new PasswordService();

        var currentUserMock = new Mock<ICurrentUserService>();
        currentUserMock.Setup(x => x.TenantId).Returns(_tenantId);
        currentUserMock.Setup(x => x.UserId).Returns(Guid.NewGuid());

        var loggerMock = new Mock<ILogger<UserService>>();
        _userService = new UserService(_context, _passwordService, currentUserMock.Object, loggerMock.Object);

        SeedData();
    }

    private void SeedData()
    {
        var tenant = new Tenant
        {
            Id = _tenantId,
            Name = "Test Restaurant",
            Slug = "test-restaurant",
            Status = TenantStatus.Active
        };
        _context.Tenants.Add(tenant);

        var role = new Role
        {
            Id = _roleId,
            TenantId = _tenantId,
            Name = "Admin",
            NormalizedName = "ADMIN"
        };
        _context.Roles.Add(role);

        var user = new User
        {
            Id = _existingUserId,
            TenantId = _tenantId,
            Email = "existing@test.com",
            PasswordHash = _passwordService.HashPassword("Admin123!"),
            FirstName = "Existing",
            LastName = "User",
            Status = UserStatus.Active,
            EmailVerified = true
        };
        _context.Users.Add(user);

        var userRole = new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        };
        _context.UserRoles.Add(userRole);

        _context.SaveChangesAsync().GetAwaiter().GetResult();
    }

    [Fact]
    public async Task GetAll_RetornaUsuariosPaginados()
    {
        // Arrange
        var request = new PagedRequest(Page: 1, PageSize: 10);

        // Act
        var result = await _userService.GetAllAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.TotalCount.Should().BeGreaterThanOrEqualTo(1);
        result.Data.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_ConUsuarioExistente_RetornaUsuario()
    {
        // Act
        var result = await _userService.GetByIdAsync(_existingUserId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Email.Should().Be("existing@test.com");
        result.Data.FirstName.Should().Be("Existing");
        result.Data.Roles.Should().Contain("Admin");
    }

    [Fact]
    public async Task GetById_ConUsuarioInexistente_RetornaFallo()
    {
        // Act
        var result = await _userService.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Usuario no encontrado");
    }

    [Fact]
    public async Task Create_ConDatosValidos_RetornaExito()
    {
        // Arrange
        var request = new CreateUserRequest(
            Email: "nuevo@test.com",
            Password: "NuevoPass123!",
            FirstName: "Nuevo",
            LastName: "Usuario",
            Phone: "0999999999",
            RoleIds: new[] { _roleId }
        );

        // Act
        var result = await _userService.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Email.Should().Be("nuevo@test.com");
        result.Data.FirstName.Should().Be("Nuevo");
        result.Data.LastName.Should().Be("Usuario");
        result.Data.Roles.Should().Contain("Admin");
    }

    [Fact]
    public async Task Create_ConEmailDuplicado_RetornaFallo()
    {
        // Arrange
        var request = new CreateUserRequest(
            Email: "existing@test.com",
            Password: "NuevoPass123!",
            FirstName: "Duplicado",
            LastName: "Usuario",
            Phone: null,
            RoleIds: new[] { _roleId }
        );

        // Act
        var result = await _userService.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("El email ya está registrado");
    }

    [Fact]
    public async Task Update_ConDatosValidos_RetornaExito()
    {
        // Arrange
        var request = new UpdateUserRequest(
            FirstName: "Actualizado",
            LastName: "Test",
            Phone: "0911111111",
            Status: null,
            RoleIds: null
        );

        // Act
        var result = await _userService.UpdateAsync(_existingUserId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.FirstName.Should().Be("Actualizado");
        result.Data.LastName.Should().Be("Test");
    }

    [Fact]
    public async Task Delete_ConUsuarioExistente_RetornaExito()
    {
        // Act
        var result = await _userService.DeleteAsync(_existingUserId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        // Verificar soft delete (status cambia a Inactive)
        // Usamos un nuevo contexto para leer sin cache
        using var readContext = TestDbContextFactory.Create(_dbName);
        var user = readContext.Users.IgnoreQueryFilters().First(u => u.Id == _existingUserId);
        user.Status.Should().Be(UserStatus.Inactive);
    }

    [Fact]
    public async Task ResetPassword_CambiaPasswordHash()
    {
        // Arrange
        // Obtenemos el hash original antes del reset
        using var readContextBefore = TestDbContextFactory.Create(_dbName);
        var userBefore = readContextBefore.Users.IgnoreQueryFilters().First(u => u.Id == _existingUserId);
        var originalHash = userBefore.PasswordHash;

        // Act
        var result = await _userService.ResetPasswordAsync(_existingUserId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNullOrEmpty(); // retorna password temporal

        using var readContextAfter = TestDbContextFactory.Create(_dbName);
        var userAfter = readContextAfter.Users.IgnoreQueryFilters().First(u => u.Id == _existingUserId);
        userAfter.PasswordHash.Should().NotBe(originalHash);
        userAfter.RefreshToken.Should().BeNull();
    }

    [Fact]
    public async Task Create_ConRolInexistente_RetornaFallo()
    {
        // Arrange
        var fakeRoleId = Guid.NewGuid();
        var request = new CreateUserRequest(
            Email: "nuevo2@test.com",
            Password: "NuevoPass123!",
            FirstName: "Nuevo",
            LastName: "Usuario",
            Phone: null,
            RoleIds: new[] { fakeRoleId }
        );

        // Act
        var result = await _userService.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Uno o más roles no existen");
    }

    [Fact]
    public async Task Update_ConUsuarioInexistente_RetornaFallo()
    {
        // Arrange
        var request = new UpdateUserRequest(
            FirstName: "NoExiste",
            LastName: "Nadie",
            Phone: null,
            Status: null,
            RoleIds: null
        );

        // Act
        var result = await _userService.UpdateAsync(Guid.NewGuid(), request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Usuario no encontrado");
    }

    [Fact]
    public async Task Delete_ConUsuarioInexistente_RetornaFallo()
    {
        // Act
        var result = await _userService.DeleteAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Usuario no encontrado");
    }

    [Fact]
    public async Task ResetPassword_ConUsuarioInexistente_RetornaFallo()
    {
        // Act
        var result = await _userService.ResetPasswordAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Usuario no encontrado");
    }

    [Fact]
    public async Task GetAll_ConBusqueda_FiltraResultados()
    {
        // Arrange
        var request = new PagedRequest(Page: 1, PageSize: 10, Search: "existing");

        // Act
        var result = await _userService.GetAllAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().Contain(u => u.Email == "existing@test.com");
    }

    [Fact]
    public async Task Update_ConCambioDeRoles_ActualizaRoles()
    {
        // Arrange: crear un segundo rol
        var newRoleId = Guid.NewGuid();
        using var seedContext = TestDbContextFactory.Create(_dbName);
        seedContext.Roles.Add(new Role
        {
            Id = newRoleId,
            TenantId = _tenantId,
            Name = "Mesero",
            NormalizedName = "MESERO"
        });
        await seedContext.SaveChangesAsync();

        var request = new UpdateUserRequest(
            FirstName: "Existing",
            LastName: "User",
            Phone: null,
            Status: null,
            RoleIds: new[] { newRoleId }
        );

        // Act
        var result = await _userService.UpdateAsync(_existingUserId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Roles.Should().Contain("Mesero");
        result.Data.Roles.Should().NotContain("Admin");
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
