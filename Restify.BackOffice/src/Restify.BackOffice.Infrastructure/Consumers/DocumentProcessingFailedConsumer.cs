using MassTransit;
using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.Events;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Infrastructure.Consumers;

/// <summary>
/// Consume evento de fallo en procesamiento desde FElectonica.
/// Actualiza el ElectronicDocument local con el error y la etapa donde fallo.
/// </summary>
public class DocumentProcessingFailedConsumer : IConsumer<DocumentProcessingFailedEvent>
{
    private readonly IElectronicDocumentRepository _repository;
    private readonly ILogger<DocumentProcessingFailedConsumer> _logger;

    public DocumentProcessingFailedConsumer(
        IElectronicDocumentRepository repository,
        ILogger<DocumentProcessingFailedConsumer> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DocumentProcessingFailedEvent> context)
    {
        var message = context.Message;
        _logger.LogError(
            "Recibido DocumentProcessingFailedEvent: AccessKey={AccessKey}, Stage={Stage}, Error={Error}",
            message.AccessKey, message.Stage, message.ErrorMessage);

        var document = await _repository.GetByAccessKeyAsync(message.AccessKey, context.CancellationToken);

        if (document is null)
        {
            _logger.LogWarning(
                "ElectronicDocument no encontrado para AccessKey={AccessKey}. Evento ignorado.",
                message.AccessKey);
            return;
        }

        document.Status = ElectronicDocumentStatus.Error;
        document.SriErrors = $"[Stage: {message.Stage}] {message.ErrorMessage}";
        document.SendAttempts++;
        document.LastSendAttempt = DateTime.UtcNow;
        document.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(document, context.CancellationToken);

        _logger.LogInformation(
            "ElectronicDocument {DocumentId} actualizado a Error. Stage={Stage}, Attempts={Attempts}",
            document.Id, message.Stage, document.SendAttempts);
    }
}
