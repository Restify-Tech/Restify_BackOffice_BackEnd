using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.BackOffice.Infrastructure.Services;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Tests.Services;

public class ShiftServiceTests : IDisposable
{
    private readonly BackOfficeDbContext _context;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly ShiftService _sut;

    private static readonly Guid TenantId = Guid.NewGuid();

    public ShiftServiceTests()
    {
        var options = new DbContextOptionsBuilder<BackOfficeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new BackOfficeDbContext(options);
        _currentUserMock = new Mock<ICurrentUserService>();
        _currentUserMock.Setup(c => c.TenantId).Returns(TenantId);

        _sut = new ShiftService(_context, _currentUserMock.Object);
    }

    public void Dispose() => _context.Dispose();

    #region GetTemplatesAsync

    [Fact]
    public async Task GetTemplatesAsync_ReturnsTemplates()
    {
        // Arrange
        _context.ShiftTemplates.AddRange(
            CreateTemplate("Turno Manana"),
            CreateTemplate("Turno Tarde"));
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetTemplatesAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetTemplatesAsync_ReturnsEmpty_WhenNoTemplates()
    {
        // Act
        var result = await _sut.GetTemplatesAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    #endregion

    #region CreateTemplateAsync

    [Fact]
    public async Task CreateTemplateAsync_CreatesTemplate_WithValidTimes()
    {
        // Arrange
        var request = new CreateShiftTemplateRequest(
            Name: "Turno Noche",
            StartTime: "20:00",
            EndTime: "04:00",
            DaysOfWeek: new List<int> { 5, 6, 7 },
            BranchId: null);

        // Act
        var result = await _sut.CreateTemplateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("Turno Noche");
        _context.ShiftTemplates.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateTemplateAsync_ReturnsFailure_WithInvalidStartTime()
    {
        // Arrange
        var request = new CreateShiftTemplateRequest(
            Name: "Turno Invalido",
            StartTime: "25:99",
            EndTime: "16:00",
            DaysOfWeek: new List<int> { 1, 2, 3, 4, 5 },
            BranchId: null);

        // Act
        var result = await _sut.CreateTemplateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("inicio invalida");
    }

    [Fact]
    public async Task CreateTemplateAsync_ReturnsFailure_WithInvalidEndTime()
    {
        // Arrange
        var request = new CreateShiftTemplateRequest(
            Name: "Turno Invalido",
            StartTime: "08:00",
            EndTime: "noeshorario",
            DaysOfWeek: new List<int> { 1, 2, 3, 4, 5 },
            BranchId: null);

        // Act
        var result = await _sut.CreateTemplateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("fin invalida");
    }

    #endregion

    #region CreateAssignmentAsync

    [Fact]
    public async Task CreateAssignmentAsync_AssignsShift_WhenEmployeeExists()
    {
        // Arrange
        var employee = CreateEmployee("Carlos", "Ruiz");
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        var request = new CreateShiftAssignmentRequest(
            EmployeeId: employee.Id,
            ShiftTemplateId: null,
            Date: DateTime.Today,
            ScheduledStart: "08:00",
            ScheduledEnd: "16:00");

        // Act
        var result = await _sut.CreateAssignmentAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.EmployeeId.Should().Be(employee.Id);
        result.Data.Status.Should().Be(ShiftStatus.Scheduled);
        _context.ShiftAssignments.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateAssignmentAsync_ReturnsFailure_WhenEmployeeNotFound()
    {
        // Arrange
        var request = new CreateShiftAssignmentRequest(
            EmployeeId: Guid.NewGuid(),
            ShiftTemplateId: null,
            Date: DateTime.Today,
            ScheduledStart: null,
            ScheduledEnd: null);

        // Act
        var result = await _sut.CreateAssignmentAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrado");
    }

    #endregion

    #region ClockInAsync

    [Fact]
    public async Task ClockInAsync_RecordsClockIn_WhenAssignmentExists()
    {
        // Arrange
        var employee = CreateEmployee("Ana", "Torres");
        _context.Employees.Add(employee);

        var assignment = CreateAssignment(employee, scheduledStart: TimeSpan.FromHours(8));
        _context.ShiftAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        var request = new ClockInRequest(
            AssignmentId: assignment.Id,
            Method: "Manual",
            Location: null);

        // Act
        var result = await _sut.ClockInAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.ActualClockIn.Should().NotBeNull();
    }

    [Fact]
    public async Task ClockInAsync_ReturnsFailure_WhenAssignmentNotFound()
    {
        // Arrange
        var request = new ClockInRequest(
            AssignmentId: Guid.NewGuid(),
            Method: "Manual",
            Location: null);

        // Act
        var result = await _sut.ClockInAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    [Fact]
    public async Task ClockInAsync_ReturnsFailure_WhenAlreadyClockedIn()
    {
        // Arrange
        var employee = CreateEmployee("Luis", "Paredes");
        _context.Employees.Add(employee);

        var assignment = CreateAssignment(employee);
        assignment.ActualClockIn = DateTime.UtcNow.AddHours(-2);
        _context.ShiftAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        var request = new ClockInRequest(
            AssignmentId: assignment.Id,
            Method: "Manual",
            Location: null);

        // Act
        var result = await _sut.ClockInAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("ya registro su entrada");
    }

    [Fact]
    public async Task ClockInAsync_MarksLate_WhenAfter15Minutes()
    {
        // Arrange
        var employee = CreateEmployee("Pedro", "Jimenez");
        _context.Employees.Add(employee);

        // Turno programado para hace 30 minutos (ya es tarde)
        var scheduledStart = DateTime.UtcNow.TimeOfDay.Add(TimeSpan.FromMinutes(-30));
        var assignment = CreateAssignment(employee, scheduledStart: scheduledStart);
        assignment.Date = DateTime.UtcNow.Date;
        _context.ShiftAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        var request = new ClockInRequest(
            AssignmentId: assignment.Id,
            Method: "Manual",
            Location: null);

        // Act
        var result = await _sut.ClockInAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Status.Should().Be(ShiftStatus.Late);
    }

    #endregion

    #region ClockOutAsync

    [Fact]
    public async Task ClockOutAsync_CalculatesHoursWorked()
    {
        // Arrange
        var employee = CreateEmployee("Maria", "Salazar");
        _context.Employees.Add(employee);

        var assignment = CreateAssignment(employee);
        assignment.ActualClockIn = DateTime.UtcNow.AddHours(-8);
        assignment.Status = ShiftStatus.Present;
        _context.ShiftAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        var request = new ClockOutRequest(AssignmentId: assignment.Id);

        // Act
        var result = await _sut.ClockOutAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.ActualClockOut.Should().NotBeNull();
        result.Data.HoursWorked.Should().BeApproximately(8m, 0.1m);
        result.Data.Status.Should().Be(ShiftStatus.Completed);
    }

    [Fact]
    public async Task ClockOutAsync_ReturnsFailure_WhenNotClockedIn()
    {
        // Arrange
        var employee = CreateEmployee("Sofia", "Leon");
        _context.Employees.Add(employee);

        var assignment = CreateAssignment(employee);
        // Sin ActualClockIn
        _context.ShiftAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        var request = new ClockOutRequest(AssignmentId: assignment.Id);

        // Act
        var result = await _sut.ClockOutAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no ha registrado su entrada");
    }

    #endregion

    #region Helpers

    private static ShiftTemplate CreateTemplate(string name) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = TenantId,
        Name = name,
        StartTime = TimeSpan.FromHours(8),
        EndTime = TimeSpan.FromHours(16),
        DaysOfWeek = "[1,2,3,4,5]",
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    private static Employee CreateEmployee(string firstName, string lastName) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = TenantId,
        FirstName = firstName,
        LastName = lastName,
        IdentificationNumber = "1234567890",
        Email = $"{firstName.ToLower()}@demo.com",
        Position = "Mesero",
        HireDate = DateTime.UtcNow.AddYears(-1),
        BaseSalary = 500m,
        EmploymentType = EmploymentType.FullTime,
        Status = EmployeeStatus.Active,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    private static ShiftAssignment CreateAssignment(Employee employee, TimeSpan? scheduledStart = null) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = TenantId,
        EmployeeId = employee.Id,
        Employee = employee,
        Date = DateTime.UtcNow.Date,
        ScheduledStart = scheduledStart ?? TimeSpan.FromHours(8),
        ScheduledEnd = TimeSpan.FromHours(16),
        Status = ShiftStatus.Scheduled,
        CreatedAt = DateTime.UtcNow
    };

    #endregion
}
