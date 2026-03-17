using MassTransit;
using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Messaging;

/// <summary>
/// Publishes events to RabbitMQ via MassTransit.
/// </summary>
public class RabbitMqPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<RabbitMqPublisher> _logger;

    public RabbitMqPublisher(IPublishEndpoint publishEndpoint, ILogger<RabbitMqPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishAsync<T>(T eventMessage, CancellationToken ct = default) where T : class
    {
        try
        {
            await _publishEndpoint.Publish(eventMessage, ct);
            _logger.LogInformation("Event {EventType} published successfully", typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing event {EventType}", typeof(T).Name);
            // No lanzar excepcion - el evento es fire-and-forget
            // El polling del SyncService es el fallback
        }
    }
}
