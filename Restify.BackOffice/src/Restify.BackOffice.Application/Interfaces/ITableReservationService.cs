using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface ITableReservationService
{
    Task<Result<List<TableReservationDto>>> GetAllAsync(DateTime? date = null, ReservationStatus? status = null, Guid? branchId = null, CancellationToken cancellationToken = default);
    Task<Result<TableReservationDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<TableReservationDto>> CreateAsync(CreateReservationRequest request, CancellationToken cancellationToken = default);
    Task<Result<TableReservationDto>> ConfirmAsync(Guid id, ConfirmReservationRequest request, CancellationToken cancellationToken = default);
    Task<Result<TableReservationDto>> MarkAsSeatedAsync(Guid id, SeatReservationRequest request, CancellationToken cancellationToken = default);
    Task<Result<TableReservationDto>> CancelAsync(Guid id, CancelReservationRequest request, CancellationToken cancellationToken = default);
    Task<Result<List<TableReservationDto>>> GetTodayAsync(CancellationToken cancellationToken = default);
}
