using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.DTOs;

public record PoolDriverListDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string IdentificationNumber,
    VehicleType? VehicleType,
    string? VehiclePlate,
    DriverVerificationStatus VerificationStatus,
    decimal? Rating,
    int TotalDeliveries,
    bool IsActive,
    Guid? DeliveryZoneId,
    DateTime CreatedAt
);

public record PoolDriverDetailDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string IdentificationNumber,
    VehicleType? VehicleType,
    string? VehiclePlate,
    string? VehicleBrand,
    string? VehicleModel,
    int? VehicleYear,
    string? VehicleColor,
    string? PhotoUrl,
    DriverVerificationStatus VerificationStatus,
    DateTime? VerifiedAt,
    string? RejectionReason,
    decimal? Rating,
    int TotalDeliveries,
    bool IsActive,
    Guid? DeliveryZoneId,
    DateTime CreatedAt,
    IEnumerable<DriverDocumentDto> Documents
);

public record DriverDocumentDto(
    Guid Id,
    DocumentType DocumentType,
    string FileUrl,
    string FileName,
    DocumentStatus Status,
    DateTime? ReviewedAt,
    string? RejectionReason,
    DateTime UploadedAt
);

public record ApproveDriverRequest(
    string? Notes
);

public record RejectDriverRequest(
    string RejectionReason
);

public record ReviewDocumentRequest(
    DocumentStatus Status,
    string? RejectionReason
);
