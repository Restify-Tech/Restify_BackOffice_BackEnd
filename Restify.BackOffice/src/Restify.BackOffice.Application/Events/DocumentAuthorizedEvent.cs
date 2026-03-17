namespace Restify.BackOffice.Application.Events;

public record DocumentAuthorizedEvent
{
    public string DocumentId { get; init; } = string.Empty;
    public string TenantId { get; init; } = string.Empty;
    public string AccessKey { get; init; } = string.Empty;
    public string AuthorizationNumber { get; init; } = string.Empty;
    public DateTime AuthorizationDate { get; init; }
    public string DocumentType { get; init; } = string.Empty;
    public string SourceDocumentId { get; init; } = string.Empty;
}
