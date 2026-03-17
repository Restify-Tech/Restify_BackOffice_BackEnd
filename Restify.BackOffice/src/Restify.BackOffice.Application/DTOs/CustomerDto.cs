namespace Restify.BackOffice.Application.DTOs;

public record CustomerDto(
    Guid Id, string FirstName, string LastName, string Email, string? Phone,
    string? IdentificationNumber, string? IdentificationType,
    bool IsActive, bool EmailVerified, DateTime? LastLoginAt,
    int OrderCount, DateTime CreatedAt);

public record CustomerListDto(
    Guid Id, string FullName, string Email, string? Phone,
    bool IsActive, DateTime? LastLoginAt, int OrderCount);

public record CreateCustomerRequest(
    string FirstName, string LastName, string Email, string Password,
    string? Phone, string? IdentificationNumber, string? IdentificationType);

public record UpdateCustomerRequest(
    string? FirstName, string? LastName, string? Phone,
    string? IdentificationNumber, string? IdentificationType, bool? IsActive);
