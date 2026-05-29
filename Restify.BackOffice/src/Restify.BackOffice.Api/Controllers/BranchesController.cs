using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/v1/branches")]
[Authorize]
public class BranchesController : ControllerBase
{
    private readonly IBranchService _branchService;
    private readonly IManagerAssignmentService _managerAssignmentService;
    private readonly ILogger<BranchesController> _logger;

    public BranchesController(
        IBranchService branchService,
        IManagerAssignmentService managerAssignmentService,
        ILogger<BranchesController> logger)
    {
        _branchService = branchService;
        _managerAssignmentService = managerAssignmentService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las sucursales
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _branchService.GetAllAsync(cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener sucursales");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene reporte consolidado de todas las sucursales
    /// </summary>
    [HttpGet("consolidated")]
    public async Task<IActionResult> GetConsolidatedReport(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _branchService.GetConsolidatedReportAsync(from, to, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener reporte consolidado de sucursales");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene una sucursal por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _branchService.GetByIdAsync(id, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener sucursal {BranchId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene estadisticas de una sucursal
    /// </summary>
    [HttpGet("{id}/stats")]
    public async Task<IActionResult> GetStats(
        Guid id,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _branchService.GetStatsAsync(id, from, to, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadisticas de sucursal {BranchId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crea una nueva sucursal
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBranchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _branchService.CreateAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear sucursal");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Actualiza una sucursal existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBranchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _branchService.UpdateAsync(id, request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar sucursal {BranchId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Elimina una sucursal
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _branchService.DeleteAsync(id, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar sucursal {BranchId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // ──── Manager Assignment Endpoints ────

    /// <summary>
    /// Obtiene los gerentes asignados a una sucursal
    /// </summary>
    [HttpGet("{branchId}/managers")]
    public async Task<IActionResult> GetManagers(Guid branchId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _managerAssignmentService.GetByBranchAsync(branchId, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener gerentes de sucursal {BranchId}", branchId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Asigna un gerente a una sucursal
    /// </summary>
    [HttpPost("{branchId}/managers")]
    public async Task<IActionResult> AddManager(Guid branchId, [FromBody] CreateManagerAssignmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            // Asegurar que el BranchId del request coincide con la ruta
            if (request.BranchId != branchId)
                return BadRequest("El BranchId del request no coincide con la ruta");

            var result = await _managerAssignmentService.CreateAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asignar gerente a sucursal {BranchId}", branchId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Remueve (desactiva) la asignacion de un gerente
    /// </summary>
    [HttpDelete("{branchId}/managers/{id}")]
    public async Task<IActionResult> RemoveManager(Guid branchId, Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _managerAssignmentService.DeleteAsync(id, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al remover gerente {AssignmentId} de sucursal {BranchId}", id, branchId);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
