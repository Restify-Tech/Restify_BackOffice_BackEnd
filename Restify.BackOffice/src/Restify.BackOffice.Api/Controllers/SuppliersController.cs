using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _service;

    public SuppliersController(ISupplierService service)
    {
        _service = service;
    }

    /// <summary>
    /// Obtiene todos los proveedores
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _service.GetAllAsync(ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtiene un proveedor por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Crea un nuevo proveedor
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSupplierRequest request, CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>
    /// Actualiza un proveedor existente
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSupplierRequest request, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, request, ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Elimina un proveedor
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }

    /// <summary>
    /// Obtiene lista para lookups (solo id y nombre)
    /// </summary>
    [HttpGet("lookup")]
    public async Task<IActionResult> GetLookup(CancellationToken ct)
    {
        var result = await _service.GetAllAsync(ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        var lookup = result.Data!
            .Where(s => s.IsActive)
            .Select(s => new { id = s.Id, name = s.Name })
            .OrderBy(s => s.name);

        return Ok(lookup);
    }
}
