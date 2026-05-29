namespace Restify.BackOffice.Application.DTOs;

/// <summary>
/// DTO de sucursal
/// </summary>
public record BranchDto(
    Guid Id,
    string Name,
    string? Address,
    string? City,
    string? Phone,
    string? Email,
    string? Timezone,
    bool IsActive,
    string? OpeningHours,
    string? Notes,
    int ActiveTablesCount,
    int ActiveEmployeesCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

/// <summary>
/// Request para crear sucursal
/// </summary>
public record CreateBranchRequest(
    string Name,
    string? Address,
    string? City,
    string? Phone,
    string? Email,
    string? OpeningHours,
    string? Notes);

/// <summary>
/// Request para actualizar sucursal
/// </summary>
public record UpdateBranchRequest(
    string Name,
    string? Address,
    string? City,
    string? Phone,
    string? Email,
    bool IsActive,
    string? OpeningHours,
    string? Notes);

/// <summary>
/// Estadisticas de una sucursal
/// </summary>
public record BranchStatsDto(
    int TotalOrders,
    decimal TotalRevenue,
    int ActiveTables,
    int ActiveEmployees,
    decimal TodaySales,
    decimal WeekSales,
    decimal MonthSales);

/// <summary>
/// Reporte consolidado de todas las sucursales
/// </summary>
public record ConsolidatedBranchReportDto(
    IEnumerable<BranchSummaryDto> Branches,
    decimal GrandTotal,
    int TotalOrders);

/// <summary>
/// Resumen de una sucursal para reporte consolidado
/// </summary>
public record BranchSummaryDto(
    Guid BranchId,
    string BranchName,
    decimal Revenue,
    int Orders,
    decimal AvgTicket);

/// <summary>
/// DTO de asignacion de gerente a sucursal
/// </summary>
public record ManagerAssignmentDto(
    Guid Id,
    Guid BranchId,
    string BranchName,
    Guid UserId,
    string UserName,
    string UserEmail,
    bool CanApproveCashClosing,
    bool CanVoidOrders,
    decimal MaxDiscountPercent,
    bool IsActive,
    DateTime? ValidFrom,
    DateTime? ValidTo,
    DateTime CreatedAt);

/// <summary>
/// Request para crear asignacion de gerente
/// </summary>
public record CreateManagerAssignmentRequest(
    Guid BranchId,
    Guid UserId,
    string UserName,
    string UserEmail,
    bool CanApproveCashClosing,
    bool CanVoidOrders,
    decimal MaxDiscountPercent,
    DateTime? ValidFrom,
    DateTime? ValidTo);
