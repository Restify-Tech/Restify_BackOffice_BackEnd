namespace Restify.BackOffice.Application.DTOs.Reports;

/// <summary>
/// DTOs agregados que coinciden con los tipos del frontend (reports.ts)
/// </summary>

// =====================================================
// Sales Report (agregado para GET /api/reports/sales)
// =====================================================

public class FrontendSalesReportDto
{
    public FrontendSalesReportSummaryDto Summary { get; set; } = new();
    public List<FrontendSalesByDateDto> SalesByDate { get; set; } = new();
    public List<FrontendSalesByHourDto> SalesByHour { get; set; } = new();
    public List<FrontendSalesByDayOfWeekDto> SalesByDayOfWeek { get; set; } = new();
    public List<FrontendSalesByPaymentMethodDto> SalesByPaymentMethod { get; set; } = new();
}

public class FrontendSalesReportSummaryDto
{
    public decimal TotalSales { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageDailySales { get; set; }
    public decimal AverageTicket { get; set; }
    public int TotalInvoices { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal? ComparisonPercentage { get; set; }
}

public class FrontendSalesByDateDto
{
    public string Date { get; set; } = string.Empty;
    public int InvoiceCount { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public int OrderCount { get; set; }
    public decimal AverageTicket { get; set; }
}

public class FrontendSalesByHourDto
{
    public int Hour { get; set; }
    public string HourLabel { get; set; } = string.Empty;
    public int InvoiceCount { get; set; }
    public decimal Total { get; set; }
    public int OrderCount { get; set; }
}

public class FrontendSalesByDayOfWeekDto
{
    public int DayOfWeek { get; set; }
    public string DayName { get; set; } = string.Empty;
    public int InvoiceCount { get; set; }
    public decimal Total { get; set; }
    public decimal AverageTicket { get; set; }
}

public class FrontendSalesByPaymentMethodDto
{
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentMethodName { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Total { get; set; }
    public decimal Percentage { get; set; }
}

// =====================================================
// Product Report (agregado para GET /api/reports/products)
// =====================================================

public class FrontendProductReportDto
{
    public List<FrontendTopProductDto> TopProducts { get; set; } = new();
    public List<FrontendProductPerformanceDto> ProductPerformance { get; set; } = new();
    public List<FrontendCategoryPerformanceDto> CategoryPerformance { get; set; } = new();
    public int TotalProductsSold { get; set; }
    public int TotalUniqueProducts { get; set; }
}

public class FrontendTopProductDto
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AveragePrice { get; set; }
    public decimal Percentage { get; set; }
}

public class FrontendProductPerformanceDto
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal Revenue { get; set; }
    public decimal Cost { get; set; }
    public decimal Profit { get; set; }
    public decimal ProfitMargin { get; set; }
}

public class FrontendCategoryPerformanceDto
{
    public string CategoryId { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public int QuantitySold { get; set; }
    public decimal Revenue { get; set; }
    public decimal Percentage { get; set; }
}

// =====================================================
// Staff Report (agregado para GET /api/reports/staff)
// =====================================================

public class FrontendStaffReportDto
{
    public List<FrontendStaffPerformanceDto> StaffPerformance { get; set; } = new();
    public int TotalStaff { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalSales { get; set; }
    public decimal AveragePerformance { get; set; }
}

public class FrontendStaffPerformanceDto
{
    public string StaffId { get; set; } = string.Empty;
    public string StaffName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int OrdersServed { get; set; }
    public decimal TotalSales { get; set; }
    public decimal AverageTicket { get; set; }
    public double AverageServiceTime { get; set; }
    public decimal Tips { get; set; }
    public decimal? Rating { get; set; }
}

// =====================================================
// Dashboard Summary (para GET /api/reports/dashboard-summary)
// =====================================================

public class FrontendDashboardSummaryDto
{
    public decimal TodaySales { get; set; }
    public int TodayOrders { get; set; }
    public decimal AverageTicket { get; set; }
    public decimal SalesGrowthPercentage { get; set; }
    public int TablesOccupied { get; set; }
    public int TotalTables { get; set; }
    public int OrdersPending { get; set; }
    public int OrdersInProgress { get; set; }
    public int OrdersReady { get; set; }
    public List<FrontendSalesByPaymentMethodDto> PaymentMethodBreakdown { get; set; } = new();
    public List<FrontendTopProductDto> TopProducts { get; set; } = new();
}

// =====================================================
// Export Request (para POST /api/reports/export)
// =====================================================

public class ExportReportRequestDto
{
    public string ReportType { get; set; } = string.Empty; // "sales", "products", "staff"
    public string Format { get; set; } = string.Empty; // "pdf", "excel", "csv"
    public ReportFiltersDto Filters { get; set; } = new();
}

public class ReportFiltersDto
{
    public string Period { get; set; } = string.Empty;
    public string DateFrom { get; set; } = string.Empty;
    public string DateTo { get; set; } = string.Empty;
    public string? CategoryId { get; set; }
    public string? StaffId { get; set; }
    public string? PaymentMethod { get; set; }
}
