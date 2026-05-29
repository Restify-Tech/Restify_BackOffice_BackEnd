using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.BackOffice.Infrastructure.Services;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Tests.Services;

public class ManagerAssignmentServiceTests : IDisposable
{
    private readonly BackOfficeDbContext _context;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly ManagerAssignmentService _sut;

    private static readonly Guid TenantId = Guid.NewGuid();

    public ManagerAssignmentServiceTests()
    {
        var options = new DbContextOptionsBuilder<BackOfficeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new BackOfficeDbContext(options);
        _currentUserMock = new Mock<ICurrentUserService>();
        _currentUserMock.Setup(c => c.TenantId).Returns(TenantId);

        _sut = new ManagerAssignmentService(_context, _currentUserMock.Object);
    }

    public void Dispose() => _context.Dispose();

    #region GetByBranchAsync

    [Fact]
    public async Task GetByBranchAsync_ReturnsAssignments_WhenBranchExists()
    {
        // Arrange
        var branch = CreateBranch("Sucursal A");
        _context.Branches.Add(branch);

        var assignment = CreateAssignment(branch.Id, Guid.NewGuid(), "Juan Perez", "juan@demo.com");
        _context.ManagerAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetByBranchAsync(branch.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByBranchAsync_ReturnsFailure_WhenBranchNotFound()
    {
        // Act
        var result = await _sut.GetByBranchAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_CreatesAssignment_WhenBranchExists()
    {
        // Arrange
        var branch = CreateBranch("Sucursal B");
        _context.Branches.Add(branch);
        await _context.SaveChangesAsync();

        var userId = Guid.NewGuid();
        var request = new CreateManagerAssignmentRequest(
            BranchId: branch.Id,
            UserId: userId,
            UserName: "Maria Lopez",
            UserEmail: "maria@demo.com",
            CanApproveCashClosing: true,
            CanVoidOrders: false,
            MaxDiscountPercent: 10,
            ValidFrom: null,
            ValidTo: null);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.UserName.Should().Be("Maria Lopez");
        result.Data.CanApproveCashClosing.Should().BeTrue();
        _context.ManagerAssignments.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFailure_WhenBranchNotFound()
    {
        // Arrange
        var request = new CreateManagerAssignmentRequest(
            BranchId: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            UserName: "Ana Torres",
            UserEmail: "ana@demo.com",
            CanApproveCashClosing: false,
            CanVoidOrders: false,
            MaxDiscountPercent: 0,
            ValidFrom: null,
            ValidTo: null);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    [Fact]
    public async Task CreateAsync_ReturnsFailure_WhenUserAlreadyAssigned()
    {
        // Arrange
        var branch = CreateBranch("Sucursal C");
        _context.Branches.Add(branch);

        var userId = Guid.NewGuid();
        var existingAssignment = CreateAssignment(branch.Id, userId, "Pedro Ruiz", "pedro@demo.com");
        existingAssignment.IsActive = true;
        _context.ManagerAssignments.Add(existingAssignment);
        await _context.SaveChangesAsync();

        var request = new CreateManagerAssignmentRequest(
            BranchId: branch.Id,
            UserId: userId,
            UserName: "Pedro Ruiz",
            UserEmail: "pedro@demo.com",
            CanApproveCashClosing: false,
            CanVoidOrders: false,
            MaxDiscountPercent: 0,
            ValidFrom: null,
            ValidTo: null);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("ya tiene una asignacion activa");
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_DeactivatesAssignment_WhenFound()
    {
        // Arrange
        var branch = CreateBranch("Sucursal D");
        _context.Branches.Add(branch);

        var assignment = CreateAssignment(branch.Id, Guid.NewGuid(), "Carlos Mora", "carlos@demo.com");
        _context.ManagerAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.DeleteAsync(assignment.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();

        var updated = await _context.ManagerAssignments.FindAsync(assignment.Id);
        updated!.IsActive.Should().BeFalse();
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

    private static ManagerAssignment CreateAssignment(Guid branchId, Guid userId, string userName, string email) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = TenantId,
        BranchId = branchId,
        UserId = userId,
        UserName = userName,
        UserEmail = email,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    #endregion
}
