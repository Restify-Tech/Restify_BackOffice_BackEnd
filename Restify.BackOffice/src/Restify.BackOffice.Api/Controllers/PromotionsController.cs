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

    public PromotionsController(IPromotionService service)
    {
        _service = service;
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
}
