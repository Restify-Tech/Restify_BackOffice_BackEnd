using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

/// <summary>
/// Servicio de asignacion inteligente de mesas
/// </summary>
public class TableAssignmentService : ITableAssignmentService
{
    private readonly BackOfficeDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<TableAssignmentService> _logger;

    public TableAssignmentService(
        BackOfficeDbContext dbContext,
        ICurrentUserService currentUserService,
        ILogger<TableAssignmentService> logger)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result<TableSuggestionResponse>> SuggestTableAsync(int guestCount, CancellationToken ct = default)
    {
        if (guestCount <= 0)
            return Result<TableSuggestionResponse>.Failure("La cantidad de personas debe ser mayor a cero");

        // 1. Obtener todas las mesas disponibles y activas
        var availableTables = await _dbContext.Tables
            .Where(t => t.Status == TableStatus.Available && t.IsActive)
            .OrderBy(t => t.Capacity)
            .ToListAsync(ct);

        // 2. Filtrar: capacidad >= guestCount AND minCapacity <= guestCount
        var fittingTables = availableTables
            .Where(t => t.Capacity >= guestCount && t.MinCapacity <= guestCount)
            .ToList();

        // 3. Construir sugerencias con la primera marcada como "Recomendada"
        var isFirst = true;
        var suggestions = fittingTables.Select(t =>
        {
            var dto = new TableSuggestionDto(
                Id: t.Id,
                Number: t.Number,
                Name: t.Name,
                Capacity: t.Capacity,
                MinCapacity: t.MinCapacity,
                Zone: t.Zone,
                IsRecommended: isFirst
            );
            isFirst = false;
            return dto;
        }).ToList();

        string? warningMessage = null;
        var hasWarning = false;

        if (!suggestions.Any())
        {
            // Si no hay mesas que cumplan el criterio exacto, buscar mesas con capacidad suficiente
            // pero minCapacity > guestCount (submesas suboptimas)
            var suboptimalTables = availableTables
                .Where(t => t.Capacity >= guestCount)
                .ToList();

            if (suboptimalTables.Any())
            {
                hasWarning = true;
                warningMessage = $"No hay mesas optimas para {guestCount} personas. Se muestran mesas con capacidad suficiente pero minimo recomendado superior.";

                isFirst = true;
                suggestions = suboptimalTables.Select(t =>
                {
                    var dto = new TableSuggestionDto(
                        Id: t.Id,
                        Number: t.Number,
                        Name: t.Name,
                        Capacity: t.Capacity,
                        MinCapacity: t.MinCapacity,
                        Zone: t.Zone,
                        IsRecommended: isFirst
                    );
                    isFirst = false;
                    return dto;
                }).ToList();
            }
            else
            {
                hasWarning = true;
                warningMessage = $"No hay mesas disponibles con capacidad para {guestCount} personas";
            }
        }

        var response = new TableSuggestionResponse(
            Tables: suggestions,
            GuestCount: guestCount,
            HasWarning: hasWarning,
            WarningMessage: warningMessage
        );

        _logger.LogInformation(
            "Sugerencia de mesa para {GuestCount} personas: {Count} mesas encontradas",
            guestCount, suggestions.Count);

        return Result<TableSuggestionResponse>.Success(response);
    }

    public async Task<Result> AssignTableAsync(Guid tableId, Guid orderId, int guestCount, Guid? assignedByUserId, CancellationToken ct = default)
    {
        // 1. Obtener la mesa
        var table = await _dbContext.Tables.FirstOrDefaultAsync(t => t.Id == tableId, ct);
        if (table == null)
            return Result.Failure("Mesa no encontrada");

        // 2. Obtener la orden
        var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId, ct);
        if (order == null)
            return Result.Failure("Pedido no encontrado");

        // 3. Validar que la mesa este disponible
        if (table.Status != TableStatus.Available)
            return Result.Failure($"La mesa {table.Number} no esta disponible. Estado actual: {table.Status}");

        // 4. Validar que la mesa este activa
        if (!table.IsActive)
            return Result.Failure($"La mesa {table.Number} no esta activa");

        // 5. BLOQUEAR si guestCount > table.Capacity
        if (guestCount > table.Capacity)
            return Result.Failure(
                $"La cantidad de personas ({guestCount}) excede la capacidad de la mesa {table.Number} ({table.Capacity} personas)");

        // 6. WARN si guestCount < table.MinCapacity (pero permitir)
        if (guestCount < table.MinCapacity)
        {
            _logger.LogWarning(
                "Asignacion suboptima: {GuestCount} personas en mesa {TableNumber} (minimo recomendado: {MinCapacity})",
                guestCount, table.Number, table.MinCapacity);
        }

        // 7. Actualizar la mesa
        table.Status = TableStatus.Occupied;
        table.CurrentOrderId = orderId;
        table.OccupiedSince = DateTime.UtcNow;
        table.UpdatedAt = DateTime.UtcNow;

        // 8. Actualizar la orden
        order.TableId = tableId;
        order.GuestCount = guestCount;
        order.TableAssignedAt = DateTime.UtcNow;
        order.TableAssignedByUserId = assignedByUserId ?? _currentUserService.UserId;
        order.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Mesa {TableNumber} asignada al pedido {OrderId} para {GuestCount} personas",
            table.Number, orderId, guestCount);

        return Result.Success();
    }

    public async Task<Result> ReleaseTableAsync(Guid tableId, CancellationToken ct = default)
    {
        var table = await _dbContext.Tables.FirstOrDefaultAsync(t => t.Id == tableId, ct);
        if (table == null)
            return Result.Failure("Mesa no encontrada");

        if (table.Status == TableStatus.Available)
            return Result.Failure($"La mesa {table.Number} ya esta disponible");

        // Limpiar datos de ocupacion
        table.Status = TableStatus.Available;
        table.CurrentOrderId = null;
        table.OccupiedSince = null;
        table.CurrentCustomerName = null;
        table.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Mesa {TableNumber} liberada", table.Number);

        return Result.Success();
    }
}
