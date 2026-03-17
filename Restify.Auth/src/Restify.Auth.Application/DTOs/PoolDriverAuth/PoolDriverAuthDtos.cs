namespace Restify.Auth.Application.DTOs.PoolDriverAuth;

public record PoolDriverRegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string Password,
    string IdentificationNumber,
    Guid DeliveryZoneId,
    int VehicleType,
    string? VehiclePlate,
    string? VehicleBrand,
    string? VehicleModel,
    int? VehicleYear,
    string? VehicleColor
);

public record PoolDriverLoginRequest(
    string Email,
    string Password
);

public record PoolDriverLoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    PoolDriverInfoDto Driver
);

public record PoolDriverInfoDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Phone,
    Guid? DeliveryZoneId,
    bool IsVerified,
    int VerificationStatus
);

public record PoolDriverRefreshTokenRequest(
    string AccessToken,
    string RefreshToken
);
