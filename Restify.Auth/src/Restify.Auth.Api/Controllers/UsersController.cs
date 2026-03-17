using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.Users;
using Restify.Auth.Application.Interfaces;

namespace Restify.Auth.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Obtener lista paginada de usuarios
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<UserListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.GetAllAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtener usuario por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Crear usuario
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.CreateAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>
    /// Actualizar usuario
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateAsync(id, request, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error?.Contains("no encontrado") == true)
                return NotFound(new { error = result.Error });

            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Eliminar usuario (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _userService.DeleteAsync(id, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return NoContent();
    }

    /// <summary>
    /// Resetear contraseña de usuario
    /// </summary>
    [HttpPost("{id:guid}/reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetPassword(Guid id, CancellationToken cancellationToken)
    {
        var result = await _userService.ResetPasswordAsync(id, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(new { temporaryPassword = result.Data });
    }
}
