using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.DTOs;

/// <summary>
/// DTO de cuenta contable
/// </summary>
public class AccountingAccountDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AccountType AccountType { get; set; }
    public Guid? ParentId { get; set; }
    public string? ParentName { get; set; }
    public int Level { get; set; }
    public bool AcceptsEntries { get; set; }
    public bool IsActive { get; set; }
    public List<AccountingAccountDto> Children { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Request para crear cuenta contable
/// </summary>
public class CreateAccountingAccountRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AccountType AccountType { get; set; }
    public Guid? ParentId { get; set; }
    public bool AcceptsEntries { get; set; }
}

/// <summary>
/// Request para actualizar cuenta contable
/// </summary>
public class UpdateAccountingAccountRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
