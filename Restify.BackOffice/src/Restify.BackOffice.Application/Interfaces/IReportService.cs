using Restify.BackOffice.Application.DTOs.Reports;
using Restify.BackOffice.Domain.Enums;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IReportService
{
    // ========== FRONTEND AGGREGATED REPORTS ==========

    /// <summary>
    /// Reporte de ventas agregado para el frontend
    /// </summary>
    Task<Result<FrontendSalesReportDto>> GetFrontendSalesReportAsync(DateTime from, DateTime to, string? paymentMethod, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reporte de productos agregado para el frontend
    /// </summary>
    Task<Result<FrontendProductReportDto>> GetFrontendProductReportAsync(DateTime from, DateTime to, string? categoryId, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reporte de personal agregado para el frontend
    /// </summary>
    Task<Result<FrontendStaffReportDto>> GetFrontendStaffReportAsync(DateTime from, DateTime to, string? staffId, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resumen del dashboard para el frontend
    /// </summary>
    Task<Result<FrontendDashboardSummaryDto>> GetFrontendDashboardSummaryAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exportar reporte a CSV
    /// </summary>
    Task<Result<byte[]>> ExportReportAsync(ExportReportRequestDto request, Guid tenantId, CancellationToken cancellationToken = default);

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
