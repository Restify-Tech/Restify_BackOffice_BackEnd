using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/v1/shifts")]
[Authorize]
public class ShiftsController : ControllerBase
{
    private readonly IShiftService _service;
    private readonly ILogger<ShiftsController> _logger;

    public ShiftsController(IShiftService service, ILogger<ShiftsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// GET /api/v1/shifts/templates
    /// Obtiene todas las plantillas de turno
    /// </summary>
    [HttpGet("templates")]
    public async Task<ActionResult<Result<IEnumerable<ShiftTemplateDto>>>> GetTemplates(CancellationToken cancellationToken)
    {
        var result = await _service.GetTemplatesAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// POST /api/v1/shifts/templates
    /// Crea una nueva plantilla de turno
    /// </summary>
    [HttpPost("templates")]
    public async Task<ActionResult<Result<ShiftTemplateDto>>> CreateTemplate(
        [FromBody] CreateShiftTemplateRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.CreateTemplateAsync(request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear plantilla de turno");
            return StatusCode(500, Result<ShiftTemplateDto>.Failure("Error interno al crear plantilla"));
        }
    }

    /// <summary>
    /// POST /api/v1/shifts/assignments
    /// Crea una asignacion de turno para un empleado
    /// </summary>
    [HttpPost("assignments")]
    public async Task<ActionResult<Result<ShiftAssignmentDto>>> CreateAssignment(
        [FromBody] CreateShiftAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.CreateAssignmentAsync(request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear asignacion de turno");
            return StatusCode(500, Result<ShiftAssignmentDto>.Failure("Error interno al crear asignacion"));
        }
    }

    /// <summary>
    /// POST /api/v1/shifts/clock-in
    /// Registra la entrada del empleado
    /// </summary>
    [HttpPost("clock-in")]
    public async Task<ActionResult<Result<ShiftAssignmentDto>>> ClockIn(
        [FromBody] ClockInRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.ClockInAsync(request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar entrada de empleado");
            return StatusCode(500, Result<ShiftAssignmentDto>.Failure("Error interno al registrar entrada"));
        }
    }

    /// <summary>
    /// POST /api/v1/shifts/clock-out
    /// Registra la salida del empleado y calcula horas trabajadas
    /// </summary>
    [HttpPost("clock-out")]
    public async Task<ActionResult<Result<ShiftAssignmentDto>>> ClockOut(
        [FromBody] ClockOutRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.ClockOutAsync(request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar salida de empleado");
            return StatusCode(500, Result<ShiftAssignmentDto>.Failure("Error interno al registrar salida"));
        }
    }

    /// <summary>
    /// GET /api/v1/shifts/weekly-schedule?weekStart=...&branchId=...
    /// Obtiene el horario semanal agrupado por empleado
    /// </summary>
    [HttpGet("weekly-schedule")]
    public async Task<ActionResult<Result<WeeklyScheduleDto>>> GetWeeklySchedule(
        [FromQuery] DateTime weekStart,
        [FromQuery] Guid? branchId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.GetWeeklyScheduleAsync(weekStart, branchId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// GET /api/v1/shifts/employee/{employeeId}?from=...&to=...
    /// Obtiene los turnos de un empleado en un rango de fechas
    /// </summary>
    [HttpGet("employee/{employeeId:guid}")]
    public async Task<ActionResult<Result<IEnumerable<ShiftAssignmentDto>>>> GetEmployeeShifts(
        Guid employeeId,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.GetEmployeeShiftsAsync(employeeId, from, to, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }
}
