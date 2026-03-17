namespace Restify.Auth.Domain.Exceptions;

/// <summary>
/// Excepción para errores de autenticación/autorización
/// </summary>
public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message = "No autorizado")
        : base(message, "UNAUTHORIZED")
    {
    }
}
