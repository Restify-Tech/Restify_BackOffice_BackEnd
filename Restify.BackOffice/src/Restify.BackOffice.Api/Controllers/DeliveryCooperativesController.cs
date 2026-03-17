using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DeliveryCooperativesController : ControllerBase
{
    private readonly IDeliveryCooperativeService _service;
    private readonly ILogger<DeliveryCooperativesController> _logger;

    public DeliveryCooperativesController(
        IDeliveryCooperativeService service,
        ILogger<DeliveryCooperativesController> logger)
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
            _logger.LogError(ex, "Error al obtener cooperativas");
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
            _logger.LogError(ex, "Error al obtener cooperativa {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDeliveryCooperativeRequest request, CancellationToken cancellationToken)
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
            _logger.LogError(ex, "Error al crear cooperativa");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDeliveryCooperativeRequest request, CancellationToken cancellationToken)
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
            _logger.LogError(ex, "Error al actualizar cooperativa {Id}", id);
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
            _logger.LogError(ex, "Error al eliminar cooperativa {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
