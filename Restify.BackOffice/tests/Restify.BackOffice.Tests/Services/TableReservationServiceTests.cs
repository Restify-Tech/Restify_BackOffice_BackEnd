using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.BackOffice.Infrastructure.Services;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Tests.Services;

public class TableReservationServiceTests : IDisposable
{
    private readonly BackOfficeDbContext _context;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly Mock<INotificationClient> _notificationClientMock;
    private readonly TableReservationService _sut;

    private static readonly Guid TenantId = Guid.NewGuid();

    public TableReservationServiceTests()
    {
        var options = new DbContextOptionsBuilder<BackOfficeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new BackOfficeDbContext(options);
        _currentUserMock = new Mock<ICurrentUserService>();
        _currentUserMock.Setup(c => c.TenantId).Returns(TenantId);
        _currentUserMock.Setup(c => c.Email).Returns("staff@demo.com");

        _notificationClientMock = new Mock<INotificationClient>();
        _notificationClientMock
            .Setup(n => n.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _sut = new TableReservationService(
            _context,
            _currentUserMock.Object,
            _notificationClientMock.Object,
            NullLogger<TableReservationService>.Instance);
    }

    public void Dispose() => _context.Dispose();

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_CreatesReservation_WithConfirmationCode()
    {
        // Arrange
        var request = new CreateReservationRequest(
            TableId: null,
            BranchId: null,
            CustomerName: "Maria Garcia",
            CustomerPhone: "0991234567",
            CustomerEmail: "maria@example.com",
            ReservationDateTime: DateTime.UtcNow.AddDays(1),
            PartySize: 4,
            SpecialRequests: "Mesa cerca de la ventana");

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.CustomerName.Should().Be("Maria Garcia");
        result.Data.Status.Should().Be(ReservationStatus.Pending);
        result.Data.ConfirmationCode.Should().NotBeNullOrEmpty();
        _context.TableReservations.Should().HaveCount(1);
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_ReturnsReservation_WhenFound()
    {
        // Arrange
        var reservation = CreateReservation("Carlos Lopez", ReservationStatus.Pending);
        _context.TableReservations.Add(reservation);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetByIdAsync(reservation.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.CustomerName.Should().Be("Carlos Lopez");
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

    #region ConfirmAsync

    [Fact]
    public async Task ConfirmAsync_ChangesStatusToConfirmed_WhenPending()
    {
        // Arrange
        var reservation = CreateReservation("Ana Torres", ReservationStatus.Pending);
        _context.TableReservations.Add(reservation);
        await _context.SaveChangesAsync();

        var request = new ConfirmReservationRequest(TableId: null);

        // Act
        var result = await _sut.ConfirmAsync(reservation.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Status.Should().Be(ReservationStatus.Confirmed);
    }

    [Fact]
    public async Task ConfirmAsync_ReturnsFailure_WhenNotInPendingStatus()
    {
        // Arrange
        var reservation = CreateReservation("Pedro Morales", ReservationStatus.Confirmed);
        _context.TableReservations.Add(reservation);
        await _context.SaveChangesAsync();

        var request = new ConfirmReservationRequest(TableId: null);

        // Act
        var result = await _sut.ConfirmAsync(reservation.Id, request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Pendiente");
    }

    #endregion

    #region CancelAsync

    [Fact]
    public async Task CancelAsync_CancelsReservation_WhenNotCompleted()
    {
        // Arrange
        var reservation = CreateReservation("Laura Diaz", ReservationStatus.Confirmed);
        _context.TableReservations.Add(reservation);
        await _context.SaveChangesAsync();

        var request = new CancelReservationRequest(Reason: "El cliente llamo para cancelar");

        // Act
        var result = await _sut.CancelAsync(reservation.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Status.Should().Be(ReservationStatus.Cancelled);
        result.Data.CancellationReason.Should().Be("El cliente llamo para cancelar");
    }

    [Fact]
    public async Task CancelAsync_ReturnsFailure_WhenAlreadyCancelled()
    {
        // Arrange
        var reservation = CreateReservation("Jose Rios", ReservationStatus.Cancelled);
        _context.TableReservations.Add(reservation);
        await _context.SaveChangesAsync();

        var request = new CancelReservationRequest(Reason: "Intento de cancelar ya cancelada");

        // Act
        var result = await _sut.CancelAsync(reservation.Id, request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("cancelada");
    }

    #endregion

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_FiltersReservationsByDate()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var reservationToday = CreateReservation("Hoy Cliente", ReservationStatus.Pending);
        reservationToday.ReservationDateTime = today.AddHours(12);

        var reservationTomorrow = CreateReservation("Manana Cliente", ReservationStatus.Pending);
        reservationTomorrow.ReservationDateTime = tomorrow.AddHours(14);

        _context.TableReservations.AddRange(reservationToday, reservationTomorrow);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllAsync(date: today);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        result.Data!.First().CustomerName.Should().Be("Hoy Cliente");
    }

    #endregion

    #region Helpers

    private static TableReservation CreateReservation(string customerName, ReservationStatus status) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = TenantId,
        CustomerName = customerName,
        CustomerPhone = "0999999999",
        ReservationDateTime = DateTime.UtcNow.AddDays(1),
        PartySize = 2,
        Status = status,
        ConfirmationCode = "123456",
        CreatedAt = DateTime.UtcNow
    };

    #endregion
}
