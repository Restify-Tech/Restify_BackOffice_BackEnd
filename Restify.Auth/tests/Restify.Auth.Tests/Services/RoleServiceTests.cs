using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.Roles;
using Restify.Auth.Application.Interfaces;
using Restify.Auth.Domain.Entities;
using Restify.Auth.Domain.Enums;
using Restify.Auth.Infrastructure.Persistence;
using Restify.Auth.Infrastructure.Services;
using Restify.Auth.Tests.Helpers;

namespace Restify.Auth.Tests.Services;

/// <summary>
/// Tests para el servicio de gestion de roles
/// </summary>
public class RoleServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly RoleService _roleService;
    private readonly Guid _tenantId = Guid.NewGuid();
    private readonly string _dbName;

    // IDs para datos seed
    private readonly Guid _existingRoleId = Guid.NewGuid();
    private readonly Guid _systemRoleId = Guid.NewGuid();
    private readonly Guid _permissionId1 = Guid.NewGuid();
    private readonly Guid _permissionId2 = Guid.NewGuid();
    private readonly Guid _roleWithUsersId = Guid.NewGuid();

    public RoleServiceTests()
    {
        _dbName = Guid.NewGuid().ToString();
        _context = TestDbContextFactory.CreateWithTenant(_tenantId, _dbName);

        var currentUserMock = new Mock<ICurrentUserService>();
        currentUserMock.Setup(x => x.TenantId).Returns(_tenantId);

        var loggerMock = new Mock<ILogger<RoleService>>();
        _roleService = new RoleService(_context, currentUserMock.Object, loggerMock.Object);

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

        // Permisos
        var perm1 = new Permission { Id = _permissionId1, Code = "users.view", Name = "Ver usuarios", Module = "users", DisplayOrder = 1 };
        var perm2 = new Permission { Id = _permissionId2, Code = "users.create", Name = "Crear usuarios", Module = "users", DisplayOrder = 2 };
        _context.Permissions.AddRange(perm1, perm2);

        // Rol normal (editable)
        var role = new Role
        {
            Id = _existingRoleId,
            TenantId = _tenantId,
            Name = "Mesero",
            NormalizedName = "MESERO",
            Description = "Rol de mesero",
            IsSystem = false
        };
        _context.Roles.Add(role);

        _context.RolePermissions.Add(new RolePermission
        {
            RoleId = _existingRoleId,
            PermissionId = _permissionId1
        });

        // Rol del sistema (no editable, no eliminable)
        var systemRole = new Role
        {
            Id = _systemRoleId,
            TenantId = _tenantId,
            Name = "Administrador",
            NormalizedName = "ADMINISTRADOR",
            IsSystem = true
        };
        _context.Roles.Add(systemRole);

        // Rol con usuarios asignados (no eliminable)
        var roleWithUsers = new Role
        {
            Id = _roleWithUsersId,
            TenantId = _tenantId,
            Name = "Cajero",
            NormalizedName = "CAJERO",
            IsSystem = false
        };
        _context.Roles.Add(roleWithUsers);

        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantId,
            Email = "cajero@test.com",
            PasswordHash = "hash",
            FirstName = "Test",
            LastName = "Cajero",
            Status = UserStatus.Active
        };
        _context.Users.Add(user);

        _context.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = _roleWithUsersId
        });

        _context.SaveChangesAsync().GetAwaiter().GetResult();
    }

    [Fact]
    public async Task GetAll_RetornaRolesPaginados()
    {
        // Arrange
        var request = new PagedRequest(Page: 1, PageSize: 10);

        // Act
        var result = await _roleService.GetAllAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.TotalCount.Should().BeGreaterThanOrEqualTo(3);
        result.Data.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_ConRolExistente_RetornaRolConPermisos()
    {
        // Act
        var result = await _roleService.GetByIdAsync(_existingRoleId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Mesero");
        result.Data.Permissions.Should().NotBeEmpty();
        result.Data.Permissions.Should().Contain(p => p.Code == "users.view");
    }

    [Fact]
    public async Task GetById_ConRolInexistente_RetornaFallo()
    {
        // Act
        var result = await _roleService.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Rol no encontrado");
    }

    [Fact]
    public async Task Create_ConDatosValidos_RetornaExito()
    {
        // Arrange
        var request = new CreateRoleRequest(
            Name: "Cocinero",
            Description: "Rol de cocina",
            PermissionIds: new[] { _permissionId1, _permissionId2 }
        );

        // Act
        var result = await _roleService.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Cocinero");
        result.Data.Description.Should().Be("Rol de cocina");
        result.Data.IsSystem.Should().BeFalse();
        result.Data.Permissions.Should().HaveCount(2);
    }

    [Fact]
    public async Task Create_ConNombreDuplicado_RetornaFallo()
    {
        // Arrange
        var request = new CreateRoleRequest(
            Name: "Mesero",
            Description: "Duplicado",
            PermissionIds: new[] { _permissionId1 }
        );

        // Act
        var result = await _roleService.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Ya existe un rol con ese nombre");
    }

    [Fact]
    public async Task Update_ActualizaPermisos()
    {
        // Arrange: actualizar el rol existente con ambos permisos
        var request = new UpdateRoleRequest(
            Name: "Mesero Senior",
            Description: "Mesero con mas permisos",
            PermissionIds: new[] { _permissionId1, _permissionId2 }
        );

        // Act
        var result = await _roleService.UpdateAsync(_existingRoleId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Mesero Senior");
        result.Data.Permissions.Should().HaveCount(2);
    }

    [Fact]
    public async Task Update_RolSistema_RetornaFallo()
    {
        // Arrange
        var request = new UpdateRoleRequest(
            Name: "Otro Nombre",
            Description: "Intentando modificar",
            PermissionIds: new[] { _permissionId1 }
        );

        // Act
        var result = await _roleService.UpdateAsync(_systemRoleId, request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("No se puede modificar un rol del sistema");
    }

    [Fact]
    public async Task Delete_RolNoSistema_SinUsuarios_RetornaExito()
    {
        // Act
        var result = await _roleService.DeleteAsync(_existingRoleId);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_RolSistema_RetornaFallo()
    {
        // Act
        var result = await _roleService.DeleteAsync(_systemRoleId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("No se puede eliminar un rol del sistema");
    }

    [Fact]
    public async Task Delete_RolConUsuarios_RetornaFallo()
    {
        // Act
        var result = await _roleService.DeleteAsync(_roleWithUsersId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("No se puede eliminar un rol asignado a usuarios");
    }

    [Fact]
    public async Task GetAllPermissions_RetornaPermisos()
    {
        // Act
        var result = await _roleService.GetAllPermissionsAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task Create_ConPermisoInexistente_RetornaFallo()
    {
        // Arrange
        var fakePermissionId = Guid.NewGuid();
        var request = new CreateRoleRequest(
            Name: "NuevoRol",
            Description: null,
            PermissionIds: new[] { _permissionId1, fakePermissionId }
        );

        // Act
        var result = await _roleService.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Uno o más permisos no existen");
    }

    [Fact]
    public async Task Update_ConNombreDuplicado_RetornaFallo()
    {
        // Arrange: intentar renombrar "Mesero" a "Cajero" que ya existe
        var request = new UpdateRoleRequest(
            Name: "Cajero",
            Description: null,
            PermissionIds: new[] { _permissionId1 }
        );

        // Act
        var result = await _roleService.UpdateAsync(_existingRoleId, request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Ya existe un rol con ese nombre");
    }

    [Fact]
    public async Task Update_RolInexistente_RetornaFallo()
    {
        // Arrange
        var request = new UpdateRoleRequest(
            Name: "Algo",
            Description: null,
            PermissionIds: new[] { _permissionId1 }
        );

        // Act
        var result = await _roleService.UpdateAsync(Guid.NewGuid(), request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Rol no encontrado");
    }

    [Fact]
    public async Task Update_ConPermisoInexistente_RetornaFallo()
    {
        // Arrange
        var fakePermissionId = Guid.NewGuid();
        var request = new UpdateRoleRequest(
            Name: "Mesero",
            Description: null,
            PermissionIds: new[] { fakePermissionId }
        );

        // Act
        var result = await _roleService.UpdateAsync(_existingRoleId, request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Uno o más permisos no existen");
    }

    [Fact]
    public async Task Delete_RolInexistente_RetornaFallo()
    {
        // Act
        var result = await _roleService.DeleteAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Rol no encontrado");
    }

    [Fact]
    public async Task GetAll_ConBusqueda_FiltraResultados()
    {
        // Arrange
        var request = new PagedRequest(Page: 1, PageSize: 10, Search: "mesero");

        // Act
        var result = await _roleService.GetAllAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().Contain(r => r.Name == "Mesero");
        result.Data.Items.Should().NotContain(r => r.Name == "Cajero");
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
