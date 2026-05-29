namespace Restify.Auth.Domain.Constants;

/// <summary>
/// Constantes globales del servicio Auth. Valores configurables tambien disponibles via GeneralValues en BD.
/// </summary>
public static class AppConstants
{
    public static class Security
    {
        public const int BcryptWorkFactor = 12;
        public const int MaxFailedLoginAttempts = 5;
        public const int LockoutDurationMinutes = 30;
        public const int AccessTokenExpiryMinutes = 60;
        public const int RefreshTokenExpiryDays = 7;
        public const int MinJwtSecretLength = 32;
    }

    public static class Roles
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string TenantAdmin = "Administrador";
        public const string Manager = "Gestion";
        public const string Staff = "Mesero";
    }

    public static class Pagination
    {
        public const int DefaultSize = 20;
        public const int MaxSize = 100;
    }

    public static class Seeds
    {
        public const string DemoEmail = "admin@demo.com";
        public const string SuperAdminEmail = "superadmin@restosaas.com";
    }
}
