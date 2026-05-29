using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Configuracion de webhook outbound para notificaciones externas
/// </summary>
public class WebhookConfig : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Secret { get; set; }
    public List<string> Events { get; set; } = new List<string>();
    public bool IsActive { get; set; } = true;
    public int? TimeoutMs { get; set; } = 5000;
    public string? LastPingAt { get; set; }
    public string? LastPingStatus { get; set; }

    public ICollection<WebhookDelivery> Deliveries { get; set; } = new List<WebhookDelivery>();
}

/// <summary>
/// Registro de entrega de un webhook
/// </summary>
public class WebhookDelivery : TenantEntity
{
    public Guid WebhookConfigId { get; set; }
    public WebhookConfig WebhookConfig { get; set; } = null!;

    public string EventType { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;
    public int HttpStatusCode { get; set; }
    public string? ResponseBody { get; set; }
    public bool Success { get; set; }
    public DateTime DeliveredAt { get; set; } = DateTime.UtcNow;
    public int AttemptNumber { get; set; } = 1;
}
