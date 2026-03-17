namespace Restify.BackOffice.Application.DTOs;

public record QRCodeDto(
    Guid Id,
    Guid? TableId,
    string? TableNumber,
    string Code,
    string Url,
    string Type,
    bool IsActive,
    DateTime? LastScannedAt,
    int ScanCount,
    DateTime CreatedAt);

public record GenerateQRCodeRequest(
    Guid? TableId,
    int Type,
    string? BaseUrl);

public record QRCodeResolveResponse(
    string RestaurantSlug,
    Guid TenantId,
    Guid? TableId,
    string? TableNumber,
    string QRCodeType);
