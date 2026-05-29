using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/ai")]
[Authorize]
public class AIImageController : ControllerBase
{
    private readonly IAIImageService _aiImageService;
    private readonly IAIImagePromptTemplateService _templateService;
    private readonly ILogger<AIImageController> _logger;

    public AIImageController(
        IAIImageService aiImageService,
        IAIImagePromptTemplateService templateService,
        ILogger<AIImageController> logger)
    {
        _aiImageService = aiImageService;
        _templateService = templateService;
        _logger = logger;
    }

    /// <summary>
    /// Genera una imagen con IA para un producto
    /// </summary>
    [HttpPost("generate-image")]
    public async Task<IActionResult> GenerateImage([FromBody] GenerateImageRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _aiImageService.GenerateImageAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar imagen para producto {ProductId}", request.ProductId);
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Genera una imagen para un producto específico (endpoint de conveniencia)
    /// </summary>
    [HttpPost("products/{productId}/generate-image")]
    public async Task<IActionResult> GenerateImageForProduct(Guid productId, [FromBody] GenerateImageForProductRequest? request, CancellationToken cancellationToken)
    {
        try
        {
            var generateRequest = new GenerateImageRequest
            {
                ProductId = productId,
                PromptTemplateId = request?.PromptTemplateId,
                CustomPrompt = request?.CustomPrompt,
                ReferenceImageUrl = request?.ReferenceImageUrl
            };

            var result = await _aiImageService.GenerateImageAsync(generateRequest, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar imagen para producto {ProductId}", productId);
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene una generación por ID
    /// </summary>
    [HttpGet("generations/{id}")]
    public async Task<IActionResult> GetGeneration(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _aiImageService.GetGenerationByIdAsync(id, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener generación {GenerationId}", id);
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene las generaciones de un producto
    /// </summary>
    [HttpGet("products/{productId}/generations")]
    public async Task<IActionResult> GetGenerationsByProduct(Guid productId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _aiImageService.GetGenerationsByProductIdAsync(productId, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener generaciones del producto {ProductId}", productId);
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    // =====================================================
    // Prompt Templates CRUD
    // =====================================================

    /// <summary>
    /// Lista todas las plantillas de prompt
    /// </summary>
    [HttpGet("templates")]
    public async Task<IActionResult> GetAllTemplates(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _templateService.GetAllAsync(cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener plantillas de prompt");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene una plantilla por ID
    /// </summary>
    [HttpGet("templates/{id}")]
    public async Task<IActionResult> GetTemplate(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _templateService.GetByIdAsync(id, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener plantilla {TemplateId}", id);
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Crea una nueva plantilla de prompt
    /// </summary>
    [HttpPost("templates")]
    public async Task<IActionResult> CreateTemplate([FromBody] CreateAIImagePromptTemplateRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _templateService.CreateAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return CreatedAtAction(nameof(GetTemplate), new { id = result.Data!.Id }, result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear plantilla de prompt");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza una plantilla de prompt
    /// </summary>
    [HttpPut("templates/{id}")]
    public async Task<IActionResult> UpdateTemplate(Guid id, [FromBody] UpdateAIImagePromptTemplateRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _templateService.UpdateAsync(id, request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar plantilla {TemplateId}", id);
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Elimina una plantilla de prompt
    /// </summary>
    [HttpDelete("templates/{id}")]
    public async Task<IActionResult> DeleteTemplate(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _templateService.DeleteAsync(id, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(new { error = result.Error });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar plantilla {TemplateId}", id);
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
}

/// <summary>
/// Request para generar imagen desde el endpoint de conveniencia (sin ProductId en body)
/// </summary>
public class GenerateImageForProductRequest
{
    public Guid? PromptTemplateId { get; set; }
    public string? CustomPrompt { get; set; }
    public string? ReferenceImageUrl { get; set; }
}
