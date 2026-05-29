using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.BackOffice.Infrastructure.Services;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Tests.Services;

public class CashClosingServiceTests : IDisposable
{
    private readonly BackOfficeDbContext _context;
    private readonly Mock<IFileStorageService> _fileStorageMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly CashClosingService _sut;

    private static readonly Guid TenantId = Guid.NewGuid();

    public CashClosingServiceTests()
    {
        var options = new DbContextOptionsBuilder<BackOfficeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new BackOfficeDbContext(options);
        _fileStorageMock = new Mock<IFileStorageService>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _currentUserMock.Setup(c => c.TenantId).Returns(TenantId);

        _sut = new CashClosingService(_context, _fileStorageMock.Object, _currentUserMock.Object);
    }

    public void Dispose() => _context.Dispose();

    #region InitiateAsync

    [Fact]
    public async Task InitiateAsync_CreatesNewClosingInDraftStatus()
    {
        // Arrange
        var (session, _) = await CreateSessionWithRegisterAsync(openingBalance: 100m);

        var request = new InitiateCashClosingRequest(
            SessionId: session.Id,
            ClosedBy: "user-001",
            ClosedByName: "Juan Cajero");

        // Act
        var result = await _sut.InitiateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Status.Should().Be(CashClosingStatus.Draft);
        result.Data.ClosedBy.Should().Be("user-001");
        result.Data.ClosedByName.Should().Be("Juan Cajero");
        _context.CashClosings.Should().HaveCount(1);
    }

    [Fact]
    public async Task InitiateAsync_ReturnsFailure_WhenSessionNotFound()
    {
        // Arrange
        var request = new InitiateCashClosingRequest(
            SessionId: Guid.NewGuid(),
            ClosedBy: "user-001",
            ClosedByName: "Juan Cajero");

        // Act
        var result = await _sut.InitiateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    [Fact]
    public async Task InitiateAsync_ReturnsFailure_WhenClosingAlreadyExists()
    {
        // Arrange
        var (session, _) = await CreateSessionWithRegisterAsync();

        var existingClosing = new CashClosing
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            CashRegisterSessionId = session.Id,
            ClosedBy = "user-001",
            ClosedByName = "Juan Cajero",
            Status = CashClosingStatus.Draft,
            ClosingDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        _context.CashClosings.Add(existingClosing);
        await _context.SaveChangesAsync();

        var request = new InitiateCashClosingRequest(
            SessionId: session.Id,
            ClosedBy: "user-001",
            ClosedByName: "Juan Cajero");

        // Act
        var result = await _sut.InitiateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Ya existe un cierre en proceso");
    }

    #endregion

    #region SubmitDenominationsAsync

    [Fact]
    public async Task SubmitDenominationsAsync_CalculatesDifference()
    {
        // Arrange
        var (session, _) = await CreateSessionWithRegisterAsync(openingBalance: 100m);
        var closing = await CreateClosingAsync(session.Id, CashClosingStatus.Draft, totalExpected: 150m);

        // Denominaciones que suman 160 (diferencia = +10)
        var request = new SubmitDenominationsRequest(
            ClosingId: closing.Id,
            Denominations: new Dictionary<string, int> { { "100", 1 }, { "50", 1 }, { "10", 1 } },
            BankDepositAmount: 150m,
            BankName: "Banco Pichincha",
            BankReference: "REF-001",
            DifferenceReason: null);

        // Act
        var result = await _sut.SubmitDenominationsAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.TotalCounted.Should().Be(160m);
        result.Data.Difference.Should().Be(10m); // 160 - 150
        result.Data.Status.Should().Be(CashClosingStatus.PendingReview);
    }

    [Fact]
    public async Task SubmitDenominationsAsync_ReturnsFailure_WhenNotInDraftStatus()
    {
        // Arrange
        var (session, _) = await CreateSessionWithRegisterAsync();
        var closing = await CreateClosingAsync(session.Id, CashClosingStatus.PendingReview);

        var request = new SubmitDenominationsRequest(
            ClosingId: closing.Id,
            Denominations: new Dictionary<string, int> { { "100", 1 } },
            BankDepositAmount: 0,
            BankName: null,
            BankReference: null,
            DifferenceReason: null);

        // Act
        var result = await _sut.SubmitDenominationsAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Borrador");
    }

    #endregion

    #region ApproveAsync

    [Fact]
    public async Task ApproveAsync_ChangesStatusToApproved_WhenApprovedIsTrue()
    {
        // Arrange
        var (session, _) = await CreateSessionWithRegisterAsync();
        var closing = await CreateClosingAsync(session.Id, CashClosingStatus.PendingReview);

        var request = new ApproveCashClosingRequest(
            ClosingId: closing.Id,
            ApprovedBy: "gerente-001",
            Approved: true,
            RejectionReason: null);

        // Act
        var result = await _sut.ApproveAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Status.Should().Be(CashClosingStatus.Approved);
        result.Data.ApprovedBy.Should().Be("gerente-001");
    }

    [Fact]
    public async Task ApproveAsync_ChangesStatusToDisputed_WhenApprovedIsFalse()
    {
        // Arrange
        var (session, _) = await CreateSessionWithRegisterAsync();
        var closing = await CreateClosingAsync(session.Id, CashClosingStatus.PendingReview);

        var request = new ApproveCashClosingRequest(
            ClosingId: closing.Id,
            ApprovedBy: "gerente-001",
            Approved: false,
            RejectionReason: "Hay diferencia injustificada");

        // Act
        var result = await _sut.ApproveAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Status.Should().Be(CashClosingStatus.Disputed);
        result.Data.RejectionReason.Should().Be("Hay diferencia injustificada");
    }

    [Fact]
    public async Task ApproveAsync_ReturnsFailure_WhenRejectionReasonMissing()
    {
        // Arrange
        var (session, _) = await CreateSessionWithRegisterAsync();
        var closing = await CreateClosingAsync(session.Id, CashClosingStatus.PendingReview);

        var request = new ApproveCashClosingRequest(
            ClosingId: closing.Id,
            ApprovedBy: "gerente-001",
            Approved: false,
            RejectionReason: null);

        // Act
        var result = await _sut.ApproveAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("motivo del rechazo");
    }

    [Fact]
    public async Task ApproveAsync_ReturnsFailure_WhenNotInPendingReviewStatus()
    {
        // Arrange
        var (session, _) = await CreateSessionWithRegisterAsync();
        var closing = await CreateClosingAsync(session.Id, CashClosingStatus.Draft);

        var request = new ApproveCashClosingRequest(
            ClosingId: closing.Id,
            ApprovedBy: "gerente-001",
            Approved: true,
            RejectionReason: null);

        // Act
        var result = await _sut.ApproveAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Pendiente de Revision");
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_ReturnsClosing_WhenFound()
    {
        // Arrange
        var (session, _) = await CreateSessionWithRegisterAsync();
        var closing = await CreateClosingAsync(session.Id, CashClosingStatus.Draft);

        // Act
        var result = await _sut.GetByIdAsync(closing.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.SessionId.Should().Be(session.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsFailure_WhenNotFound()
    {
        // Act
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrado");
    }

    #endregion

    #region GetHistoryAsync

    [Fact]
    public async Task GetHistoryAsync_ReturnsAllClosings_WhenNoFilter()
    {
        // Arrange
        var (session, _) = await CreateSessionWithRegisterAsync();
        await CreateClosingAsync(session.Id, CashClosingStatus.Approved);
        await CreateClosingAsync(session.Id, CashClosingStatus.Disputed);

        // Act
        var result = await _sut.GetHistoryAsync(null, null, null);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    #endregion

    #region Helpers

    private async Task<(CashRegisterSession session, CashRegister register)> CreateSessionWithRegisterAsync(decimal openingBalance = 100m)
    {
        var register = new CashRegister
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            Name = "Caja 1",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Sessions = new List<CashRegisterSession>()
        };

        var session = new CashRegisterSession
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            CashRegisterId = register.Id,
            CashRegister = register,
            OpenedBy = "user-001",
            OpenedAt = DateTime.UtcNow.AddHours(-8),
            OpeningBalance = openingBalance,
            CreatedAt = DateTime.UtcNow,
            Movements = new List<CashRegisterMovement>()
        };

        _context.CashRegisters.Add(register);
        _context.CashRegisterSessions.Add(session);
        await _context.SaveChangesAsync();

        return (session, register);
    }

    private async Task<CashClosing> CreateClosingAsync(Guid sessionId, CashClosingStatus status, decimal totalExpected = 150m)
    {
        var closing = new CashClosing
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            CashRegisterSessionId = sessionId,
            ClosedBy = "user-001",
            ClosedByName = "Juan Cajero",
            Status = status,
            TotalExpected = totalExpected,
            TotalCounted = 0,
            Difference = 0,
            ClosingDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _context.CashClosings.Add(closing);
        await _context.SaveChangesAsync();

        return closing;
    }

    #endregion
}
