using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Sucursal del restaurante
/// </summary>
public class Branch : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Timezone { get; set; } = "America/Guayaquil";
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Horarios de apertura en formato JSON (ej: {"monday": "08:00-22:00", ...})
    /// </summary>
    public string? OpeningHours { get; set; }

    public string? Notes { get; set; }

    // Navegacion
    public ICollection<Table> Tables { get; set; } = new List<Table>();
    public ICollection<CashRegister> CashRegisters { get; set; } = new List<CashRegister>();
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public ICollection<ManagerAssignment> ManagerAssignments { get; set; } = new List<ManagerAssignment>();
}
