using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PayrollController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly IPayrollPeriodService _payrollPeriodService;
    private readonly IDeductionTypeService _deductionTypeService;
    private readonly ILogger<PayrollController> _logger;

    public PayrollController(
        IEmployeeService employeeService,
        IPayrollPeriodService payrollPeriodService,
        IDeductionTypeService deductionTypeService,
        ILogger<PayrollController> logger)
    {
        _employeeService = employeeService;
        _payrollPeriodService = payrollPeriodService;
        _deductionTypeService = deductionTypeService;
        _logger = logger;
    }

    // ========== Employees ==========

    /// <summary>
    /// Obtiene todos los empleados
    /// </summary>
    [HttpGet("employees")]
    public async Task<IActionResult> GetAllEmployees(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _employeeService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener empleados");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene un empleado por ID
    /// </summary>
    [HttpGet("employees/{id}")]
    public async Task<IActionResult> GetEmployeeById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _employeeService.GetByIdAsync(id, cancellationToken);
            if (!result.IsSuccess) return NotFound(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener empleado {EmployeeId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crea un nuevo empleado
    /// </summary>
    [HttpPost("employees")]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _employeeService.CreateAsync(request, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = result.Data!.Id }, result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear empleado");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Actualiza un empleado
    /// </summary>
    [HttpPut("employees/{id}")]
    public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _employeeService.UpdateAsync(id, request, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar empleado {EmployeeId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Elimina un empleado
    /// </summary>
    [HttpDelete("employees/{id}")]
    public async Task<IActionResult> DeleteEmployee(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _employeeService.DeleteAsync(id, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar empleado {EmployeeId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Activa un empleado
    /// </summary>
    [HttpPatch("employees/{id}/activate")]
    public async Task<IActionResult> ActivateEmployee(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _employeeService.ActivateAsync(id, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al activar empleado {EmployeeId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Desactiva un empleado
    /// </summary>
    [HttpPatch("employees/{id}/deactivate")]
    public async Task<IActionResult> DeactivateEmployee(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _employeeService.DeactivateAsync(id, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar empleado {EmployeeId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // ========== Payroll Periods ==========

    /// <summary>
    /// Obtiene todos los periodos de nomina
    /// </summary>
    [HttpGet("periods")]
    public async Task<IActionResult> GetAllPeriods(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _payrollPeriodService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener periodos de nomina");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene un periodo de nomina por ID
    /// </summary>
    [HttpGet("periods/{id}")]
    public async Task<IActionResult> GetPeriodById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _payrollPeriodService.GetByIdAsync(id, cancellationToken);
            if (!result.IsSuccess) return NotFound(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener periodo de nomina {PeriodId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crea un nuevo periodo de nomina
    /// </summary>
    [HttpPost("periods")]
    public async Task<IActionResult> CreatePeriod([FromBody] CreatePayrollPeriodRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _payrollPeriodService.CreateAsync(request, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return CreatedAtAction(nameof(GetPeriodById), new { id = result.Data!.Id }, result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear periodo de nomina");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Calcula la nomina del periodo
    /// </summary>
    [HttpPost("periods/{id}/calculate")]
    public async Task<IActionResult> CalculatePayroll(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _payrollPeriodService.CalculatePayrollAsync(id, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al calcular nomina del periodo {PeriodId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Aprueba un periodo de nomina calculado
    /// </summary>
    [HttpPatch("periods/{id}/approve")]
    public async Task<IActionResult> ApprovePeriod(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var approvedBy = User.FindFirst("email")?.Value ?? "sistema";
            var result = await _payrollPeriodService.ApproveAsync(id, approvedBy, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al aprobar periodo de nomina {PeriodId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Marca como pagado un periodo de nomina aprobado
    /// </summary>
    [HttpPatch("periods/{id}/pay")]
    public async Task<IActionResult> MarkPaid(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var paidBy = User.FindFirst("email")?.Value ?? "sistema";
            var result = await _payrollPeriodService.MarkPaidAsync(id, paidBy, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al marcar como pagado periodo de nomina {PeriodId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Elimina un periodo de nomina en borrador
    /// </summary>
    [HttpDelete("periods/{id}")]
    public async Task<IActionResult> DeletePeriod(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _payrollPeriodService.DeleteAsync(id, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar periodo de nomina {PeriodId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // ========== Deduction Types ==========

    /// <summary>
    /// Obtiene todos los tipos de deduccion
    /// </summary>
    [HttpGet("deduction-types")]
    public async Task<IActionResult> GetAllDeductionTypes(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _deductionTypeService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tipos de deduccion");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crea un nuevo tipo de deduccion
    /// </summary>
    [HttpPost("deduction-types")]
    public async Task<IActionResult> CreateDeductionType([FromBody] CreateDeductionTypeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _deductionTypeService.CreateAsync(request, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Created("", result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear tipo de deduccion");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Actualiza un tipo de deduccion
    /// </summary>
    [HttpPut("deduction-types/{id}")]
    public async Task<IActionResult> UpdateDeductionType(Guid id, [FromBody] UpdateDeductionTypeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _deductionTypeService.UpdateAsync(id, request, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar tipo de deduccion {DeductionTypeId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Elimina un tipo de deduccion
    /// </summary>
    [HttpDelete("deduction-types/{id}")]
    public async Task<IActionResult> DeleteDeductionType(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _deductionTypeService.DeleteAsync(id, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar tipo de deduccion {DeductionTypeId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
