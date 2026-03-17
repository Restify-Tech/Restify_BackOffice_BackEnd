using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.DTOs;

/// <summary>
/// DTO de empleado
/// </summary>
public class EmployeeDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string IdentificationNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Position { get; set; } = string.Empty;
    public string? Department { get; set; }
    public DateTime HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public decimal BaseSalary { get; set; }
    public EmploymentType EmploymentType { get; set; }
    public EmployeeStatus Status { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? SocialSecurityNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Request para crear empleado
/// </summary>
public class CreateEmployeeRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string IdentificationNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Position { get; set; } = string.Empty;
    public string? Department { get; set; }
    public DateTime HireDate { get; set; }
    public decimal BaseSalary { get; set; }
    public EmploymentType EmploymentType { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? SocialSecurityNumber { get; set; }
}

/// <summary>
/// Request para actualizar empleado
/// </summary>
public class UpdateEmployeeRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Position { get; set; } = string.Empty;
    public string? Department { get; set; }
    public decimal BaseSalary { get; set; }
    public EmploymentType EmploymentType { get; set; }
    public EmployeeStatus Status { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? SocialSecurityNumber { get; set; }
    public bool IsActive { get; set; }
}
