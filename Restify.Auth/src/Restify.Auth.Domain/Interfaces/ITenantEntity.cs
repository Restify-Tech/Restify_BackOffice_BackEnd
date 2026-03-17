namespace Restify.Auth.Domain.Interfaces;

/// <summary>
/// Interface para entidades que pertenecen a un Tenant
/// </summary>
public interface ITenantEntity
{
    Guid TenantId { get; set; }
}
