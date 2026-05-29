using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface ISplitPaymentService
{
    /// <summary>
    /// Inicia un pago dividido para un pedido
    /// </summary>
    Task<Result<SplitPaymentDto>> InitiateAsync(CreateSplitPaymentRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Procesa (paga) un item especifico del pago dividido por su indice
    /// </summary>
    Task<Result<SplitPaymentDto>> ProcessItemAsync(ProcessSplitItemRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el pago dividido mas reciente de un pedido
    /// </summary>
    Task<Result<SplitPaymentDto>> GetByOrderAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancela un pago dividido activo
    /// </summary>
    Task<Result<bool>> CancelAsync(Guid id, CancellationToken cancellationToken = default);
}
