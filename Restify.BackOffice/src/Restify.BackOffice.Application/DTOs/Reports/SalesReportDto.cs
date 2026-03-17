using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.DTOs.Reports;

/// <summary>
/// Reporte de ventas por período
/// </summary>
public class SalesReportDto
{
    public DateTime Date { get; set; }
    public int InvoiceCount { get; set; }
    public decimal TotalSales { get; set; }
    public decimal AverageTicket { get; set; }
    public int CustomerCount { get; set; }
}

/// <summary>
/// Resumen de ventas con KPIs
/// </summary>
public class SalesSummaryDto
{
    public decimal TotalSales { get; set; }
    public int TotalInvoices { get; set; }
    public decimal AverageTicket { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalDiscount { get; set; }
    
    // Comparativa
    public decimal? PreviousPeriodSales { get; set; }
    public decimal? GrowthPercentage { get; set; }
    
    // Desglose diario
    public List<SalesReportDto> DailyBreakdown { get; set; } = new();
}

/// <summary>
/// Ventas por método de pago
/// </summary>
public class SalesByPaymentMethodDto
{
    public PaymentMethod PaymentMethod { get; set; }
    public string PaymentMethodName { get; set; } = string.Empty;
    public int InvoiceCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Percentage { get; set; }
}

/// <summary>
/// Reporte comparativo entre dos períodos
/// </summary>
public class ComparativeSalesReportDto
{
    public PeriodInfo CurrentPeriod { get; set; } = new();
    public PeriodInfo PreviousPeriod { get; set; } = new();
    public ComparisonMetrics Comparison { get; set; } = new();
}

public class PeriodInfo
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalSales { get; set; }
    public int InvoiceCount { get; set; }
    public decimal AverageTicket { get; set; }
    public int CustomerCount { get; set; }
}

public class ComparisonMetrics
{
    public decimal SalesGrowth { get; set; }
    public decimal SalesGrowthPercentage { get; set; }
    public int InvoiceGrowth { get; set; }
    public decimal InvoiceGrowthPercentage { get; set; }
    public decimal TicketGrowth { get; set; }
    public decimal TicketGrowthPercentage { get; set; }
}

/// <summary>
/// Ventas por franja horaria
/// </summary>
public class SalesByTimeSlotDto
{
    public TimeSlotType TimeSlot { get; set; }
    public string TimeSlotName { get; set; } = string.Empty;
    public string TimeRange { get; set; } = string.Empty; // "06:00 - 11:00"
    public int InvoiceCount { get; set; }
    public decimal TotalSales { get; set; }
    public decimal AverageTicket { get; set; }
    public decimal Percentage { get; set; } // % del total de ventas
}

/// <summary>
/// Horas pico de ventas
/// </summary>
public class PeakHoursDto
{
    public int Hour { get; set; } // 0-23
    public string HourRange { get; set; } = string.Empty; // "14:00 - 15:00"
    public int OrderCount { get; set; }
    public decimal TotalSales { get; set; }
    public bool IsPeakHour { get; set; }
}
