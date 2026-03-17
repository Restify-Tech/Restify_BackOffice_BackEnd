using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.Interfaces;

public interface IFiscalConfigurationRepository
{
    Task<FiscalConfiguration?> GetByTenantIdAsync(Guid tenantId, CancellationToken ct = default);
    Task<FiscalConfiguration> CreateAsync(FiscalConfiguration config, CancellationToken ct = default);
    Task UpdateAsync(FiscalConfiguration config, CancellationToken ct = default);
    Task<int> GetAndIncrementSequentialAsync(Guid tenantId, SriDocumentType documentType, CancellationToken ct = default);
}
