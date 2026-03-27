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

    private const string VisualCreativeClientName = "VisualCreative";
    private const string ConfigKeyBaseUrl = "VisualCreative:BaseUrl";
    private const string ConfigKeyTier = "VisualCreative:DefaultTier";
    private const string ConfigKeyCountry = "VisualCreative:DefaultCountry";
    private const string ConfigKeyCuisine = "VisualCreative:DefaultCuisine";
    private const string DefaultBaseUrl = "http://localhost:5600";
    private const string GenerateEndpoint = "/api/visual/generate";

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
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
            return Result<AIImageGenerationDto>.Failure("Producto no encontrado");

        var finalPrompt = await BuildPromptAsync(request, product, cancellationToken);

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var generation = new AIImageGeneration
        {
            TenantId = tenantId,
            ProductId = request.ProductId,
            PromptTemplateId = request.PromptTemplateId.HasValue
                ? (await _templateRepository.GetByIdAsync(request.PromptTemplateId.Value, cancellationToken))?.Id
                : null,
            FinalPrompt = finalPrompt,
            Status = AIImageGenerationStatus.Generating,
            ProviderUsed = "visual-creative"
        };

        generation = await _generationRepository.CreateAsync(generation, cancellationToken);

        try
        {
            var client = _httpClientFactory.CreateClient(VisualCreativeClientName);
            var tier = _configuration[ConfigKeyTier] ?? "Free";
            var country = _configuration[ConfigKeyCountry] ?? "EC";
            var cuisine = _configuration[ConfigKeyCuisine] ?? "Ecuatoriana";

            var visualRequest = new
            {
                rawPrompt = finalPrompt,
                context = "Restaurant",
                country,
                cuisine,
                tier,
                referenceImageUrl = request.ReferenceImageUrl
            };

            _logger.LogInformation("Enviando solicitud a VisualCreative para producto {ProductId}: {Prompt}",
                product.Id, finalPrompt);

            var response = await client.PostAsJsonAsync(GenerateEndpoint, visualRequest, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("VisualCreative API error: {StatusCode} - {Error}", response.StatusCode, errorContent);

                generation.Status = AIImageGenerationStatus.Failed;
                generation.ErrorMessage = $"Error de VisualCreative API: {response.StatusCode}";
                await _generationRepository.UpdateAsync(generation, cancellationToken);

                return Result<AIImageGenerationDto>.Failure($"Error al generar imagen: {response.StatusCode}");
            }

            var visualResponse = await response.Content.ReadFromJsonAsync<VisualCreativeResponse>(
                cancellationToken: cancellationToken);

            if (visualResponse == null || string.IsNullOrEmpty(visualResponse.ImageBase64))
            {
                generation.Status = AIImageGenerationStatus.Failed;
                generation.ErrorMessage = "No se recibio imagen en la respuesta de VisualCreative";
                await _generationRepository.UpdateAsync(generation, cancellationToken);

                return Result<AIImageGenerationDto>.Failure("No se recibio imagen de VisualCreative");
            }

            var imageDataUrl = $"data:{visualResponse.ContentType};base64,{visualResponse.ImageBase64}";

            generation.GeneratedImageUrl = imageDataUrl;
            generation.Status = AIImageGenerationStatus.Completed;
            generation.ProviderUsed = visualResponse.ProviderUsed ?? "visual-creative";
            generation.CostUsd = tier == "Premium" ? 0.04m : 0m;
            await _generationRepository.UpdateAsync(generation, cancellationToken);

            product.ImageUrl = imageDataUrl;
            await _productRepository.UpdateAsync(product, cancellationToken);

            _logger.LogInformation(
                "Imagen generada via VisualCreative para producto {ProductId} (prompt: {PromptProvider}, imagen: {ImageProvider})",
                product.Id, visualResponse.PromptProviderUsed, visualResponse.ProviderUsed);

            generation = await _generationRepository.GetByIdAsync(generation.Id, cancellationToken);

            return Result<AIImageGenerationDto>.Success(generation!.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generando imagen via VisualCreative para producto {ProductId}", request.ProductId);

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

    private async Task<string> BuildPromptAsync(GenerateImageRequest request, Product product, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(request.CustomPrompt))
            return request.CustomPrompt;

        AIImagePromptTemplate? template = null;

        if (request.PromptTemplateId.HasValue)
            template = await _templateRepository.GetByIdAsync(request.PromptTemplateId.Value, ct);

        template ??= await _templateRepository.GetDefaultAsync(ct);

        if (template != null)
        {
            return template.PromptTemplate
                .Replace("{product_name}", product.Name)
                .Replace("{product_description}", product.Description ?? product.Name);
        }

        return $"{product.Name}. {product.Description ?? ""}";
    }
}

internal class VisualCreativeResponse
{
    [JsonPropertyName("imageBase64")]
    public string? ImageBase64 { get; set; }

    [JsonPropertyName("contentType")]
    public string? ContentType { get; set; }

    [JsonPropertyName("providerUsed")]
    public string? ProviderUsed { get; set; }

    [JsonPropertyName("promptProviderUsed")]
    public string? PromptProviderUsed { get; set; }
}
