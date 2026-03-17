using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.Users;

namespace Restify.Auth.Application.Interfaces;

/// <summary>
/// Servicio de gestión de usuarios
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Obtiene un usuario por su ID
    /// </summary>
    Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene lista paginada de usuarios
    /// </summary>
    Task<Result<PagedResponse<UserListDto>>> GetAllAsync(PagedRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crea un nuevo usuario
    /// </summary>
    Task<Result<UserDto>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza un usuario existente
    /// </summary>
    Task<Result<UserDto>> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina un usuario (soft delete)
    /// </summary>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resetea la contraseña de un usuario
    /// </summary>
    Task<Result<string>> ResetPasswordAsync(Guid id, CancellationToken cancellationToken = default);
}
