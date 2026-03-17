using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.DTOs.Reports;

/// <summary>
/// Dashboard ejecutivo con KPIs del día
/// </summary>
public class DashboardKpisDto
{
    // Ventas de hoy
    public decimal TodaySales { get; set; }
    public int TodayInvoices { get; set; }
    public decimal TodayAverageTicket { get; set; }
    
    // Comparativa con ayer
    public decimal YesterdaySales { get; set; }
    public decimal SalesGrowth { get; set; }
    public decimal SalesGrowthPercentage { get; set; }
    
    // Estado de operación actual
    public int TablesOccupied { get; set; }
    public int TotalTables { get; set; }
    public decimal OccupancyPercentage { get; set; }
    
    // Pedidos en proceso
    public int OrdersPending { get; set; }
    public int OrdersInProgress { get; set; }
    public int OrdersReady { get; set; }
    
    // Métodos de pago del día
    public List<SalesByPaymentMethodDto> PaymentMethodBreakdown { get; set; } = new();
    
    // Top 5 productos del día
    public List<TopSellingProductDto> TopProducts { get; set; } = new();
    
    // Alertas
    public List<DashboardAlertDto> Alerts { get; set; } = new();
}

/// <summary>
/// Alertas para el dashboard
/// </summary>
public class DashboardAlertDto
{
    public AlertType Type { get; set; }
    public AlertSeverity Severity { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

public enum AlertType
{
    LowSales,           // Ventas bajas vs promedio
    HighCancellation,   // Muchas cancelaciones
    LongWaitTime,       // Tiempos de espera altos
    TableIdle,          // Mesa ocupada sin ordenar
    ProductOutOfStock,  // Producto agotado
    CashDifference,     // Diferencia en caja
    EmployeeIssue       // Problema con empleado
}

public enum AlertSeverity
{
    Info,
    Warning,
    Critical
}

/// <summary>
/// Análisis de facturas canceladas
/// </summary>
public class CancelledInvoicesReportDto
{
    public Guid InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string? OrderNumber { get; set; }
    public string? EmployeeName { get; set; }
    public decimal Amount { get; set; }
    public string? CancelReason { get; set; }
    public DateTime CancelledAt { get; set; }
    public string? CancelledBy { get; set; }
}

/// <summary>
/// Análisis de descuentos aplicados
/// </summary>
public class DiscountsReportDto
{
    public Guid InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string? EmployeeName { get; set; }
    public decimal OriginalAmount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string? Reason { get; set; }
    public DateTime AppliedAt { get; set; }
    public string? AppliedBy { get; set; }
}

/// <summary>
/// Resumen de descuentos
/// </summary>
public class DiscountsSummaryDto
{
    public int TotalDiscountsApplied { get; set; }
    public decimal TotalDiscountAmount { get; set; }
    public decimal AverageDiscountPercentage { get; set; }
    public decimal RevenueImpactPercentage { get; set; }
    public List<DiscountsByEmployeeDto> ByEmployee { get; set; } = new();
}

public class DiscountsByEmployeeDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int DiscountsApplied { get; set; }
    public decimal TotalDiscountAmount { get; set; }
    public decimal AverageDiscountPercentage { get; set; }
}
