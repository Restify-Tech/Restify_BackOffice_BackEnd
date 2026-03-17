namespace Restify.BackOffice.Domain.Enums;

/// <summary>
/// Franjas horarias para análisis de ventas
/// </summary>
public enum TimeSlotType
{
    Breakfast = 1,      // 06:00 - 11:00
    Lunch = 2,          // 11:00 - 15:00
    Afternoon = 3,      // 15:00 - 18:00
    Dinner = 4,         // 18:00 - 23:00
    LateNight = 5       // 23:00 - 06:00
}
