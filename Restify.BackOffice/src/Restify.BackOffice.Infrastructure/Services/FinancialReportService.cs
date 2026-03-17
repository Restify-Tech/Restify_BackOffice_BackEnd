using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Application.Mappings;
using Restify.BackOffice.Domain.Enums;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Infrastructure.Services;

public class FinancialReportService : IFinancialReportService
{
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IAccountingAccountRepository _accountRepository;
    private readonly IAccountingPeriodRepository _periodRepository;

    public FinancialReportService(
        IJournalEntryRepository journalEntryRepository,
        IAccountingAccountRepository accountRepository,
        IAccountingPeriodRepository periodRepository)
    {
        _journalEntryRepository = journalEntryRepository;
        _accountRepository = accountRepository;
        _periodRepository = periodRepository;
    }

    public async Task<Result<TrialBalanceDto>> GetTrialBalanceAsync(int year, int month, CancellationToken cancellationToken = default)
    {
        // Buscar el periodo
        var period = await _periodRepository.GetByYearMonthAsync(year, month, cancellationToken);
        if (period == null)
            return Result<TrialBalanceDto>.Failure($"No se encontró el periodo contable para {month}/{year}");

        // Obtener asientos contabilizados del periodo
        var entries = await _journalEntryRepository.GetByPeriodIdAsync(period.Id, cancellationToken);
        var postedEntries = entries.Where(e => e.Status == JournalEntryStatus.Posted).ToList();

        // Obtener todas las cuentas
        var accounts = await _accountRepository.GetAllAsync(cancellationToken);
        var accountsDict = accounts.ToDictionary(a => a.Id);

        // Agrupar líneas por cuenta y sumar débitos y créditos
        var allLines = postedEntries.SelectMany(e => e.Lines).ToList();
        var groupedByAccount = allLines
            .GroupBy(l => l.AccountId)
            .Select(g =>
            {
                var accountId = g.Key;
                accountsDict.TryGetValue(accountId, out var account);

                return new TrialBalanceLineDto
                {
                    AccountCode = account?.Code ?? string.Empty,
                    AccountName = account?.Name ?? string.Empty,
                    AccountType = account?.AccountType ?? AccountType.Asset,
                    Debit = g.Sum(l => l.Debit),
                    Credit = g.Sum(l => l.Credit)
                };
            })
            .OrderBy(l => l.AccountCode)
            .ToList();

        var result = new TrialBalanceDto
        {
            PeriodName = period.Name,
            Year = year,
            Month = month,
            GeneratedAt = DateTime.UtcNow,
            Lines = groupedByAccount,
            TotalDebit = groupedByAccount.Sum(l => l.Debit),
            TotalCredit = groupedByAccount.Sum(l => l.Credit)
        };

        return Result<TrialBalanceDto>.Success(result);
    }

    public async Task<Result<IncomeStatementDto>> GetIncomeStatementAsync(int year, int month, CancellationToken cancellationToken = default)
    {
        // Buscar el periodo
        var period = await _periodRepository.GetByYearMonthAsync(year, month, cancellationToken);
        if (period == null)
            return Result<IncomeStatementDto>.Failure($"No se encontró el periodo contable para {month}/{year}");

        // Obtener asientos contabilizados del periodo
        var entries = await _journalEntryRepository.GetByPeriodIdAsync(period.Id, cancellationToken);
        var postedEntries = entries.Where(e => e.Status == JournalEntryStatus.Posted).ToList();

        // Obtener todas las cuentas
        var accounts = await _accountRepository.GetAllAsync(cancellationToken);
        var accountsDict = accounts.ToDictionary(a => a.Id);

        // Agrupar líneas por cuenta
        var allLines = postedEntries.SelectMany(e => e.Lines).ToList();
        var groupedByAccount = allLines
            .GroupBy(l => l.AccountId)
            .Select(g =>
            {
                accountsDict.TryGetValue(g.Key, out var account);

                return new
                {
                    AccountCode = account?.Code ?? string.Empty,
                    AccountName = account?.Name ?? string.Empty,
                    AccountType = account?.AccountType ?? AccountType.Asset,
                    Debit = g.Sum(l => l.Debit),
                    Credit = g.Sum(l => l.Credit)
                };
            })
            .ToList();

        // Sección de Ingresos (Revenue): saldo = Credit - Debit
        var revenueLines = groupedByAccount
            .Where(a => a.AccountType == AccountType.Revenue)
            .Select(a => new TrialBalanceLineDto
            {
                AccountCode = a.AccountCode,
                AccountName = a.AccountName,
                AccountType = a.AccountType,
                Debit = a.Debit,
                Credit = a.Credit
            })
            .OrderBy(l => l.AccountCode)
            .ToList();

        var revenueTotal = revenueLines.Sum(l => l.Credit - l.Debit);

        // Sección de Gastos (Expense): saldo = Debit - Credit
        var expenseLines = groupedByAccount
            .Where(a => a.AccountType == AccountType.Expense)
            .Select(a => new TrialBalanceLineDto
            {
                AccountCode = a.AccountCode,
                AccountName = a.AccountName,
                AccountType = a.AccountType,
                Debit = a.Debit,
                Credit = a.Credit
            })
            .OrderBy(l => l.AccountCode)
            .ToList();

        var expenseTotal = expenseLines.Sum(l => l.Debit - l.Credit);

        var result = new IncomeStatementDto
        {
            PeriodName = period.Name,
            Year = year,
            Month = month,
            GeneratedAt = DateTime.UtcNow,
            RevenueSection = new IncomeStatementSectionDto
            {
                Title = "Ingresos",
                Lines = revenueLines,
                Total = revenueTotal
            },
            ExpenseSection = new IncomeStatementSectionDto
            {
                Title = "Gastos",
                Lines = expenseLines,
                Total = expenseTotal
            },
            NetIncome = revenueTotal - expenseTotal
        };

        return Result<IncomeStatementDto>.Success(result);
    }

    public async Task<Result<BalanceSheetDto>> GetBalanceSheetAsync(int year, int month, CancellationToken cancellationToken = default)
    {
        // Buscar el periodo solicitado
        var period = await _periodRepository.GetByYearMonthAsync(year, month, cancellationToken);
        if (period == null)
            return Result<BalanceSheetDto>.Failure($"No se encontró el periodo contable para {month}/{year}");

        // Obtener todos los periodos hasta el periodo solicitado (acumulativo)
        var allPeriods = await _periodRepository.GetAllAsync(cancellationToken);
        var periodsUpTo = allPeriods
            .Where(p => p.Year < year || (p.Year == year && p.Month <= month))
            .ToList();

        // Obtener todas las cuentas
        var accounts = await _accountRepository.GetAllAsync(cancellationToken);
        var accountsDict = accounts.ToDictionary(a => a.Id);

        // Recopilar todas las líneas de asientos contabilizados de todos los periodos acumulados
        var allPostedLines = new List<Domain.Entities.JournalEntryLine>();

        foreach (var p in periodsUpTo)
        {
            var entries = await _journalEntryRepository.GetByPeriodIdAsync(p.Id, cancellationToken);
            var postedLines = entries
                .Where(e => e.Status == JournalEntryStatus.Posted)
                .SelectMany(e => e.Lines);

            allPostedLines.AddRange(postedLines);
        }

        // Agrupar por cuenta
        var groupedByAccount = allPostedLines
            .GroupBy(l => l.AccountId)
            .Select(g =>
            {
                accountsDict.TryGetValue(g.Key, out var account);

                return new
                {
                    AccountCode = account?.Code ?? string.Empty,
                    AccountName = account?.Name ?? string.Empty,
                    AccountType = account?.AccountType ?? AccountType.Asset,
                    TotalDebit = g.Sum(l => l.Debit),
                    TotalCredit = g.Sum(l => l.Credit)
                };
            })
            .ToList();

        // Sección de Activos: saldo = Debit - Credit
        var assetLines = groupedByAccount
            .Where(a => a.AccountType == AccountType.Asset)
            .Select(a => new TrialBalanceLineDto
            {
                AccountCode = a.AccountCode,
                AccountName = a.AccountName,
                AccountType = a.AccountType,
                Debit = a.TotalDebit,
                Credit = a.TotalCredit
            })
            .OrderBy(l => l.AccountCode)
            .ToList();

        var assetTotal = assetLines.Sum(l => l.Debit - l.Credit);

        // Sección de Pasivos: saldo = Credit - Debit
        var liabilityLines = groupedByAccount
            .Where(a => a.AccountType == AccountType.Liability)
            .Select(a => new TrialBalanceLineDto
            {
                AccountCode = a.AccountCode,
                AccountName = a.AccountName,
                AccountType = a.AccountType,
                Debit = a.TotalDebit,
                Credit = a.TotalCredit
            })
            .OrderBy(l => l.AccountCode)
            .ToList();

        var liabilityTotal = liabilityLines.Sum(l => l.Credit - l.Debit);

        // Sección de Patrimonio: saldo = Credit - Debit
        var equityLines = groupedByAccount
            .Where(a => a.AccountType == AccountType.Equity)
            .Select(a => new TrialBalanceLineDto
            {
                AccountCode = a.AccountCode,
                AccountName = a.AccountName,
                AccountType = a.AccountType,
                Debit = a.TotalDebit,
                Credit = a.TotalCredit
            })
            .OrderBy(l => l.AccountCode)
            .ToList();

        var equityTotal = equityLines.Sum(l => l.Credit - l.Debit);

        // Calcular utilidad neta del periodo actual para incluir en patrimonio
        var currentPeriodEntries = await _journalEntryRepository.GetByPeriodIdAsync(period.Id, cancellationToken);
        var currentPostedLines = currentPeriodEntries
            .Where(e => e.Status == JournalEntryStatus.Posted)
            .SelectMany(e => e.Lines)
            .ToList();

        var currentGrouped = currentPostedLines
            .GroupBy(l => l.AccountId)
            .Select(g =>
            {
                accountsDict.TryGetValue(g.Key, out var account);
                return new
                {
                    AccountType = account?.AccountType ?? AccountType.Asset,
                    TotalDebit = g.Sum(l => l.Debit),
                    TotalCredit = g.Sum(l => l.Credit)
                };
            })
            .ToList();

        var currentRevenue = currentGrouped
            .Where(a => a.AccountType == AccountType.Revenue)
            .Sum(a => a.TotalCredit - a.TotalDebit);

        var currentExpenses = currentGrouped
            .Where(a => a.AccountType == AccountType.Expense)
            .Sum(a => a.TotalDebit - a.TotalCredit);

        var netIncome = currentRevenue - currentExpenses;

        // Agregar utilidad neta a la sección de patrimonio
        equityTotal += netIncome;

        var result = new BalanceSheetDto
        {
            PeriodName = period.Name,
            Year = year,
            Month = month,
            GeneratedAt = DateTime.UtcNow,
            AssetSection = new BalanceSheetSectionDto
            {
                Title = "Activos",
                Lines = assetLines,
                Total = assetTotal
            },
            LiabilitySection = new BalanceSheetSectionDto
            {
                Title = "Pasivos",
                Lines = liabilityLines,
                Total = liabilityTotal
            },
            EquitySection = new BalanceSheetSectionDto
            {
                Title = "Patrimonio",
                Lines = equityLines,
                Total = equityTotal
            },
            TotalAssets = assetTotal,
            TotalLiabilitiesAndEquity = liabilityTotal + equityTotal
        };

        return Result<BalanceSheetDto>.Success(result);
    }
}
