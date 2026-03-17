using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.Roles;
using Restify.Auth.Application.Interfaces;

namespace Restify.Auth.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// Obtener lista paginada de roles
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<RoleListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request, CancellationToken cancellationToken)
    {
        var result = await _roleService.GetAllAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtener rol por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _roleService.GetByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtener todos los permisos disponibles
    /// </summary>
    [HttpGet("permissions")]
    [ProducesResponseType(typeof(IEnumerable<PermissionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPermissions(CancellationToken cancellationToken)
    {
        var result = await _roleService.GetAllPermissionsAsync(cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Crear rol
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await _roleService.CreateAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>
    /// Actualizar rol
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await _roleService.UpdateAsync(id, request, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error?.Contains("no encontrado") == true)
                return NotFound(new { error = result.Error });

            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Eliminar rol
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _roleService.DeleteAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error?.Contains("no encontrado") == true)
                return NotFound(new { error = result.Error });

            return BadRequest(new { error = result.Error });
        }

        return NoContent();
    }
}
