using MassTransit;
using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.Events;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Infrastructure.Consumers;

/// <summary>
/// Consume evento de documento autorizado por el SRI desde FElectonica.
/// Actualiza el ElectronicDocument local con el numero y fecha de autorizacion.
/// </summary>
public class DocumentAuthorizedConsumer : IConsumer<DocumentAuthorizedEvent>
{
    private readonly IElectronicDocumentRepository _repository;
    private readonly ILogger<DocumentAuthorizedConsumer> _logger;

    public DocumentAuthorizedConsumer(
        IElectronicDocumentRepository repository,
        ILogger<DocumentAuthorizedConsumer> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DocumentAuthorizedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation(
            "Recibido DocumentAuthorizedEvent: AccessKey={AccessKey}, AuthorizationNumber={AuthorizationNumber}",
            message.AccessKey, message.AuthorizationNumber);

        var document = await _repository.GetByAccessKeyAsync(message.AccessKey, context.CancellationToken);

        if (document is null)
        {
            _logger.LogWarning(
                "ElectronicDocument no encontrado para AccessKey={AccessKey}. Evento ignorado.",
                message.AccessKey);
            return;
        }

        document.Status = ElectronicDocumentStatus.Authorized;
        document.AuthorizationCode = message.AuthorizationNumber;
        document.AuthorizationDate = message.AuthorizationDate;
        document.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(document, context.CancellationToken);

        _logger.LogInformation(
            "ElectronicDocument {DocumentId} actualizado a Authorized. AuthorizationNumber={AuthorizationNumber}",
            document.Id, message.AuthorizationNumber);
    }
}
