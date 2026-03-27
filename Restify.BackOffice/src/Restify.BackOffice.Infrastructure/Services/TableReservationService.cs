using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class TableReservationService : ITableReservationService
{
    private readonly BackOfficeDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationClient _notificationClient;
    private readonly ILogger<TableReservationService> _logger;

    public TableReservationService(
        BackOfficeDbContext context,
        ICurrentUserService currentUserService,
        INotificationClient notificationClient,
        ILogger<TableReservationService> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _notificationClient = notificationClient;
        _logger = logger;
    }

    public async Task<Result<List<TableReservationDto>>> GetAllAsync(
        DateTime? date = null,
        ReservationStatus? status = null,
        Guid? branchId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.TableReservations
            .Include(r => r.Table)
            .AsQueryable();

        if (date.HasValue)
        {
            var day = date.Value.Date;
            query = query.Where(r => r.ReservationDateTime.Date == day);
        }

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        if (branchId.HasValue)
            query = query.Where(r => r.BranchId == branchId.Value);

        var reservations = await query
            .OrderBy(r => r.ReservationDateTime)
            .ToListAsync(cancellationToken);

        return Result<List<TableReservationDto>>.Success(reservations.Select(MapToDto).ToList());
    }

    public async Task<Result<TableReservationDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var reservation = await _context.TableReservations
            .Include(r => r.Table)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (reservation == null)
            return Result<TableReservationDto>.Failure("Reserva no encontrada");

        return Result<TableReservationDto>.Success(MapToDto(reservation));
    }

    public async Task<Result<TableReservationDto>> CreateAsync(CreateReservationRequest request, CancellationToken cancellationToken = default)
    {
        var code = GenerateConfirmationCode();

        var reservation = new TableReservation
        {
            TableId = request.TableId,
            BranchId = request.BranchId,
            CustomerName = request.CustomerName,
            CustomerPhone = request.CustomerPhone,
            CustomerEmail = request.CustomerEmail,
            ReservationDateTime = request.ReservationDateTime,
            PartySize = request.PartySize,
            SpecialRequests = request.SpecialRequests,
            Status = ReservationStatus.Pending,
            ConfirmationCode = code
        };

        _context.TableReservations.Add(reservation);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(reservation.Id, cancellationToken);
    }

    public async Task<Result<TableReservationDto>> ConfirmAsync(Guid id, ConfirmReservationRequest request, CancellationToken cancellationToken = default)
    {
        var reservation = await _context.TableReservations.FindAsync([id], cancellationToken);
        if (reservation == null)
            return Result<TableReservationDto>.Failure("Reserva no encontrada");

        if (reservation.Status != ReservationStatus.Pending)
            return Result<TableReservationDto>.Failure("Solo se pueden confirmar reservas en estado Pendiente");

        reservation.Status = ReservationStatus.Confirmed;
        reservation.ConfirmedAt = DateTime.UtcNow;
        if (request.TableId.HasValue)
            reservation.TableId = request.TableId;

        await _context.SaveChangesAsync(cancellationToken);

        // SMS de confirmacion (fire-and-forget, no bloquea el flujo)
        if (!string.IsNullOrWhiteSpace(reservation.CustomerPhone))
        {
            var phone = reservation.CustomerPhone;
            var partySize = reservation.PartySize;
            var dateTime = reservation.ReservationDateTime;
            var code = reservation.ConfirmationCode;
            var notificationClient = _notificationClient;
            var logger = _logger;
            _ = Task.Run(async () =>
            {
                try
                {
                    await notificationClient.SendSmsAsync(
                        phone,
                        $"Reserva confirmada para {partySize} personas el {dateTime:dd/MM/yyyy HH:mm}. Codigo: {code}",
                        CancellationToken.None);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Error enviando SMS de confirmacion para reserva {Id}", id);
                }
            });
        }

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<Result<TableReservationDto>> MarkAsSeatedAsync(Guid id, SeatReservationRequest request, CancellationToken cancellationToken = default)
    {
        var reservation = await _context.TableReservations.FindAsync([id], cancellationToken);
        if (reservation == null)
            return Result<TableReservationDto>.Failure("Reserva no encontrada");

        if (reservation.Status is not (ReservationStatus.Pending or ReservationStatus.Confirmed))
            return Result<TableReservationDto>.Failure("La reserva no puede ser marcada como sentada en su estado actual");

        reservation.Status = ReservationStatus.Seated;
        if (request.TableId.HasValue)
            reservation.TableId = request.TableId;

        await _context.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<Result<TableReservationDto>> CancelAsync(Guid id, CancelReservationRequest request, CancellationToken cancellationToken = default)
    {
        var reservation = await _context.TableReservations.FindAsync([id], cancellationToken);
        if (reservation == null)
            return Result<TableReservationDto>.Failure("Reserva no encontrada");

        if (reservation.Status is ReservationStatus.Completed or ReservationStatus.Cancelled)
            return Result<TableReservationDto>.Failure("La reserva ya esta completada o cancelada");

        reservation.Status = ReservationStatus.Cancelled;
        reservation.CancelledBy = _currentUserService.Email;
        reservation.CancellationReason = request.Reason;

        await _context.SaveChangesAsync(cancellationToken);

        // SMS de cancelacion (fire-and-forget, no bloquea el flujo)
        if (!string.IsNullOrWhiteSpace(reservation.CustomerPhone))
        {
            var phone = reservation.CustomerPhone;
            var dateTime = reservation.ReservationDateTime;
            var code = reservation.ConfirmationCode;
            var reason = request.Reason;
            var notificationClient = _notificationClient;
            var logger = _logger;
            _ = Task.Run(async () =>
            {
                try
                {
                    var reasonText = !string.IsNullOrWhiteSpace(reason) ? $" Motivo: {reason}." : string.Empty;
                    await notificationClient.SendSmsAsync(
                        phone,
                        $"Su reserva del {dateTime:dd/MM/yyyy HH:mm} (Codigo: {code}) ha sido cancelada.{reasonText}",
                        CancellationToken.None);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Error enviando SMS de cancelacion para reserva {Id}", id);
                }
            });
        }

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<Result<List<TableReservationDto>>> GetTodayAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllAsync(DateTime.UtcNow.Date, null, null, cancellationToken);
    }

    private static string GenerateConfirmationCode()
    {
        return Random.Shared.Next(100000, 999999).ToString();
    }

    private static TableReservationDto MapToDto(TableReservation r)
    {
        return new TableReservationDto(
            r.Id,
            r.TableId,
            r.Table?.Name,
            r.BranchId,
            r.CustomerName,
            r.CustomerPhone,
            r.CustomerEmail,
            r.ReservationDateTime,
            r.PartySize,
            r.SpecialRequests,
            r.Status,
            r.Status.ToString(),
            r.ConfirmationCode,
            r.ConfirmedAt,
            r.CancelledBy,
            r.CancellationReason,
            r.CreatedAt
        );
    }
}
