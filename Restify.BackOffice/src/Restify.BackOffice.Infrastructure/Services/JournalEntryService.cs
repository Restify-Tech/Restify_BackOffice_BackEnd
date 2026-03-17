using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Application.Mappings;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class JournalEntryService : IJournalEntryService
{
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IAccountingPeriodRepository _periodRepository;
    private readonly IAccountingAccountRepository _accountRepository;
    private readonly ICurrentUserService _currentUserService;

    public JournalEntryService(
        IJournalEntryRepository journalEntryRepository,
        IAccountingPeriodRepository periodRepository,
        IAccountingAccountRepository accountRepository,
        ICurrentUserService currentUserService)
    {
        _journalEntryRepository = journalEntryRepository;
        _periodRepository = periodRepository;
        _accountRepository = accountRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<JournalEntryDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entry = await _journalEntryRepository.GetByIdAsync(id, cancellationToken);

        if (entry == null)
            return Result<JournalEntryDto>.Failure("Asiento contable no encontrado");

        return Result<JournalEntryDto>.Success(entry.ToDto());
    }

    public async Task<Result<IEnumerable<JournalEntryDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entries = await _journalEntryRepository.GetAllAsync(cancellationToken);
        var dtos = entries.Select(e => e.ToDto());

        return Result<IEnumerable<JournalEntryDto>>.Success(dtos);
    }

    public async Task<Result<IEnumerable<JournalEntryDto>>> GetByPeriodIdAsync(Guid periodId, CancellationToken cancellationToken = default)
    {
        var entries = await _journalEntryRepository.GetByPeriodIdAsync(periodId, cancellationToken);
        var dtos = entries.Select(e => e.ToDto());

        return Result<IEnumerable<JournalEntryDto>>.Success(dtos);
    }

    public async Task<Result<JournalEntryDto>> CreateAsync(CreateJournalEntryRequest request, CancellationToken cancellationToken = default)
    {
        // Validar que tenga al menos 2 líneas
        if (request.Lines == null || request.Lines.Count < 2)
            return Result<JournalEntryDto>.Failure("El asiento debe tener al menos 2 líneas");

        // Validar descripción
        if (string.IsNullOrWhiteSpace(request.Description))
            return Result<JournalEntryDto>.Failure("La descripción del asiento es requerida");

        // Validar que el periodo exista y esté abierto
        var period = await _periodRepository.GetByIdAsync(request.PeriodId, cancellationToken);
        if (period == null)
            return Result<JournalEntryDto>.Failure("El periodo contable no existe");

        if (period.Status != AccountingPeriodStatus.Open)
            return Result<JournalEntryDto>.Failure("El periodo contable no está abierto");

        // Calcular totales de débitos y créditos
        var totalDebit = request.Lines.Sum(l => l.Debit);
        var totalCredit = request.Lines.Sum(l => l.Credit);

        if (totalDebit != totalCredit)
            return Result<JournalEntryDto>.Failure("Los débitos y créditos deben ser iguales");

        // Validar que cada cuenta exista y acepte movimientos
        foreach (var line in request.Lines)
        {
            var account = await _accountRepository.GetByIdAsync(line.AccountId, cancellationToken);
            if (account == null)
                return Result<JournalEntryDto>.Failure($"La cuenta con ID '{line.AccountId}' no existe");

            if (!account.AcceptsEntries)
                return Result<JournalEntryDto>.Failure($"La cuenta {account.Code} no acepta movimientos");
        }

        // Obtener siguiente número de asiento
        var entryNumber = await _journalEntryRepository.GetNextEntryNumberAsync(cancellationToken);

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var entity = request.ToEntity(tenantId, entryNumber);

        var created = await _journalEntryRepository.AddAsync(entity, cancellationToken);

        return Result<JournalEntryDto>.Success(created.ToDto());
    }

    public async Task<Result<JournalEntryDto>> PostAsync(Guid id, string postedBy, CancellationToken cancellationToken = default)
    {
        var entry = await _journalEntryRepository.GetByIdAsync(id, cancellationToken);

        if (entry == null)
            return Result<JournalEntryDto>.Failure("Asiento contable no encontrado");

        if (entry.Status != JournalEntryStatus.Draft)
            return Result<JournalEntryDto>.Failure("Solo se pueden contabilizar asientos en estado borrador");

        // Validar que el periodo esté abierto
        var period = await _periodRepository.GetByIdAsync(entry.PeriodId, cancellationToken);
        if (period == null)
            return Result<JournalEntryDto>.Failure("El periodo contable no existe");

        if (period.Status != AccountingPeriodStatus.Open)
            return Result<JournalEntryDto>.Failure("El periodo contable no está abierto");

        // Re-validar que débitos y créditos sean iguales
        var totalDebit = entry.Lines.Sum(l => l.Debit);
        var totalCredit = entry.Lines.Sum(l => l.Credit);

        if (totalDebit != totalCredit)
            return Result<JournalEntryDto>.Failure("Los débitos y créditos deben ser iguales");

        entry.Status = JournalEntryStatus.Posted;
        entry.PostedBy = postedBy;
        entry.PostedAt = DateTime.UtcNow;
        entry.UpdatedAt = DateTime.UtcNow;

        await _journalEntryRepository.UpdateAsync(entry, cancellationToken);

        return Result<JournalEntryDto>.Success(entry.ToDto());
    }

    public async Task<Result<JournalEntryDto>> ReverseAsync(Guid id, string reversedBy, CancellationToken cancellationToken = default)
    {
        var entry = await _journalEntryRepository.GetByIdAsync(id, cancellationToken);

        if (entry == null)
            return Result<JournalEntryDto>.Failure("Asiento contable no encontrado");

        if (entry.Status != JournalEntryStatus.Posted)
            return Result<JournalEntryDto>.Failure("Solo se pueden reversar asientos contabilizados");

        // Validar que el periodo esté abierto
        var period = await _periodRepository.GetByIdAsync(entry.PeriodId, cancellationToken);
        if (period == null)
            return Result<JournalEntryDto>.Failure("El periodo contable no existe");

        if (period.Status != AccountingPeriodStatus.Open)
            return Result<JournalEntryDto>.Failure("El periodo contable no está abierto para realizar reversiones");

        // Marcar el asiento original como reversado
        entry.Status = JournalEntryStatus.Reversed;
        entry.ReversedBy = reversedBy;
        entry.ReversedAt = DateTime.UtcNow;
        entry.UpdatedAt = DateTime.UtcNow;

        await _journalEntryRepository.UpdateAsync(entry, cancellationToken);

        // Crear asiento de reversión con débitos y créditos invertidos
        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var reversalEntryNumber = await _journalEntryRepository.GetNextEntryNumberAsync(cancellationToken);

        var reversalEntry = new JournalEntry
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            EntryNumber = reversalEntryNumber,
            PeriodId = entry.PeriodId,
            Date = DateTime.UtcNow,
            Description = $"Reversión de asiento {entry.EntryNumber}: {entry.Description}",
            Reference = entry.Reference,
            EntryType = JournalEntryType.Adjustment,
            SourceId = entry.Id,
            Status = JournalEntryStatus.Posted,
            PostedBy = reversedBy,
            PostedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        // Invertir débitos y créditos en cada línea
        foreach (var line in entry.Lines)
        {
            var reversalLine = new JournalEntryLine
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                JournalEntryId = reversalEntry.Id,
                AccountId = line.AccountId,
                Description = $"Reversión: {line.Description}",
                Debit = line.Credit,
                Credit = line.Debit,
                CreatedAt = DateTime.UtcNow
            };

            reversalEntry.Lines.Add(reversalLine);
        }

        var createdReversal = await _journalEntryRepository.AddAsync(reversalEntry, cancellationToken);

        return Result<JournalEntryDto>.Success(createdReversal.ToDto());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entry = await _journalEntryRepository.GetByIdAsync(id, cancellationToken);

        if (entry == null)
            return Result<bool>.Failure("Asiento contable no encontrado");

        if (entry.Status != JournalEntryStatus.Draft)
            return Result<bool>.Failure("Solo se pueden eliminar asientos en estado borrador");

        await _journalEntryRepository.DeleteAsync(entry, cancellationToken);

        return Result<bool>.Success(true);
    }
}
