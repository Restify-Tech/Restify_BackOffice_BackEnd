using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Application.Mappings;
using Restify.BackOffice.Domain.Enums;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public EmployeeService(
        IEmployeeRepository repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<EmployeeDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<EmployeeDto>.Failure("Empleado no encontrado");

        return Result<EmployeeDto>.Success(entity.ToDto());
    }

    public async Task<Result<IEnumerable<EmployeeDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        var dtos = entities.Select(e => e.ToDto());
        return Result<IEnumerable<EmployeeDto>>.Success(dtos);
    }

    public async Task<Result<IEnumerable<EmployeeDto>>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetActiveAsync(cancellationToken);
        var dtos = entities.Select(e => e.ToDto());
        return Result<IEnumerable<EmployeeDto>>.Success(dtos);
    }

    public async Task<Result<EmployeeDto>> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName))
            return Result<EmployeeDto>.Failure("El nombre es requerido");

        if (string.IsNullOrWhiteSpace(request.LastName))
            return Result<EmployeeDto>.Failure("El apellido es requerido");

        if (string.IsNullOrWhiteSpace(request.Email))
            return Result<EmployeeDto>.Failure("El email es requerido");

        // Verificar identificacion duplicada
        if (!string.IsNullOrWhiteSpace(request.IdentificationNumber))
        {
            var existing = await _repository.GetByIdentificationAsync(request.IdentificationNumber, cancellationToken);
            if (existing != null)
                return Result<EmployeeDto>.Failure($"Ya existe un empleado con la identificacion '{request.IdentificationNumber}'");
        }

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var entity = request.ToEntity(tenantId);

        var created = await _repository.AddAsync(entity, cancellationToken);

        return Result<EmployeeDto>.Success(created.ToDto());
    }

    public async Task<Result<EmployeeDto>> UpdateAsync(Guid id, UpdateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<EmployeeDto>.Failure("Empleado no encontrado");

        if (string.IsNullOrWhiteSpace(request.FirstName))
            return Result<EmployeeDto>.Failure("El nombre es requerido");

        if (string.IsNullOrWhiteSpace(request.LastName))
            return Result<EmployeeDto>.Failure("El apellido es requerido");

        if (string.IsNullOrWhiteSpace(request.Email))
            return Result<EmployeeDto>.Failure("El email es requerido");

        entity.UpdateFrom(request);
        await _repository.UpdateAsync(entity, cancellationToken);

        return Result<EmployeeDto>.Success(entity.ToDto());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<bool>.Failure("Empleado no encontrado");

        // Verificar que no tenga entradas de nomina
        if (entity.PayrollEntries != null && entity.PayrollEntries.Count > 0)
            return Result<bool>.Failure("No se puede eliminar un empleado que tiene entradas de nomina asociadas");

        await _repository.DeleteAsync(entity, cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<EmployeeDto>> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<EmployeeDto>.Failure("Empleado no encontrado");

        entity.IsActive = true;
        entity.Status = EmployeeStatus.Active;
        entity.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(entity, cancellationToken);

        return Result<EmployeeDto>.Success(entity.ToDto());
    }

    public async Task<Result<EmployeeDto>> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<EmployeeDto>.Failure("Empleado no encontrado");

        entity.IsActive = false;
        entity.Status = EmployeeStatus.Terminated;
        entity.TerminationDate = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(entity, cancellationToken);

        return Result<EmployeeDto>.Success(entity.ToDto());
    }
}
