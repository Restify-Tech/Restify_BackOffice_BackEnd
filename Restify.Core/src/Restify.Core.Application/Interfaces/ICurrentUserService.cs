namespace Restify.Core.Application.Interfaces;

/// <summary>
/// Servicio para obtener información del usuario actual
/// </summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? TenantId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    bool IsSuperAdmin { get; }
    string? UserType { get; }
}
