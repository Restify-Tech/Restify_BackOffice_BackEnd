using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GlobalModifiersController : ControllerBase
{
    private readonly IGlobalModifierService _modifierService;
    private readonly ILogger<GlobalModifiersController> _logger;

    public GlobalModifiersController(
        IGlobalModifierService modifierService,
        ILogger<GlobalModifiersController> logger)
    {
        _modifierService = modifierService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los modificadores globales
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _modifierService.GetAllAsync(cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener modificadores globales");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene un modificador global por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _modifierService.GetByIdAsync(id, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener modificador global {ModifierId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene modificadores globales por tipo
    /// </summary>
    [HttpGet("type/{type}")]
    public async Task<IActionResult> GetByType(ModifierType type, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _modifierService.GetByTypeAsync(type, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener modificadores globales por tipo {Type}", type);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crea un nuevo modificador global
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGlobalModifierRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _modifierService.CreateAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear modificador global");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Actualiza un modificador global existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGlobalModifierRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _modifierService.UpdateAsync(id, request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar modificador global {ModifierId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Elimina un modificador global
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _modifierService.DeleteAsync(id, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar modificador global {ModifierId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Asigna un modificador global a un producto
    /// </summary>
    [HttpPost("{modifierId}/products/{productId}")]
    public async Task<IActionResult> AssignToProduct(
        Guid modifierId, 
        Guid productId, 
        [FromBody] AssignGlobalModifierRequest request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _modifierService.AssignToProductAsync(modifierId, productId, request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asignar modificador {ModifierId} al producto {ProductId}", modifierId, productId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Remueve un modificador global de un producto
    /// </summary>
    [HttpDelete("{modifierId}/products/{productId}")]
    public async Task<IActionResult> RemoveFromProduct(Guid modifierId, Guid productId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _modifierService.RemoveFromProductAsync(modifierId, productId, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al remover modificador {ModifierId} del producto {ProductId}", modifierId, productId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene los modificadores globales de un producto
    /// </summary>
    [HttpGet("products/{productId}")]
    public async Task<IActionResult> GetProductModifiers(Guid productId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _modifierService.GetProductModifiersAsync(productId, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener modificadores del producto {ProductId}", productId);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
