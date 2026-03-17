namespace Restify.Auth.Domain.Constants;

/// <summary>
/// Nombres de claims JWT usados en todo el sistema
/// </summary>
public static class ClaimConstants
{
    public const string TenantId = "tenant_id";
    public const string UserType = "user_type";
    public const string IsSuperAdmin = "is_superadmin";
    public const string IsPoolDriver = "is_pool_driver";
    public const string Permission = "permission";
    public const string FirstName = "first_name";
    public const string LastName = "last_name";
    public const string DeliveryZoneId = "delivery_zone_id";
}
