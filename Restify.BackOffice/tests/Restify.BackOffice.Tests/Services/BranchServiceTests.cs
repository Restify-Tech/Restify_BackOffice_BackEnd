using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.BackOffice.Infrastructure.Services;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Tests.Services;

public class BranchServiceTests : IDisposable
{
    private readonly BackOfficeDbContext _context;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly BranchService _sut;

    private static readonly Guid TenantId = Guid.NewGuid();

    public BranchServiceTests()
    {
        var options = new DbContextOptionsBuilder<BackOfficeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new BackOfficeDbContext(options);
        _currentUserMock = new Mock<ICurrentUserService>();
        _currentUserMock.Setup(c => c.TenantId).Returns(TenantId);

        _sut = new BranchService(_context, _currentUserMock.Object);
    }

    public void Dispose() => _context.Dispose();

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_ReturnsBranchesForTenant()
    {
        // Arrange
        _context.Branches.AddRange(
            CreateBranch("Sucursal Norte"),
            CreateBranch("Sucursal Sur"));
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyWhenNoBranches()
    {
        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectBranch()
    {
        // Arrange
        var branch = CreateBranch("Sucursal Centro");
        _context.Branches.Add(branch);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetByIdAsync(branch.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("Sucursal Centro");
        result.Data.Id.Should().Be(branch.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsFailure_WhenNotFound()
    {
        // Act
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_CreatesNewBranch()
    {
        // Arrange
        var request = new CreateBranchRequest(
            Name: "Sucursal Este",
            Address: "Av. Este 123",
            City: "Guayaquil",
            Phone: "0991234567",
            Email: null,
            OpeningHours: null,
            Notes: null);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("Sucursal Este");
        result.Data.City.Should().Be("Guayaquil");
        _context.Branches.Should().HaveCount(1);
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_UpdatesBranch()
    {
        // Arrange
        var branch = CreateBranch("Sucursal Vieja");
        _context.Branches.Add(branch);
        await _context.SaveChangesAsync();

        var request = new UpdateBranchRequest(
            Name: "Sucursal Actualizada",
            Address: null,
            City: "Cuenca",
            Phone: null,
            Email: null,
            IsActive: true,
            OpeningHours: null,
            Notes: null);

        // Act
        var result = await _sut.UpdateAsync(branch.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("Sucursal Actualizada");
        result.Data.City.Should().Be("Cuenca");
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFailure_WhenNotFound()
    {
        // Arrange
        var request = new UpdateBranchRequest(
            Name: "No Existe",
            Address: null,
            City: null,
            Phone: null,
            Email: null,
            IsActive: true,
            OpeningHours: null,
            Notes: null);

        // Act
        var result = await _sut.UpdateAsync(Guid.NewGuid(), request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_DeletesBranch_WhenNoActiveTables()
    {
        // Arrange
        var branch = CreateBranch("Sucursal a Eliminar");
        _context.Branches.Add(branch);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.DeleteAsync(branch.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
        _context.Branches.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFailure_WhenHasActiveTables()
    {
        // Arrange
        var branch = CreateBranch("Sucursal con Mesas");
        _context.Branches.Add(branch);
        await _context.SaveChangesAsync();

        var table = new Table
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            Number = "1",
            Capacity = 4,
            IsActive = true,
            BranchId = branch.Id,
            CreatedAt = DateTime.UtcNow
        };
        _context.Tables.Add(table);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.DeleteAsync(branch.Id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("mesas activas");
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFailure_WhenNotFound()
    {
        // Act
        var result = await _sut.DeleteAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    #endregion

    #region GetStatsAsync

    [Fact]
    public async Task GetStatsAsync_ReturnsStats_WhenBranchExists()
    {
        // Arrange
        var branch = CreateBranch("Sucursal Stats");
        _context.Branches.Add(branch);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetStatsAsync(branch.Id, null, null);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.TotalOrders.Should().Be(0);
        result.Data.TotalRevenue.Should().Be(0);
    }

    [Fact]
    public async Task GetStatsAsync_ReturnsFailure_WhenBranchNotFound()
    {
        // Act
        var result = await _sut.GetStatsAsync(Guid.NewGuid(), null, null);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    #endregion

    #region Helpers

    private static Branch CreateBranch(string name) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = TenantId,
        Name = name,
        IsActive = true,
        Timezone = "America/Guayaquil",
        CreatedAt = DateTime.UtcNow,
        Tables = new List<Table>(),
        Employees = new List<Employee>()
    };

    #endregion
}
