using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Enums;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PurchaseOrdersController : ControllerBase
{
    private readonly IPurchaseOrderService _service;

    public PurchaseOrdersController(IPurchaseOrderService service)
    {
        _service = service;
    }

    /// <summary>
    /// Obtiene todas las órdenes de compra (opcionalmente filtrado por estado)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PurchaseOrderStatus? status, CancellationToken ct)
    {
        var result = await _service.GetAllAsync(status, ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtiene una orden de compra por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Crea una nueva orden de compra
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderRequest request, CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>
    /// Actualiza una orden de compra (solo en estado Draft)
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePurchaseOrderRequest request, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, request, ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Elimina una orden de compra (solo en estado Draft)
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }

    /// <summary>
    /// Envía la orden al proveedor (Draft → Sent)
    /// </summary>
    [HttpPost("{id:guid}/send")]
    public async Task<IActionResult> Send(Guid id, CancellationToken ct)
    {
        var result = await _service.SendAsync(id, ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Recibe la orden (total o parcialmente) y actualiza el inventario
    /// </summary>
    [HttpPost("{id:guid}/receive")]
    public async Task<IActionResult> Receive(Guid id, [FromBody] ReceivePurchaseOrderRequest request, CancellationToken ct)
    {
        var result = await _service.ReceiveAsync(id, request, ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Cancela una orden de compra
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelOrderRequest request, CancellationToken ct)
    {
        var result = await _service.CancelAsync(id, request.Reason, ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }
}

/// <summary>
/// Request para cancelar orden
/// </summary>
public class CancelOrderRequest
{
    public string Reason { get; set; } = string.Empty;
}
