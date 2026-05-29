using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs.Reports;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Constants;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Api.Controllers;

/// <summary>
/// Controller de reportes agregados para el frontend.
/// Ruta: /api/reports
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(
        IReportService reportService,
        ICurrentUserService currentUserService,
        ILogger<ReportsController> logger)
    {
        _reportService = reportService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// GET /api/reports/sales?dateFrom=2026-01-01&dateTo=2026-01-31&paymentMethod=Cash
    /// Reporte de ventas agregado
    /// </summary>
    [HttpGet("sales")]
    public async Task<IActionResult> GetSalesReport(
        [FromQuery] string dateFrom,
        [FromQuery] string dateTo,
        [FromQuery] string? paymentMethod = null,
        CancellationToken cancellationToken = default)
    {
        if (!TryParseDateRange(dateFrom, dateTo, out var from, out var to))
            return BadRequest(Result<FrontendSalesReportDto>.Failure(ReportConstants.ErrorInvalidDateRange));

        var tenantId = _currentUserService.TenantId;
        if (!tenantId.HasValue)
            return Unauthorized();

        var result = await _reportService.GetFrontendSalesReportAsync(from, to, paymentMethod, tenantId.Value, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// GET /api/reports/products?dateFrom=2026-01-01&dateTo=2026-01-31&categoryId=...
    /// Reporte de productos agregado
    /// </summary>
    [HttpGet("products")]
    public async Task<IActionResult> GetProductReport(
        [FromQuery] string dateFrom,
        [FromQuery] string dateTo,
        [FromQuery] string? categoryId = null,
        CancellationToken cancellationToken = default)
    {
        if (!TryParseDateRange(dateFrom, dateTo, out var from, out var to))
            return BadRequest(Result<FrontendProductReportDto>.Failure(ReportConstants.ErrorInvalidDateRange));

        var tenantId = _currentUserService.TenantId;
        if (!tenantId.HasValue)
            return Unauthorized();

        var result = await _reportService.GetFrontendProductReportAsync(from, to, categoryId, tenantId.Value, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// GET /api/reports/staff?dateFrom=2026-01-01&dateTo=2026-01-31&staffId=...
    /// Reporte de personal agregado
    /// </summary>
    [HttpGet("staff")]
    public async Task<IActionResult> GetStaffReport(
        [FromQuery] string dateFrom,
        [FromQuery] string dateTo,
        [FromQuery] string? staffId = null,
        CancellationToken cancellationToken = default)
    {
        if (!TryParseDateRange(dateFrom, dateTo, out var from, out var to))
            return BadRequest(Result<FrontendStaffReportDto>.Failure(ReportConstants.ErrorInvalidDateRange));

        var tenantId = _currentUserService.TenantId;
        if (!tenantId.HasValue)
            return Unauthorized();

        var result = await _reportService.GetFrontendStaffReportAsync(from, to, staffId, tenantId.Value, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// POST /api/reports/export
    /// Exportar reporte a CSV
    /// </summary>
    [HttpPost("export")]
    public async Task<IActionResult> ExportReport(
        [FromBody] ExportReportRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var tenantId = _currentUserService.TenantId;
        if (!tenantId.HasValue)
            return Unauthorized();

        var result = await _reportService.ExportReportAsync(request, tenantId.Value, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { isSuccess = false, error = result.Error });

        var fileName = request.ReportType.ToLowerInvariant() switch
        {
            ReportConstants.ReportTypeSales => ReportConstants.CsvSalesFileName,
            ReportConstants.ReportTypeProducts => ReportConstants.CsvProductsFileName,
            ReportConstants.ReportTypeStaff => ReportConstants.CsvStaffFileName,
            _ => "reporte.csv"
        };

        return File(result.Data!, ReportConstants.CsvContentType, fileName);
    }

    /// <summary>
    /// GET /api/reports/dashboard-summary
    /// Resumen rapido para el dashboard principal
    /// </summary>
    [HttpGet("dashboard-summary")]
    public async Task<IActionResult> GetDashboardSummary(CancellationToken cancellationToken = default)
    {
        var tenantId = _currentUserService.TenantId;
        if (!tenantId.HasValue)
            return Unauthorized();

        var result = await _reportService.GetFrontendDashboardSummaryAsync(tenantId.Value, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// GET /api/reports/insights
    /// Insights automaticos basados en los datos del dia actual.
    /// </summary>
    [HttpGet("insights")]
    public async Task<IActionResult> GetInsights(CancellationToken cancellationToken = default)
    {
        var tenantId = _currentUserService.TenantId;
        if (!tenantId.HasValue)
            return Unauthorized();

        try
        {
            var result = await _reportService.GetInsightsAsync(tenantId.Value, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo insights del dia");
            return StatusCode(500, Result<object>.Failure("Error interno del servidor"));
        }
    }

    /// <summary>
    /// GET /api/reports/my-ranking
    /// Ranking del mesero autenticado vs sus companeros en el dia actual.
    /// </summary>
    [HttpGet("my-ranking")]
    public async Task<IActionResult> GetMyRanking(CancellationToken cancellationToken = default)
    {
        var tenantId = _currentUserService.TenantId;
        if (!tenantId.HasValue)
            return Unauthorized();

        var identifier = _currentUserService.Email;
        if (string.IsNullOrEmpty(identifier))
            return Unauthorized();

        try
        {
            var result = await _reportService.GetMyRankingAsync(tenantId.Value, identifier, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo ranking del mesero");
            return StatusCode(500, Result<object>.Failure("Error interno del servidor"));
        }
    }

    // ========== HELPERS ==========

    private static bool TryParseDateRange(string dateFrom, string dateTo, out DateTime from, out DateTime to)
    {
        from = default;
        to = default;

        if (string.IsNullOrEmpty(dateFrom) || string.IsNullOrEmpty(dateTo))
            return false;

        if (!DateTime.TryParse(dateFrom, out from) || !DateTime.TryParse(dateTo, out to))
            return false;

        if (from > to)
            return false;

        if ((to - from).Days > ReportConstants.MaxDateRangeDays)
            return false;

        return true;
    }
}
