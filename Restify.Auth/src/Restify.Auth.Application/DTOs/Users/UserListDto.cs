using Restify.Auth.Domain.Enums;

namespace Restify.Auth.Application.DTOs.Users;

/// <summary>
/// DTO resumido para listado de usuarios
/// </summary>
public record UserListDto(
    Guid Id,
    string Email,
    string? Username,
    string FullName,
    UserStatus Status,
    DateTime? LastLoginAt,
    IEnumerable<string> Roles
);
