using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.Core.Application.DTOs.Grid;
using Restify.Core.Application.Interfaces;

namespace Restify.Core.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GridConfigurationsController : ControllerBase
{
    private readonly IGridConfigurationService _service;

    public GridConfigurationsController(IGridConfigurationService service)
    {
        _service = service;
    }

    /// <summary>
    /// Obtiene todas las configuraciones de grid
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _service.GetAllAsync(cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtiene una configuración de grid por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtiene una configuración de grid por nombre de entidad
    /// </summary>
    [HttpGet("entity/{entityName}")]
    [AllowAnonymous] // El frontend necesita acceder sin auth para configurar grids
    public async Task<IActionResult> GetByEntityName(string entityName, CancellationToken cancellationToken)
    {
        var result = await _service.GetByEntityNameAsync(entityName, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Crea una nueva configuración de grid
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGridConfigurationRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>
    /// Actualiza una configuración de grid
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGridConfigurationRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id)
            return BadRequest(new { error = "ID mismatch" });

        var result = await _service.UpdateAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Elimina una configuración de grid
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }

    #region Columns

    /// <summary>
    /// Agrega una columna a un grid
    /// </summary>
    [HttpPost("columns")]
    public async Task<IActionResult> AddColumn([FromBody] CreateGridColumnRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.AddColumnAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Actualiza una columna
    /// </summary>
    [HttpPut("columns/{columnId:guid}")]
    public async Task<IActionResult> UpdateColumn(Guid columnId, [FromBody] CreateGridColumnRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateColumnAsync(columnId, request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Elimina una columna
    /// </summary>
    [HttpDelete("columns/{columnId:guid}")]
    public async Task<IActionResult> DeleteColumn(Guid columnId, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteColumnAsync(columnId, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }

    #endregion

    #region Validations

    /// <summary>
    /// Agrega una validación a una columna
    /// </summary>
    [HttpPost("columns/validations")]
    public async Task<IActionResult> AddValidation([FromBody] CreateGridColumnValidationRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.AddValidationAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Elimina una validación
    /// </summary>
    [HttpDelete("columns/validations/{validationId:guid}")]
    public async Task<IActionResult> DeleteValidation(Guid validationId, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteValidationAsync(validationId, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }

    #endregion

    #region Lookups

    /// <summary>
    /// Configura el lookup de una columna
    /// </summary>
    [HttpPost("columns/lookups")]
    public async Task<IActionResult> SetLookup([FromBody] CreateGridColumnLookupRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.SetLookupAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Elimina el lookup de una columna
    /// </summary>
    [HttpDelete("columns/{columnId:guid}/lookup")]
    public async Task<IActionResult> RemoveLookup(Guid columnId, CancellationToken cancellationToken)
    {
        var result = await _service.RemoveLookupAsync(columnId, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }

    #endregion
}
