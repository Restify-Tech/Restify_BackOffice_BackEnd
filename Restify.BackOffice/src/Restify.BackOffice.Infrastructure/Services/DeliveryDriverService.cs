using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Application.Mappings;
using Restify.BackOffice.Domain.Entities;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Infrastructure.Services;

public class DeliveryDriverService : IDeliveryDriverService
{
    private readonly IDeliveryDriverRepository _repository;
    private readonly IDeliveryCooperativeRepository _cooperativeRepository;
    private readonly ILogger<DeliveryDriverService> _logger;

    public DeliveryDriverService(
        IDeliveryDriverRepository repository,
        IDeliveryCooperativeRepository cooperativeRepository,
        ILogger<DeliveryDriverService> logger)
    {
        _repository = repository;
        _cooperativeRepository = cooperativeRepository;
        _logger = logger;
    }

    public async Task<Result<DeliveryDriverDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<DeliveryDriverDto>.Failure("Motorizado no encontrado");

        return Result<DeliveryDriverDto>.Success(entity.ToDto());
    }

    public async Task<Result<IEnumerable<DeliveryDriverDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        var dtos = entities.Select(e => e.ToDto());
        return Result<IEnumerable<DeliveryDriverDto>>.Success(dtos);
    }

    public async Task<Result<IEnumerable<DeliveryDriverDto>>> GetAvailableAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAvailableDriversAsync(cancellationToken);
        var dtos = entities.Select(e => e.ToDto());
        return Result<IEnumerable<DeliveryDriverDto>>.Success(dtos);
    }

    public async Task<Result<DeliveryDriverDto>> CreateAsync(CreateDeliveryDriverRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName))
            return Result<DeliveryDriverDto>.Failure("El nombre es requerido");

        if (string.IsNullOrWhiteSpace(request.LastName))
            return Result<DeliveryDriverDto>.Failure("El apellido es requerido");

        if (string.IsNullOrWhiteSpace(request.Email))
            return Result<DeliveryDriverDto>.Failure("El email es requerido");

        if (string.IsNullOrWhiteSpace(request.IdentificationNumber))
            return Result<DeliveryDriverDto>.Failure("El número de identificación es requerido");

        if (string.IsNullOrWhiteSpace(request.Password))
            return Result<DeliveryDriverDto>.Failure("La contraseña es requerida");

        var existingByEmail = await _repository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingByEmail != null)
            return Result<DeliveryDriverDto>.Failure($"Ya existe un motorizado con el email '{request.Email}'");

        if (request.CooperativeId.HasValue)
        {
            var cooperative = await _cooperativeRepository.GetByIdAsync(request.CooperativeId.Value, cancellationToken);
            if (cooperative == null)
                return Result<DeliveryDriverDto>.Failure("Cooperativa no encontrada");
        }

        var passwordHash = HashPassword(request.Password);
        var entity = request.ToEntity(passwordHash);
        var created = await _repository.AddAsync(entity, cancellationToken);

        _logger.LogInformation("Motorizado creado: {Email}", request.Email);

        return Result<DeliveryDriverDto>.Success(created.ToDto());
    }

    public async Task<Result<DeliveryDriverDto>> UpdateAsync(Guid id, UpdateDeliveryDriverRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<DeliveryDriverDto>.Failure("Motorizado no encontrado");

        if (request.CooperativeId.HasValue)
        {
            var cooperative = await _cooperativeRepository.GetByIdAsync(request.CooperativeId.Value, cancellationToken);
            if (cooperative == null)
                return Result<DeliveryDriverDto>.Failure("Cooperativa no encontrada");
        }

        entity.UpdateFrom(request);
        await _repository.UpdateAsync(entity, cancellationToken);

        return Result<DeliveryDriverDto>.Success(entity.ToDto());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<bool>.Failure("Motorizado no encontrado");

        await _repository.DeleteAsync(entity, cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<DeliveryDriverDto>> UpdateStatusAsync(Guid id, UpdateDriverStatusRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<DeliveryDriverDto>.Failure("Motorizado no encontrado");

        entity.Status = (DriverStatus)request.Status;
        await _repository.UpdateAsync(entity, cancellationToken);

        _logger.LogInformation("Estado de motorizado {DriverId} actualizado a {Status}", id, entity.Status);

        return Result<DeliveryDriverDto>.Success(entity.ToDto());
    }

    public async Task<Result<DeliveryDriverDto>> VerifyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<DeliveryDriverDto>.Failure("Motorizado no encontrado");

        entity.IsVerified = true;
        await _repository.UpdateAsync(entity, cancellationToken);

        _logger.LogInformation("Motorizado {DriverId} verificado", id);

        return Result<DeliveryDriverDto>.Success(entity.ToDto());
    }

    private static string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
