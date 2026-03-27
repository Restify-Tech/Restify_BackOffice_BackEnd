namespace Restify.BackOffice.Application.DTOs;

public record WebhookConfigDto(
    Guid Id,
    string Name,
    string Url,
    List<string> Events,
    bool IsActive,
    string? LastPingAt,
    string? LastPingStatus
);

public record CreateWebhookRequest(
    string Name,
    string Url,
    string? Secret,
    List<string> Events,
    int? TimeoutMs
);

public record WebhookDeliveryDto(
    Guid Id,
    string EventType,
    int HttpStatusCode,
    bool Success,
    DateTime DeliveredAt,
    int AttemptNumber
);
