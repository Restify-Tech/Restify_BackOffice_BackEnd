using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Restify.Core.Application.Interfaces;
using Restify.Core.Domain.Constants;

namespace Restify.Core.Infrastructure.Services;

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
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userId, out var id) ? id : null;
        }
    }

    public Guid? TenantId
    {
        get
        {
            var tenantId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimConstants.TenantId);
            return Guid.TryParse(tenantId, out var id) ? id : null;
        }
    }

    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public bool IsSuperAdmin =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimConstants.IsSuperAdmin) == "true";

    public string? UserType =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimConstants.UserType);
}
