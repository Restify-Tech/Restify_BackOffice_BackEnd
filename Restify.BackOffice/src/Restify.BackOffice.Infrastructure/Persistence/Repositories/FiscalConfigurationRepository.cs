using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class FiscalConfigurationRepository : IFiscalConfigurationRepository
{
    private readonly BackOfficeDbContext _context;

    public FiscalConfigurationRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<FiscalConfiguration?> GetByTenantIdAsync(Guid tenantId, CancellationToken ct = default)
    {
        return await _context.FiscalConfigurations
            .FirstOrDefaultAsync(fc => fc.TenantId == tenantId, ct);
    }

    public async Task<FiscalConfiguration> CreateAsync(FiscalConfiguration config, CancellationToken ct = default)
    {
        _context.FiscalConfigurations.Add(config);
        await _context.SaveChangesAsync(ct);
        return config;
    }

    public async Task UpdateAsync(FiscalConfiguration config, CancellationToken ct = default)
    {
        _context.FiscalConfigurations.Update(config);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<int> GetAndIncrementSequentialAsync(Guid tenantId, SriDocumentType documentType, CancellationToken ct = default)
    {
        // Usar transaccion implicita con optimistic concurrency
        var config = await _context.FiscalConfigurations
            .FirstOrDefaultAsync(fc => fc.TenantId == tenantId, ct)
            ?? throw new InvalidOperationException("Configuración fiscal no encontrada para el tenant");

        int currentSequential;

        switch (documentType)
        {
            case SriDocumentType.Invoice:
                currentSequential = config.NextInvoiceSequential;
                config.NextInvoiceSequential++;
                break;
            case SriDocumentType.CreditNote:
                currentSequential = config.NextCreditNoteSequential;
                config.NextCreditNoteSequential++;
                break;
            case SriDocumentType.WithholdingVoucher:
                currentSequential = config.NextWithholdingSequential;
                config.NextWithholdingSequential++;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(documentType));
        }

        await _context.SaveChangesAsync(ct);

        return currentSequential;
    }
}
