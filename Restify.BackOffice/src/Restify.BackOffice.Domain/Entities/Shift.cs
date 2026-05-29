using Restify.BackOffice.Domain.Enums;
using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Plantilla de turno reutilizable (ej: "Turno Manana 8-16h")
/// </summary>
public class ShiftTemplate : TenantEntity
{
    public string Name { get; set; } = string.Empty;    // "Turno Manana 8-16h"
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string DaysOfWeek { get; set; } = string.Empty; // JSON: [1,2,3,4,5] (1=Lunes)
    public Guid? BranchId { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<ShiftAssignment> Assignments { get; set; } = new List<ShiftAssignment>();
}

/// <summary>
/// Asignacion de turno a un empleado en una fecha especifica
/// </summary>
public class ShiftAssignment : TenantEntity
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public Guid? ShiftTemplateId { get; set; }
    public ShiftTemplate? ShiftTemplate { get; set; }

    public DateTime Date { get; set; }                  // fecha de la jornada
    public TimeSpan? ScheduledStart { get; set; }
    public TimeSpan? ScheduledEnd { get; set; }
    public DateTime? ActualClockIn { get; set; }
    public DateTime? ActualClockOut { get; set; }

    public ClockInMethod ClockInMethod { get; set; } = ClockInMethod.Manual;
    public string? ClockInLocation { get; set; }        // lat,lng si es GPS

    public ShiftStatus Status { get; set; } = ShiftStatus.Scheduled;
    public string? Notes { get; set; }
    public string? ApprovedBy { get; set; }

    // Horas trabajadas calculadas
    public decimal? HoursWorked { get; set; }           // calculado al hacer clock-out
}

public enum ClockInMethod
{
    Manual = 1,
    PIN = 2,
    QR = 3,
    GPS = 4
}

public enum ShiftStatus
{
    Scheduled = 1,
    Present = 2,
    Late = 3,
    Absent = 4,
    Override = 5,
    Completed = 6
}
