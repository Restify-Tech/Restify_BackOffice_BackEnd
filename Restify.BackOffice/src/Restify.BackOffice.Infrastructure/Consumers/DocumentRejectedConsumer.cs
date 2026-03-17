using System.Text.Json;
using MassTransit;
using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.Events;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Infrastructure.Consumers;

/// <summary>
/// Consume evento de documento rechazado por el SRI desde FElectonica.
/// Actualiza el ElectronicDocument local con el motivo de rechazo y errores.
/// </summary>
public class DocumentRejectedConsumer : IConsumer<DocumentRejectedEvent>
{
    private readonly IElectronicDocumentRepository _repository;
    private readonly ILogger<DocumentRejectedConsumer> _logger;

    public DocumentRejectedConsumer(
        IElectronicDocumentRepository repository,
        ILogger<DocumentRejectedConsumer> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DocumentRejectedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation(
            "Recibido DocumentRejectedEvent: AccessKey={AccessKey}, Reason={Reason}",
            message.AccessKey, message.RejectionReason);

        var document = await _repository.GetByAccessKeyAsync(message.AccessKey, context.CancellationToken);

        if (document is null)
        {
            _logger.LogWarning(
                "ElectronicDocument no encontrado para AccessKey={AccessKey}. Evento ignorado.",
                message.AccessKey);
            return;
        }

        document.Status = ElectronicDocumentStatus.Rejected;
        document.SriResponse = message.RejectionReason;
        document.SriErrors = JsonSerializer.Serialize(message.Errors);
        document.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(document, context.CancellationToken);

        _logger.LogInformation(
            "ElectronicDocument {DocumentId} actualizado a Rejected. Reason={Reason}, Errors={ErrorCount}",
            document.Id, message.RejectionReason, message.Errors.Length);
    }
}
