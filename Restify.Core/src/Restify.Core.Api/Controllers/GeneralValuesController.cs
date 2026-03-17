using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.Core.Application.DTOs.General;
using Restify.Core.Application.Interfaces;

namespace Restify.Core.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous] // Temporal mientras se configura auth
public class GeneralValuesController : ControllerBase
{
    private readonly IGeneralValueService _service;

    public GeneralValuesController(IGeneralValueService service)
    {
        _service = service;
    }

    /// <summary>
    /// Obtiene un valor por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtiene un valor por código de tabla y código de valor
    /// </summary>
    [HttpGet("table/{tableCode}/value/{valueCode}")]
    public async Task<IActionResult> GetByCode(string tableCode, string valueCode, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetByCodeAsync(tableCode, valueCode, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtiene valores de una tabla por ID de tabla
    /// </summary>
    [HttpGet("table/{tableId:guid}")]
    public async Task<IActionResult> GetByTableId(Guid tableId, [FromQuery] bool? activeOnly = true, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetByTableIdAsync(tableId, activeOnly, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtiene valores de una tabla por código de tabla
    /// </summary>
    [HttpGet("table/code/{tableCode}")]
    public async Task<IActionResult> GetByTableCode(string tableCode, [FromQuery] bool? activeOnly = true, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetByTableCodeAsync(tableCode, activeOnly, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Crea un nuevo valor
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGeneralValueRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _service.CreateAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>
    /// Actualiza un valor
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGeneralValueRequest request, CancellationToken cancellationToken = default)
    {
        if (id != request.Id)
            return BadRequest(new { error = "ID mismatch" });

        var result = await _service.UpdateAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Elimina un valor
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
    /// Establece un valor como default
    /// </summary>
    [HttpPost("{id:guid}/set-default")]
    public async Task<IActionResult> SetDefault(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _service.SetDefaultAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { success = true });
    }

    /// <summary>
    /// Reordena los valores de una tabla
    /// </summary>
    [HttpPost("table/{tableId:guid}/reorder")]
    public async Task<IActionResult> Reorder(Guid tableId, [FromBody] List<Guid> orderedIds, CancellationToken cancellationToken = default)
    {
        var result = await _service.ReorderAsync(tableId, orderedIds, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { success = true });
    }

    /// <summary>
    /// Obtiene lookup de valores de una tabla (id, name)
    /// </summary>
    [HttpGet("table/{tableId:guid}/lookup")]
    public async Task<IActionResult> GetLookup(Guid tableId, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetLookupAsync(tableId, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtiene lookup de valores por código de tabla (id, name)
    /// </summary>
    [HttpGet("table/code/{tableCode}/lookup")]
    public async Task<IActionResult> GetLookupByTableCode(string tableCode, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetLookupByTableCodeAsync(tableCode, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }
}
