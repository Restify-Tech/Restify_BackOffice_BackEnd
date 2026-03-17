using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IFinancialReportService
{
    Task<Result<TrialBalanceDto>> GetTrialBalanceAsync(int year, int month, CancellationToken cancellationToken = default);
    Task<Result<IncomeStatementDto>> GetIncomeStatementAsync(int year, int month, CancellationToken cancellationToken = default);
    Task<Result<BalanceSheetDto>> GetBalanceSheetAsync(int year, int month, CancellationToken cancellationToken = default);
}
