using FluentAssertions;
using FluentValidation.TestHelper;
using Restify.Auth.Application.DTOs.Auth;
using Restify.Auth.Application.DTOs.Roles;
using Restify.Auth.Application.DTOs.Users;
using Restify.Auth.Application.Validators;

namespace Restify.Auth.Tests.Validators;

/// <summary>
/// Tests para validadores de FluentValidation
/// </summary>
public class ValidatorTests
{
    // ===== LoginRequest Validator =====

    private readonly LoginRequestValidator _loginValidator = new();

    [Fact]
    public void LoginRequest_EmailVacio_SinUsername_Falla()
    {
        // Ni email ni username → error a nivel raiz
        var request = new LoginRequest("", null, "Admin123!");
        var result = _loginValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Debe proporcionar un email o nombre de usuario");
    }

    [Fact]
    public void LoginRequest_EmailInvalido_Falla()
    {
        var request = new LoginRequest("no-es-email", null, "Admin123!");
        var result = _loginValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void LoginRequest_PasswordVacio_Falla()
    {
        var request = new LoginRequest("user@test.com", null, "");
        var result = _loginValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("La contraseña es requerida");
    }

    [Fact]
    public void LoginRequest_DatosValidos_Pasa()
    {
        var request = new LoginRequest("user@test.com", null, "Admin123!");
        var result = _loginValidator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    // ===== CreateUserRequest Validator =====

    private readonly CreateUserRequestValidator _createUserValidator = new();

    [Fact]
    public void CreateUser_EmailVacio_Falla()
    {
        var request = new CreateUserRequest("", null, "Admin123!", "Juan", "Perez", null, new[] { Guid.NewGuid() });
        var result = _createUserValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void CreateUser_PasswordCorto_Falla()
    {
        var request = new CreateUserRequest("user@test.com", null, "Ab1!", "Juan", "Perez", null, new[] { Guid.NewGuid() });
        var result = _createUserValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("La contraseña debe tener al menos 8 caracteres");
    }

    [Fact]
    public void CreateUser_PasswordSinMayuscula_Falla()
    {
        var request = new CreateUserRequest("user@test.com", null, "abcdefgh1!", "Juan", "Perez", null, new[] { Guid.NewGuid() });
        var result = _createUserValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("La contraseña debe contener al menos una mayúscula");
    }

    [Fact]
    public void CreateUser_NombreVacio_Falla()
    {
        var request = new CreateUserRequest("user@test.com", null, "Admin123!", "", "Perez", null, new[] { Guid.NewGuid() });
        var result = _createUserValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void CreateUser_ApellidoVacio_Falla()
    {
        var request = new CreateUserRequest("user@test.com", null, "Admin123!", "Juan", "", null, new[] { Guid.NewGuid() });
        var result = _createUserValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void CreateUser_SinRoles_Falla()
    {
        var request = new CreateUserRequest("user@test.com", null, "Admin123!", "Juan", "Perez", null, Enumerable.Empty<Guid>());
        var result = _createUserValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.RoleIds);
    }

    [Fact]
    public void CreateUser_DatosValidos_Pasa()
    {
        var request = new CreateUserRequest("user@test.com", null, "Admin123!", "Juan", "Perez", "0999999999", new[] { Guid.NewGuid() });
        var result = _createUserValidator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    // ===== CreateRoleRequest Validator =====

    private readonly CreateRoleRequestValidator _createRoleValidator = new();

    [Fact]
    public void CreateRole_NombreVacio_Falla()
    {
        var request = new CreateRoleRequest("", null, new[] { Guid.NewGuid() });
        var result = _createRoleValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("El nombre es requerido");
    }

    [Fact]
    public void CreateRole_SinPermisos_Falla()
    {
        var request = new CreateRoleRequest("Mesero", null, Enumerable.Empty<Guid>());
        var result = _createRoleValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.PermissionIds);
    }

    [Fact]
    public void CreateRole_DatosValidos_Pasa()
    {
        var request = new CreateRoleRequest("Mesero", "Atiende mesas", new[] { Guid.NewGuid() });
        var result = _createRoleValidator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateRole_DescripcionMuyLarga_Falla()
    {
        var descripcionLarga = new string('A', 501);
        var request = new CreateRoleRequest("Mesero", descripcionLarga, new[] { Guid.NewGuid() });
        var result = _createRoleValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void CreateRole_NombreMuyLargo_Falla()
    {
        var nombreLargo = new string('A', 101);
        var request = new CreateRoleRequest(nombreLargo, null, new[] { Guid.NewGuid() });
        var result = _createRoleValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("El nombre no puede exceder 100 caracteres");
    }

    // ===== ChangePasswordRequest Validator =====

    private readonly ChangePasswordRequestValidator _changePasswordValidator = new();

    [Fact]
    public void ChangePassword_PasswordActualVacio_Falla()
    {
        var request = new ChangePasswordRequest("", "NuevoPass123!", "NuevoPass123!");
        var result = _changePasswordValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.CurrentPassword)
            .WithErrorMessage("La contraseña actual es requerida");
    }

    [Fact]
    public void ChangePassword_NuevoPasswordCorto_Falla()
    {
        var request = new ChangePasswordRequest("OldPass1!", "Ab1!", "Ab1!");
        var result = _changePasswordValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("La contraseña debe tener al menos 8 caracteres");
    }

    [Fact]
    public void ChangePassword_SinMayuscula_Falla()
    {
        var request = new ChangePasswordRequest("OldPass1!", "abcdefgh1!", "abcdefgh1!");
        var result = _changePasswordValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("La contraseña debe contener al menos una mayúscula");
    }

    [Fact]
    public void ChangePassword_SinMinuscula_Falla()
    {
        var request = new ChangePasswordRequest("OldPass1!", "ABCDEFGH1!", "ABCDEFGH1!");
        var result = _changePasswordValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("La contraseña debe contener al menos una minúscula");
    }

    [Fact]
    public void ChangePassword_SinNumero_Falla()
    {
        var request = new ChangePasswordRequest("OldPass1!", "Abcdefgh!", "Abcdefgh!");
        var result = _changePasswordValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("La contraseña debe contener al menos un número");
    }

    [Fact]
    public void ChangePassword_SinCaracterEspecial_Falla()
    {
        var request = new ChangePasswordRequest("OldPass1!", "Abcdefgh1", "Abcdefgh1");
        var result = _changePasswordValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("La contraseña debe contener al menos un caracter especial");
    }

    [Fact]
    public void ChangePassword_ConfirmacionNoCoincide_Falla()
    {
        var request = new ChangePasswordRequest("OldPass1!", "NuevoPass123!", "OtraPass456!");
        var result = _changePasswordValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword)
            .WithErrorMessage("Las contraseñas no coinciden");
    }

    [Fact]
    public void ChangePassword_DatosValidos_Pasa()
    {
        var request = new ChangePasswordRequest("OldPass1!", "NuevoPass123!", "NuevoPass123!");
        var result = _changePasswordValidator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    // ===== UpdateUserRequest Validator =====

    private readonly UpdateUserRequestValidator _updateUserValidator = new();

    [Fact]
    public void UpdateUser_NombreVacio_Falla()
    {
        var request = new UpdateUserRequest("", "Perez", null, null, null);
        var result = _updateUserValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void UpdateUser_ApellidoVacio_Falla()
    {
        var request = new UpdateUserRequest("Juan", "", null, null, null);
        var result = _updateUserValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void UpdateUser_NombreMuyLargo_Falla()
    {
        var nombreLargo = new string('A', 101);
        var request = new UpdateUserRequest(nombreLargo, "Perez", null, null, null);
        var result = _updateUserValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("El nombre no puede exceder 100 caracteres");
    }

    [Fact]
    public void UpdateUser_TelefonoMuyLargo_Falla()
    {
        var telefonoLargo = new string('9', 21);
        var request = new UpdateUserRequest("Juan", "Perez", telefonoLargo, null, null);
        var result = _updateUserValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage("El teléfono no puede exceder 20 caracteres");
    }

    [Fact]
    public void UpdateUser_DatosValidos_Pasa()
    {
        var request = new UpdateUserRequest("Juan", "Perez", "0999999999", null, null);
        var result = _updateUserValidator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    // ===== UpdateRoleRequest Validator =====

    private readonly UpdateRoleRequestValidator _updateRoleValidator = new();

    [Fact]
    public void UpdateRole_NombreVacio_Falla()
    {
        var request = new UpdateRoleRequest("", null, new[] { Guid.NewGuid() });
        var result = _updateRoleValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("El nombre es requerido");
    }

    [Fact]
    public void UpdateRole_SinPermisos_Falla()
    {
        var request = new UpdateRoleRequest("Mesero", null, Enumerable.Empty<Guid>());
        var result = _updateRoleValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.PermissionIds);
    }

    [Fact]
    public void UpdateRole_DescripcionMuyLarga_Falla()
    {
        var descripcionLarga = new string('A', 501);
        var request = new UpdateRoleRequest("Mesero", descripcionLarga, new[] { Guid.NewGuid() });
        var result = _updateRoleValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void UpdateRole_DatosValidos_Pasa()
    {
        var request = new UpdateRoleRequest("Mesero", "Descripcion", new[] { Guid.NewGuid() });
        var result = _updateRoleValidator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    // ===== Parameterized tests =====

    [Theory]
    [InlineData("invalid")]
    [InlineData("@missing-local")]
    [InlineData("missing-domain@")]
    public void LoginRequest_EmailsInvalidos_Fallan(string email)
    {
        // Email con formato incorrecto (no vacio) → error en propiedad Email
        var request = new LoginRequest(email, null, "Admin123!");
        var result = _loginValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("short1!")]        // menos de 8 chars
    [InlineData("nouppercase1!")]  // sin mayuscula
    [InlineData("NOLOWERCASE1!")]  // sin minuscula
    [InlineData("NoNumber!!")]     // sin numero
    [InlineData("NoSpecial123")]   // sin caracter especial
    public void CreateUser_PasswordsInvalidos_Fallan(string password)
    {
        var request = new CreateUserRequest("user@test.com", null, password, "Juan", "Perez", null, new[] { Guid.NewGuid() });
        var result = _createUserValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
