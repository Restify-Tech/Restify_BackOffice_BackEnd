using Restify.Auth.Domain.Enums;

namespace Restify.Auth.Application.DTOs.Tenant;

/// <summary>
/// Request para registrar un nuevo restaurante (tenant)
/// </summary>
public record TenantRegisterRequest(
    IdentificationType IdentificationType,
    string IdentificationNumber,
    string BusinessName,
    string Slug,
    string AdminEmail,
    string AdminPassword,
    string Phone,
    string City
);

/// <summary>
/// Response del registro exitoso de un tenant
/// </summary>
public record TenantRegisterResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    Guid TenantId,
    Guid AdminUserId
);
