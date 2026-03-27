namespace Restify.BackOffice.Application.Interfaces;

/// <summary>
/// Cliente para envio de notificaciones externas (SMS, etc.)
/// </summary>
public interface INotificationClient
{
    /// <summary>
    /// Envia un SMS al numero indicado
    /// </summary>
    /// <param name="to">Numero de telefono destino (ej: +593...)</param>
    /// <param name="message">Contenido del mensaje</param>
    /// <param name="ct">Token de cancelacion</param>
    Task SendSmsAsync(string to, string message, CancellationToken ct = default);
}
