using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/v1/franchise")]
[Authorize]
public class FranchiseController : ControllerBase
{
    private readonly IFranchiseService _franchiseService;
    private readonly ILogger<FranchiseController> _logger;

    public FranchiseController(IFranchiseService franchiseService, ILogger<FranchiseController> logger)
    {
        _franchiseService = franchiseService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene la configuracion de franquicia del tenant actual
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyFranchise(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _franchiseService.GetMyFranchiseAsync(cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener franquicia");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crea la configuracion de franquicia para el tenant actual
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFranchiseConfigRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _franchiseService.CreateAsync(request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetMyFranchise), result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear franquicia");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Actualiza la configuracion de franquicia
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateFranchiseConfigRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _franchiseService.UpdateAsync(id, request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar franquicia {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene la lista de franquiciados
    /// </summary>
    [HttpGet("{id:guid}/franchisees")]
    public async Task<IActionResult> GetFranchisees(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _franchiseService.GetFranchiseesAsync(id, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener franquiciados de {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Agrega un franquiciado a la franquicia
    /// </summary>
    [HttpPost("{id:guid}/franchisees")]
    public async Task<IActionResult> AddFranchisee(Guid id, [FromBody] AddFranchiseeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _franchiseService.AddFranchiseeAsync(id, request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar franquiciado a {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Elimina un franquiciado de la franquicia
    /// </summary>
    [HttpDelete("{id:guid}/franchisees/{franchiseeId:guid}")]
    public async Task<IActionResult> RemoveFranchisee(Guid id, Guid franchiseeId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _franchiseService.RemoveFranchiseeAsync(id, franchiseeId, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar franquiciado {FranchiseeId}", franchiseeId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene el reporte consolidado de la franquicia
    /// </summary>
    [HttpGet("{id:guid}/consolidated-report")]
    public async Task<IActionResult> GetConsolidatedReport(
        Guid id,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _franchiseService.GetConsolidatedReportAsync(id, from, to, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener reporte consolidado de {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
