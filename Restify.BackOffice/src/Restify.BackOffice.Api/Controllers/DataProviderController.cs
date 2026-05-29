using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.Core.Application.DTOs.DataProvider;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Api.Controllers;

/// <summary>
/// Controller genérico para proveer datos de cualquier entidad registrada.
/// Similar al patrón DataProvider del RP3 Framework.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DataProviderController : ControllerBase
{
    private readonly IDataProviderService _dataProvider;

    public DataProviderController(IDataProviderService dataProvider)
    {
        _dataProvider = dataProvider;
    }

    /// <summary>
    /// Obtiene las entidades registradas disponibles
    /// </summary>
    [HttpGet("entities")]
    [AllowAnonymous]
    public IActionResult GetRegisteredEntities()
    {
        var entities = _dataProvider.GetRegisteredEntities();
        return Ok(new { entities });
    }

    /// <summary>
    /// Obtiene la metadata de una entidad (configuración del grid, campos, etc.)
    /// </summary>
    [HttpGet("{entityName}/metadata")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMetadata(
        string entityName,
        [FromQuery] string viewName = "default",
        CancellationToken cancellationToken = default)
    {
        var result = await _dataProvider.GetMetadataAsync(
            new DataProviderMetadataRequest { EntityName = entityName, ViewName = viewName },
            cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Ejecuta una consulta sobre una entidad (GET con query params)
    /// </summary>
    [HttpGet("{entityName}/query")]
    [Authorize]
    public async Task<IActionResult> Query(
        string entityName,
        [FromQuery] string viewName = "default",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortField = null,
        [FromQuery] string sortDirection = "asc",
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var request = new DataProviderQueryRequest
        {
            EntityName = entityName,
            ViewName = viewName,
            Page = page,
            PageSize = pageSize,
            SortField = sortField,
            SortDirection = sortDirection,
            GlobalSearch = search
        };

        var result = await _dataProvider.QueryAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Ejecuta una consulta sobre una entidad (POST con body para filtros complejos)
    /// </summary>
    [HttpPost("{entityName}/query")]
    public async Task<IActionResult> QueryPost(
        string entityName,
        [FromBody] DataProviderQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        request.EntityName = entityName;

        var result = await _dataProvider.QueryAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtiene un registro por ID
    /// </summary>
    [HttpGet("{entityName}/{id:guid}")]
    public async Task<IActionResult> GetById(
        string entityName,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _dataProvider.GetByIdAsync(entityName, id, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Crea un nuevo registro
    /// </summary>
    [HttpPost("{entityName}")]
    public async Task<IActionResult> Create(
        string entityName,
        [FromBody] Dictionary<string, object?> data,
        CancellationToken cancellationToken = default)
    {
        var result = await _dataProvider.CreateAsync(entityName, data, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        var id = result.Data?.TryGetValue("id", out var idValue) == true ? idValue : null;
        return CreatedAtAction(nameof(GetById), new { entityName, id }, result.Data);
    }

    /// <summary>
    /// Actualiza un registro existente
    /// </summary>
    [HttpPut("{entityName}/{id:guid}")]
    public async Task<IActionResult> Update(
        string entityName,
        Guid id,
        [FromBody] Dictionary<string, object?> data,
        CancellationToken cancellationToken = default)
    {
        var result = await _dataProvider.UpdateAsync(entityName, id, data, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Elimina un registro
    /// </summary>
    [HttpDelete("{entityName}/{id:guid}")]
    public async Task<IActionResult> Delete(
        string entityName,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _dataProvider.DeleteAsync(entityName, id, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }
}
