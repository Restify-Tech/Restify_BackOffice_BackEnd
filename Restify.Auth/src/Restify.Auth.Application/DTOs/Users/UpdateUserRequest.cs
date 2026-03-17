using Restify.Auth.Domain.Enums;

namespace Restify.Auth.Application.DTOs.Users;

/// <summary>
/// Request para actualizar un usuario
/// </summary>
public record UpdateUserRequest(
    string FirstName,
    string LastName,
    string? Phone,
    UserStatus? Status,
    IEnumerable<Guid>? RoleIds
);
