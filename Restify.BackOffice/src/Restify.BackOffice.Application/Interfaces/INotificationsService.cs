namespace Restify.BackOffice.Application.Interfaces;

public interface INotificationsService
{
    Task SendAsync(Guid tenantId, string title, string message, string severity,
        string? href = null, CancellationToken cancellationToken = default);
}
