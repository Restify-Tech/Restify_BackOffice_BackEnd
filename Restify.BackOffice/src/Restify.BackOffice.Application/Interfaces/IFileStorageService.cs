namespace Restify.BackOffice.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);
    string GetFileUrl(string filePath);
}
