using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.DTOs;

public record CashClosingDto(
    Guid Id,
    Guid SessionId,
    string ClosedBy,
    string ClosedByName,
    string? SupervisedBy,
    string? SupervisedByName,
    decimal TotalCounted,
    decimal TotalExpected,
    decimal Difference,
    string? DifferenceReason,
    decimal? BankDepositAmount,
    string? BankName,
    string? BankReference,
    string? ReportZUrl,
    CashClosingStatus Status,
    string? ApprovedBy,
    DateTime? ApprovedAt,
    string? RejectionReason,
    DateTime ClosingDate
);

public record InitiateCashClosingRequest(
    Guid SessionId,
    string ClosedBy,
    string ClosedByName
);

public record SubmitDenominationsRequest(
    Guid ClosingId,
    Dictionary<string, int> Denominations,
    decimal BankDepositAmount,
    string? BankName,
    string? BankReference,
    string? DifferenceReason
);

public record ApproveCashClosingRequest(
    Guid ClosingId,
    string ApprovedBy,
    bool Approved,
    string? RejectionReason
);

public record CashClosingSummaryDto(
    Guid Id,
    Guid SessionId,
    string CashRegisterName,
    string ClosedByName,
    decimal TotalCounted,
    decimal TotalExpected,
    decimal Difference,
    CashClosingStatus Status,
    DateTime ClosingDate
);
