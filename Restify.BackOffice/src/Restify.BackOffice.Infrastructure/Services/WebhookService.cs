using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class WebhookService : IWebhookService
{
    private readonly BackOfficeDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WebhookService> _logger;

    public WebhookService(
        BackOfficeDbContext context,
        ICurrentUserService currentUserService,
        IHttpClientFactory httpClientFactory,
        ILogger<WebhookService> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<WebhookConfigDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var webhooks = await _context.WebhookConfigs
            .OrderBy(w => w.CreatedAt)
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<WebhookConfigDto>>.Success(webhooks.Select(MapToDto));
    }

    public async Task<Result<WebhookConfigDto>> CreateAsync(CreateWebhookRequest request, CancellationToken cancellationToken = default)
    {
        var tenantId = _currentUserService.TenantId ?? Guid.Empty;

        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<WebhookConfigDto>.Failure("El nombre del webhook es requerido");

        if (string.IsNullOrWhiteSpace(request.Url))
            return Result<WebhookConfigDto>.Failure("La URL del webhook es requerida");

        var webhook = new WebhookConfig
        {
            TenantId = tenantId,
            Name = request.Name,
            Url = request.Url,
            Secret = request.Secret,
            Events = request.Events ?? new List<string>(),
            IsActive = true,
            TimeoutMs = request.TimeoutMs ?? 5000
        };

        _context.WebhookConfigs.Add(webhook);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Webhook creado: {Name} para tenant {TenantId}", webhook.Name, tenantId);

        return Result<WebhookConfigDto>.Success(MapToDto(webhook));
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var webhook = await _context.WebhookConfigs.FindAsync(new object[] { id }, cancellationToken);
        if (webhook == null)
            return Result<bool>.Failure("Webhook no encontrado");

        _context.WebhookConfigs.Remove(webhook);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ToggleActiveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var webhook = await _context.WebhookConfigs.FindAsync(new object[] { id }, cancellationToken);
        if (webhook == null)
            return Result<bool>.Failure("Webhook no encontrado");

        webhook.IsActive = !webhook.IsActive;
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(webhook.IsActive);
    }

    public async Task<Result<bool>> PingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var webhook = await _context.WebhookConfigs.FindAsync(new object[] { id }, cancellationToken);
        if (webhook == null)
            return Result<bool>.Failure("Webhook no encontrado");

        var payload = new { eventType = "ping", timestamp = DateTime.UtcNow, message = "Restify webhook test" };
        var (success, statusCode, responseBody) = await SendWebhookAsync(webhook, "ping", payload);

        webhook.LastPingAt = DateTime.UtcNow.ToString("O");
        webhook.LastPingStatus = success ? $"OK ({statusCode})" : $"Error ({statusCode})";
        await _context.SaveChangesAsync(cancellationToken);

        if (!success)
            return Result<bool>.Failure($"Ping fallido: HTTP {statusCode}");

        return Result<bool>.Success(true);
    }

    public async Task DispatchEventAsync(string tenantId, string eventType, object payload)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                if (!Guid.TryParse(tenantId, out var tenantGuid))
                    return;

                var webhooks = await _context.WebhookConfigs
                    .Where(w => w.TenantId == tenantGuid && w.IsActive && w.Events.Contains(eventType))
                    .ToListAsync();

                foreach (var webhook in webhooks)
                {
                    await DispatchToWebhookAsync(webhook, eventType, payload);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al despachar evento {EventType} para tenant {TenantId}", eventType, tenantId);
            }
        });

        await Task.CompletedTask;
    }

    public async Task<Result<IEnumerable<WebhookDeliveryDto>>> GetDeliveriesAsync(Guid webhookId, CancellationToken cancellationToken = default)
    {
        var deliveries = await _context.WebhookDeliveries
            .Where(d => d.WebhookConfigId == webhookId)
            .OrderByDescending(d => d.DeliveredAt)
            .Take(50)
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<WebhookDeliveryDto>>.Success(deliveries.Select(MapDeliveryToDto));
    }

    private async Task DispatchToWebhookAsync(WebhookConfig webhook, string eventType, object payload)
    {
        var (success, statusCode, responseBody) = await SendWebhookAsync(webhook, eventType, payload);

        var tenantId = webhook.TenantId;
        var payloadJson = JsonSerializer.Serialize(payload);

        var delivery = new WebhookDelivery
        {
            TenantId = tenantId,
            WebhookConfigId = webhook.Id,
            EventType = eventType,
            PayloadJson = payloadJson,
            HttpStatusCode = statusCode,
            ResponseBody = responseBody,
            Success = success,
            DeliveredAt = DateTime.UtcNow,
            AttemptNumber = 1
        };

        _context.WebhookDeliveries.Add(delivery);
        await _context.SaveChangesAsync();
    }

    private async Task<(bool Success, int StatusCode, string? ResponseBody)> SendWebhookAsync(WebhookConfig webhook, string eventType, object payload)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("Webhook");
            var payloadJson = JsonSerializer.Serialize(payload);

            var content = new StringContent(payloadJson, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, webhook.Url)
            {
                Content = content
            };

            request.Headers.Add("X-Restify-Event", eventType);
            request.Headers.Add("X-Restify-Timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());

            if (!string.IsNullOrWhiteSpace(webhook.Secret))
            {
                var signature = ComputeHmacSha256(payloadJson, webhook.Secret);
                request.Headers.Add("X-Restify-Signature", $"sha256={signature}");
            }

            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(webhook.TimeoutMs ?? 5000));
            var response = await client.SendAsync(request, cts.Token);

            var responseBody = await response.Content.ReadAsStringAsync();
            var success = response.IsSuccessStatusCode;

            _logger.LogInformation("Webhook {Name} dispatched event {EventType}: HTTP {StatusCode}", webhook.Name, eventType, (int)response.StatusCode);

            return (success, (int)response.StatusCode, responseBody);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar webhook {Name} para evento {EventType}", webhook.Name, eventType);
            return (false, 0, ex.Message);
        }
    }

    private static string ComputeHmacSha256(string payload, string secret)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);

        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(payloadBytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    private static WebhookConfigDto MapToDto(WebhookConfig w) => new(
        Id: w.Id,
        Name: w.Name,
        Url: w.Url,
        Events: w.Events,
        IsActive: w.IsActive,
        LastPingAt: w.LastPingAt,
        LastPingStatus: w.LastPingStatus
    );

    private static WebhookDeliveryDto MapDeliveryToDto(WebhookDelivery d) => new(
        Id: d.Id,
        EventType: d.EventType,
        HttpStatusCode: d.HttpStatusCode,
        Success: d.Success,
        DeliveredAt: d.DeliveredAt,
        AttemptNumber: d.AttemptNumber
    );
}
