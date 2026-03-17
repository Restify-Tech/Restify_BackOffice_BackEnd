using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.DTOs;

public class TableDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string? Name { get; set; }
    public int Capacity { get; set; }
    public TableStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int? PositionX { get; set; }
    public int? PositionY { get; set; }
    public TableShape? Shape { get; set; }
    public string? ShapeName { get; set; }
    public string? Zone { get; set; }
    public string? Notes { get; set; }
    public Guid? CurrentOrderId { get; set; }
    public string? CurrentCustomerName { get; set; }
    public DateTime? OccupiedSince { get; set; }
    public TimeSpan? OccupiedDuration { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateTableRequest
{
    public string Number { get; set; } = string.Empty;
    public string? Name { get; set; }
    public int Capacity { get; set; }
    public bool IsActive { get; set; } = true;
    public int? PositionX { get; set; }
    public int? PositionY { get; set; }
    public TableShape? Shape { get; set; }
    public string? Zone { get; set; }
    public string? Notes { get; set; }
}

public class UpdateTableRequest
{
    public string Number { get; set; } = string.Empty;
    public string? Name { get; set; }
    public int Capacity { get; set; }
    public bool IsActive { get; set; }
    public int? PositionX { get; set; }
    public int? PositionY { get; set; }
    public TableShape? Shape { get; set; }
    public string? Zone { get; set; }
    public string? Notes { get; set; }
}

public class UpdateTableStatusRequest
{
    public TableStatus Status { get; set; }
    public string? CurrentCustomerName { get; set; }
    public Guid? CurrentOrderId { get; set; }
}

public class TableLayoutDto
{
    public List<TableDto> Tables { get; set; } = new();
    public List<string> Zones { get; set; } = new();
    public int AvailableCount { get; set; }
    public int OccupiedCount { get; set; }
    public int ReservedCount { get; set; }
    public int TotalCapacity { get; set; }
}
