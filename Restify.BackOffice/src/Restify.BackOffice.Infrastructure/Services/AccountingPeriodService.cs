using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Application.Mappings;
using Restify.BackOffice.Domain.Enums;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class AccountingPeriodService : IAccountingPeriodService
{
    private readonly IAccountingPeriodRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public AccountingPeriodService(
        IAccountingPeriodRepository repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<AccountingPeriodDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var period = await _repository.GetByIdAsync(id, cancellationToken);

        if (period == null)
            return Result<AccountingPeriodDto>.Failure("Periodo contable no encontrado");

        return Result<AccountingPeriodDto>.Success(period.ToDto());
    }

    public async Task<Result<IEnumerable<AccountingPeriodDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var periods = await _repository.GetAllAsync(cancellationToken);
        var dtos = periods.Select(p => p.ToDto());

        return Result<IEnumerable<AccountingPeriodDto>>.Success(dtos);
    }

    public async Task<Result<AccountingPeriodDto>> CreateAsync(CreateAccountingPeriodRequest request, CancellationToken cancellationToken = default)
    {
        // Verificar que no exista un periodo para el mismo año y mes
        var existing = await _repository.GetByYearMonthAsync(request.Year, request.Month, cancellationToken);
        if (existing != null)
            return Result<AccountingPeriodDto>.Failure($"Ya existe un periodo contable para {request.Month}/{request.Year}");

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var entity = request.ToEntity(tenantId);

        var created = await _repository.AddAsync(entity, cancellationToken);

        return Result<AccountingPeriodDto>.Success(created.ToDto());
    }

    public async Task<Result<AccountingPeriodDto>> CloseAsync(Guid id, string closedBy, CancellationToken cancellationToken = default)
    {
        var period = await _repository.GetByIdAsync(id, cancellationToken);

        if (period == null)
            return Result<AccountingPeriodDto>.Failure("Periodo contable no encontrado");

        if (period.Status != AccountingPeriodStatus.Open)
            return Result<AccountingPeriodDto>.Failure("Solo se puede cerrar un periodo que esté abierto");

        period.Status = AccountingPeriodStatus.Closed;
        period.ClosedBy = closedBy;
        period.ClosedAt = DateTime.UtcNow;
        period.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(period, cancellationToken);

        return Result<AccountingPeriodDto>.Success(period.ToDto());
    }

    public async Task<Result<AccountingPeriodDto>> LockAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var period = await _repository.GetByIdAsync(id, cancellationToken);

        if (period == null)
            return Result<AccountingPeriodDto>.Failure("Periodo contable no encontrado");

        if (period.Status != AccountingPeriodStatus.Closed)
            return Result<AccountingPeriodDto>.Failure("Solo se puede bloquear un periodo que esté cerrado");

        period.Status = AccountingPeriodStatus.Locked;
        period.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(period, cancellationToken);

        return Result<AccountingPeriodDto>.Success(period.ToDto());
    }

    public async Task<Result<AccountingPeriodDto>> ReopenAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var period = await _repository.GetByIdAsync(id, cancellationToken);

        if (period == null)
            return Result<AccountingPeriodDto>.Failure("Periodo contable no encontrado");

        if (period.Status != AccountingPeriodStatus.Closed)
            return Result<AccountingPeriodDto>.Failure("Solo se puede reabrir un periodo que esté cerrado. Los periodos bloqueados no pueden reabrirse");

        period.Status = AccountingPeriodStatus.Open;
        period.ClosedBy = null;
        period.ClosedAt = null;
        period.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(period, cancellationToken);

        return Result<AccountingPeriodDto>.Success(period.ToDto());
    }
}
