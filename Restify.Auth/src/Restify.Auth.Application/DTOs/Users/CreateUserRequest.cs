namespace Restify.Auth.Application.DTOs.Users;

/// <summary>
/// Request para crear un usuario
/// </summary>
public record CreateUserRequest(
    string Email,
    string? Username,
    string Password,
    string FirstName,
    string LastName,
    string? Phone,
    IEnumerable<Guid> RoleIds
);
