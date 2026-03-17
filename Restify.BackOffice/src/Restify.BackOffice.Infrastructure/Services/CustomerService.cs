using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Application.Mappings;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CustomerDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<CustomerDto>.Failure("Cliente no encontrado");

        return Result<CustomerDto>.Success(entity.ToDto());
    }

    public async Task<Result<IEnumerable<CustomerListDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        var dtos = entities.Select(e => e.ToListDto());
        return Result<IEnumerable<CustomerListDto>>.Success(dtos);
    }

    public async Task<Result<CustomerDto>> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        // Validaciones básicas
        if (string.IsNullOrWhiteSpace(request.FirstName))
            return Result<CustomerDto>.Failure("El nombre es requerido");

        if (string.IsNullOrWhiteSpace(request.LastName))
            return Result<CustomerDto>.Failure("El apellido es requerido");

        if (string.IsNullOrWhiteSpace(request.Email))
            return Result<CustomerDto>.Failure("El email es requerido");

        if (string.IsNullOrWhiteSpace(request.Password))
            return Result<CustomerDto>.Failure("La contraseña es requerida");

        // Verificar email único
        if (await _repository.ExistsAsync(request.Email, cancellationToken: cancellationToken))
            return Result<CustomerDto>.Failure($"Ya existe un cliente con el email '{request.Email}'");

        // Hash de password - almacenado como placeholder; la autenticación real se maneja en Auth service
        var passwordHash = HashPassword(request.Password);

        var entity = request.ToEntity(passwordHash);
        await _repository.AddAsync(entity, cancellationToken);

        return Result<CustomerDto>.Success(entity.ToDto());
    }

    public async Task<Result<CustomerDto>> UpdateAsync(Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<CustomerDto>.Failure("Cliente no encontrado");

        entity.UpdateFrom(request);
        await _repository.UpdateAsync(entity, cancellationToken);

        return Result<CustomerDto>.Success(entity.ToDto());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<bool>.Failure("Cliente no encontrado");

        if (entity.Orders?.Count > 0)
            return Result<bool>.Failure("No se puede eliminar un cliente que tiene pedidos asociados");

        await _repository.DeleteAsync(entity, cancellationToken);
        return Result<bool>.Success(true);
    }

    /// <summary>
    /// Hash simple para almacenar contraseña.
    /// La autenticación real del cliente se maneja desde el servicio Auth.
    /// TODO: Reemplazar con BCrypt cuando se agregue el paquete BCrypt.Net-Next.
    /// </summary>
    private static string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
