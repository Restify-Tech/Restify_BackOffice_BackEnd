using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Notifications;

/// <summary>
/// Implementacion Null Object para INotificationClient.
/// Se usa cuando el Notification Service no esta configurado.
/// No hace nada, no lanza excepciones.
/// </summary>
public class NullNotificationClient : INotificationClient
{
    public Task SendSmsAsync(string to, string message, CancellationToken ct = default)
        => Task.CompletedTask;
}
