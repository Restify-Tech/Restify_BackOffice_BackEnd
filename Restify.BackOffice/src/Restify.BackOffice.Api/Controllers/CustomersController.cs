using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _service;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(
        ICustomerService service,
        ILogger<CustomersController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los clientes
    /// </summary>
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
            _logger.LogError(ex, "Error al obtener clientes");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene un cliente por ID
    /// </summary>
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
            _logger.LogError(ex, "Error al obtener cliente {CustomerId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crea un nuevo cliente
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
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
            _logger.LogError(ex, "Error al crear cliente");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Actualiza un cliente existente
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerRequest request, CancellationToken cancellationToken)
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
            _logger.LogError(ex, "Error al actualizar cliente {CustomerId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Elimina un cliente
    /// </summary>
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
            _logger.LogError(ex, "Error al eliminar cliente {CustomerId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
