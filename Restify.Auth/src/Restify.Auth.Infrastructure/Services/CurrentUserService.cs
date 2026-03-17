using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Restify.Auth.Application.Interfaces;
using Restify.Auth.Domain.Constants;

namespace Restify.Auth.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de usuario actual
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userId, out var id) ? id : null;
        }
    }

    public Guid? TenantId
    {
        get
        {
            var tenantId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimConstants.TenantId)?.Value;
            return Guid.TryParse(tenantId, out var id) ? id : null;
        }
    }

    public string? Email => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

    public IEnumerable<string> Roles =>
        _httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? [];

    public IEnumerable<string> Permissions =>
        _httpContextAccessor.HttpContext?.User.FindAll(ClaimConstants.Permission).Select(c => c.Value) ?? [];

    public bool IsSuperAdmin =>
        _httpContextAccessor.HttpContext?.User.FindFirst(ClaimConstants.IsSuperAdmin)?.Value == "true";

    public string? UserType =>
        _httpContextAccessor.HttpContext?.User.FindFirst(ClaimConstants.UserType)?.Value;

    public bool HasPermission(string permission) => Permissions.Contains(permission);

    public bool IsInRole(string role) => Roles.Contains(role);
}
