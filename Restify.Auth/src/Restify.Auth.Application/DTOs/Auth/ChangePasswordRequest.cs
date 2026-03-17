namespace Restify.Auth.Application.DTOs.Auth;

/// <summary>
/// Request para cambiar contraseña
/// </summary>
public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword
);
