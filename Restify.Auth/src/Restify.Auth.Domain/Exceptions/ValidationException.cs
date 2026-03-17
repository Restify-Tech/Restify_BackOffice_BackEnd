namespace Restify.Auth.Domain.Exceptions;

/// <summary>
/// Excepción para errores de validación
/// </summary>
public class ValidationException : DomainException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("Se encontraron uno o más errores de validación.", "VALIDATION_ERROR")
    {
        Errors = errors;
    }

    public ValidationException(string propertyName, string errorMessage)
        : base($"Error de validación en {propertyName}: {errorMessage}", "VALIDATION_ERROR")
    {
        Errors = new Dictionary<string, string[]>
        {
            { propertyName, new[] { errorMessage } }
        };
    }
}
