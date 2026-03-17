using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

/// <summary>
/// Servicio de asignacion inteligente de mesas
/// </summary>
public interface ITableAssignmentService
{
    /// <summary>
    /// Sugiere mesas disponibles para un grupo de personas, ordenadas por mejor ajuste
    /// </summary>
    Task<Result<TableSuggestionResponse>> SuggestTableAsync(int guestCount, CancellationToken ct = default);

    /// <summary>
    /// Asigna una mesa a un pedido existente
    /// </summary>
    Task<Result> AssignTableAsync(Guid tableId, Guid orderId, int guestCount, Guid? assignedByUserId, CancellationToken ct = default);

    /// <summary>
    /// Libera una mesa ocupada (la pone en Available y limpia datos de ocupacion)
    /// </summary>
    Task<Result> ReleaseTableAsync(Guid tableId, CancellationToken ct = default);
}
