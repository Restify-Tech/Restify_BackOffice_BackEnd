using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/v1/webhooks")]
[Authorize]
public class WebhooksController : ControllerBase
{
    private readonly IWebhookService _webhookService;
    private readonly ILogger<WebhooksController> _logger;

    public WebhooksController(IWebhookService webhookService, ILogger<WebhooksController> logger)
    {
        _webhookService = webhookService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los webhooks configurados
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _webhookService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener webhooks");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crea un nuevo webhook
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWebhookRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _webhookService.CreateAsync(request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetAll), result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear webhook");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Elimina un webhook
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _webhookService.DeleteAsync(id, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar webhook {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Activa o desactiva un webhook
    /// </summary>
    [HttpPut("{id:guid}/toggle")]
    public async Task<IActionResult> ToggleActive(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _webhookService.ToggleActiveAsync(id, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(new { isActive = result.Data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al activar/desactivar webhook {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Envia un payload de prueba al webhook
    /// </summary>
    [HttpPost("{id:guid}/ping")]
    public async Task<IActionResult> Ping(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _webhookService.PingAsync(id, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(new { message = "Ping enviado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al hacer ping al webhook {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene el historial de entregas del webhook
    /// </summary>
    [HttpGet("{id:guid}/deliveries")]
    public async Task<IActionResult> GetDeliveries(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _webhookService.GetDeliveriesAsync(id, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener entregas del webhook {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
