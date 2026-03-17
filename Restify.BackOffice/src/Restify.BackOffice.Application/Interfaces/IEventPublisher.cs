namespace Restify.BackOffice.Application.Interfaces;

/// <summary>
/// Abstraction for publishing events to message broker (RabbitMQ via MassTransit).
/// </summary>
public interface IEventPublisher
{
    Task PublishAsync<T>(T eventMessage, CancellationToken ct = default) where T : class;
}
