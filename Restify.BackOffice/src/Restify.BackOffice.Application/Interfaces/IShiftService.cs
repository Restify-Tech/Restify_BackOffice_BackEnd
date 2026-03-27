using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IShiftService
{
    /// <summary>
    /// Obtiene todas las plantillas de turno del tenant
    /// </summary>
    Task<Result<IEnumerable<ShiftTemplateDto>>> GetTemplatesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Crea una nueva plantilla de turno
    /// </summary>
    Task<Result<ShiftTemplateDto>> CreateTemplateAsync(CreateShiftTemplateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crea una asignacion de turno para un empleado
    /// </summary>
    Task<Result<ShiftAssignmentDto>> CreateAssignmentAsync(CreateShiftAssignmentRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra la entrada del empleado (clock-in)
    /// </summary>
    Task<Result<ShiftAssignmentDto>> ClockInAsync(ClockInRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra la salida del empleado (clock-out) y calcula horas trabajadas
    /// </summary>
    Task<Result<ShiftAssignmentDto>> ClockOutAsync(ClockOutRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el horario semanal agrupado por empleado
    /// </summary>
    Task<Result<WeeklyScheduleDto>> GetWeeklyScheduleAsync(DateTime weekStart, Guid? branchId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene los turnos de un empleado en un rango de fechas
    /// </summary>
    Task<Result<IEnumerable<ShiftAssignmentDto>>> GetEmployeeShiftsAsync(Guid employeeId, DateTime? from, DateTime? to, CancellationToken cancellationToken = default);
}
