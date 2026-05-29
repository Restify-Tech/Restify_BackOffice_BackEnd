using Microsoft.AspNetCore.SignalR;
using Restify.BackOffice.Api.Hubs;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Api.Services;

public class NotificationsService : INotificationsService
{
    private readonly IHubContext<NotificationsHub> _hub;
    private readonly ILogger<NotificationsService> _logger;

    public NotificationsService(
        IHubContext<NotificationsHub> hub,
        ILogger<NotificationsService> logger)
    {
        _hub = hub;
        _logger = logger;
    }

    public async Task SendAsync(Guid tenantId, string title, string message, string severity,
        string? href = null, CancellationToken cancellationToken = default)
    {
        var group = $"tenant:{tenantId}:notifications";
        var payload = new { title, message, severity, href };

        await _hub.Clients.Group(group).SendAsync("NotificationReceived", payload, cancellationToken);

        _logger.LogDebug(
            "Notification sent to tenant {TenantId}: [{Severity}] {Title}",
            tenantId, severity, title);
    }
}
