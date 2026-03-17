using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Mappings;

public static class EmployeeMappingExtensions
{
    public static EmployeeDto ToDto(this Employee entity)
    {
        return new EmployeeDto
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            FullName = entity.FirstName + " " + entity.LastName,
            IdentificationNumber = entity.IdentificationNumber,
            Email = entity.Email,
            Phone = entity.Phone,
            Address = entity.Address,
            Position = entity.Position,
            Department = entity.Department,
            HireDate = entity.HireDate,
            TerminationDate = entity.TerminationDate,
            BaseSalary = entity.BaseSalary,
            EmploymentType = entity.EmploymentType,
            Status = entity.Status,
            BankName = entity.BankName,
            BankAccountNumber = entity.BankAccountNumber,
            SocialSecurityNumber = entity.SocialSecurityNumber,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static Employee ToEntity(this CreateEmployeeRequest request, Guid tenantId)
    {
        return new Employee
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            IdentificationNumber = request.IdentificationNumber,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            Position = request.Position,
            Department = request.Department,
            HireDate = request.HireDate,
            BaseSalary = request.BaseSalary,
            EmploymentType = request.EmploymentType,
            BankName = request.BankName,
            BankAccountNumber = request.BankAccountNumber,
            SocialSecurityNumber = request.SocialSecurityNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateFrom(this Employee entity, UpdateEmployeeRequest request)
    {
        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.Email = request.Email;
        entity.Phone = request.Phone;
        entity.Address = request.Address;
        entity.Position = request.Position;
        entity.Department = request.Department;
        entity.BaseSalary = request.BaseSalary;
        entity.EmploymentType = request.EmploymentType;
        entity.Status = request.Status;
        entity.BankName = request.BankName;
        entity.BankAccountNumber = request.BankAccountNumber;
        entity.SocialSecurityNumber = request.SocialSecurityNumber;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
    }
}
