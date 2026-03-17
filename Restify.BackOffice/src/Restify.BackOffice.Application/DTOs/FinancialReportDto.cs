using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.DTOs;

/// <summary>
/// DTO de balance de comprobacion
/// </summary>
public class TrialBalanceDto
{
    public string PeriodName { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Month { get; set; }
    public DateTime GeneratedAt { get; set; }
    public List<TrialBalanceLineDto> Lines { get; set; } = new();
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
}

/// <summary>
/// Linea del balance de comprobacion
/// </summary>
public class TrialBalanceLineDto
{
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
}

/// <summary>
/// DTO de estado de resultados
/// </summary>
public class IncomeStatementDto
{
    public string PeriodName { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Month { get; set; }
    public DateTime GeneratedAt { get; set; }
    public IncomeStatementSectionDto RevenueSection { get; set; } = new();
    public IncomeStatementSectionDto ExpenseSection { get; set; } = new();
    public decimal NetIncome { get; set; }
}

/// <summary>
/// Seccion del estado de resultados
/// </summary>
public class IncomeStatementSectionDto
{
    public string Title { get; set; } = string.Empty;
    public List<TrialBalanceLineDto> Lines { get; set; } = new();
    public decimal Total { get; set; }
}

/// <summary>
/// DTO de balance general
/// </summary>
public class BalanceSheetDto
{
    public string PeriodName { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Month { get; set; }
    public DateTime GeneratedAt { get; set; }
    public BalanceSheetSectionDto AssetSection { get; set; } = new();
    public BalanceSheetSectionDto LiabilitySection { get; set; } = new();
    public BalanceSheetSectionDto EquitySection { get; set; } = new();
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilitiesAndEquity { get; set; }
}

/// <summary>
/// Seccion del balance general
/// </summary>
public class BalanceSheetSectionDto
{
    public string Title { get; set; } = string.Empty;
    public List<TrialBalanceLineDto> Lines { get; set; } = new();
    public decimal Total { get; set; }
}
