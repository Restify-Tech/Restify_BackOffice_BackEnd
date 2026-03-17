namespace Restify.BackOffice.Domain.Enums;

/// <summary>
/// Estado del empleado
/// </summary>
public enum EmployeeStatus
{
    /// <summary>
    /// Activo
    /// </summary>
    Active = 1,

    /// <summary>
    /// En licencia o permiso
    /// </summary>
    OnLeave = 2,

    /// <summary>
    /// Desvinculado
    /// </summary>
    Terminated = 3
}
