using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.DTOs.Reports;

/// <summary>
/// Rendimiento de empleado (mesero)
/// </summary>
public class EmployeePerformanceDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string? Role { get; set; }
    
    // Métricas de pedidos
    public int TotalOrders { get; set; }
    public int CompletedOrders { get; set; }
    public int CancelledOrders { get; set; }
    public decimal CancellationRate { get; set; }
    
    // Métricas de ventas
    public decimal TotalSales { get; set; }
    public decimal AverageTicket { get; set; }
    public int TotalCustomers { get; set; }
    
    // Métricas de tiempo
    public double AverageServiceTimeMinutes { get; set; }
    public double FastestServiceMinutes { get; set; }
    public double SlowestServiceMinutes { get; set; }
    
    // Propinas (si aplica)
    public decimal TotalTips { get; set; }
    public decimal AverageTipPercentage { get; set; }
    
    // Rating (si se implementa)
    public decimal? AverageRating { get; set; }
    
    // Comparativa
    public decimal PerformanceScore { get; set; } // 0-100
    public int Rank { get; set; } // Posición en el ranking
}

/// <summary>
/// Mejor empleado del período
/// </summary>
public class TopPerformerDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string? Role { get; set; }
    public decimal TotalSales { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageTicket { get; set; }
    public decimal PerformanceScore { get; set; }
    public string Highlight { get; set; } = string.Empty; // Ej: "Mayor vendedor del mes"
}

/// <summary>
/// Errores/incidencias por empleado
/// </summary>
public class EmployeeErrorsDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int OrdersCancelled { get; set; }
    public int OrdersModified { get; set; }
    public int ComplaintsReceived { get; set; }
    public decimal ErrorRate { get; set; }
}

/// <summary>
/// Productividad por turno
/// </summary>
public class EmployeeProductivityByShiftDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public TimeSlotType TimeSlot { get; set; }
    public string TimeSlotName { get; set; } = string.Empty;
    public int OrdersServed { get; set; }
    public decimal TotalSales { get; set; }
    public double AverageServiceTimeMinutes { get; set; }
}

/// <summary>
/// Comparativa entre empleados
/// </summary>
public class EmployeeComparisonDto
{
    public string MetricName { get; set; } = string.Empty;
    public List<EmployeeMetricValue> EmployeeValues { get; set; } = new();
}

public class EmployeeMetricValue
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string FormattedValue { get; set; } = string.Empty;
}
