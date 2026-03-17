using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountingController : ControllerBase
{
    private readonly IAccountingAccountService _accountService;
    private readonly IAccountingPeriodService _periodService;
    private readonly IJournalEntryService _journalEntryService;
    private readonly IFinancialReportService _reportService;
    private readonly ILogger<AccountingController> _logger;

    public AccountingController(
        IAccountingAccountService accountService,
        IAccountingPeriodService periodService,
        IJournalEntryService journalEntryService,
        IFinancialReportService reportService,
        ILogger<AccountingController> logger)
    {
        _accountService = accountService;
        _periodService = periodService;
        _journalEntryService = journalEntryService;
        _reportService = reportService;
        _logger = logger;
    }

    // ========== Accounts ==========

    /// <summary>
    /// Obtiene todas las cuentas contables
    /// </summary>
    [HttpGet("accounts")]
    public async Task<IActionResult> GetAllAccounts(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _accountService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cuentas contables");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene una cuenta contable por ID
    /// </summary>
    [HttpGet("accounts/{id}")]
    public async Task<IActionResult> GetAccountById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _accountService.GetByIdAsync(id, cancellationToken);
            if (!result.IsSuccess) return NotFound(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cuenta contable {AccountId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crea una nueva cuenta contable
    /// </summary>
    [HttpPost("accounts")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountingAccountRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _accountService.CreateAsync(request, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return CreatedAtAction(nameof(GetAccountById), new { id = result.Data!.Id }, result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear cuenta contable");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Actualiza una cuenta contable
    /// </summary>
    [HttpPut("accounts/{id}")]
    public async Task<IActionResult> UpdateAccount(Guid id, [FromBody] UpdateAccountingAccountRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _accountService.UpdateAsync(id, request, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar cuenta contable {AccountId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Elimina una cuenta contable
    /// </summary>
    [HttpDelete("accounts/{id}")]
    public async Task<IActionResult> DeleteAccount(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _accountService.DeleteAsync(id, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar cuenta contable {AccountId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // ========== Periods ==========

    /// <summary>
    /// Obtiene todos los periodos contables
    /// </summary>
    [HttpGet("periods")]
    public async Task<IActionResult> GetAllPeriods(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _periodService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener periodos contables");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene un periodo contable por ID
    /// </summary>
    [HttpGet("periods/{id}")]
    public async Task<IActionResult> GetPeriodById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _periodService.GetByIdAsync(id, cancellationToken);
            if (!result.IsSuccess) return NotFound(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener periodo contable {PeriodId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crea un nuevo periodo contable
    /// </summary>
    [HttpPost("periods")]
    public async Task<IActionResult> CreatePeriod([FromBody] CreateAccountingPeriodRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _periodService.CreateAsync(request, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return CreatedAtAction(nameof(GetPeriodById), new { id = result.Data!.Id }, result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear periodo contable");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Cierra un periodo contable
    /// </summary>
    [HttpPatch("periods/{id}/close")]
    public async Task<IActionResult> ClosePeriod(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var closedBy = User.FindFirst("email")?.Value ?? "sistema";
            var result = await _periodService.CloseAsync(id, closedBy, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cerrar periodo contable {PeriodId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Bloquea un periodo contable
    /// </summary>
    [HttpPatch("periods/{id}/lock")]
    public async Task<IActionResult> LockPeriod(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _periodService.LockAsync(id, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al bloquear periodo contable {PeriodId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Reabre un periodo contable cerrado
    /// </summary>
    [HttpPatch("periods/{id}/reopen")]
    public async Task<IActionResult> ReopenPeriod(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _periodService.ReopenAsync(id, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al reabrir periodo contable {PeriodId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // ========== Journal Entries ==========

    /// <summary>
    /// Obtiene todos los asientos contables
    /// </summary>
    [HttpGet("journal-entries")]
    public async Task<IActionResult> GetAllJournalEntries(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _journalEntryService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener asientos contables");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene un asiento contable por ID
    /// </summary>
    [HttpGet("journal-entries/{id}")]
    public async Task<IActionResult> GetJournalEntryById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _journalEntryService.GetByIdAsync(id, cancellationToken);
            if (!result.IsSuccess) return NotFound(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener asiento contable {EntryId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crea un nuevo asiento contable
    /// </summary>
    [HttpPost("journal-entries")]
    public async Task<IActionResult> CreateJournalEntry([FromBody] CreateJournalEntryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _journalEntryService.CreateAsync(request, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return CreatedAtAction(nameof(GetJournalEntryById), new { id = result.Data!.Id }, result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear asiento contable");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Contabiliza un asiento (Draft -> Posted)
    /// </summary>
    [HttpPatch("journal-entries/{id}/post")]
    public async Task<IActionResult> PostJournalEntry(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var postedBy = User.FindFirst("email")?.Value ?? "sistema";
            var result = await _journalEntryService.PostAsync(id, postedBy, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al contabilizar asiento {EntryId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Reversa un asiento contabilizado
    /// </summary>
    [HttpPatch("journal-entries/{id}/reverse")]
    public async Task<IActionResult> ReverseJournalEntry(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var reversedBy = User.FindFirst("email")?.Value ?? "sistema";
            var result = await _journalEntryService.ReverseAsync(id, reversedBy, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al reversar asiento {EntryId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Elimina un asiento en borrador
    /// </summary>
    [HttpDelete("journal-entries/{id}")]
    public async Task<IActionResult> DeleteJournalEntry(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _journalEntryService.DeleteAsync(id, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar asiento {EntryId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // ========== Reports ==========

    /// <summary>
    /// Obtiene el Balance de Comprobacion
    /// </summary>
    [HttpGet("reports/trial-balance")]
    public async Task<IActionResult> GetTrialBalance([FromQuery] int year, [FromQuery] int month, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _reportService.GetTrialBalanceAsync(year, month, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar balance de comprobacion {Year}/{Month}", year, month);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene el Estado de Resultados
    /// </summary>
    [HttpGet("reports/income-statement")]
    public async Task<IActionResult> GetIncomeStatement([FromQuery] int year, [FromQuery] int month, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _reportService.GetIncomeStatementAsync(year, month, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar estado de resultados {Year}/{Month}", year, month);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene el Balance General
    /// </summary>
    [HttpGet("reports/balance-sheet")]
    public async Task<IActionResult> GetBalanceSheet([FromQuery] int year, [FromQuery] int month, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _reportService.GetBalanceSheetAsync(year, month, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar balance general {Year}/{Month}", year, month);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
