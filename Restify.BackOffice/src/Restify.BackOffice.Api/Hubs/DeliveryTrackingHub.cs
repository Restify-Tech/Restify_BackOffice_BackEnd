using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Api.Hubs;

/// <summary>
/// SignalR Hub para GPS tracking en tiempo real de entregas.
/// Los repartidores envían su ubicación, y los clientes/restaurantes la reciben.
/// </summary>
[Authorize]
public class DeliveryTrackingHub : Hub
{
    private readonly IDeliveryService _deliveryService;
    private readonly ILogger<DeliveryTrackingHub> _logger;

    public DeliveryTrackingHub(
        IDeliveryService deliveryService,
        ILogger<DeliveryTrackingHub> logger)
    {
        _deliveryService = deliveryService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation(
            "Client connected to DeliveryTrackingHub. ConnectionId: {ConnectionId}",
            Context.ConnectionId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation(
            "Client disconnected from DeliveryTrackingHub. ConnectionId: {ConnectionId}",
            Context.ConnectionId);

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// El repartidor envía su ubicación GPS actual.
    /// Actualiza la entrega y el repartidor, luego broadcast al grupo de tracking.
    /// </summary>
    public async Task UpdateLocation(Guid deliveryId, double latitude, double longitude)
    {
        try
        {
            // Actualizar la ubicación en BD usando el servicio existente
            var locationRequest = new UpdateDriverLocationRequest((decimal)latitude, (decimal)longitude);
            var result = await _deliveryService.UpdateLocationAsync(deliveryId, locationRequest);

            if (!result.IsSuccess)
            {
                _logger.LogWarning(
                    "Error al actualizar ubicación desde hub: {Error}. DeliveryId: {DeliveryId}",
                    result.Error, deliveryId);
                await Clients.Caller.SendAsync("LocationUpdateError", result.Error);
                return;
            }

            // Broadcast al grupo de tracking de esta entrega
            var broadcast = new DeliveryLocationBroadcast(
                DeliveryId: deliveryId,
                DriverId: result.Data!.DriverId ?? Guid.Empty,
                Latitude: latitude,
                Longitude: longitude,
                UpdatedAt: DateTime.UtcNow
            );

            await Clients.Group($"delivery:{deliveryId}:tracking")
                .SendAsync("LocationUpdated", broadcast);

            _logger.LogDebug(
                "Location broadcast for delivery {DeliveryId}: ({Lat}, {Lon})",
                deliveryId, latitude, longitude);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en UpdateLocation para delivery {DeliveryId}", deliveryId);
            await Clients.Caller.SendAsync("LocationUpdateError", "Error interno al actualizar ubicación");
        }
    }

    /// <summary>
    /// Unirse al grupo de tracking de una entrega específica.
    /// Los clientes y restaurantes usan esto para recibir actualizaciones GPS.
    /// </summary>
    public async Task JoinDeliveryTracking(Guid deliveryId)
    {
        var groupName = $"delivery:{deliveryId}:tracking";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        _logger.LogDebug(
            "Client {ConnectionId} joined tracking group for delivery {DeliveryId}",
            Context.ConnectionId, deliveryId);
    }

    /// <summary>
    /// Salir del grupo de tracking de una entrega.
    /// </summary>
    public async Task LeaveDeliveryTracking(Guid deliveryId)
    {
        var groupName = $"delivery:{deliveryId}:tracking";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        _logger.LogDebug(
            "Client {ConnectionId} left tracking group for delivery {DeliveryId}",
            Context.ConnectionId, deliveryId);
    }

    /// <summary>
    /// Unirse al grupo de entregas del tenant para recibir todas las actualizaciones.
    /// Útil para el dashboard del restaurante.
    /// </summary>
    public async Task JoinTenantDeliveries(Guid tenantId)
    {
        var groupName = $"tenant:{tenantId}:delivery-tracking";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        _logger.LogDebug(
            "Client {ConnectionId} joined tenant delivery tracking for tenant {TenantId}",
            Context.ConnectionId, tenantId);
    }

    /// <summary>
    /// Salir del grupo de entregas del tenant.
    /// </summary>
    public async Task LeaveTenantDeliveries(Guid tenantId)
    {
        var groupName = $"tenant:{tenantId}:delivery-tracking";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        _logger.LogDebug(
            "Client {ConnectionId} left tenant delivery tracking for tenant {TenantId}",
            Context.ConnectionId, tenantId);
    }
}
