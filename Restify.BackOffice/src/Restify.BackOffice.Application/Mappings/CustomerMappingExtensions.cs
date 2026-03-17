using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Mappings;

public static class CustomerMappingExtensions
{
    public static CustomerDto ToDto(this Customer entity)
    {
        return new CustomerDto(
            Id: entity.Id,
            FirstName: entity.FirstName,
            LastName: entity.LastName,
            Email: entity.Email,
            Phone: entity.Phone,
            IdentificationNumber: entity.IdentificationNumber,
            IdentificationType: entity.IdentificationType?.ToString(),
            IsActive: entity.IsActive,
            EmailVerified: entity.EmailVerified,
            LastLoginAt: entity.LastLoginAt,
            OrderCount: entity.Orders?.Count ?? 0,
            CreatedAt: entity.CreatedAt
        );
    }

    public static CustomerListDto ToListDto(this Customer entity)
    {
        return new CustomerListDto(
            Id: entity.Id,
            FullName: entity.FullName,
            Email: entity.Email,
            Phone: entity.Phone,
            IsActive: entity.IsActive,
            LastLoginAt: entity.LastLoginAt,
            OrderCount: entity.Orders?.Count ?? 0
        );
    }

    public static Customer ToEntity(this CreateCustomerRequest request, string passwordHash)
    {
        return new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            IdentificationNumber = request.IdentificationNumber,
            IdentificationType = request.IdentificationType != null
                ? Enum.Parse<IdentificationType>(request.IdentificationType, ignoreCase: true)
                : null,
            PasswordHash = passwordHash,
            IsActive = true,
            EmailVerified = false
        };
    }

    public static void UpdateFrom(this Customer entity, UpdateCustomerRequest request)
    {
        if (request.FirstName != null)
            entity.FirstName = request.FirstName;

        if (request.LastName != null)
            entity.LastName = request.LastName;

        if (request.Phone != null)
            entity.Phone = request.Phone;

        if (request.IdentificationNumber != null)
            entity.IdentificationNumber = request.IdentificationNumber;

        if (request.IdentificationType != null)
            entity.IdentificationType = Enum.Parse<IdentificationType>(request.IdentificationType, ignoreCase: true);

        if (request.IsActive.HasValue)
            entity.IsActive = request.IsActive.Value;
    }
}
