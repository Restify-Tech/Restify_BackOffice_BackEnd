using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Notifications;

/// <summary>
/// Cliente HTTP real para el Notification Service.
/// Envia SMS via POST a {NotificationService:BaseUrl}/api/sms
/// con header X-Api-Key.
/// Timeout de 2000ms, fire-and-forget: si falla solo loguea warning.
/// </summary>
public class HttpNotificationClient : INotificationClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpNotificationClient> _logger;

    public HttpNotificationClient(HttpClient httpClient, ILogger<HttpNotificationClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task SendSmsAsync(string to, string message, CancellationToken ct = default)
    {
        try
        {
            var payload = new
            {
                to,
                message,
                channel = "sms"
            };

            var response = await _httpClient.PostAsJsonAsync("/api/sms", payload, ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Notification Service retorno {StatusCode} al enviar SMS a {To}",
                    response.StatusCode, to);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error al enviar SMS a {To} via Notification Service", to);
        }
    }
}
