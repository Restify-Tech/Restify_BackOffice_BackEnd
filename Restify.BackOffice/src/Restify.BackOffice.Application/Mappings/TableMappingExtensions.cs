using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Mappings;

public static class TableMappingExtensions
{
    public static TableDto ToDto(this Table table)
    {
        var dto = new TableDto
        {
            Id = table.Id,
            Number = table.Number,
            Name = table.Name,
            Capacity = table.Capacity,
            Status = table.Status,
            StatusName = table.Status.ToString(),
            IsActive = table.IsActive,
            PositionX = table.PositionX,
            PositionY = table.PositionY,
            Shape = table.Shape,
            ShapeName = table.Shape?.ToString(),
            Zone = table.Zone,
            Notes = table.Notes,
            CurrentOrderId = table.CurrentOrderId,
            CurrentCustomerName = table.CurrentCustomerName,
            OccupiedSince = table.OccupiedSince,
            CreatedAt = table.CreatedAt,
            UpdatedAt = table.UpdatedAt
        };

        if (table.OccupiedSince.HasValue)
        {
            dto.OccupiedDuration = DateTime.UtcNow - table.OccupiedSince.Value;
        }

        return dto;
    }

    public static Table ToEntity(this CreateTableRequest request, Guid tenantId)
    {
        return new Table
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Number = request.Number,
            Name = request.Name,
            Capacity = request.Capacity,
            Status = TableStatus.Available,
            IsActive = request.IsActive,
            PositionX = request.PositionX,
            PositionY = request.PositionY,
            Shape = request.Shape,
            Zone = request.Zone,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateFromRequest(this Table table, UpdateTableRequest request)
    {
        table.Number = request.Number;
        table.Name = request.Name;
        table.Capacity = request.Capacity;
        table.IsActive = request.IsActive;
        table.PositionX = request.PositionX;
        table.PositionY = request.PositionY;
        table.Shape = request.Shape;
        table.Zone = request.Zone;
        table.Notes = request.Notes;
        table.UpdatedAt = DateTime.UtcNow;
    }

    public static void UpdateStatus(this Table table, UpdateTableStatusRequest request)
    {
        var previousStatus = table.Status;
        table.Status = request.Status;
        table.UpdatedAt = DateTime.UtcNow;

        // Si cambia a ocupada, registrar cuando y por quien
        if (request.Status == TableStatus.Occupied && previousStatus != TableStatus.Occupied)
        {
            table.OccupiedSince = DateTime.UtcNow;
            table.CurrentCustomerName = request.CurrentCustomerName;
            table.CurrentOrderId = request.CurrentOrderId;
        }
        // Si deja de estar ocupada, limpiar info
        else if (request.Status != TableStatus.Occupied && previousStatus == TableStatus.Occupied)
        {
            table.OccupiedSince = null;
            table.CurrentCustomerName = null;
            table.CurrentOrderId = null;
        }
    }
}
