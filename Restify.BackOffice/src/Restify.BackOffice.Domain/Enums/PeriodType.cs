namespace Restify.BackOffice.Domain.Enums;

/// <summary>
/// Tipos de período para análisis comparativo
/// </summary>
public enum PeriodType
{
    Today = 0,
    Yesterday = 1,
    ThisWeek = 2,
    LastWeek = 3,
    ThisMonth = 4,
    LastMonth = 5,
    ThisYear = 6,
    LastYear = 7,
    Custom = 99
}
