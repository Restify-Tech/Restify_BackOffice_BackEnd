using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Domain.Entities;
using Restify.Auth.Domain.Enums;
using Restify.Auth.Infrastructure.Services;
using Restify.Auth.Tests.Helpers;

namespace Restify.Auth.Tests.Services;

public class ScreenPermissionServiceTests : IDisposable
{
    private readonly Infrastructure.Persistence.AppDbContext _context;
    private readonly ScreenPermissionService _service;

    public ScreenPermissionServiceTests()
    {
        _context = TestDbContextFactory.Create();
        var loggerMock = new Mock<ILogger<ScreenPermissionService>>();
        _service = new ScreenPermissionService(_context, loggerMock.Object);
        SeedData();
    }

    private void SeedData()
    {
        var tenantId = Guid.NewGuid();
        
        var tenant = new Tenant
        {
            Id = tenantId,
            Name = "Test Restaurant",
            Slug = "test",
            Status = TenantStatus.Active,
            Currency = "USD",
            TaxPercentage = 12
        };
        _context.Tenants.Add(tenant);

        var screenPermissions = new[]
        {
            new ScreenPermission { Id = Guid.NewGuid(), ScreenCode = "dashboard", ScreenName = "Dashboard", Module = "General", Route = "/dashboard", RequiredPermission = "dashboard.view", DisplayOrder = 1 },
            new ScreenPermission { Id = Guid.NewGuid(), ScreenCode = "orders", ScreenName = "Pedidos", Module = "Operaciones", Route = "/orders", RequiredPermission = "orders.view", DisplayOrder = 2 },
            new ScreenPermission { Id = Guid.NewGuid(), ScreenCode = "kitchen", ScreenName = "Cocina", Module = "Operaciones", Route = "/kitchen", RequiredPermission = "kitchen.view", DisplayOrder = 3 },
        };
        _context.ScreenPermissions.AddRange(screenPermissions);

        _context.SaveChangesAsync().GetAwaiter().GetResult();
    }

    [Fact]
    public async Task GetAll_RetornaTodasLasPantallas()
    {
        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
        result.Data.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetByModule_AgrupaPorModulo()
    {
        // Act
        var result = await _service.GetByModuleAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        var data = result.Data.ToList();
        data.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAccessibleScreens_ConPermisosValidos_RetornaPantallasAccesibles()
    {
        // Arrange
        var permissions = new[] { "dashboard.view", "orders.view" };

        // Act
        var result = await _service.GetAccessibleScreensAsync(permissions);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAccessibleScreens_SinPermisos_RetornaVacio()
    {
        // Arrange
        var permissions = Array.Empty<string>();

        // Act
        var result = await _service.GetAccessibleScreensAsync(permissions);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
