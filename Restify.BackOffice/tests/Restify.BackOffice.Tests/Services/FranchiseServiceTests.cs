using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.BackOffice.Infrastructure.Services;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Tests.Services;

public class FranchiseServiceTests : IDisposable
{
    private readonly BackOfficeDbContext _context;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly Mock<ILogger<FranchiseService>> _loggerMock;
    private readonly FranchiseService _sut;

    private static readonly Guid TenantId = Guid.NewGuid();

    public FranchiseServiceTests()
    {
        var options = new DbContextOptionsBuilder<BackOfficeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new BackOfficeDbContext(options);
        _currentUserMock = new Mock<ICurrentUserService>();
        _currentUserMock.Setup(c => c.TenantId).Returns(TenantId);
        _loggerMock = new Mock<ILogger<FranchiseService>>();

        _sut = new FranchiseService(_context, _currentUserMock.Object, _loggerMock.Object);
    }

    public void Dispose() => _context.Dispose();

    #region GetMyFranchiseAsync

    [Fact]
    public async Task GetMyFranchiseAsync_ReturnsNull_WhenNoFranchiseConfigured()
    {
        // Act
        var result = await _sut.GetMyFranchiseAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetMyFranchiseAsync_ReturnsFranchiseConfig_WhenConfigured()
    {
        // Arrange
        var config = CreateFranchiseConfig("Mi Franquicia");
        _context.FranchiseConfigs.Add(config);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetMyFranchiseAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.FranchiseName.Should().Be("Mi Franquicia");
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_CreatesFranchiseConfig_WhenNotExists()
    {
        // Arrange
        var request = new CreateFranchiseConfigRequest(
            FranchiseName: "Restify Franquicias",
            Description: "Red nacional de restaurantes",
            AllowLocalMenuOverrides: true,
            AllowLocalPromotions: false,
            SyncMenuAutomatically: true,
            ContactEmail: "franquicias@restify.com");

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.FranchiseName.Should().Be("Restify Franquicias");
        result.Data.AllowLocalMenuOverrides.Should().BeTrue();
        _context.FranchiseConfigs.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFailure_WhenFranchiseAlreadyExists()
    {
        // Arrange
        var config = CreateFranchiseConfig("Franquicia Existente");
        _context.FranchiseConfigs.Add(config);
        await _context.SaveChangesAsync();

        var request = new CreateFranchiseConfigRequest(
            FranchiseName: "Otra Franquicia",
            Description: null,
            AllowLocalMenuOverrides: true,
            AllowLocalPromotions: true,
            SyncMenuAutomatically: false,
            ContactEmail: null);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Ya existe una configuracion");
    }

    #endregion

    #region AddFranchiseeAsync

    [Fact]
    public async Task AddFranchiseeAsync_AddsFranchisee_WhenConfigExists()
    {
        // Arrange
        var config = CreateFranchiseConfig("Franquicia Master");
        _context.FranchiseConfigs.Add(config);
        await _context.SaveChangesAsync();

        var franchiseeTenantId = Guid.NewGuid();
        var request = new AddFranchiseeRequest(
            FranchiseeTenantId: franchiseeTenantId,
            FranchiseeName: "Sucursal Norte S.A.",
            FranchiseeCity: "Quito",
            RoyaltyPercentage: "5%");

        // Act
        var result = await _sut.AddFranchiseeAsync(config.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.FranchiseeName.Should().Be("Sucursal Norte S.A.");
        result.Data.FranchiseeCity.Should().Be("Quito");
        _context.FranchiseeRelations.Should().HaveCount(1);
    }

    [Fact]
    public async Task AddFranchiseeAsync_ReturnsFailure_WhenFranchiseeAlreadyAdded()
    {
        // Arrange
        var config = CreateFranchiseConfig("Franquicia Duplicado");
        _context.FranchiseConfigs.Add(config);

        var franchiseeTenantId = Guid.NewGuid();
        var existing = new FranchiseeRelation
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            FranchiseConfigId = config.Id,
            FranchiseeTenantId = franchiseeTenantId,
            FranchiseeName = "Franquiciado Existente",
            IsActive = true,
            JoinedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        _context.FranchiseeRelations.Add(existing);
        await _context.SaveChangesAsync();

        var request = new AddFranchiseeRequest(
            FranchiseeTenantId: franchiseeTenantId,
            FranchiseeName: "Franquiciado Existente",
            FranchiseeCity: null,
            RoyaltyPercentage: null);

        // Act
        var result = await _sut.AddFranchiseeAsync(config.Id, request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("ya esta registrado");
    }

    #endregion

    #region GetConsolidatedReportAsync

    [Fact]
    public async Task GetConsolidatedReportAsync_ReturnsReport_WithMockData()
    {
        // Arrange
        var config = CreateFranchiseConfig("Franquicia Reporte");
        _context.FranchiseConfigs.Add(config);

        var franchisee = new FranchiseeRelation
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            FranchiseConfigId = config.Id,
            FranchiseeTenantId = Guid.NewGuid(),
            FranchiseeName = "Franquiciado Sur",
            FranchiseeCity = "Guayaquil",
            IsActive = true,
            JoinedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        _context.FranchiseeRelations.Add(franchisee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetConsolidatedReportAsync(config.Id, null, null);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.FranchiseName.Should().Be("Franquicia Reporte");
        result.Data.Franchisees.Should().HaveCount(1);
        result.Data.TotalRevenue.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetConsolidatedReportAsync_ReturnsFailure_WhenConfigNotFound()
    {
        // Act
        var result = await _sut.GetConsolidatedReportAsync(Guid.NewGuid(), null, null);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    #endregion

    #region Helpers

    private FranchiseConfig CreateFranchiseConfig(string name) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = TenantId,
        FranchiseName = name,
        AllowLocalMenuOverrides = true,
        AllowLocalPromotions = true,
        SyncMenuAutomatically = false,
        CreatedAt = DateTime.UtcNow,
        Franchisees = new List<FranchiseeRelation>()
    };

    #endregion
}
