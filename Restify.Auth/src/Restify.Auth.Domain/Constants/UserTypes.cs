namespace Restify.Auth.Domain.Constants;

/// <summary>
/// Tipos de usuario del sistema (valores del claim user_type)
/// </summary>
public static class UserTypes
{
    public const string SuperAdmin = "superadmin";
    public const string Staff = "staff";
    public const string Customer = "customer";
    public const string Driver = "driver";
    public const string PoolDriver = "pool_driver";
}
