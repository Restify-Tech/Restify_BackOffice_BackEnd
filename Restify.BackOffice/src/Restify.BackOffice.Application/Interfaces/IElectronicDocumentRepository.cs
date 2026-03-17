using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.Interfaces;

public interface IElectronicDocumentRepository
{
    Task<ElectronicDocument?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ElectronicDocument?> GetByAccessKeyAsync(string accessKey, CancellationToken ct = default);
    Task<ElectronicDocument?> GetByInvoiceIdAsync(Guid invoiceId, CancellationToken ct = default);
    Task<IReadOnlyList<ElectronicDocument>> GetPendingAuthorizationAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ElectronicDocument>> GetByFilterAsync(ElectronicDocumentFilter filter, CancellationToken ct = default);
    Task<ElectronicDocument> CreateAsync(ElectronicDocument document, CancellationToken ct = default);
    Task UpdateAsync(ElectronicDocument document, CancellationToken ct = default);
}
