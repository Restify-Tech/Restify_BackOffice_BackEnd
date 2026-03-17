namespace Restify.Core.Domain.Enums;

/// <summary>
/// Tipos de validación soportados para columnas
/// </summary>
public enum ValidationType
{
    Required = 0,
    MinLength = 1,
    MaxLength = 2,
    MinValue = 3,
    MaxValue = 4,
    Regex = 5,
    Email = 6,
    Phone = 7,
    Url = 8,
    Range = 9,
    Unique = 10,
    Custom = 11
}
