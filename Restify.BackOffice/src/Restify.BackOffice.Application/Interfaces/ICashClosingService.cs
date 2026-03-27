using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface ICashClosingService
{
    /// <summary>
    /// Inicia un cierre formal para una sesion de caja
    /// </summary>
    Task<Result<CashClosingDto>> InitiateAsync(InitiateCashClosingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envia las denominaciones contadas y cambia el estado a PendingReview
    /// </summary>
    Task<Result<CashClosingDto>> SubmitDenominationsAsync(SubmitDenominationsRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// El gerente aprueba o rechaza el cierre
    /// </summary>
    Task<Result<CashClosingDto>> ApproveAsync(ApproveCashClosingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el historial de cierres, opcionalmente filtrado por caja y fechas
    /// </summary>
    Task<Result<IEnumerable<CashClosingSummaryDto>>> GetHistoryAsync(
        Guid? cashRegisterId,
        DateTime? from,
        DateTime? to,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un cierre por ID
    /// </summary>
    Task<Result<CashClosingDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Genera el Reporte Z en PDF y retorna la URL del archivo
    /// </summary>
    Task<Result<string>> GenerateZReportAsync(Guid closingId, CancellationToken cancellationToken = default);
}
