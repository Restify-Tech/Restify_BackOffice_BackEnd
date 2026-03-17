using Microsoft.Extensions.Logging;
using Restify.Auth.Application.Interfaces;

namespace Restify.Auth.Infrastructure.Services;

/// <summary>
/// Implementación local de almacenamiento de archivos.
/// Guarda archivos en el directorio wwwroot/uploads/ del servidor.
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly ILogger<LocalFileStorageService> _logger;
    private readonly string _basePath;

    public LocalFileStorageService(ILogger<LocalFileStorageService> logger)
    {
        _logger = logger;
        _basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
    }

    public async Task<string> SaveFileAsync(
        Stream stream,
        string fileName,
        string folder,
        CancellationToken cancellationToken = default)
    {
        // Crear directorio si no existe
        var folderPath = Path.Combine(_basePath, folder);
        Directory.CreateDirectory(folderPath);

        // Generar nombre único para evitar colisiones
        var extension = Path.GetExtension(fileName);
        var uniqueName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(folderPath, uniqueName);

        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await stream.CopyToAsync(fileStream, cancellationToken);

        var relativeUrl = $"/uploads/{folder}/{uniqueName}";

        _logger.LogInformation("Archivo guardado: {Path}", relativeUrl);

        return relativeUrl;
    }

    public Task DeleteFileAsync(string relativeUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(relativeUrl))
            return Task.CompletedTask;

        // Convertir URL relativa a path del sistema
        var filePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            relativeUrl.TrimStart('/'));

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            _logger.LogInformation("Archivo eliminado: {Path}", relativeUrl);
        }

        return Task.CompletedTask;
    }
}
