namespace Restify.BackOffice.Domain.Enums;

/// <summary>
/// Tipo de calculo para deducciones de nomina
/// </summary>
public enum DeductionCalculationType
{
    /// <summary>
    /// Monto fijo
    /// </summary>
    FixedAmount = 1,

    /// <summary>
    /// Porcentaje del salario bruto
    /// </summary>
    PercentageOfGross = 2,

    /// <summary>
    /// Porcentaje del salario base
    /// </summary>
    PercentageOfBase = 3
}
