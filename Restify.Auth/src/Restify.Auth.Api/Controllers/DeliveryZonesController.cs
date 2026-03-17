using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.DeliveryZones;
using Restify.Auth.Application.Interfaces;

namespace Restify.Auth.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DeliveryZonesController : ControllerBase
{
    private readonly IDeliveryZoneService _service;
    private readonly ICurrentUserService _currentUser;

    public DeliveryZonesController(IDeliveryZoneService service, ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsSuperAdmin)
            return Forbid();

        var result = await _service.GetAllAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsSuperAdmin)
            return Forbid();

        var result = await _service.GetByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDeliveryZoneRequest request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsSuperAdmin)
            return Forbid();

        var result = await _service.CreateAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDeliveryZoneRequest request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsSuperAdmin)
            return Forbid();

        var result = await _service.UpdateAsync(id, request, cancellationToken);

        if (!result.IsSuccess)
            return result.Error?.Contains("no encontrad") == true
                ? NotFound(new { error = result.Error })
                : BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsSuperAdmin)
            return Forbid();

        var result = await _service.DeleteAsync(id, cancellationToken);

        if (!result.IsSuccess)
            return result.Error?.Contains("no encontrad") == true
                ? NotFound(new { error = result.Error })
                : BadRequest(new { error = result.Error });

        return NoContent();
    }
}
