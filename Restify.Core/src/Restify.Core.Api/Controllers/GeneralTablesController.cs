using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.Core.Application.DTOs.General;
using Restify.Core.Application.Interfaces;

namespace Restify.Core.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous] // Temporal mientras se configura auth
public class GeneralTablesController : ControllerBase
{
    private readonly IGeneralTableService _service;

    public GeneralTablesController(IGeneralTableService service)
    {
        _service = service;
    }

    /// <summary>
    /// Obtiene todas las tablas
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool? activeOnly = true, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetAllAsync(activeOnly, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtiene una tabla por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, [FromQuery] bool includeValues = false, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetByIdAsync(id, includeValues, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtiene una tabla por código
    /// </summary>
    [HttpGet("code/{code}")]
    public async Task<IActionResult> GetByCode(string code, [FromQuery] bool includeValues = false, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetByCodeAsync(code, includeValues, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtiene tablas por código de aplicación
    /// </summary>
    [HttpGet("application/{applicationCode}")]
    public async Task<IActionResult> GetByApplicationCode(string applicationCode, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetByApplicationCodeAsync(applicationCode, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtiene las tablas hijas de una tabla padre
    /// </summary>
    [HttpGet("{parentId:guid}/children")]
    public async Task<IActionResult> GetChildren(Guid parentId, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetChildrenAsync(parentId, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtiene el árbol jerárquico de tablas
    /// </summary>
    [HttpGet("tree")]
    public async Task<IActionResult> GetTree([FromQuery] string? applicationCode = null, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetTreeAsync(applicationCode, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Crea una nueva tabla
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGeneralTableRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _service.CreateAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>
    /// Actualiza una tabla
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGeneralTableRequest request, CancellationToken cancellationToken = default)
    {
        if (id != request.Id)
            return BadRequest(new { error = "ID mismatch" });

        var result = await _service.UpdateAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Elimina una tabla
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }

    /// <summary>
    /// Obtiene lookup de tablas (id, name)
    /// </summary>
    [HttpGet("lookup")]
    public async Task<IActionResult> GetLookup(CancellationToken cancellationToken = default)
    {
        var result = await _service.GetLookupAsync(cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }
}
