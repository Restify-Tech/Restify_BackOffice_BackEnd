using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.Auth.Application.DTOs.Plans;
using Restify.Auth.Application.Interfaces;

namespace Restify.Auth.Api.Controllers;

/// <summary>
/// Gestión de planes de suscripción (SuperAdmin)
/// </summary>
[Authorize]
[ApiController]
[Route("api/plans")]
public class PlansController : ControllerBase
{
    private readonly IPlanService _planService;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<PlansController> _logger;

    public PlansController(IPlanService planService, ICurrentUserService currentUser, ILogger<PlansController> logger)
    {
        _planService = planService;
        _currentUser = currentUser;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los planes de suscripción
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _planService.GetAllAsync(cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtiene un plan por ID con las pantallas incluidas
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _planService.GetByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Crea un nuevo plan (solo SuperAdmin)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePlanRequest request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsSuperAdmin)
            return Forbid();

        var result = await _planService.CreateAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>
    /// Actualiza un plan existente (solo SuperAdmin)
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePlanRequest request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsSuperAdmin)
            return Forbid();

        var result = await _planService.UpdateAsync(id, request, cancellationToken);

        if (!result.IsSuccess)
            return result.Error?.Contains("no encontrad") == true
                ? NotFound(new { error = result.Error })
                : BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Elimina un plan (solo SuperAdmin)
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsSuperAdmin)
            return Forbid();

        var result = await _planService.DeleteAsync(id, cancellationToken);

        if (!result.IsSuccess)
            return result.Error?.Contains("no encontrad") == true
                ? NotFound(new { error = result.Error })
                : BadRequest(new { error = result.Error });

        return NoContent();
    }
}
