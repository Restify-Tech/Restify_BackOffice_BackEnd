using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Cliente registrado del restaurante (para pedidos QR/web)
/// </summary>
public class Customer : TenantEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? IdentificationNumber { get; set; }
    public IdentificationType? IdentificationType { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool EmailVerified { get; set; } = false;
    public DateTime? LastLoginAt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }

    // Navegación
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    // Propiedades calculadas
    public string FullName => $"{FirstName} {LastName}".Trim();
}

/// <summary>
/// Tipo de identificación del cliente
/// </summary>
public enum IdentificationType
{
    Cedula = 1,
    Ruc = 2,
    Pasaporte = 3
}
