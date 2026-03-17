using Restify.BackOffice.Domain.Enums;
using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Empleado del restaurante
/// </summary>
public class Employee : TenantEntity
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
    public DateTime? TerminationDate { get; set; }
    public decimal BaseSalary { get; set; }
    public EmploymentType EmploymentType { get; set; }
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? SocialSecurityNumber { get; set; }
    public bool IsActive { get; set; } = true;

    // Navegacion
    public ICollection<PayrollEntry> PayrollEntries { get; set; } = new List<PayrollEntry>();
}
