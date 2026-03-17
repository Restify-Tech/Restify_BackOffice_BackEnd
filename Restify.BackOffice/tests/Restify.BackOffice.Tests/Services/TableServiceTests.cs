using FluentAssertions;
using Moq;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Services;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Tests.Services;

public class TableServiceTests
{
    private readonly Mock<ITableRepository> _tableRepoMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly TableService _sut;
    private readonly Guid _tenantId = Guid.NewGuid();

    public TableServiceTests()
    {
        _tableRepoMock = new Mock<ITableRepository>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _currentUserMock.Setup(c => c.TenantId).Returns(_tenantId);

        _sut = new TableService(_tableRepoMock.Object, _currentUserMock.Object);
    }

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsTable()
    {
        // Arrange
        var id = Guid.NewGuid();
        var table = CreateTable(id, "1", 4);
        _tableRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(table);

        // Act
        var result = await _sut.GetByIdAsync(id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Number.Should().Be("1");
        result.Data.Capacity.Should().Be(4);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsFailure()
    {
        // Arrange
        _tableRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Table?)null);

        // Act
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    #endregion

    #region GetByNumberAsync

    [Fact]
    public async Task GetByNumberAsync_WhenExists_ReturnsTable()
    {
        // Arrange
        var table = CreateTable(Guid.NewGuid(), "5", 6);
        _tableRepoMock.Setup(r => r.GetByNumberAsync("5", It.IsAny<CancellationToken>()))
            .ReturnsAsync(table);

        // Act
        var result = await _sut.GetByNumberAsync("5");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Number.Should().Be("5");
    }

    [Fact]
    public async Task GetByNumberAsync_WhenNotFound_ReturnsFailure()
    {
        // Arrange
        _tableRepoMock.Setup(r => r.GetByNumberAsync("99", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Table?)null);

        // Act
        var result = await _sut.GetByNumberAsync("99");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateTableRequest
        {
            Number = "10",
            Capacity = 4,
            Zone = "Terraza",
            IsActive = true
        };

        _tableRepoMock.Setup(r => r.GetByNumberAsync("10", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Table?)null);
        _tableRepoMock.Setup(r => r.CreateAsync(It.IsAny<Table>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Table t, CancellationToken _) => t);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Number.Should().Be("10");
        result.Data.Capacity.Should().Be(4);
    }

    [Theory]
    [InlineData("", 4)]
    [InlineData("  ", 4)]
    public async Task CreateAsync_WithEmptyNumber_ReturnsFailure(string number, int capacity)
    {
        // Arrange
        var request = new CreateTableRequest { Number = number, Capacity = capacity };

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("número de mesa");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public async Task CreateAsync_WithInvalidCapacity_ReturnsFailure(int capacity)
    {
        // Arrange
        var request = new CreateTableRequest { Number = "1", Capacity = capacity };

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("capacidad");
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateNumber_ReturnsFailure()
    {
        // Arrange
        var existing = CreateTable(Guid.NewGuid(), "1", 4);
        var request = new CreateTableRequest { Number = "1", Capacity = 4 };

        _tableRepoMock.Setup(r => r.GetByNumberAsync("1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Ya existe");
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existing = CreateTable(id, "1", 4);
        var request = new UpdateTableRequest
        {
            Number = "1A",
            Capacity = 6,
            IsActive = true,
            Zone = "Interior"
        };

        _tableRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _tableRepoMock.Setup(r => r.GetByNumberAsync("1A", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Table?)null);
        _tableRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Table>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Table t, CancellationToken _) => t);

        // Act
        var result = await _sut.UpdateAsync(id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Number.Should().Be("1A");
        result.Data.Capacity.Should().Be(6);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotFound_ReturnsFailure()
    {
        // Arrange
        var request = new UpdateTableRequest { Number = "1", Capacity = 4 };
        _tableRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Table?)null);

        // Act
        var result = await _sut.UpdateAsync(Guid.NewGuid(), request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    [Fact]
    public async Task UpdateAsync_WithDuplicateNumber_ReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        var otherId = Guid.NewGuid();
        var existing = CreateTable(id, "1", 4);
        var otherTable = CreateTable(otherId, "2", 4);
        var request = new UpdateTableRequest { Number = "2", Capacity = 4 };

        _tableRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _tableRepoMock.Setup(r => r.GetByNumberAsync("2", It.IsAny<CancellationToken>()))
            .ReturnsAsync(otherTable);

        // Act
        var result = await _sut.UpdateAsync(id, request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Ya existe otra");
    }

    [Fact]
    public async Task UpdateAsync_SameNumberSameTable_ReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existing = CreateTable(id, "1", 4);
        var request = new UpdateTableRequest { Number = "1", Capacity = 6 };

        _tableRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _tableRepoMock.Setup(r => r.GetByNumberAsync("1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _tableRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Table>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Table t, CancellationToken _) => t);

        // Act
        var result = await _sut.UpdateAsync(id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    #endregion

    #region UpdateStatusAsync

    [Fact]
    public async Task UpdateStatusAsync_ToOccupied_SetsOccupiedFields()
    {
        // Arrange
        var id = Guid.NewGuid();
        var table = CreateTable(id, "1", 4, TableStatus.Available);
        var orderId = Guid.NewGuid();
        var request = new UpdateTableStatusRequest
        {
            Status = TableStatus.Occupied,
            CurrentCustomerName = "Juan",
            CurrentOrderId = orderId
        };

        _tableRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(table);
        _tableRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Table>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Table t, CancellationToken _) => t);

        // Act
        var result = await _sut.UpdateStatusAsync(id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        table.Status.Should().Be(TableStatus.Occupied);
        table.CurrentCustomerName.Should().Be("Juan");
        table.CurrentOrderId.Should().Be(orderId);
        table.OccupiedSince.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateStatusAsync_FromOccupiedToCleaning_ClearsOccupiedFields()
    {
        // Arrange
        var id = Guid.NewGuid();
        var table = CreateTable(id, "1", 4, TableStatus.Occupied);
        table.CurrentCustomerName = "Juan";
        table.CurrentOrderId = Guid.NewGuid();
        table.OccupiedSince = DateTime.UtcNow.AddHours(-1);

        var request = new UpdateTableStatusRequest { Status = TableStatus.Cleaning };

        _tableRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(table);
        _tableRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Table>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Table t, CancellationToken _) => t);

        // Act
        var result = await _sut.UpdateStatusAsync(id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        table.Status.Should().Be(TableStatus.Cleaning);
        table.CurrentCustomerName.Should().BeNull();
        table.CurrentOrderId.Should().BeNull();
        table.OccupiedSince.Should().BeNull();
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_WhenAvailable_ReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var table = CreateTable(id, "1", 4, TableStatus.Available);
        _tableRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(table);
        _tableRepoMock.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteAsync(id);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_WhenOccupied_ReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        var table = CreateTable(id, "1", 4, TableStatus.Occupied);
        _tableRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(table);

        // Act
        var result = await _sut.DeleteAsync(id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("ocupada");
    }

    [Fact]
    public async Task DeleteAsync_WhenNotFound_ReturnsFailure()
    {
        // Arrange
        _tableRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Table?)null);

        // Act
        var result = await _sut.DeleteAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    #endregion

    #region GetLayoutAsync

    [Fact]
    public async Task GetLayoutAsync_ReturnsCorrectCounts()
    {
        // Arrange
        var tables = new List<Table>
        {
            CreateTable(Guid.NewGuid(), "1", 4, TableStatus.Available),
            CreateTable(Guid.NewGuid(), "2", 4, TableStatus.Occupied),
            CreateTable(Guid.NewGuid(), "3", 6, TableStatus.Reserved),
            CreateTable(Guid.NewGuid(), "4", 2, TableStatus.Available)
        };
        var zones = new List<string> { "Interior", "Terraza" };

        _tableRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tables);
        _tableRepoMock.Setup(r => r.GetZonesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(zones);

        // Act
        var result = await _sut.GetLayoutAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.AvailableCount.Should().Be(2);
        result.Data.OccupiedCount.Should().Be(1);
        result.Data.ReservedCount.Should().Be(1);
        result.Data.TotalCapacity.Should().Be(16);
        result.Data.Zones.Should().HaveCount(2);
    }

    #endregion

    #region Helpers

    private static Table CreateTable(Guid id, string number, int capacity, TableStatus status = TableStatus.Available)
    {
        return new Table
        {
            Id = id,
            Number = number,
            Capacity = capacity,
            Status = status,
            IsActive = true
        };
    }

    #endregion
}
