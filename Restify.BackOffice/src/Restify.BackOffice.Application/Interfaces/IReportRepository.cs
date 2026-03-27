namespace Restify.BackOffice.Application.Interfaces;

/// <summary>
/// Repositorio especializado para queries complejas de reportes
/// </summary>
public interface IReportRepository
{
    // ========== SALES QUERIES ==========
    
    /// <summary>
    /// Obtiene ventas totales por período
    /// </summary>
    Task<(decimal totalSales, int invoiceCount, decimal averageTicket)> GetSalesTotalsAsync(
        DateTime from, 
        DateTime to, 
        Guid tenantId, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtiene ventas agrupadas por día
    /// </summary>
    Task<List<(DateTime date, decimal sales, int invoiceCount, int customerCount)>> GetDailySalesAsync(
        DateTime from, 
        DateTime to, 
        Guid tenantId, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtiene ventas por método de pago
    /// </summary>
    Task<List<(int paymentMethod, int invoiceCount, decimal totalAmount)>> GetSalesByPaymentMethodAsync(
        DateTime from, 
        DateTime to, 
        Guid tenantId, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtiene ventas por hora del día
    /// </summary>
    Task<List<(int hour, int orderCount, decimal totalSales)>> GetSalesByHourAsync(
        DateTime from, 
        DateTime to, 
        Guid tenantId, 
        CancellationToken cancellationToken = default);
    
    // ========== PRODUCT QUERIES ==========
    
    /// <summary>
    /// Obtiene productos más vendidos
    /// </summary>
    Task<List<(Guid productId, string productName, string? categoryName, int quantity, decimal revenue)>> GetTopSellingProductsAsync(
        DateTime from, 
        DateTime to, 
        int limit, 
        Guid tenantId, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtiene todos los productos con su volumen de ventas (para análisis ABC)
    /// </summary>
    Task<List<(Guid productId, string productName, string? categoryName, int quantity, decimal revenue)>> GetAllProductSalesAsync(
        DateTime from, 
        DateTime to, 
        Guid tenantId, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtiene productos con bajo movimiento
    /// </summary>
    Task<List<(Guid productId, string productName, string? categoryName, int quantity, decimal revenue, DateTime? lastSaleDate)>> GetLowMovementProductsAsync(
        DateTime from, 
        DateTime to, 
        int maxSalesThreshold, 
        Guid tenantId, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtiene rendimiento por categoría
    /// </summary>
    Task<List<(Guid categoryId, string categoryName, int productCount, int totalQuantity, decimal totalRevenue)>> GetCategoryPerformanceAsync(
        DateTime from, 
        DateTime to, 
        Guid tenantId, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtiene modificadores más usados
    /// </summary>
    Task<List<(Guid modifierId, string modifierName, int timesOrdered, decimal totalRevenue, List<string> topProducts)>> GetPopularModifiersAsync(
        DateTime from, 
        DateTime to, 
        int limit, 
        Guid tenantId, 
        CancellationToken cancellationToken = default);
    
    // ========== EMPLOYEE QUERIES ==========
    
    /// <summary>
    /// Obtiene métricas de empleados
    /// </summary>
    Task<List<(
        Guid employeeId, 
        string employeeName, 
        int totalOrders, 
        int completedOrders, 
        int cancelledOrders, 
        decimal totalSales, 
        int customerCount, 
        double avgServiceTimeMinutes)>> GetEmployeePerformanceAsync(
        DateTime from, 
        DateTime to, 
        Guid tenantId, 
        CancellationToken cancellationToken = default);
    
    // ========== DASHBOARD QUERIES ==========
    
    /// <summary>
    /// Obtiene conteo de mesas ocupadas
    /// </summary>
    Task<(int occupied, int total)> GetTableOccupancyAsync(Guid tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtiene conteo de pedidos por estado
    /// </summary>
    Task<(int pending, int inProgress, int ready)> GetOrderStatusCountsAsync(Guid tenantId, CancellationToken cancellationToken = default);
    
    // ========== AGGREGATED QUERIES (Frontend) ==========

    /// <summary>
    /// Obtiene ventas diarias con desglose de impuestos y descuentos
    /// </summary>
    Task<List<(DateTime date, decimal subtotal, decimal tax, decimal discount, decimal total, int invoiceCount, int orderCount)>> GetDailySalesDetailedAsync(
        DateTime from,
        DateTime to,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene totales de impuestos y descuentos en un periodo
    /// </summary>
    Task<(decimal totalTax, decimal totalDiscount)> GetTaxAndDiscountTotalsAsync(
        DateTime from,
        DateTime to,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene ventas agrupadas por dia de la semana
    /// </summary>
    Task<List<(int dayOfWeek, int invoiceCount, decimal totalSales)>> GetSalesByDayOfWeekAsync(
        DateTime from,
        DateTime to,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    // ========== CONTROL QUERIES ==========
    
    /// <summary>
    /// Obtiene facturas canceladas
    /// </summary>
    Task<List<(
        Guid invoiceId, 
        string invoiceNumber, 
        string? orderNumber, 
        string? employeeName, 
        decimal amount, 
        string? cancelReason, 
        DateTime cancelledAt, 
        string? cancelledBy)>> GetCancelledInvoicesAsync(
        DateTime from, 
        DateTime to, 
        Guid tenantId, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtiene facturas con descuentos
    /// </summary>
    Task<List<(
        Guid invoiceId, 
        string invoiceNumber, 
        string? employeeName, 
        decimal originalAmount, 
        decimal discountPercentage, 
        decimal discountAmount, 
        decimal finalAmount, 
        DateTime createdAt, 
        string? appliedBy)>> GetDiscountsAsync(
        DateTime from, 
        DateTime to, 
        Guid tenantId, 
        CancellationToken cancellationToken = default);
}
