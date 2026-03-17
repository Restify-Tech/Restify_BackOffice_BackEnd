using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Application.Mappings;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class AccountingAccountService : IAccountingAccountService
{
    private readonly IAccountingAccountRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public AccountingAccountService(
        IAccountingAccountRepository repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<AccountingAccountDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var account = await _repository.GetByIdAsync(id, cancellationToken);

        if (account == null)
            return Result<AccountingAccountDto>.Failure("Cuenta contable no encontrada");

        return Result<AccountingAccountDto>.Success(account.ToDto(includeChildren: true));
    }

    public async Task<Result<IEnumerable<AccountingAccountDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var accounts = await _repository.GetAllAsync(cancellationToken);
        var dtos = accounts.Select(a => a.ToDto());

        return Result<IEnumerable<AccountingAccountDto>>.Success(dtos);
    }

    public async Task<Result<AccountingAccountDto>> CreateAsync(CreateAccountingAccountRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
            return Result<AccountingAccountDto>.Failure("El código de la cuenta es requerido");

        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<AccountingAccountDto>.Failure("El nombre de la cuenta es requerido");

        // Verificar código duplicado
        var existing = await _repository.GetByCodeAsync(request.Code, cancellationToken);
        if (existing != null)
            return Result<AccountingAccountDto>.Failure($"Ya existe una cuenta con el código '{request.Code}'");

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var entity = request.ToEntity(tenantId);

        // Calcular nivel basado en la cuenta padre
        if (request.ParentId.HasValue)
        {
            var parent = await _repository.GetByIdAsync(request.ParentId.Value, cancellationToken);
            if (parent == null)
                return Result<AccountingAccountDto>.Failure("La cuenta padre no existe");

            entity.Level = parent.Level + 1;
        }
        else
        {
            entity.Level = 1;
        }

        var created = await _repository.AddAsync(entity, cancellationToken);

        return Result<AccountingAccountDto>.Success(created.ToDto());
    }

    public async Task<Result<AccountingAccountDto>> UpdateAsync(Guid id, UpdateAccountingAccountRequest request, CancellationToken cancellationToken = default)
    {
        var account = await _repository.GetByIdAsync(id, cancellationToken);

        if (account == null)
            return Result<AccountingAccountDto>.Failure("Cuenta contable no encontrada");

        account.UpdateFrom(request);
        await _repository.UpdateAsync(account, cancellationToken);

        return Result<AccountingAccountDto>.Success(account.ToDto());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var account = await _repository.GetByIdAsync(id, cancellationToken);

        if (account == null)
            return Result<bool>.Failure("Cuenta contable no encontrada");

        // Verificar que no tenga subcuentas
        if (account.Children != null && account.Children.Count > 0)
            return Result<bool>.Failure("No se puede eliminar una cuenta con subcuentas");

        // Verificar que no tenga asientos contables si acepta movimientos
        if (account.AcceptsEntries && account.JournalEntryLines != null && account.JournalEntryLines.Count > 0)
            return Result<bool>.Failure("No se puede eliminar una cuenta con asientos contables");

        await _repository.DeleteAsync(account, cancellationToken);

        return Result<bool>.Success(true);
    }
}
