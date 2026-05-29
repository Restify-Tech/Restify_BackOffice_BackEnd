using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IPromotionService
{
    Task<Result<IEnumerable<PromotionDto>>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<Result<PromotionDto>> CreateAsync(CreatePromotionRequest request, CancellationToken cancellationToken = default);
    Task<Result<PromotionDto>> UpdateAsync(Guid id, CreatePromotionRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> ToggleActiveAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ApplicablePromotionDto>>> EvaluateForOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<CalculatePromotionsResponse>> CalculateAsync(CalculatePromotionsRequest request, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<UpsellSuggestionDto>>> GetUpsellSuggestionsAsync(List<Guid> productIds, CancellationToken cancellationToken = default);
}

public interface IStockAlertService
{
    Task CheckAndNotifyLowStockAsync(Guid inventoryItemId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<LowStockAlertDto>>> GetCurrentAlertsAsync(CancellationToken cancellationToken = default);
}
