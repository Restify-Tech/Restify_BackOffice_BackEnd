namespace Restify.Auth.Application.Interfaces;

/// <summary>
/// Servicio para almacenamiento de archivos
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Guarda un archivo y retorna la URL relativa
    /// </summary>
    /// <param name="stream">Contenido del archivo</param>
    /// <param name="fileName">Nombre original del archivo</param>
    /// <param name="folder">Subcarpeta de destino (ej: "tenant-logos")</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>URL relativa del archivo guardado</returns>
    Task<string> SaveFileAsync(Stream stream, string fileName, string folder, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina un archivo por su URL relativa
    /// </summary>
    Task DeleteFileAsync(string relativeUrl, CancellationToken cancellationToken = default);
}
