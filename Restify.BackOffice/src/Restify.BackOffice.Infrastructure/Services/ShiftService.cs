using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;
using System.Text.Json;

namespace Restify.BackOffice.Infrastructure.Services;

public class ShiftService : IShiftService
{
    private readonly BackOfficeDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ShiftService(BackOfficeDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<IEnumerable<ShiftTemplateDto>>> GetTemplatesAsync(CancellationToken cancellationToken = default)
    {
        var templates = await _context.ShiftTemplates
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

        var dtos = templates.Select(ToTemplateDto);
        return Result<IEnumerable<ShiftTemplateDto>>.Success(dtos);
    }

    public async Task<Result<ShiftTemplateDto>> CreateTemplateAsync(CreateShiftTemplateRequest request, CancellationToken cancellationToken = default)
    {
        if (!TimeSpan.TryParse(request.StartTime, out var start))
            return Result<ShiftTemplateDto>.Failure("Hora de inicio invalida (use formato HH:mm)");

        if (!TimeSpan.TryParse(request.EndTime, out var end))
            return Result<ShiftTemplateDto>.Failure("Hora de fin invalida (use formato HH:mm)");

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;

        var template = new ShiftTemplate
        {
            TenantId = tenantId,
            Name = request.Name,
            StartTime = start,
            EndTime = end,
            DaysOfWeek = JsonSerializer.Serialize(request.DaysOfWeek),
            BranchId = request.BranchId,
            IsActive = true
        };

        _context.ShiftTemplates.Add(template);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<ShiftTemplateDto>.Success(ToTemplateDto(template));
    }

    public async Task<Result<ShiftAssignmentDto>> CreateAssignmentAsync(CreateShiftAssignmentRequest request, CancellationToken cancellationToken = default)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

        if (employee == null)
            return Result<ShiftAssignmentDto>.Failure("Empleado no encontrado");

        TimeSpan? scheduledStart = null;
        TimeSpan? scheduledEnd = null;

        // Si viene plantilla, cargar horarios de ella
        ShiftTemplate? template = null;
        if (request.ShiftTemplateId.HasValue)
        {
            template = await _context.ShiftTemplates
                .FirstOrDefaultAsync(t => t.Id == request.ShiftTemplateId.Value, cancellationToken);
            if (template != null)
            {
                scheduledStart = template.StartTime;
                scheduledEnd = template.EndTime;
            }
        }

        // Sobrescribir si se pasan explicitamente
        if (!string.IsNullOrEmpty(request.ScheduledStart) && TimeSpan.TryParse(request.ScheduledStart, out var ts))
            scheduledStart = ts;
        if (!string.IsNullOrEmpty(request.ScheduledEnd) && TimeSpan.TryParse(request.ScheduledEnd, out var te))
            scheduledEnd = te;

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;

        var assignment = new ShiftAssignment
        {
            TenantId = tenantId,
            EmployeeId = request.EmployeeId,
            ShiftTemplateId = request.ShiftTemplateId,
            Date = request.Date.Date,
            ScheduledStart = scheduledStart,
            ScheduledEnd = scheduledEnd,
            Status = ShiftStatus.Scheduled
        };

        _context.ShiftAssignments.Add(assignment);
        await _context.SaveChangesAsync(cancellationToken);

        var fullName = $"{employee.FirstName} {employee.LastName}";
        return Result<ShiftAssignmentDto>.Success(ToAssignmentDto(assignment, fullName, template?.Name));
    }

    public async Task<Result<ShiftAssignmentDto>> ClockInAsync(ClockInRequest request, CancellationToken cancellationToken = default)
    {
        var assignment = await _context.ShiftAssignments
            .Include(a => a.Employee)
            .Include(a => a.ShiftTemplate)
            .FirstOrDefaultAsync(a => a.Id == request.AssignmentId, cancellationToken);

        if (assignment == null)
            return Result<ShiftAssignmentDto>.Failure("Asignacion de turno no encontrada");

        if (assignment.ActualClockIn.HasValue)
            return Result<ShiftAssignmentDto>.Failure("El empleado ya registro su entrada");

        var now = DateTime.UtcNow;
        assignment.ActualClockIn = now;
        assignment.ClockInLocation = request.Location;

        if (Enum.TryParse<ClockInMethod>(request.Method, true, out var method))
            assignment.ClockInMethod = method;

        // Determinar si llega tarde (> 15 minutos del inicio programado)
        if (assignment.ScheduledStart.HasValue)
        {
            var scheduledDateTime = assignment.Date.Add(assignment.ScheduledStart.Value);
            var lateThreshold = scheduledDateTime.AddMinutes(15);
            assignment.Status = now > lateThreshold ? ShiftStatus.Late : ShiftStatus.Present;
        }
        else
        {
            assignment.Status = ShiftStatus.Present;
        }

        await _context.SaveChangesAsync(cancellationToken);

        var fullName = $"{assignment.Employee.FirstName} {assignment.Employee.LastName}";
        return Result<ShiftAssignmentDto>.Success(ToAssignmentDto(assignment, fullName, assignment.ShiftTemplate?.Name));
    }

    public async Task<Result<ShiftAssignmentDto>> ClockOutAsync(ClockOutRequest request, CancellationToken cancellationToken = default)
    {
        var assignment = await _context.ShiftAssignments
            .Include(a => a.Employee)
            .Include(a => a.ShiftTemplate)
            .FirstOrDefaultAsync(a => a.Id == request.AssignmentId, cancellationToken);

        if (assignment == null)
            return Result<ShiftAssignmentDto>.Failure("Asignacion de turno no encontrada");

        if (!assignment.ActualClockIn.HasValue)
            return Result<ShiftAssignmentDto>.Failure("El empleado no ha registrado su entrada");

        if (assignment.ActualClockOut.HasValue)
            return Result<ShiftAssignmentDto>.Failure("El empleado ya registro su salida");

        var now = DateTime.UtcNow;
        assignment.ActualClockOut = now;
        assignment.HoursWorked = (decimal)(now - assignment.ActualClockIn.Value).TotalHours;
        assignment.Status = ShiftStatus.Completed;

        await _context.SaveChangesAsync(cancellationToken);

        var fullName = $"{assignment.Employee.FirstName} {assignment.Employee.LastName}";
        return Result<ShiftAssignmentDto>.Success(ToAssignmentDto(assignment, fullName, assignment.ShiftTemplate?.Name));
    }

    public async Task<Result<WeeklyScheduleDto>> GetWeeklyScheduleAsync(DateTime weekStart, Guid? branchId, CancellationToken cancellationToken = default)
    {
        // Normalizar al inicio de la semana (lunes)
        var start = weekStart.Date;
        var end = start.AddDays(7);

        var query = _context.ShiftAssignments
            .Include(a => a.Employee)
            .Include(a => a.ShiftTemplate)
            .Where(a => a.Date >= start && a.Date < end)
            .AsQueryable();

        if (branchId.HasValue)
            query = query.Where(a => a.Employee.BranchId == branchId.Value);

        var assignments = await query
            .OrderBy(a => a.Date)
            .ThenBy(a => a.ScheduledStart)
            .ToListAsync(cancellationToken);

        var groupedByEmployee = assignments
            .GroupBy(a => a.EmployeeId)
            .Select(g =>
            {
                var firstAssignment = g.First();
                var empName = $"{firstAssignment.Employee.FirstName} {firstAssignment.Employee.LastName}";
                var days = g.Select(a => ToAssignmentDto(a, empName, a.ShiftTemplate?.Name));
                return new EmployeeWeekScheduleDto(g.Key, empName, days);
            });

        var weekly = new WeeklyScheduleDto(start, end.AddDays(-1), groupedByEmployee);
        return Result<WeeklyScheduleDto>.Success(weekly);
    }

    public async Task<Result<IEnumerable<ShiftAssignmentDto>>> GetEmployeeShiftsAsync(Guid employeeId, DateTime? from, DateTime? to, CancellationToken cancellationToken = default)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken);

        if (employee == null)
            return Result<IEnumerable<ShiftAssignmentDto>>.Failure("Empleado no encontrado");

        var query = _context.ShiftAssignments
            .Include(a => a.ShiftTemplate)
            .Where(a => a.EmployeeId == employeeId)
            .AsQueryable();

        if (from.HasValue)
            query = query.Where(a => a.Date >= from.Value.Date);

        if (to.HasValue)
            query = query.Where(a => a.Date <= to.Value.Date);

        var assignments = await query
            .OrderByDescending(a => a.Date)
            .ToListAsync(cancellationToken);

        var fullName = $"{employee.FirstName} {employee.LastName}";
        var dtos = assignments.Select(a => ToAssignmentDto(a, fullName, a.ShiftTemplate?.Name));

        return Result<IEnumerable<ShiftAssignmentDto>>.Success(dtos);
    }

    // ===== Helpers =====

    private static ShiftTemplateDto ToTemplateDto(ShiftTemplate t)
    {
        return new ShiftTemplateDto(
            t.Id,
            t.Name,
            t.StartTime.ToString(@"hh\:mm"),
            t.EndTime.ToString(@"hh\:mm"),
            t.DaysOfWeek,
            t.BranchId,
            t.IsActive
        );
    }

    private static ShiftAssignmentDto ToAssignmentDto(ShiftAssignment a, string employeeName, string? templateName)
    {
        return new ShiftAssignmentDto(
            a.Id,
            a.EmployeeId,
            employeeName,
            a.ShiftTemplateId,
            templateName,
            a.Date,
            a.ScheduledStart?.ToString(@"hh\:mm"),
            a.ScheduledEnd?.ToString(@"hh\:mm"),
            a.ActualClockIn,
            a.ActualClockOut,
            a.HoursWorked,
            a.ClockInMethod,
            a.ClockInLocation,
            a.Status,
            a.Notes
        );
    }
}
