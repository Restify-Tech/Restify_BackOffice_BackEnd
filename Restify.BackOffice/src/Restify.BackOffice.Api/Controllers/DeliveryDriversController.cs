using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DeliveryDriversController : ControllerBase
{
    private readonly IDeliveryDriverService _service;
    private readonly ILogger<DeliveryDriversController> _logger;

    public DeliveryDriversController(
        IDeliveryDriverService service,
        ILogger<DeliveryDriversController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetAllAsync(cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener motorizados");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetByIdAsync(id, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener motorizado {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetAvailableAsync(cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener motorizados disponibles");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDeliveryDriverRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.CreateAsync(request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear motorizado");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDeliveryDriverRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.UpdateAsync(id, request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar motorizado {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.DeleteAsync(id, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar motorizado {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateDriverStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.UpdateStatusAsync(id, request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar estado del motorizado {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPatch("{id:guid}/verify")]
    public async Task<IActionResult> Verify(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.VerifyAsync(id, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar motorizado {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
