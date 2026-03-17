using Restify.BackOffice.Application.DTOs.Reports;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Infrastructure.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ITableRepository _tableRepository;

    public ReportService(
        IReportRepository reportRepository,
        IInvoiceRepository invoiceRepository,
        IOrderRepository orderRepository,
        ITableRepository tableRepository)
    {
        _reportRepository = reportRepository;
        _invoiceRepository = invoiceRepository;
        _orderRepository = orderRepository;
        _tableRepository = tableRepository;
    }

    // ========== SALES REPORTS ==========

    public async Task<SalesSummaryDto> GetSalesSummaryAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var (totalSales, invoiceCount, averageTicket) = await _reportRepository
            .GetSalesTotalsAsync(from, to, tenantId, cancellationToken);

        var dailyBreakdown = await _reportRepository
            .GetDailySalesAsync(from, to, tenantId, cancellationToken);

        // Calcular período anterior para comparación
        var periodDays = (to - from).Days + 1;
        var previousFrom = from.AddDays(-periodDays);
        var previousTo = from.AddDays(-1);

        var (previousSales, _, _) = await _reportRepository
            .GetSalesTotalsAsync(previousFrom, previousTo, tenantId, cancellationToken);

        decimal? growthPercentage = null;
        if (previousSales > 0)
        {
            growthPercentage = ((totalSales - previousSales) / previousSales) * 100;
        }

        return new SalesSummaryDto
        {
            TotalSales = totalSales,
            TotalInvoices = invoiceCount,
            AverageTicket = averageTicket,
            PreviousPeriodSales = previousSales,
            GrowthPercentage = growthPercentage,
            DailyBreakdown = dailyBreakdown.Select(d => new SalesReportDto
            {
                Date = d.date,
                InvoiceCount = d.invoiceCount,
                TotalSales = d.sales,
                AverageTicket = d.invoiceCount > 0 ? d.sales / d.invoiceCount : 0,
                CustomerCount = d.customerCount
            }).ToList()
        };
    }

    public async Task<ComparativeSalesReportDto> GetComparativeSalesAsync(PeriodType periodType, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var (currentFrom, currentTo, previousFrom, previousTo) = GetComparativePeriods(periodType);

        var currentData = await GetPeriodDataAsync(currentFrom, currentTo, tenantId, cancellationToken);
        var previousData = await GetPeriodDataAsync(previousFrom, previousTo, tenantId, cancellationToken);

        var comparison = CalculateComparison(currentData, previousData);

        return new ComparativeSalesReportDto
        {
            CurrentPeriod = currentData,
            PreviousPeriod = previousData,
            Comparison = comparison
        };
    }

    public async Task<List<SalesByPaymentMethodDto>> GetSalesByPaymentMethodAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var data = await _reportRepository.GetSalesByPaymentMethodAsync(from, to, tenantId, cancellationToken);
        var totalAmount = data.Sum(d => d.totalAmount);

        return data.Select(d => new SalesByPaymentMethodDto
        {
            PaymentMethod = (PaymentMethod)d.paymentMethod,
            PaymentMethodName = ((PaymentMethod)d.paymentMethod).ToString(),
            InvoiceCount = d.invoiceCount,
            TotalAmount = d.totalAmount,
            Percentage = totalAmount > 0 ? (d.totalAmount / totalAmount) * 100 : 0
        }).ToList();
    }

    public async Task<List<SalesByTimeSlotDto>> GetSalesByTimeSlotAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var hourlyData = await _reportRepository.GetSalesByHourAsync(from, to, tenantId, cancellationToken);
        var totalSales = hourlyData.Sum(h => h.totalSales);

        // Agrupar por franjas horarias
        var timeSlots = new Dictionary<TimeSlotType, (int invoiceCount, decimal totalSales)>
        {
            [TimeSlotType.Breakfast] = (0, 0),
            [TimeSlotType.Lunch] = (0, 0),
            [TimeSlotType.Afternoon] = (0, 0),
            [TimeSlotType.Dinner] = (0, 0),
            [TimeSlotType.LateNight] = (0, 0)
        };

        foreach (var hour in hourlyData)
        {
            var slot = GetTimeSlotForHour(hour.hour);
            var current = timeSlots[slot];
            timeSlots[slot] = (current.invoiceCount + hour.orderCount, current.totalSales + hour.totalSales);
        }

        return timeSlots.Select(kvp => new SalesByTimeSlotDto
        {
            TimeSlot = kvp.Key,
            TimeSlotName = kvp.Key.ToString(),
            TimeRange = GetTimeRangeForSlot(kvp.Key),
            InvoiceCount = kvp.Value.invoiceCount,
            TotalSales = kvp.Value.totalSales,
            AverageTicket = kvp.Value.invoiceCount > 0 ? kvp.Value.totalSales / kvp.Value.invoiceCount : 0,
            Percentage = totalSales > 0 ? (kvp.Value.totalSales / totalSales) * 100 : 0
        }).OrderBy(s => s.TimeSlot).ToList();
    }

    public async Task<List<PeakHoursDto>> GetPeakHoursAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var hourlyData = await _reportRepository.GetSalesByHourAsync(from, to, tenantId, cancellationToken);
        
        if (!hourlyData.Any()) return new List<PeakHoursDto>();

        var avgSales = hourlyData.Average(h => h.totalSales);
        var threshold = avgSales * 1.2m; // 20% arriba del promedio

        return hourlyData.Select(h => new PeakHoursDto
        {
            Hour = h.hour,
            HourRange = $"{h.hour:00}:00 - {(h.hour + 1):00}:00",
            OrderCount = h.orderCount,
            TotalSales = h.totalSales,
            IsPeakHour = h.totalSales >= threshold
        }).OrderBy(p => p.Hour).ToList();
    }

    // ========== PRODUCT REPORTS ==========

    public async Task<List<TopSellingProductDto>> GetTopSellingProductsAsync(DateTime from, DateTime to, int limit, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var products = await _reportRepository.GetTopSellingProductsAsync(from, to, limit, tenantId, cancellationToken);
        var totalRevenue = products.Sum(p => p.revenue);

        return products.Select(p => new TopSellingProductDto
        {
            ProductId = p.productId,
            ProductName = p.productName,
            CategoryName = p.categoryName,
            QuantitySold = p.quantity,
            TotalRevenue = p.revenue,
            AveragePrice = p.quantity > 0 ? p.revenue / p.quantity : 0,
            Percentage = totalRevenue > 0 ? (p.revenue / totalRevenue) * 100 : 0
        }).ToList();
    }

    public async Task<AbcAnalysisSummaryDto> GetProductAbcAnalysisAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var allProducts = await _reportRepository.GetAllProductSalesAsync(from, to, tenantId, cancellationToken);
        
        if (!allProducts.Any())
        {
            return new AbcAnalysisSummaryDto();
        }

        // Ordenar por revenue descendente
        var orderedProducts = allProducts.OrderByDescending(p => p.revenue).ToList();
        var totalRevenue = orderedProducts.Sum(p => p.revenue);
        var totalProducts = orderedProducts.Count;

        var analysis = new List<ProductAbcAnalysisDto>();
        decimal cumulativeRevenue = 0;

        foreach (var product in orderedProducts)
        {
            cumulativeRevenue += product.revenue;
            var cumulativePercentage = (cumulativeRevenue / totalRevenue) * 100;

            // Clasificación ABC según curva de Pareto
            var category = cumulativePercentage switch
            {
                <= 80 => ProductAbcCategory.A,
                <= 95 => ProductAbcCategory.B,
                _ => ProductAbcCategory.C
            };

            analysis.Add(new ProductAbcAnalysisDto
            {
                ProductId = product.productId,
                ProductName = product.productName,
                CategoryName = product.categoryName,
                QuantitySold = product.quantity,
                TotalRevenue = product.revenue,
                RevenuePercentage = (product.revenue / totalRevenue) * 100,
                CumulativePercentage = cumulativePercentage,
                AbcCategory = category,
                AbcCategoryName = category.ToString()
            });
        }

        // Calcular stats por categoría
        var categoryA = analysis.Where(p => p.AbcCategory == ProductAbcCategory.A).ToList();
        var categoryB = analysis.Where(p => p.AbcCategory == ProductAbcCategory.B).ToList();
        var categoryC = analysis.Where(p => p.AbcCategory == ProductAbcCategory.C).ToList();

        return new AbcAnalysisSummaryDto
        {
            CategoryA = new CategoryStats
            {
                ProductCount = categoryA.Count,
                ProductPercentage = (decimal)categoryA.Count / totalProducts * 100,
                TotalRevenue = categoryA.Sum(p => p.TotalRevenue),
                RevenuePercentage = categoryA.Sum(p => p.RevenuePercentage)
            },
            CategoryB = new CategoryStats
            {
                ProductCount = categoryB.Count,
                ProductPercentage = (decimal)categoryB.Count / totalProducts * 100,
                TotalRevenue = categoryB.Sum(p => p.TotalRevenue),
                RevenuePercentage = categoryB.Sum(p => p.RevenuePercentage)
            },
            CategoryC = new CategoryStats
            {
                ProductCount = categoryC.Count,
                ProductPercentage = (decimal)categoryC.Count / totalProducts * 100,
                TotalRevenue = categoryC.Sum(p => p.TotalRevenue),
                RevenuePercentage = categoryC.Sum(p => p.RevenuePercentage)
            },
            Products = analysis
        };
    }

    public async Task<List<LowMovementProductDto>> GetLowMovementProductsAsync(DateTime from, DateTime to, int maxSalesThreshold, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var products = await _reportRepository.GetLowMovementProductsAsync(from, to, maxSalesThreshold, tenantId, cancellationToken);

        return products.Select(p => new LowMovementProductDto
        {
            ProductId = p.productId,
            ProductName = p.productName,
            CategoryName = p.categoryName,
            QuantitySold = p.quantity,
            TotalRevenue = p.revenue,
            LastSaleDate = p.lastSaleDate,
            DaysSinceLastSale = p.lastSaleDate.HasValue 
                ? (int)(DateTime.UtcNow - p.lastSaleDate.Value).TotalDays 
                : int.MaxValue,
            IsActive = true // Esto se puede obtener del producto si es necesario
        }).OrderBy(p => p.QuantitySold).ToList();
    }

    public async Task<List<CategoryPerformanceDto>> GetCategoryPerformanceAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var categories = await _reportRepository.GetCategoryPerformanceAsync(from, to, tenantId, cancellationToken);
        var totalRevenue = categories.Sum(c => c.totalRevenue);

        return categories.Select(c => new CategoryPerformanceDto
        {
            CategoryId = c.categoryId,
            CategoryName = c.categoryName,
            ProductCount = c.productCount,
            TotalQuantitySold = c.totalQuantity,
            TotalRevenue = c.totalRevenue,
            AverageProductRevenue = c.productCount > 0 ? c.totalRevenue / c.productCount : 0,
            Percentage = totalRevenue > 0 ? (c.totalRevenue / totalRevenue) * 100 : 0
        }).OrderByDescending(c => c.TotalRevenue).ToList();
    }

    public async Task<List<PopularModifierDto>> GetPopularModifiersAsync(DateTime from, DateTime to, int limit, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var modifiers = await _reportRepository.GetPopularModifiersAsync(from, to, limit, tenantId, cancellationToken);

        return modifiers.Select(m => new PopularModifierDto
        {
            ModifierId = m.modifierId,
            ModifierName = m.modifierName,
            TimesOrdered = m.timesOrdered,
            TotalRevenue = m.totalRevenue,
            PopularWithProducts = m.topProducts
        }).ToList();
    }

    // ========== EMPLOYEE REPORTS ==========

    public async Task<List<EmployeePerformanceDto>> GetEmployeePerformanceAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var employees = await _reportRepository.GetEmployeePerformanceAsync(from, to, tenantId, cancellationToken);

        var performanceList = employees.Select(e =>
        {
            var cancellationRate = e.totalOrders > 0 ? (decimal)e.cancelledOrders / e.totalOrders * 100 : 0;
            var avgTicket = e.customerCount > 0 ? e.totalSales / e.customerCount : 0;

            // Score simple (puede mejorarse con pesos más sofisticados)
            var score = CalculatePerformanceScore(e.totalSales, e.totalOrders, cancellationRate, e.avgServiceTimeMinutes);

            return new EmployeePerformanceDto
            {
                EmployeeId = e.employeeId,
                EmployeeName = e.employeeName,
                TotalOrders = e.totalOrders,
                CompletedOrders = e.completedOrders,
                CancelledOrders = e.cancelledOrders,
                CancellationRate = cancellationRate,
                TotalSales = e.totalSales,
                AverageTicket = avgTicket,
                TotalCustomers = e.customerCount,
                AverageServiceTimeMinutes = e.avgServiceTimeMinutes,
                PerformanceScore = score
            };
        }).OrderByDescending(e => e.PerformanceScore).ToList();

        // Asignar ranking
        for (int i = 0; i < performanceList.Count; i++)
        {
            performanceList[i].Rank = i + 1;
        }

        return performanceList;
    }

    public async Task<TopPerformerDto?> GetTopPerformerAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var employees = await GetEmployeePerformanceAsync(from, to, tenantId, cancellationToken);
        var topEmployee = employees.FirstOrDefault();

        if (topEmployee == null) return null;

        return new TopPerformerDto
        {
            EmployeeId = topEmployee.EmployeeId,
            EmployeeName = topEmployee.EmployeeName,
            TotalSales = topEmployee.TotalSales,
            TotalOrders = topEmployee.TotalOrders,
            AverageTicket = topEmployee.AverageTicket,
            PerformanceScore = topEmployee.PerformanceScore,
            Highlight = "Mejor vendedor del período"
        };
    }

    public async Task<List<EmployeeErrorsDto>> GetEmployeeErrorsAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var employees = await _reportRepository.GetEmployeePerformanceAsync(from, to, tenantId, cancellationToken);

        return employees.Select(e => new EmployeeErrorsDto
        {
            EmployeeId = e.employeeId,
            EmployeeName = e.employeeName,
            OrdersCancelled = e.cancelledOrders,
            OrdersModified = 0, // TODO: Implementar tracking de modificaciones
            ComplaintsReceived = 0, // TODO: Implementar sistema de quejas
            ErrorRate = e.totalOrders > 0 ? (decimal)e.cancelledOrders / e.totalOrders * 100 : 0
        }).OrderByDescending(e => e.ErrorRate).ToList();
    }

    // ========== DASHBOARD ==========

    public async Task<DashboardKpisDto> GetDashboardKpisAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);

        // Ventas de hoy
        var (todaySales, todayInvoices, todayAvgTicket) = await _reportRepository
            .GetSalesTotalsAsync(today, today, tenantId, cancellationToken);

        // Ventas de ayer
        var (yesterdaySales, _, _) = await _reportRepository
            .GetSalesTotalsAsync(yesterday, yesterday, tenantId, cancellationToken);

        // Estado de mesas
        var (tablesOccupied, totalTables) = await _reportRepository
            .GetTableOccupancyAsync(tenantId, cancellationToken);

        // Estado de pedidos
        var (ordersPending, ordersInProgress, ordersReady) = await _reportRepository
            .GetOrderStatusCountsAsync(tenantId, cancellationToken);

        // Métodos de pago del día
        var paymentMethods = await GetSalesByPaymentMethodAsync(today, today, tenantId, cancellationToken);

        // Top 5 productos del día
        var topProducts = await GetTopSellingProductsAsync(today, today, 5, tenantId, cancellationToken);

        // Generar alertas
        var alerts = await GenerateDashboardAlertsAsync(todaySales, yesterdaySales, ordersReady, tenantId, cancellationToken);

        return new DashboardKpisDto
        {
            TodaySales = todaySales,
            TodayInvoices = todayInvoices,
            TodayAverageTicket = todayAvgTicket,
            YesterdaySales = yesterdaySales,
            SalesGrowth = todaySales - yesterdaySales,
            SalesGrowthPercentage = yesterdaySales > 0 
                ? ((todaySales - yesterdaySales) / yesterdaySales) * 100 
                : 0,
            TablesOccupied = tablesOccupied,
            TotalTables = totalTables,
            OccupancyPercentage = totalTables > 0 
                ? (decimal)tablesOccupied / totalTables * 100 
                : 0,
            OrdersPending = ordersPending,
            OrdersInProgress = ordersInProgress,
            OrdersReady = ordersReady,
            PaymentMethodBreakdown = paymentMethods,
            TopProducts = topProducts,
            Alerts = alerts
        };
    }

    // ========== CONTROL REPORTS ==========

    public async Task<List<CancelledInvoicesReportDto>> GetCancelledInvoicesAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var cancelled = await _reportRepository.GetCancelledInvoicesAsync(from, to, tenantId, cancellationToken);

        return cancelled.Select(c => new CancelledInvoicesReportDto
        {
            InvoiceId = c.invoiceId,
            InvoiceNumber = c.invoiceNumber,
            OrderNumber = c.orderNumber,
            EmployeeName = c.employeeName,
            Amount = c.amount,
            CancelReason = c.cancelReason,
            CancelledAt = c.cancelledAt,
            CancelledBy = c.cancelledBy
        }).OrderByDescending(c => c.CancelledAt).ToList();
    }

    public async Task<List<DiscountsReportDto>> GetDiscountsReportAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var discounts = await _reportRepository.GetDiscountsAsync(from, to, tenantId, cancellationToken);

        return discounts.Select(d => new DiscountsReportDto
        {
            InvoiceId = d.invoiceId,
            InvoiceNumber = d.invoiceNumber,
            EmployeeName = d.employeeName,
            OriginalAmount = d.originalAmount,
            DiscountPercentage = d.discountPercentage,
            DiscountAmount = d.discountAmount,
            FinalAmount = d.finalAmount,
            AppliedAt = d.createdAt,
            AppliedBy = d.appliedBy
        }).OrderByDescending(d => d.AppliedAt).ToList();
    }

    public async Task<DiscountsSummaryDto> GetDiscountsSummaryAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var discounts = await _reportRepository.GetDiscountsAsync(from, to, tenantId, cancellationToken);
        var (totalSales, _, _) = await _reportRepository.GetSalesTotalsAsync(from, to, tenantId, cancellationToken);

        var totalDiscountAmount = discounts.Sum(d => d.discountAmount);
        var avgDiscountPercentage = discounts.Any() 
            ? discounts.Average(d => d.discountPercentage) 
            : 0;

        // Agrupar por empleado
        var byEmployee = discounts
            .Where(d => !string.IsNullOrEmpty(d.appliedBy))
            .GroupBy(d => new { EmployeeName = d.employeeName ?? d.appliedBy! })
            .Select(g => new DiscountsByEmployeeDto
            {
                EmployeeName = g.Key.EmployeeName,
                DiscountsApplied = g.Count(),
                TotalDiscountAmount = g.Sum(d => d.discountAmount),
                AverageDiscountPercentage = g.Average(d => d.discountPercentage)
            })
            .OrderByDescending(e => e.TotalDiscountAmount)
            .ToList();

        return new DiscountsSummaryDto
        {
            TotalDiscountsApplied = discounts.Count,
            TotalDiscountAmount = totalDiscountAmount,
            AverageDiscountPercentage = avgDiscountPercentage,
            RevenueImpactPercentage = totalSales > 0 
                ? (totalDiscountAmount / (totalSales + totalDiscountAmount)) * 100 
                : 0,
            ByEmployee = byEmployee
        };
    }

    // ========== HELPER METHODS ==========

    private async Task<List<DashboardAlertDto>> GenerateDashboardAlertsAsync(
        decimal todaySales, 
        decimal yesterdaySales, 
        int ordersReady, 
        Guid tenantId, 
        CancellationToken cancellationToken)
    {
        var alerts = new List<DashboardAlertDto>();

        // Alerta de ventas bajas (>20% menos que ayer)
        if (yesterdaySales > 0 && todaySales < yesterdaySales * 0.8m)
        {
            var drop = ((yesterdaySales - todaySales) / yesterdaySales) * 100;
            alerts.Add(new DashboardAlertDto
            {
                Type = AlertType.LowSales,
                Severity = AlertSeverity.Warning,
                Message = $"Ventas {drop:F1}% más bajas que ayer",
                Timestamp = DateTime.UtcNow
            });
        }

        // Alerta de pedidos listos acumulados
        if (ordersReady > 5)
        {
            alerts.Add(new DashboardAlertDto
            {
                Type = AlertType.LongWaitTime,
                Severity = AlertSeverity.Warning,
                Message = $"{ordersReady} pedidos listos esperando entrega",
                Timestamp = DateTime.UtcNow
            });
        }

        // TODO: Agregar más alertas según necesidades

        return alerts;
    }

    private decimal CalculatePerformanceScore(
        decimal totalSales, 
        int totalOrders, 
        decimal cancellationRate, 
        double avgServiceTime)
    {
        // Score simple 0-100
        // 50% ventas, 30% órdenes, 10% tasa de cancelación, 10% tiempo de servicio
        
        decimal salesScore = Math.Min(totalSales / 1000m * 50, 50); // Normalizar a 50 puntos
        decimal ordersScore = Math.Min(totalOrders / 50m * 30, 30); // Normalizar a 30 puntos
        decimal cancellationScore = Math.Max(10 - (cancellationRate / 2), 0); // 10 puntos máx
        decimal timeScore = avgServiceTime > 0 && avgServiceTime < 30 ? 10 : 5; // 10 si <30 min

        return salesScore + ordersScore + cancellationScore + timeScore;
    }

    private async Task<PeriodInfo> GetPeriodDataAsync(DateTime from, DateTime to, Guid tenantId, CancellationToken cancellationToken)
    {
        var (totalSales, invoiceCount, averageTicket) = await _reportRepository
            .GetSalesTotalsAsync(from, to, tenantId, cancellationToken);

        var dailyData = await _reportRepository
            .GetDailySalesAsync(from, to, tenantId, cancellationToken);

        var customerCount = dailyData.Sum(d => d.customerCount);

        return new PeriodInfo
        {
            StartDate = from,
            EndDate = to,
            TotalSales = totalSales,
            InvoiceCount = invoiceCount,
            AverageTicket = averageTicket,
            CustomerCount = customerCount
        };
    }

    private ComparisonMetrics CalculateComparison(PeriodInfo current, PeriodInfo previous)
    {
        return new ComparisonMetrics
        {
            SalesGrowth = current.TotalSales - previous.TotalSales,
            SalesGrowthPercentage = previous.TotalSales > 0 
                ? ((current.TotalSales - previous.TotalSales) / previous.TotalSales) * 100 
                : 0,
            InvoiceGrowth = current.InvoiceCount - previous.InvoiceCount,
            InvoiceGrowthPercentage = previous.InvoiceCount > 0 
                ? ((decimal)(current.InvoiceCount - previous.InvoiceCount) / previous.InvoiceCount) * 100 
                : 0,
            TicketGrowth = current.AverageTicket - previous.AverageTicket,
            TicketGrowthPercentage = previous.AverageTicket > 0 
                ? ((current.AverageTicket - previous.AverageTicket) / previous.AverageTicket) * 100 
                : 0
        };
    }

    private (DateTime currentFrom, DateTime currentTo, DateTime previousFrom, DateTime previousTo) GetComparativePeriods(PeriodType periodType)
    {
        var now = DateTime.UtcNow.Date;

        return periodType switch
        {
            PeriodType.Today => (now, now, now.AddDays(-1), now.AddDays(-1)),
            PeriodType.Yesterday => (now.AddDays(-1), now.AddDays(-1), now.AddDays(-2), now.AddDays(-2)),
            PeriodType.ThisWeek => (now.AddDays(-(int)now.DayOfWeek), now, now.AddDays(-(int)now.DayOfWeek - 7), now.AddDays(-7)),
            PeriodType.LastWeek => (now.AddDays(-(int)now.DayOfWeek - 7), now.AddDays(-(int)now.DayOfWeek - 1), now.AddDays(-(int)now.DayOfWeek - 14), now.AddDays(-(int)now.DayOfWeek - 8)),
            PeriodType.ThisMonth => (new DateTime(now.Year, now.Month, 1), now, new DateTime(now.Year, now.Month, 1).AddMonths(-1), new DateTime(now.Year, now.Month, 1).AddDays(-1)),
            PeriodType.LastMonth => (new DateTime(now.Year, now.Month, 1).AddMonths(-1), new DateTime(now.Year, now.Month, 1).AddDays(-1), new DateTime(now.Year, now.Month, 1).AddMonths(-2), new DateTime(now.Year, now.Month, 1).AddMonths(-1).AddDays(-1)),
            _ => throw new ArgumentException($"Unsupported period type: {periodType}")
        };
    }

    private TimeSlotType GetTimeSlotForHour(int hour)
    {
        return hour switch
        {
            >= 6 and < 11 => TimeSlotType.Breakfast,
            >= 11 and < 15 => TimeSlotType.Lunch,
            >= 15 and < 18 => TimeSlotType.Afternoon,
            >= 18 and < 23 => TimeSlotType.Dinner,
            _ => TimeSlotType.LateNight
        };
    }

    private string GetTimeRangeForSlot(TimeSlotType slot)
    {
        return slot switch
        {
            TimeSlotType.Breakfast => "06:00 - 11:00",
            TimeSlotType.Lunch => "11:00 - 15:00",
            TimeSlotType.Afternoon => "15:00 - 18:00",
            TimeSlotType.Dinner => "18:00 - 23:00",
            TimeSlotType.LateNight => "23:00 - 06:00",
            _ => ""
        };
    }
}
