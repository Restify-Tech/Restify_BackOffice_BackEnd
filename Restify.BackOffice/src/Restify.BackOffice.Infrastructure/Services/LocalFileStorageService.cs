using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;
    private readonly string _baseUrl;

    public LocalFileStorageService(string basePath = "uploads", string baseUrl = "/uploads")
    {
        _basePath = basePath;
        _baseUrl = baseUrl;
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder, CancellationToken cancellationToken = default)
    {
        var folderPath = Path.Combine(_basePath, folder);
        Directory.CreateDirectory(folderPath);

        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(folderPath, uniqueFileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(stream, cancellationToken);

        return Path.Combine(folder, uniqueFileName).Replace("\\", "/");
    }

    public Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, filePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public string GetFileUrl(string filePath) => $"{_baseUrl}/{filePath}";
}
