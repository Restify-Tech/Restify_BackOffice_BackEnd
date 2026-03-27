using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Mesa del restaurante
/// </summary>
public class Table : TenantEntity
{
    public string Number { get; set; } = string.Empty;
    public string? Name { get; set; }
    public int Capacity { get; set; }

    /// <summary>
    /// Mínimo recomendado de personas (ej: mesa de 4 → mínimo 2)
    /// </summary>
    public int MinCapacity { get; set; } = 1;

    public TableStatus Status { get; set; } = TableStatus.Available;
    public bool IsActive { get; set; } = true;
    
    // Layout (opcional para representación visual)
    public int? PositionX { get; set; }
    public int? PositionY { get; set; }
    public TableShape? Shape { get; set; }
    
    // Zona/Área del restaurante (ej: Terraza, Interior, VIP)
    public string? Zone { get; set; }
    
    // Notas adicionales
    public string? Notes { get; set; }
    
    // Información del pedido actual (si está ocupada)
    public Guid? CurrentOrderId { get; set; }
    public string? CurrentCustomerName { get; set; }
    public DateTime? OccupiedSince { get; set; }

    // Sucursal
    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }
}

/// <summary>
/// Estados de una mesa
/// </summary>
public enum TableStatus
{
    Available = 1,      // Libre/Disponible
    Occupied = 2,       // Ocupada
    Reserved = 3,       // Reservada
    Cleaning = 4,       // En limpieza
    OutOfService = 5    // Fuera de servicio
}

/// <summary>
/// Formas de mesa (para representación visual)
/// </summary>
public enum TableShape
{
    Square = 1,     // Cuadrada
    Round = 2,      // Redonda
    Rectangle = 3,  // Rectangular
    Oval = 4        // Oval
}
