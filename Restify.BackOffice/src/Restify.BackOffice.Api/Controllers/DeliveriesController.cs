using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DeliveriesController : ControllerBase
{
    private readonly IDeliveryService _service;
    private readonly IDeliveryAssignmentService _assignmentService;
    private readonly IPoolCommissionService _commissionService;
    private readonly ILogger<DeliveriesController> _logger;

    public DeliveriesController(
        IDeliveryService service,
        IDeliveryAssignmentService assignmentService,
        IPoolCommissionService commissionService,
        ILogger<DeliveriesController> logger)
    {
        _service = service;
        _assignmentService = assignmentService;
        _commissionService = commissionService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetAllAsync(cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener entregas");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetByIdAsync(id, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener entrega {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("order/{orderId:guid}")]
    public async Task<IActionResult> GetByOrderId(Guid orderId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetByOrderIdAsync(orderId, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener entrega del pedido {OrderId}", orderId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("driver/{driverId:guid}")]
    public async Task<IActionResult> GetByDriverId(Guid driverId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetByDriverIdAsync(driverId, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener entregas del motorizado {DriverId}", driverId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetPendingAsync(cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener entregas pendientes");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDeliveryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.CreateAsync(request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear entrega");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPost("{id:guid}/assign")]
    public async Task<IActionResult> Assign(Guid id, [FromBody] AssignDeliveryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.AssignAsync(id, request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asignar entrega {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateDeliveryStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.UpdateStatusAsync(id, request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar estado de entrega {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPatch("{id:guid}/location")]
    public async Task<IActionResult> UpdateLocation(Guid id, [FromBody] UpdateDriverLocationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.UpdateLocationAsync(id, request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar ubicación de entrega {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("{id:guid}/tracking")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTracking(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetTrackingAsync(id, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tracking de entrega {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Asignación automática de repartidor basada en el modo de operación del tenant.
    /// Standalone (1): solo propios. Networked (2): solo pool. Hybrid (3): propios primero, luego pool.
    /// </summary>
    [HttpPost("{id:guid}/auto-assign")]
    public async Task<IActionResult> AutoAssign(Guid id, [FromBody] AutoAssignDeliveryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _assignmentService.AssignDriverAsync(
                id, request.DeliveryOperationMode, request.DeliveryZoneId, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en asignación automática de entrega {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Actualizar ubicación GPS del repartidor (REST fallback para cuando no hay WebSocket).
    /// Actualiza tanto la entrega como el DeliveryDriver.
    /// </summary>
    [HttpPost("{id:guid}/location")]
    public async Task<IActionResult> UpdateGpsLocation(Guid id, [FromBody] UpdateLocationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var locationRequest = new UpdateDriverLocationRequest((decimal)request.Latitude, (decimal)request.Longitude);
            var result = await _service.UpdateLocationAsync(id, locationRequest, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar ubicación GPS de entrega {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crear comisión para una entrega pool (se llama cuando la entrega se completa).
    /// </summary>
    [HttpPost("{id:guid}/commission")]
    public async Task<IActionResult> CreateCommission(Guid id, [FromBody] CreatePoolCommissionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _commissionService.CreateCommissionAsync(id, request.CommissionPercentage, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(new
            {
                result.Data!.Id,
                result.Data.DeliveryId,
                result.Data.DriverId,
                result.Data.OrderAmount,
                result.Data.DeliveryFee,
                result.Data.CommissionPercentage,
                result.Data.CommissionAmount,
                Status = result.Data.Status.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear comisión para entrega {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
