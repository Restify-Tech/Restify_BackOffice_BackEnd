using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.DTOs;

public record ShiftTemplateDto(
    Guid Id,
    string Name,
    string StartTime,
    string EndTime,
    string DaysOfWeek,
    Guid? BranchId,
    bool IsActive
);

public record CreateShiftTemplateRequest(
    string Name,
    string StartTime,
    string EndTime,
    List<int> DaysOfWeek,
    Guid? BranchId
);

public record ShiftAssignmentDto(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    Guid? ShiftTemplateId,
    string? ShiftTemplateName,
    DateTime Date,
    string? ScheduledStart,
    string? ScheduledEnd,
    DateTime? ActualClockIn,
    DateTime? ActualClockOut,
    decimal? HoursWorked,
    ClockInMethod ClockInMethod,
    string? ClockInLocation,
    ShiftStatus Status,
    string? Notes
);

public record CreateShiftAssignmentRequest(
    Guid EmployeeId,
    Guid? ShiftTemplateId,
    DateTime Date,
    string? ScheduledStart,
    string? ScheduledEnd
);

public record ClockInRequest(
    Guid AssignmentId,
    string Method,
    string? Location
);

public record ClockOutRequest(
    Guid AssignmentId
);

public record WeeklyScheduleDto(
    DateTime WeekStart,
    DateTime WeekEnd,
    IEnumerable<EmployeeWeekScheduleDto> Employees
);

public record EmployeeWeekScheduleDto(
    Guid EmployeeId,
    string EmployeeName,
    IEnumerable<ShiftAssignmentDto> Days
);

// DTO para "mi turno actual" (usuario autenticado)
public record CurrentShiftDto(
    Guid AssignmentId,
    DateTime Date,
    DateTime? ClockIn,
    DateTime? ClockOut,
    string Status,
    string? ShiftName,
    string? ScheduledStart,
    string? ScheduledEnd
);

// DTO para turno activo de un empleado (vista gerente)
public record ActiveShiftDto(
    Guid AssignmentId,
    Guid EmployeeId,
    string EmployeeName,
    DateTime ClockIn,
    string Status,
    string? ShiftName
);
