using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IElectronicInvoiceService
{
    Task<Result<ElectronicDocumentDto>> EmitElectronicInvoiceAsync(Guid invoiceId, CancellationToken ct = default);
    Task<Result<ElectronicDocumentDto>> EmitCreditNoteAsync(CreateCreditNoteRequest request, CancellationToken ct = default);
    Task<Result<ElectronicDocumentDto>> EmitWithholdingVoucherAsync(CreateWithholdingRequest request, CancellationToken ct = default);
    Task<Result<ElectronicDocumentDto>> CheckAuthorizationAsync(Guid documentId, CancellationToken ct = default);
    Task<Result<ElectronicDocumentDto>> ResendDocumentAsync(Guid documentId, CancellationToken ct = default);
    Task<Result<byte[]>> GetRidePdfAsync(Guid documentId, CancellationToken ct = default);
    Task<Result<IEnumerable<ElectronicDocumentSummaryDto>>> GetAllAsync(ElectronicDocumentFilter filter, CancellationToken ct = default);
    Task<Result<ElectronicDocumentDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
}
