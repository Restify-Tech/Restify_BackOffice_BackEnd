using Restify.Auth.Domain.Enums;

namespace Restify.Auth.Application.DTOs.Users;

/// <summary>
/// DTO completo de usuario
/// </summary>
public record UserDto(
    Guid Id,
    string Email,
    string? Username,
    string FirstName,
    string LastName,
    string? Phone,
    string? AvatarUrl,
    UserStatus Status,
    bool EmailVerified,
    DateTime? LastLoginAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IEnumerable<string> Roles
);
