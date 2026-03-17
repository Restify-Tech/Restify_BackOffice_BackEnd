using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.Auth.Application.DTOs.ScreenPermissions;
using Restify.Auth.Application.Interfaces;

namespace Restify.Auth.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ScreenPermissionsController : ControllerBase
{
    private readonly IScreenPermissionService _screenPermissionService;
    private readonly ICurrentUserService _currentUser;

    public ScreenPermissionsController(
        IScreenPermissionService screenPermissionService,
        ICurrentUserService currentUser)
    {
        _screenPermissionService = screenPermissionService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Obtener todas las pantallas con sus permisos requeridos
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ScreenPermissionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _screenPermissionService.GetAllAsync(cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtener pantallas agrupadas por módulo
    /// </summary>
    [HttpGet("by-module")]
    [ProducesResponseType(typeof(IEnumerable<ScreenPermissionsByModuleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByModule(CancellationToken cancellationToken)
    {
        var result = await _screenPermissionService.GetByModuleAsync(cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtener pantallas accesibles para el usuario actual
    /// </summary>
    [HttpGet("accessible")]
    [ProducesResponseType(typeof(IEnumerable<ScreenPermissionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccessible(CancellationToken cancellationToken)
    {
        var userPermissions = _currentUser.Permissions;
        var result = await _screenPermissionService.GetAccessibleScreensAsync(userPermissions, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }
}
