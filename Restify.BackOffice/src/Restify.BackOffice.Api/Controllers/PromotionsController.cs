using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/v1/promotions")]
[Authorize]
public class PromotionsController : ControllerBase
{
    private readonly IPromotionService _service;
    private readonly ILogger<PromotionsController> _logger;

    public PromotionsController(IPromotionService service, ILogger<PromotionsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las promociones (activas e inactivas)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetActive(CancellationToken ct)
    {
        var result = await _service.GetActiveAsync(ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Crea una nueva promocion
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePromotionRequest request, CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Actualiza una promocion existente
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreatePromotionRequest request, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, request, ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Activa o desactiva una promocion
    /// </summary>
    [HttpPut("{id:guid}/toggle")]
    public async Task<IActionResult> ToggleActive(Guid id, CancellationToken ct)
    {
        var result = await _service.ToggleActiveAsync(id, ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { isActive = result.Data });
    }

    /// <summary>
    /// Evalua que promociones aplican a un pedido especifico
    /// </summary>
    [HttpGet("evaluate/{orderId:guid}")]
    public async Task<IActionResult> EvaluateForOrder(Guid orderId, CancellationToken ct)
    {
        var result = await _service.EvaluateForOrderAsync(orderId, ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Calcula descuentos para una lista de items sin necesidad de crear un pedido
    /// </summary>
    [HttpPost("calculate")]
    public async Task<IActionResult> Calculate([FromBody] CalculatePromotionsRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _service.CalculateAsync(request, ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculando promociones");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene sugerencias de upselling basadas en los productos del carrito
    /// </summary>
    [HttpGet("upsell")]
    public async Task<IActionResult> GetUpsellSuggestions([FromQuery] string productIds, CancellationToken ct)
    {
        try
        {
            var ids = (productIds ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Where(id => Guid.TryParse(id.Trim(), out _))
                .Select(id => Guid.Parse(id.Trim()))
                .ToList();

            var result = await _service.GetUpsellSuggestionsAsync(ids, ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo sugerencias upsell");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
}
