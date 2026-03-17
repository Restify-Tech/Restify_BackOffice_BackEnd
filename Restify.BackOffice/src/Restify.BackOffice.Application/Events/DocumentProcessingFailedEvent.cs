namespace Restify.BackOffice.Application.Events;

public record DocumentProcessingFailedEvent
{
    public string DocumentId { get; init; } = string.Empty;
    public string TenantId { get; init; } = string.Empty;
    public string AccessKey { get; init; } = string.Empty;
    public string ErrorMessage { get; init; } = string.Empty;
    public string Stage { get; init; } = string.Empty;
    public string DocumentType { get; init; } = string.Empty;
    public string SourceDocumentId { get; init; } = string.Empty;
}
