namespace Restify.Auth.Infrastructure.Services;

/// <summary>
/// Servicio para manejo de contraseñas
/// </summary>
public class PasswordService
{
    private const int WorkFactor = 12;

    /// <summary>
    /// Genera el hash de una contraseña
    /// </summary>
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    /// <summary>
    /// Verifica si una contraseña coincide con su hash
    /// </summary>
    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
