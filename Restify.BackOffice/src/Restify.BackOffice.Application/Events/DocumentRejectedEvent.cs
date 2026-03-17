namespace Restify.BackOffice.Application.Events;

public record DocumentRejectedEvent
{
    public string DocumentId { get; init; } = string.Empty;
    public string TenantId { get; init; } = string.Empty;
    public string AccessKey { get; init; } = string.Empty;
    public string RejectionReason { get; init; } = string.Empty;
    public string[] Errors { get; init; } = [];
    public string DocumentType { get; init; } = string.Empty;
    public string SourceDocumentId { get; init; } = string.Empty;
}
