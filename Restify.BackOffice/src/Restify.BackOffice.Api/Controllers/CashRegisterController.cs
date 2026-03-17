using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/backoffice/[controller]")]
[Authorize]
public class CashRegisterController : ControllerBase
{
    private readonly ICashRegisterService _service;
    private readonly ILogger<CashRegisterController> _logger;

    public CashRegisterController(
        ICashRegisterService service,
        ILogger<CashRegisterController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // ===== CRUD de Cajas Registradoras =====

    /// <summary>
    /// GET /api/backoffice/cashregister
    /// Obtener todas las cajas registradoras
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<Result<List<CashRegisterDto>>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _service.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// GET /api/backoffice/cashregister/{id}
    /// Obtener caja por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Result<CashRegisterDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// POST /api/backoffice/cashregister
    /// Crear nueva caja registradora
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Result<CashRegisterDto>>> Create(
        [FromBody] CreateCashRegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    /// <summary>
    /// PUT /api/backoffice/cashregister/{id}
    /// Actualizar caja registradora
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Result<CashRegisterDto>>> Update(
        Guid id,
        [FromBody] UpdateCashRegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// DELETE /api/backoffice/cashregister/{id}
    /// Eliminar caja registradora
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Result<bool>>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    // ===== Operaciones de Sesión =====

    /// <summary>
    /// POST /api/backoffice/cashregister/open
    /// Abrir caja (iniciar sesión)
    /// </summary>
    [HttpPost("open")]
    public async Task<ActionResult<Result<CashRegisterSessionDto>>> OpenSession(
        [FromBody] OpenCashRegisterRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.OpenSessionAsync(request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al abrir caja");
            return StatusCode(500, Result<CashRegisterSessionDto>.Failure("Error al abrir caja"));
        }
    }

    /// <summary>
    /// POST /api/backoffice/cashregister/sessions/{sessionId}/close
    /// Cerrar caja (finalizar sesión)
    /// </summary>
    [HttpPost("sessions/{sessionId:guid}/close")]
    public async Task<ActionResult<Result<CashRegisterSessionDto>>> CloseSession(
        Guid sessionId,
        [FromBody] CloseCashRegisterRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.CloseSessionAsync(sessionId, request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cerrar caja");
            return StatusCode(500, Result<CashRegisterSessionDto>.Failure("Error al cerrar caja"));
        }
    }

    /// <summary>
    /// GET /api/backoffice/cashregister/{cashRegisterId}/active-session
    /// Obtener sesión activa de una caja
    /// </summary>
    [HttpGet("{cashRegisterId:guid}/active-session")]
    public async Task<ActionResult<Result<CashRegisterSessionDto>>> GetActiveSession(
        Guid cashRegisterId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetActiveSessionAsync(cashRegisterId, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// GET /api/backoffice/cashregister/sessions/{sessionId}
    /// Obtener sesión por ID
    /// </summary>
    [HttpGet("sessions/{sessionId:guid}")]
    public async Task<ActionResult<Result<CashRegisterSessionDto>>> GetSession(
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetSessionByIdAsync(sessionId, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// GET /api/backoffice/cashregister/sessions/history?cashRegisterId=...&from=...&to=...
    /// Obtener historial de sesiones
    /// </summary>
    [HttpGet("sessions/history")]
    public async Task<ActionResult<Result<List<CashRegisterSessionSummaryDto>>>> GetSessionHistory(
        [FromQuery] Guid? cashRegisterId = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.GetSessionHistoryAsync(cashRegisterId, from, to, cancellationToken);
        return Ok(result);
    }

    // ===== Movimientos =====

    /// <summary>
    /// POST /api/backoffice/cashregister/sessions/{sessionId}/movements
    /// Registrar movimiento en la sesión activa
    /// </summary>
    [HttpPost("sessions/{sessionId:guid}/movements")]
    public async Task<ActionResult<Result<CashRegisterMovementDto>>> RegisterMovement(
        Guid sessionId,
        [FromBody] RegisterCashMovementRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.RegisterMovementAsync(sessionId, request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar movimiento");
            return StatusCode(500, Result<CashRegisterMovementDto>.Failure("Error al registrar movimiento"));
        }
    }

    /// <summary>
    /// POST /api/backoffice/cashregister/sessions/{sessionId}/sale
    /// Registrar venta desde factura (uso interno)
    /// </summary>
    [HttpPost("sessions/{sessionId:guid}/sale")]
    public async Task<ActionResult<Result<CashRegisterMovementDto>>> RegisterSale(
        Guid sessionId,
        [FromQuery] Guid invoiceId,
        [FromQuery] decimal amount,
        CancellationToken cancellationToken)
    {
        var result = await _service.RegisterSaleFromInvoiceAsync(sessionId, invoiceId, amount, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// GET /api/backoffice/cashregister/sessions/{sessionId}/movements
    /// Obtener movimientos de una sesión
    /// </summary>
    [HttpGet("sessions/{sessionId:guid}/movements")]
    public async Task<ActionResult<Result<List<CashRegisterMovementDto>>>> GetSessionMovements(
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetSessionMovementsAsync(sessionId, cancellationToken);
        return Ok(result);
    }

    // ===== Estadísticas =====

    /// <summary>
    /// GET /api/backoffice/cashregister/statistics
    /// Obtener estadísticas generales de cajas
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<Result<CashRegisterStatisticsDto>>> GetStatistics(CancellationToken cancellationToken)
    {
        var result = await _service.GetStatisticsAsync(cancellationToken);
        return Ok(result);
    }
}
