using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Enums;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IPurchaseOrderService
{
    Task<Result<List<PurchaseOrderSummaryDto>>> GetAllAsync(PurchaseOrderStatus? status = null, CancellationToken cancellationToken = default);
    Task<Result<PurchaseOrderDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PurchaseOrderDto>> CreateAsync(CreatePurchaseOrderRequest request, CancellationToken cancellationToken = default);
    Task<Result<PurchaseOrderDto>> UpdateAsync(Guid id, UpdatePurchaseOrderRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PurchaseOrderDto>> SendAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PurchaseOrderDto>> ReceiveAsync(Guid id, ReceivePurchaseOrderRequest request, CancellationToken cancellationToken = default);
    Task<Result<PurchaseOrderDto>> CancelAsync(Guid id, string reason, CancellationToken cancellationToken = default);
}
