namespace Restify.BackOffice.Domain.Enums;

/// <summary>
/// A quien aplica la deduccion
/// </summary>
public enum DeductionAppliesTo
{
    /// <summary>
    /// Aplica al empleado
    /// </summary>
    Employee = 1,

    /// <summary>
    /// Aplica al empleador
    /// </summary>
    Employer = 2,

    /// <summary>
    /// Aplica a ambos
    /// </summary>
    Both = 3
}
