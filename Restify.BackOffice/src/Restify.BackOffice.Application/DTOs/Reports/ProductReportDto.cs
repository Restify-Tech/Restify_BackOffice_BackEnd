using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.DTOs.Reports;

/// <summary>
/// Top productos más vendidos
/// </summary>
public class TopSellingProductDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public int QuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AveragePrice { get; set; }
    public decimal Percentage { get; set; } // % del total de ventas
}

/// <summary>
/// Análisis ABC de productos (curva de Pareto)
/// </summary>
public class ProductAbcAnalysisDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public int QuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal RevenuePercentage { get; set; }
    public decimal CumulativePercentage { get; set; }
    public ProductAbcCategory AbcCategory { get; set; }
    public string AbcCategoryName { get; set; } = string.Empty;
}

/// <summary>
/// Resumen de análisis ABC
/// </summary>
public class AbcAnalysisSummaryDto
{
    public CategoryStats CategoryA { get; set; } = new();
    public CategoryStats CategoryB { get; set; } = new();
    public CategoryStats CategoryC { get; set; } = new();
    public List<ProductAbcAnalysisDto> Products { get; set; } = new();
}

public class CategoryStats
{
    public int ProductCount { get; set; }
    public decimal ProductPercentage { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal RevenuePercentage { get; set; }
}

/// <summary>
/// Productos de bajo movimiento
/// </summary>
public class LowMovementProductDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public int QuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
    public int DaysSinceLastSale { get; set; }
    public DateTime? LastSaleDate { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Rendimiento por categoría
/// </summary>
public class CategoryPerformanceDto
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageProductRevenue { get; set; }
    public decimal Percentage { get; set; }
}

/// <summary>
/// Modificadores más populares
/// </summary>
public class PopularModifierDto
{
    public Guid ModifierId { get; set; }
    public string ModifierName { get; set; } = string.Empty;
    public int TimesOrdered { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<string> PopularWithProducts { get; set; } = new(); // Top 3 productos
}
