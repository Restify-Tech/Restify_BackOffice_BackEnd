using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IFiscalConfigurationService
{
    Task<Result<FiscalConfigurationDto>> GetAsync(CancellationToken ct = default);
    Task<Result<FiscalConfigurationDto>> SaveAsync(SaveFiscalConfigurationRequest request, CancellationToken ct = default);
    Task<Result<FiscalConfigurationDto>> UploadCertificateAsync(Stream certificateStream, string password, CancellationToken ct = default);
    Task<Result<bool>> ValidateCertificateAsync(CancellationToken ct = default);
}
