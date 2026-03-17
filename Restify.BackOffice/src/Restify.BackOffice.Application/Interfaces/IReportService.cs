using Restify.BackOffice.Application.DTOs.Reports;
using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.Interfaces;

public interface IReportService
{
    // ========== SALES REPORTS ==========
    
    /// <summary>
    /// Resumen de ventas por período
    /// </summary>
    Task<SalesSummaryDto> GetSalesSummaryAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Reporte comparativo de ventas
    /// </summary>
    Task<ComparativeSalesReportDto> GetComparativeSalesAsync(PeriodType periodType, Guid tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Ventas por método de pago
    /// </summary>
    Task<List<SalesByPaymentMethodDto>> GetSalesByPaymentMethodAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Ventas por franja horaria
    /// </summary>
    Task<List<SalesByTimeSlotDto>> GetSalesByTimeSlotAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Horas pico de ventas
    /// </summary>
    Task<List<PeakHoursDto>> GetPeakHoursAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken = default);
    
    // ========== PRODUCT REPORTS ==========
    
    /// <summary>
    /// Top productos más vendidos
    /// </summary>
    Task<List<TopSellingProductDto>> GetTopSellingProductsAsync(DateTime from, DateTime to, int limit, Guid tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Análisis ABC de productos (curva de Pareto)
    /// </summary>
    Task<AbcAnalysisSummaryDto> GetProductAbcAnalysisAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Productos de bajo movimiento
    /// </summary>
    Task<List<LowMovementProductDto>> GetLowMovementProductsAsync(DateTime from, DateTime to, int maxSalesThreshold, Guid tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Rendimiento por categoría
    /// </summary>
    Task<List<CategoryPerformanceDto>> GetCategoryPerformanceAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Modificadores más populares
    /// </summary>
    Task<List<PopularModifierDto>> GetPopularModifiersAsync(DateTime from, DateTime to, int limit, Guid tenantId, CancellationToken cancellationToken = default);
    
    // ========== EMPLOYEE REPORTS ==========
    
    /// <summary>
    /// Rendimiento de empleados
    /// </summary>
    Task<List<EmployeePerformanceDto>> GetEmployeePerformanceAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Mejor empleado del período
    /// </summary>
    Task<TopPerformerDto?> GetTopPerformerAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Errores por empleado
    /// </summary>
    Task<List<EmployeeErrorsDto>> GetEmployeeErrorsAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken = default);
    
    // ========== DASHBOARD ==========
    
    /// <summary>
    /// KPIs del dashboard ejecutivo
    /// </summary>
    Task<DashboardKpisDto> GetDashboardKpisAsync(Guid tenantId, CancellationToken cancellationToken = default);
    
    // ========== CONTROL REPORTS ==========
    
    /// <summary>
    /// Facturas canceladas
    /// </summary>
    Task<List<CancelledInvoicesReportDto>> GetCancelledInvoicesAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Descuentos aplicados
    /// </summary>
    Task<List<DiscountsReportDto>> GetDiscountsReportAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Resumen de descuentos
    /// </summary>
    Task<DiscountsSummaryDto> GetDiscountsSummaryAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken = default);
}
