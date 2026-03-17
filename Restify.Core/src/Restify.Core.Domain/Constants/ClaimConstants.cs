namespace Restify.Core.Domain.Constants;

/// <summary>
/// Nombres de claims JWT que Core necesita leer
/// </summary>
public static class ClaimConstants
{
    public const string TenantId = "tenant_id";
    public const string IsSuperAdmin = "is_superadmin";
    public const string UserType = "user_type";
}
