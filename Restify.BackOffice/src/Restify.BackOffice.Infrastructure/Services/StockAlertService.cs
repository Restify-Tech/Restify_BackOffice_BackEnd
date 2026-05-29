using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Infrastructure.Services;

/// <summary>
/// Servicio de alertas de stock bajo con notificacion SignalR
/// </summary>
public class StockAlertService : IStockAlertService
{
    private readonly BackOfficeDbContext _context;
    private readonly ILogger<StockAlertService> _logger;

    // IHubContext se obtiene via DI usando el tipo del hub de la Api
    // Se resuelve desde el IServiceProvider para no generar dependencia circular entre capas
    private readonly IServiceProvider _serviceProvider;

    public StockAlertService(
        BackOfficeDbContext context,
        ILogger<StockAlertService> logger,
        IServiceProvider serviceProvider)
    {
        _context = context;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task CheckAndNotifyLowStockAsync(Guid inventoryItemId, CancellationToken cancellationToken = default)
    {
        var item = await _context.InventoryItems
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.Id == inventoryItemId, cancellationToken);

        if (item == null) return;

        if (item.CurrentStock <= item.MinimumStock && item.MinimumStock > 0)
        {
            _logger.LogWarning(
                "Stock bajo detectado: {ItemName} ({ItemId}). Stock actual: {CurrentStock}, Minimo: {MinStock}",
                item.Product?.Name, item.Id, item.CurrentStock, item.MinimumStock);

            try
            {
                // Resolver IHubContext dinamicamente para evitar dependencia directa
                var hubContextType = typeof(Microsoft.AspNetCore.SignalR.IHubContext<>)
                    .MakeGenericType(Type.GetType("Restify.BackOffice.Api.Hubs.OrderHub, Restify.BackOffice.Api")
                        ?? throw new InvalidOperationException("OrderHub not found"));

                var hubContext = _serviceProvider.GetService(hubContextType);
                if (hubContext != null)
                {
                    var clients = hubContext.GetType().GetProperty("Clients")?.GetValue(hubContext);
                    var groupMethod = clients?.GetType().GetMethod("Group");
                    var group = groupMethod?.Invoke(clients, [$"tenant:{item.TenantId}:management"]);
                    var sendMethod = group?.GetType().GetMethod("SendAsync", [typeof(string), typeof(object), typeof(CancellationToken)]);

                    var payload = new
                    {
                        itemId = item.Id,
                        itemName = item.Product?.Name,
                        currentStock = item.CurrentStock,
                        minStockLevel = item.MinimumStock,
                        unit = item.Unit
                    };

                    if (sendMethod != null)
                        await (Task)(sendMethod.Invoke(group, ["stock:low", payload, cancellationToken]) ?? Task.CompletedTask);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al notificar stock bajo para item {ItemId}", inventoryItemId);
            }
        }
    }

    public async Task<Result<IEnumerable<LowStockAlertDto>>> GetCurrentAlertsAsync(CancellationToken cancellationToken = default)
    {
        var lowStockItems = await _context.InventoryItems
            .Include(i => i.Product)
            .Where(i => i.TrackStock && i.MinimumStock > 0 && i.CurrentStock <= i.MinimumStock)
            .ToListAsync(cancellationToken);

        var alerts = lowStockItems.Select(i => new LowStockAlertDto(
            i.Id,
            i.Product?.Name ?? "Producto",
            i.CurrentStock,
            i.MinimumStock,
            i.Unit
        ));

        return Result<IEnumerable<LowStockAlertDto>>.Success(alerts);
    }
}
