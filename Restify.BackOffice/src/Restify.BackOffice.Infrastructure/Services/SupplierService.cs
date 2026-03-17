using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Application.Mappings;
using Restify.BackOffice.Domain.Entities;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Infrastructure.Services;

public class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _repository;

    public SupplierService(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<SupplierDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        return Result<List<SupplierDto>>.Success(entities.Select(e => e.ToDto()).ToList());
    }

    public async Task<Result<SupplierDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity == null 
            ? Result<SupplierDto>.Failure("Proveedor no encontrado") 
            : Result<SupplierDto>.Success(entity.ToDto());
    }

    public async Task<Result<SupplierDto>> CreateAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new Supplier
        {
            Name = request.Name,
            TaxId = request.TaxId,
            ContactName = request.ContactName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            Notes = request.Notes,
            IsActive = true
        };

        var created = await _repository.CreateAsync(entity, cancellationToken);
        return Result<SupplierDto>.Success(created.ToDto());
    }

    public async Task<Result<SupplierDto>> UpdateAsync(Guid id, UpdateSupplierRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<SupplierDto>.Failure("Proveedor no encontrado");

        entity.Name = request.Name;
        entity.TaxId = request.TaxId;
        entity.ContactName = request.ContactName;
        entity.Email = request.Email;
        entity.Phone = request.Phone;
        entity.Address = request.Address;
        entity.Notes = request.Notes;
        entity.IsActive = request.IsActive;

        var updated = await _repository.UpdateAsync(entity, cancellationToken);
        return Result<SupplierDto>.Success(updated.ToDto());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<bool>.Failure("Proveedor no encontrado");

        await _repository.DeleteAsync(id, cancellationToken);
        return Result<bool>.Success(true);
    }
}
