using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Restify.BackOffice.Domain.Constants;
using System.Security.Claims;

namespace Restify.BackOffice.Api.Hubs;

/// <summary>
/// SignalR Hub for real-time order events.
/// Clients join tenant-scoped groups for kitchen, dispatch, and order updates.
/// </summary>
[Authorize]
public class OrderHub : Hub
{
    private readonly ILogger<OrderHub> _logger;

    public OrderHub(ILogger<OrderHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var tenantId = GetTenantId();
        var userType = Context.User?.FindFirst(ClaimConstants.UserType)?.Value ?? "staff";

        if (tenantId != null)
        {
            // All authenticated users join the general orders group for their tenant
            await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant:{tenantId}:orders");

            _logger.LogInformation(
                "Client connected to OrderHub. ConnectionId: {ConnectionId}, TenantId: {TenantId}, UserType: {UserType}",
                Context.ConnectionId, tenantId, userType);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var tenantId = GetTenantId();

        if (tenantId != null)
        {
            _logger.LogInformation(
                "Client disconnected from OrderHub. ConnectionId: {ConnectionId}, TenantId: {TenantId}",
                Context.ConnectionId, tenantId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Join the kitchen group to receive kitchen-specific events
    /// </summary>
    public async Task JoinKitchen()
    {
        var tenantId = GetTenantId();
        if (tenantId == null) return;

        await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant:{tenantId}:kitchen");
        _logger.LogDebug("Client {ConnectionId} joined kitchen group for tenant {TenantId}", Context.ConnectionId, tenantId);
    }

    /// <summary>
    /// Leave the kitchen group
    /// </summary>
    public async Task LeaveKitchen()
    {
        var tenantId = GetTenantId();
        if (tenantId == null) return;

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"tenant:{tenantId}:kitchen");
    }

    /// <summary>
    /// Join the dispatch group to receive dispatch-specific events
    /// </summary>
    public async Task JoinDispatch()
    {
        var tenantId = GetTenantId();
        if (tenantId == null) return;

        await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant:{tenantId}:dispatch");
        _logger.LogDebug("Client {ConnectionId} joined dispatch group for tenant {TenantId}", Context.ConnectionId, tenantId);
    }

    /// <summary>
    /// Leave the dispatch group
    /// </summary>
    public async Task LeaveDispatch()
    {
        var tenantId = GetTenantId();
        if (tenantId == null) return;

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"tenant:{tenantId}:dispatch");
    }

    /// <summary>
    /// Join a customer-specific group for order tracking
    /// </summary>
    public async Task JoinCustomerTracking(string customerId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"customer:{customerId}");
        _logger.LogDebug("Client {ConnectionId} joined customer tracking for {CustomerId}", Context.ConnectionId, customerId);
    }

    /// <summary>
    /// Leave customer tracking group
    /// </summary>
    public async Task LeaveCustomerTracking(string customerId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"customer:{customerId}");
    }

    /// <summary>
    /// Join the delivery group to receive delivery-specific events
    /// </summary>
    public async Task JoinDelivery()
    {
        var tenantId = GetTenantId();
        if (tenantId == null) return;

        await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant:{tenantId}:delivery");
        _logger.LogDebug("Client {ConnectionId} joined delivery group for tenant {TenantId}", Context.ConnectionId, tenantId);
    }

    /// <summary>
    /// Leave the delivery group
    /// </summary>
    public async Task LeaveDelivery()
    {
        var tenantId = GetTenantId();
        if (tenantId == null) return;

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"tenant:{tenantId}:delivery");
    }

    /// <summary>
    /// Join a driver-specific group for delivery assignments
    /// </summary>
    public async Task JoinDriverTracking(string driverId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"driver:{driverId}");
        _logger.LogDebug("Client {ConnectionId} joined driver tracking for {DriverId}", Context.ConnectionId, driverId);
    }

    /// <summary>
    /// Leave driver tracking group
    /// </summary>
    public async Task LeaveDriverTracking(string driverId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"driver:{driverId}");
    }

    private string? GetTenantId()
    {
        return Context.User?.FindFirst(ClaimConstants.TenantId)?.Value;
    }
}
