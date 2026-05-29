using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TablesController : ControllerBase
{
    private readonly ITableService _tableService;
    private readonly ITableAssignmentService _tableAssignmentService;
    private readonly ILogger<TablesController> _logger;

    public TablesController(
        ITableService tableService,
        ITableAssignmentService tableAssignmentService,
        ILogger<TablesController> logger)
    {
        _tableService = tableService;
        _tableAssignmentService = tableAssignmentService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las mesas
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _tableService.GetAllAsync(cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener mesas");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene una mesa por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _tableService.GetByIdAsync(id, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener mesa {TableId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene una mesa por número
    /// </summary>
    [HttpGet("number/{number}")]
    public async Task<IActionResult> GetByNumber(string number, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _tableService.GetByNumberAsync(number, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener mesa {TableNumber}", number);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene mesas por estado
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(TableStatus status, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _tableService.GetByStatusAsync(status, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener mesas por estado {Status}", status);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene mesas por zona
    /// </summary>
    [HttpGet("zone/{zone}")]
    public async Task<IActionResult> GetByZone(string zone, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _tableService.GetByZoneAsync(zone, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener mesas de zona {Zone}", zone);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene el layout completo del restaurante
    /// </summary>
    [HttpGet("layout")]
    public async Task<IActionResult> GetLayout(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _tableService.GetLayoutAsync(cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener layout de mesas");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crea una nueva mesa
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTableRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _tableService.CreateAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear mesa");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Actualiza una mesa existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTableRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _tableService.UpdateAsync(id, request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar mesa {TableId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Actualiza el estado de una mesa
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTableStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _tableService.UpdateStatusAsync(id, request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar estado de mesa {TableId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Elimina una mesa
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _tableService.DeleteAsync(id, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar mesa {TableId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // ──── Table Assignment Endpoints ────

    /// <summary>
    /// Sugiere mesas disponibles para un grupo de personas
    /// </summary>
    [HttpGet("suggest")]
    public async Task<IActionResult> SuggestTable([FromQuery] int guestCount, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _tableAssignmentService.SuggestTableAsync(guestCount, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al sugerir mesa para {GuestCount} personas", guestCount);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Asigna una mesa a un pedido existente
    /// </summary>
    [HttpPost("{id}/assign")]
    public async Task<IActionResult> AssignTable(Guid id, [FromBody] AssignTableRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _tableAssignmentService.AssignTableAsync(
                id, request.OrderId, request.GuestCount, null, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asignar mesa {TableId} al pedido {OrderId}", id, request.OrderId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Libera una mesa ocupada
    /// </summary>
    [HttpPost("{id}/release")]
    public async Task<IActionResult> ReleaseTable(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _tableAssignmentService.ReleaseTableAsync(id, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al liberar mesa {TableId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene el perfil del cliente asociado al pedido activo en una mesa
    /// </summary>
    [HttpGet("{id}/customer-profile")]
    public async Task<IActionResult> GetCustomerProfile(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _tableService.GetCustomerProfileAsync(id, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener perfil del cliente para mesa {TableId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
