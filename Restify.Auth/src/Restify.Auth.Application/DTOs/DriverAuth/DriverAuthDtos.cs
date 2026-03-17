namespace Restify.Auth.Application.DTOs.DriverAuth;

public record DriverRegisterRequest(
    string FirstName, string LastName,
    string Email, string Password,
    string Phone, string IdentificationNumber,
    string? VehiclePlate, string? VehicleDescription,
    string TenantSlug);

public record DriverLoginRequest(string Email, string Password, string TenantSlug);

public record DriverLoginResponse(
    string AccessToken, string RefreshToken,
    DateTime ExpiresAt, DriverInfoDto Driver);

public record DriverInfoDto(
    Guid Id, string Email, string FirstName, string LastName,
    string Phone, Guid TenantId, string TenantName,
    string? VehiclePlate, bool IsVerified);

public record DriverRefreshTokenRequest(string AccessToken, string RefreshToken);
