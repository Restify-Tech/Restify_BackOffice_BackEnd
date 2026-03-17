using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/transfer-approvals")]
[Authorize]
public class TransferApprovalsController : ControllerBase
{
    private readonly ITransferApprovalService _service;

    public TransferApprovalsController(ITransferApprovalService service)
    {
        _service = service;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending(CancellationToken ct)
    {
        var result = await _service.GetPendingAsync(ct);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Data);
    }

    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetByOrderId(Guid orderId, CancellationToken ct)
    {
        var result = await _service.GetByOrderIdAsync(orderId, ct);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Data);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromBody] CreateTransferPaymentRequest request, CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPost("{id}/review")]
    public async Task<IActionResult> Review(Guid id, [FromBody] ReviewTransferPaymentRequest request, CancellationToken ct)
    {
        var result = await _service.ReviewAsync(id, request, ct);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Data);
    }
}
