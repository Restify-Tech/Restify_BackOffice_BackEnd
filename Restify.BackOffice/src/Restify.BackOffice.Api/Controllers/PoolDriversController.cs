using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PoolDriversController : ControllerBase
{
    private readonly IPoolDriverService _service;
    private readonly ICurrentUserService _currentUser;

    public PoolDriversController(IPoolDriverService service, ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.IsSuperAdmin)
            return Forbid();

        var result = await _service.GetAllPoolDriversAsync(page, pageSize, search, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsSuperAdmin)
            return Forbid();

        var result = await _service.GetPoolDriverByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Data);
    }

    [HttpPatch("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveDriverRequest request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsSuperAdmin)
            return Forbid();

        var result = await _service.ApproveDriverAsync(id, request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }

    [HttpPatch("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectDriverRequest request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsSuperAdmin)
            return Forbid();

        var result = await _service.RejectDriverAsync(id, request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }

    [HttpPatch("{driverId:guid}/documents/{documentId:guid}/review")]
    public async Task<IActionResult> ReviewDocument(Guid driverId, Guid documentId, [FromBody] ReviewDocumentRequest request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsSuperAdmin)
            return Forbid();

        var result = await _service.ReviewDocumentAsync(driverId, documentId, request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }
}
