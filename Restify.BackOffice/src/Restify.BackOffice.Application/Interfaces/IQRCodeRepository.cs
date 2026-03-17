using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface IQRCodeRepository
{
    Task<QRCode?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<QRCode?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<QRCode>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<QRCode>> GetByTableIdAsync(Guid tableId, CancellationToken cancellationToken = default);
    Task<QRCode> CreateAsync(QRCode qrCode, CancellationToken cancellationToken = default);
    Task<QRCode> UpdateAsync(QRCode qrCode, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);
}
