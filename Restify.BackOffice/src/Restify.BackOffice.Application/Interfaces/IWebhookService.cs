using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IWebhookService
{
    Task<Result<IEnumerable<WebhookConfigDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<WebhookConfigDto>> CreateAsync(CreateWebhookRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<bool>> ToggleActiveAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<bool>> PingAsync(Guid id, CancellationToken cancellationToken = default);
    Task DispatchEventAsync(string tenantId, string eventType, object payload);
    Task<Result<IEnumerable<WebhookDeliveryDto>>> GetDeliveriesAsync(Guid webhookId, CancellationToken cancellationToken = default);
}
