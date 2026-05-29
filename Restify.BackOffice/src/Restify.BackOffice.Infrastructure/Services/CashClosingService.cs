using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;
using System.Text.Json;

namespace Restify.BackOffice.Infrastructure.Services;

public class CashClosingService : ICashClosingService
{
    private readonly BackOfficeDbContext _context;
    private readonly IFileStorageService _fileStorage;
    private readonly ICurrentUserService _currentUserService;

    static CashClosingService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public CashClosingService(
        BackOfficeDbContext context,
        IFileStorageService fileStorage,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _fileStorage = fileStorage;
        _currentUserService = currentUserService;
    }

    public async Task<Result<CashClosingDto>> InitiateAsync(InitiateCashClosingRequest request, CancellationToken cancellationToken = default)
    {
        var session = await _context.CashRegisterSessions
            .Include(s => s.CashRegister)
            .Include(s => s.Movements)
            .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

        if (session == null)
            return Result<CashClosingDto>.Failure("Sesion de caja no encontrada");

        // Verificar que no exista ya un cierre en Draft o PendingReview para esta sesion
        var existingClosing = await _context.CashClosings
            .FirstOrDefaultAsync(c => c.CashRegisterSessionId == request.SessionId
                && (c.Status == CashClosingStatus.Draft || c.Status == CashClosingStatus.PendingReview),
                cancellationToken);

        if (existingClosing != null)
            return Result<CashClosingDto>.Failure("Ya existe un cierre en proceso para esta sesion");

        // Calcular el total esperado desde los movimientos de la sesion
        var totalSales = session.Movements
            .Where(m => m.Type == CashMovementType.Sale)
            .Sum(m => m.Amount);
        var totalDeposits = session.Movements
            .Where(m => m.Type == CashMovementType.Deposit)
            .Sum(m => m.Amount);
        var totalWithdrawals = session.Movements
            .Where(m => m.Type == CashMovementType.Withdrawal)
            .Sum(m => m.Amount);
        var totalExpenses = session.Movements
            .Where(m => m.Type == CashMovementType.Expense)
            .Sum(m => m.Amount);

        var totalExpected = session.OpeningBalance + totalSales + totalDeposits - totalWithdrawals - totalExpenses;

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;

        var closing = new CashClosing
        {
            TenantId = tenantId,
            CashRegisterSessionId = request.SessionId,
            ClosedBy = request.ClosedBy,
            ClosedByName = request.ClosedByName,
            TotalExpected = totalExpected,
            TotalCounted = 0,
            Difference = 0,
            Status = CashClosingStatus.Draft,
            ClosingDate = DateTime.UtcNow
        };

        _context.CashClosings.Add(closing);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<CashClosingDto>.Success(ToDto(closing, session.CashRegister?.Name ?? ""));
    }

    public async Task<Result<CashClosingDto>> SubmitDenominationsAsync(SubmitDenominationsRequest request, CancellationToken cancellationToken = default)
    {
        var closing = await _context.CashClosings
            .Include(c => c.CashRegisterSession)
                .ThenInclude(s => s.CashRegister)
            .FirstOrDefaultAsync(c => c.Id == request.ClosingId, cancellationToken);

        if (closing == null)
            return Result<CashClosingDto>.Failure("Cierre de caja no encontrado");

        if (closing.Status != CashClosingStatus.Draft)
            return Result<CashClosingDto>.Failure("El cierre solo puede actualizarse en estado Borrador");

        // Calcular TotalCounted desde denominaciones
        decimal totalCounted = 0;
        foreach (var denom in request.Denominations)
        {
            if (decimal.TryParse(denom.Key, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var denominationValue))
            {
                totalCounted += denominationValue * denom.Value;
            }
        }

        closing.DenominationsJson = JsonSerializer.Serialize(request.Denominations);
        closing.TotalCounted = totalCounted;
        closing.Difference = totalCounted - closing.TotalExpected;
        closing.DifferenceReason = request.DifferenceReason;
        closing.BankDepositAmount = request.BankDepositAmount;
        closing.BankName = request.BankName;
        closing.BankReference = request.BankReference;
        closing.Status = CashClosingStatus.PendingReview;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<CashClosingDto>.Success(ToDto(closing, closing.CashRegisterSession?.CashRegister?.Name ?? ""));
    }

    public async Task<Result<CashClosingDto>> ApproveAsync(ApproveCashClosingRequest request, CancellationToken cancellationToken = default)
    {
        var closing = await _context.CashClosings
            .Include(c => c.CashRegisterSession)
                .ThenInclude(s => s.CashRegister)
            .FirstOrDefaultAsync(c => c.Id == request.ClosingId, cancellationToken);

        if (closing == null)
            return Result<CashClosingDto>.Failure("Cierre de caja no encontrado");

        if (closing.Status != CashClosingStatus.PendingReview)
            return Result<CashClosingDto>.Failure("El cierre debe estar en estado Pendiente de Revision para aprobarlo");

        if (request.Approved)
        {
            closing.Status = CashClosingStatus.Approved;
            closing.ApprovedBy = request.ApprovedBy;
            closing.ApprovedAt = DateTime.UtcNow;
            closing.SupervisedBy = request.ApprovedBy;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.RejectionReason))
                return Result<CashClosingDto>.Failure("Debe indicar el motivo del rechazo");

            closing.Status = CashClosingStatus.Disputed;
            closing.RejectionReason = request.RejectionReason;
            closing.SupervisedBy = request.ApprovedBy;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<CashClosingDto>.Success(ToDto(closing, closing.CashRegisterSession?.CashRegister?.Name ?? ""));
    }

    public async Task<Result<IEnumerable<CashClosingSummaryDto>>> GetHistoryAsync(
        Guid? cashRegisterId,
        DateTime? from,
        DateTime? to,
        CancellationToken cancellationToken = default)
    {
        var query = _context.CashClosings
            .Include(c => c.CashRegisterSession)
                .ThenInclude(s => s.CashRegister)
            .AsQueryable();

        if (cashRegisterId.HasValue)
            query = query.Where(c => c.CashRegisterSession.CashRegisterId == cashRegisterId.Value);

        if (from.HasValue)
            query = query.Where(c => c.ClosingDate >= from.Value);

        if (to.HasValue)
            query = query.Where(c => c.ClosingDate <= to.Value);

        var closings = await query
            .OrderByDescending(c => c.ClosingDate)
            .ToListAsync(cancellationToken);

        var dtos = closings.Select(c => new CashClosingSummaryDto(
            c.Id,
            c.CashRegisterSessionId,
            c.CashRegisterSession?.CashRegister?.Name ?? "",
            c.ClosedByName,
            c.TotalCounted,
            c.TotalExpected,
            c.Difference,
            c.Status,
            c.ClosingDate
        ));

        return Result<IEnumerable<CashClosingSummaryDto>>.Success(dtos);
    }

    public async Task<Result<CashClosingDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var closing = await _context.CashClosings
            .Include(c => c.CashRegisterSession)
                .ThenInclude(s => s.CashRegister)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (closing == null)
            return Result<CashClosingDto>.Failure("Cierre de caja no encontrado");

        return Result<CashClosingDto>.Success(ToDto(closing, closing.CashRegisterSession?.CashRegister?.Name ?? ""));
    }

    public async Task<Result<string>> GenerateZReportAsync(Guid closingId, CancellationToken cancellationToken = default)
    {
        var closing = await _context.CashClosings
            .Include(c => c.CashRegisterSession)
                .ThenInclude(s => s.CashRegister)
            .Include(c => c.CashRegisterSession.Movements)
            .FirstOrDefaultAsync(c => c.Id == closingId, cancellationToken);

        if (closing == null)
            return Result<string>.Failure("Cierre de caja no encontrado");

        var session = closing.CashRegisterSession;
        var cashRegisterName = session?.CashRegister?.Name ?? "Caja";

        // Calcular totales para el reporte
        var totalSales = session?.Movements
            .Where(m => m.Type == CashMovementType.Sale)
            .Sum(m => m.Amount) ?? 0;
        var totalDeposits = session?.Movements
            .Where(m => m.Type == CashMovementType.Deposit)
            .Sum(m => m.Amount) ?? 0;
        var totalWithdrawals = session?.Movements
            .Where(m => m.Type == CashMovementType.Withdrawal)
            .Sum(m => m.Amount) ?? 0;
        var totalExpenses = session?.Movements
            .Where(m => m.Type == CashMovementType.Expense)
            .Sum(m => m.Amount) ?? 0;

        // Generar PDF con QuestPDF
        var pdfBytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(header =>
                {
                    header.Column(col =>
                    {
                        col.Item().AlignCenter().Text("REPORTE Z — CIERRE DE CAJA")
                            .Bold().FontSize(16);
                        col.Item().AlignCenter().Text(cashRegisterName)
                            .FontSize(12);
                        col.Item().AlignCenter().Text($"Fecha: {closing.ClosingDate:dd/MM/yyyy HH:mm}")
                            .FontSize(10);
                        col.Item().PaddingTop(5).LineHorizontal(1);
                    });
                });

                page.Content().PaddingTop(15).Element(content =>
                {
                    content.Column(col =>
                    {
                        // Informacion del cajero
                        col.Item().Text("INFORMACION DEL CAJERO").Bold().FontSize(11);
                        col.Item().PaddingLeft(10).Text($"Cajero: {closing.ClosedByName}");
                        col.Item().PaddingLeft(10).Text($"Supervisor: {closing.SupervisedByName ?? "-"}");
                        col.Item().PaddingVertical(5).LineHorizontal(0.5f);

                        // Resumen de movimientos
                        col.Item().Text("RESUMEN DE MOVIMIENTOS").Bold().FontSize(11);
                        col.Item().Element(c =>
                        {
                            c.Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(2);
                                });

                                BuildZRow(table, "Saldo inicial:", session?.OpeningBalance ?? 0);
                                BuildZRow(table, "Total ventas:", totalSales);
                                BuildZRow(table, "Total depositos:", totalDeposits);
                                BuildZRow(table, "Total retiros:", totalWithdrawals);
                                BuildZRow(table, "Total gastos:", totalExpenses);
                            });
                        });
                        col.Item().PaddingVertical(5).LineHorizontal(0.5f);

                        // Arqueo
                        col.Item().Text("ARQUEO DE CAJA").Bold().FontSize(11);
                        col.Item().Element(c =>
                        {
                            c.Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(2);
                                });

                                BuildZRow(table, "Total esperado (sistema):", closing.TotalExpected);
                                BuildZRow(table, "Total contado (cajero):", closing.TotalCounted);
                                BuildZRow(table, "Diferencia:", closing.Difference);
                            });
                        });

                        if (!string.IsNullOrEmpty(closing.DifferenceReason))
                        {
                            col.Item().PaddingTop(5).Text($"Motivo diferencia: {closing.DifferenceReason}");
                        }

                        col.Item().PaddingVertical(5).LineHorizontal(0.5f);

                        // Deposito bancario
                        if (closing.BankDepositAmount.HasValue)
                        {
                            col.Item().Text("DEPOSITO BANCARIO").Bold().FontSize(11);
                            col.Item().PaddingLeft(10).Text($"Banco: {closing.BankName ?? "-"}");
                            col.Item().PaddingLeft(10).Text($"Referencia: {closing.BankReference ?? "-"}");
                            col.Item().PaddingLeft(10).Text($"Monto: ${closing.BankDepositAmount:F2}");
                            col.Item().PaddingVertical(5).LineHorizontal(0.5f);
                        }

                        // Estado
                        col.Item().Text($"Estado: {closing.Status}").Bold();

                        // Firma
                        col.Item().PaddingTop(40).Element(c =>
                        {
                            c.Row(row =>
                            {
                                row.RelativeItem().Column(sig =>
                                {
                                    sig.Item().LineHorizontal(1);
                                    sig.Item().AlignCenter().Text("Firma Cajero");
                                    sig.Item().AlignCenter().Text(closing.ClosedByName);
                                });
                                row.ConstantItem(60);
                                row.RelativeItem().Column(sig =>
                                {
                                    sig.Item().LineHorizontal(1);
                                    sig.Item().AlignCenter().Text("Firma Supervisor");
                                    sig.Item().AlignCenter().Text(closing.SupervisedByName ?? "");
                                });
                            });
                        });
                    });
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Generado: ");
                    text.Span(DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"));
                });
            });
        }).GeneratePdf();

        // Guardar PDF
        using var ms = new MemoryStream(pdfBytes);
        var fileName = $"reporte_z_{closingId:N}.pdf";
        var relativePath = await _fileStorage.SaveFileAsync(ms, fileName, "cash-closings", cancellationToken);
        var url = _fileStorage.GetFileUrl(relativePath);

        // Actualizar URL en el cierre
        closing.ReportZUrl = url;
        await _context.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(url);
    }

    // ===== Helpers =====

    private static void BuildZRow(TableDescriptor table, string label, decimal value)
    {
        table.Cell().Padding(3).Text(label).Bold();
        table.Cell().Padding(3).AlignRight().Text($"${value:F2}");
    }

    private static CashClosingDto ToDto(CashClosing c, string cashRegisterName)
    {
        return new CashClosingDto(
            c.Id,
            c.CashRegisterSessionId,
            c.ClosedBy,
            c.ClosedByName,
            c.SupervisedBy,
            c.SupervisedByName,
            c.TotalCounted,
            c.TotalExpected,
            c.Difference,
            c.DifferenceReason,
            c.BankDepositAmount,
            c.BankName,
            c.BankReference,
            c.ReportZUrl,
            c.Status,
            c.ApprovedBy,
            c.ApprovedAt,
            c.RejectionReason,
            c.ClosingDate
        );
    }
}
