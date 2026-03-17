using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IQRCodeService
{
    Task<Result<QRCodeDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<QRCodeDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<QRCodeDto>> GenerateAsync(GenerateQRCodeRequest request, CancellationToken cancellationToken = default);
    Task<Result<QRCodeResolveResponse>> ResolveAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<byte[]>> DownloadQRImageAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<bool>> ToggleActiveAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
