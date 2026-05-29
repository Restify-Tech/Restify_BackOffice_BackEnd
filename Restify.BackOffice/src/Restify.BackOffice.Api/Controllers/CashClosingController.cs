using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/v1/cash-closings")]
[Authorize]
public class CashClosingController : ControllerBase
{
    private readonly ICashClosingService _service;
    private readonly ILogger<CashClosingController> _logger;

    public CashClosingController(ICashClosingService service, ILogger<CashClosingController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// POST /api/v1/cash-closings/initiate
    /// Inicia un cierre formal de caja
    /// </summary>
    [HttpPost("initiate")]
    public async Task<ActionResult<Result<CashClosingDto>>> Initiate(
        [FromBody] InitiateCashClosingRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.InitiateAsync(request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al iniciar cierre de caja");
            return StatusCode(500, Result<CashClosingDto>.Failure("Error interno al iniciar cierre de caja"));
        }
    }

    /// <summary>
    /// POST /api/v1/cash-closings/submit-denominations
    /// Envia las denominaciones contadas y cambia a estado PendingReview
    /// </summary>
    [HttpPost("submit-denominations")]
    public async Task<ActionResult<Result<CashClosingDto>>> SubmitDenominations(
        [FromBody] SubmitDenominationsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.SubmitDenominationsAsync(request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar denominaciones del cierre de caja");
            return StatusCode(500, Result<CashClosingDto>.Failure("Error interno al enviar denominaciones"));
        }
    }

    /// <summary>
    /// POST /api/v1/cash-closings/approve
    /// El gerente aprueba o rechaza el cierre
    /// </summary>
    [HttpPost("approve")]
    public async Task<ActionResult<Result<CashClosingDto>>> Approve(
        [FromBody] ApproveCashClosingRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.ApproveAsync(request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al aprobar/rechazar cierre de caja");
            return StatusCode(500, Result<CashClosingDto>.Failure("Error interno al procesar aprobacion"));
        }
    }

    /// <summary>
    /// GET /api/v1/cash-closings?cashRegisterId=...&from=...&to=...
    /// Obtiene el historial de cierres
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<Result<IEnumerable<CashClosingSummaryDto>>>> GetHistory(
        [FromQuery] Guid? cashRegisterId = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.GetHistoryAsync(cashRegisterId, from, to, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// GET /api/v1/cash-closings/{id}
    /// Obtiene un cierre por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Result<CashClosingDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// GET /api/v1/cash-closings/{id}/report-z
    /// Genera el Reporte Z en PDF y retorna la URL
    /// </summary>
    [HttpGet("{id:guid}/report-z")]
    public async Task<ActionResult<Result<string>>> GenerateZReport(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GenerateZReportAsync(id, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar Reporte Z");
            return StatusCode(500, Result<string>.Failure("Error interno al generar Reporte Z"));
        }
    }
}
