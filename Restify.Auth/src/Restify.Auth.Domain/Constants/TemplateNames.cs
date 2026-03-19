namespace Restify.Auth.Domain.Constants;

/// <summary>
/// Templates visuales disponibles para el menu QR
/// </summary>
public static class TemplateNames
{
    public const string Elegante = "elegante";
    public const string Moderno = "moderno";
    public const string Tropical = "tropical";
    public const string Oscuro = "oscuro";
    public const string Minimalista = "minimalista";

    public static readonly string[] All =
    [
        Elegante,
        Moderno,
        Tropical,
        Oscuro,
        Minimalista
    ];

    public static bool IsValid(string templateName)
        => All.Contains(templateName, StringComparer.OrdinalIgnoreCase);
}
