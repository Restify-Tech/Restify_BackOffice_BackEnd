using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Restify.BackOffice.Domain.Constants;

namespace Restify.BackOffice.Api.Hubs;

[Authorize]
public class NotificationsHub : Hub
{
    private readonly ILogger<NotificationsHub> _logger;

    public NotificationsHub(ILogger<NotificationsHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var tenantId = GetTenantId();
        if (tenantId != null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant:{tenantId}:notifications");
            _logger.LogInformation(
                "Client connected to NotificationsHub. ConnectionId: {ConnectionId}, TenantId: {TenantId}",
                Context.ConnectionId, tenantId);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var tenantId = GetTenantId();
        if (tenantId != null)
        {
            _logger.LogInformation(
                "Client disconnected from NotificationsHub. ConnectionId: {ConnectionId}, TenantId: {TenantId}",
                Context.ConnectionId, tenantId);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinNotifications()
    {
        var tenantId = GetTenantId();
        if (tenantId == null) return;

        await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant:{tenantId}:notifications");
        _logger.LogDebug("Client {ConnectionId} joined notifications group for tenant {TenantId}", Context.ConnectionId, tenantId);
    }

    public async Task LeaveNotifications()
    {
        var tenantId = GetTenantId();
        if (tenantId == null) return;

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"tenant:{tenantId}:notifications");
    }

    private string? GetTenantId() =>
        Context.User?.FindFirst(ClaimConstants.TenantId)?.Value;
}
