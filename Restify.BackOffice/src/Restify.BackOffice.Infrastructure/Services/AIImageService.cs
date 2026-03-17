using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Application.Mappings;
using Restify.BackOffice.Domain.Entities;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class AIImageService : IAIImageService
{
    private readonly IAIImageGenerationRepository _generationRepository;
    private readonly IAIImagePromptTemplateRepository _templateRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AIImageService> _logger;

    public AIImageService(
        IAIImageGenerationRepository generationRepository,
        IAIImagePromptTemplateRepository templateRepository,
        IProductRepository productRepository,
        ICurrentUserService currentUserService,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<AIImageService> logger)
    {
        _generationRepository = generationRepository;
        _templateRepository = templateRepository;
        _productRepository = productRepository;
        _currentUserService = currentUserService;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Result<AIImageGenerationDto>> GenerateImageAsync(GenerateImageRequest request, CancellationToken cancellationToken = default)
    {
        // Validate product exists
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
            return Result<AIImageGenerationDto>.Failure("Producto no encontrado");

        // Build the final prompt
        string finalPrompt;
        AIImagePromptTemplate? template = null;

        if (!string.IsNullOrWhiteSpace(request.CustomPrompt))
        {
            finalPrompt = request.CustomPrompt;
        }
        else if (request.PromptTemplateId.HasValue)
        {
            template = await _templateRepository.GetByIdAsync(request.PromptTemplateId.Value, cancellationToken);
            if (template == null)
                return Result<AIImageGenerationDto>.Failure("Plantilla de prompt no encontrada");

            finalPrompt = template.PromptTemplate
                .Replace("{product_name}", product.Name)
                .Replace("{product_description}", product.Description ?? product.Name);
        }
        else
        {
            // Use default template or fallback
            template = await _templateRepository.GetDefaultAsync(cancellationToken);
            if (template != null)
            {
                finalPrompt = template.PromptTemplate
                    .Replace("{product_name}", product.Name)
                    .Replace("{product_description}", product.Description ?? product.Name);
            }
            else
            {
                finalPrompt = $"A professional food photography shot of {product.Name}. " +
                    $"{product.Description ?? ""}. " +
                    "Beautiful plating, restaurant quality, natural lighting, appetizing presentation, " +
                    "high resolution, 4K quality, food styling.";
            }
        }

        // Create generation record
        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var generation = new AIImageGeneration
        {
            TenantId = tenantId,
            ProductId = request.ProductId,
            PromptTemplateId = template?.Id,
            FinalPrompt = finalPrompt,
            Status = AIImageGenerationStatus.Generating,
            ProviderUsed = "dall-e-3"
        };

        generation = await _generationRepository.CreateAsync(generation, cancellationToken);

        try
        {
            // Call DALL-E 3 API
            var apiKey = _configuration["OpenAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                generation.Status = AIImageGenerationStatus.Failed;
                generation.ErrorMessage = "API key de OpenAI no configurada";
                await _generationRepository.UpdateAsync(generation, cancellationToken);
                return Result<AIImageGenerationDto>.Failure("API key de OpenAI no configurada. Configure 'OpenAI:ApiKey' en appsettings.");
            }

            var client = _httpClientFactory.CreateClient("OpenAI");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            var dalleRequest = new
            {
                model = "dall-e-3",
                prompt = finalPrompt,
                n = 1,
                size = "1024x1024",
                quality = "standard",
                response_format = "url"
            };

            var response = await client.PostAsJsonAsync("https://api.openai.com/v1/images/generations", dalleRequest, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("DALL-E API error: {StatusCode} - {Error}", response.StatusCode, errorContent);

                generation.Status = AIImageGenerationStatus.Failed;
                generation.ErrorMessage = $"Error de API: {response.StatusCode}";
                await _generationRepository.UpdateAsync(generation, cancellationToken);

                return Result<AIImageGenerationDto>.Failure($"Error al generar imagen: {response.StatusCode}");
            }

            var dalleResponse = await response.Content.ReadFromJsonAsync<DallEResponse>(cancellationToken: cancellationToken);
            var imageUrl = dalleResponse?.Data?.FirstOrDefault()?.Url;

            if (string.IsNullOrEmpty(imageUrl))
            {
                generation.Status = AIImageGenerationStatus.Failed;
                generation.ErrorMessage = "No se recibió URL de imagen en la respuesta";
                await _generationRepository.UpdateAsync(generation, cancellationToken);

                return Result<AIImageGenerationDto>.Failure("No se recibió imagen de la API");
            }

            // Update generation record
            generation.GeneratedImageUrl = imageUrl;
            generation.Status = AIImageGenerationStatus.Completed;
            generation.CostUsd = 0.04m; // Standard DALL-E 3 1024x1024 price
            await _generationRepository.UpdateAsync(generation, cancellationToken);

            // Update product image
            product.ImageUrl = imageUrl;
            await _productRepository.UpdateAsync(product, cancellationToken);

            _logger.LogInformation("Image generated successfully for product {ProductId}, generation {GenerationId}", product.Id, generation.Id);

            // Reload with navigation properties
            generation = await _generationRepository.GetByIdAsync(generation.Id, cancellationToken);

            return Result<AIImageGenerationDto>.Success(generation!.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating image for product {ProductId}", request.ProductId);

            generation.Status = AIImageGenerationStatus.Failed;
            generation.ErrorMessage = ex.Message;
            await _generationRepository.UpdateAsync(generation, cancellationToken);

            return Result<AIImageGenerationDto>.Failure($"Error al generar imagen: {ex.Message}");
        }
    }

    public async Task<Result<AIImageGenerationDto>> GetGenerationByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var generation = await _generationRepository.GetByIdAsync(id, cancellationToken);
        if (generation == null)
            return Result<AIImageGenerationDto>.Failure("Generación no encontrada");

        return Result<AIImageGenerationDto>.Success(generation.ToDto());
    }

    public async Task<Result<IEnumerable<AIImageGenerationDto>>> GetGenerationsByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null)
            return Result<IEnumerable<AIImageGenerationDto>>.Failure("Producto no encontrado");

        var generations = await _generationRepository.GetByProductIdAsync(productId, cancellationToken);
        var dtos = generations.Select(g => g.ToDto());

        return Result<IEnumerable<AIImageGenerationDto>>.Success(dtos);
    }
}

// Internal DTOs for DALL-E API response
internal class DallEResponse
{
    [JsonPropertyName("data")]
    public List<DallEImageData>? Data { get; set; }
}

internal class DallEImageData
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("revised_prompt")]
    public string? RevisedPrompt { get; set; }
}
