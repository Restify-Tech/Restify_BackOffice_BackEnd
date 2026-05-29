using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Application.Mappings;
using Restify.BackOffice.Domain.Entities;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class TableService : ITableService
{
    private readonly ITableRepository _tableRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;

    public TableService(
        ITableRepository tableRepository,
        ICurrentUserService currentUserService,
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository)
    {
        _tableRepository = tableRepository;
        _currentUserService = currentUserService;
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
    }

    public async Task<Result<TableDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var table = await _tableRepository.GetByIdAsync(id, cancellationToken);

        if (table == null)
            return Result<TableDto>.Failure("Mesa no encontrada");

        return Result<TableDto>.Success(table.ToDto());
    }

    public async Task<Result<TableDto>> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        var table = await _tableRepository.GetByNumberAsync(number, cancellationToken);

        if (table == null)
            return Result<TableDto>.Failure("Mesa no encontrada");

        return Result<TableDto>.Success(table.ToDto());
    }

    public async Task<Result<IEnumerable<TableDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tables = await _tableRepository.GetAllAsync(cancellationToken);
        var dtos = tables.Select(t => t.ToDto());

        return Result<IEnumerable<TableDto>>.Success(dtos);
    }

    public async Task<Result<IEnumerable<TableDto>>> GetByStatusAsync(TableStatus status, CancellationToken cancellationToken = default)
    {
        var tables = await _tableRepository.GetByStatusAsync(status, cancellationToken);
        var dtos = tables.Select(t => t.ToDto());

        return Result<IEnumerable<TableDto>>.Success(dtos);
    }

    public async Task<Result<IEnumerable<TableDto>>> GetByZoneAsync(string zone, CancellationToken cancellationToken = default)
    {
        var tables = await _tableRepository.GetByZoneAsync(zone, cancellationToken);
        var dtos = tables.Select(t => t.ToDto());

        return Result<IEnumerable<TableDto>>.Success(dtos);
    }

    public async Task<Result<TableDto>> CreateAsync(CreateTableRequest request, CancellationToken cancellationToken = default)
    {
        // Validaciones
        if (string.IsNullOrWhiteSpace(request.Number))
            return Result<TableDto>.Failure("El número de mesa es requerido");

        if (request.Capacity <= 0)
            return Result<TableDto>.Failure("La capacidad debe ser mayor a cero");

        // Verificar que no exista otra mesa con el mismo número
        var existing = await _tableRepository.GetByNumberAsync(request.Number, cancellationToken);
        if (existing != null)
            return Result<TableDto>.Failure($"Ya existe una mesa con el número '{request.Number}'");

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var table = request.ToEntity(tenantId);

        var created = await _tableRepository.CreateAsync(table, cancellationToken);

        return Result<TableDto>.Success(created.ToDto());
    }

    public async Task<Result<TableDto>> UpdateAsync(Guid id, UpdateTableRequest request, CancellationToken cancellationToken = default)
    {
        // Validaciones
        if (string.IsNullOrWhiteSpace(request.Number))
            return Result<TableDto>.Failure("El número de mesa es requerido");

        if (request.Capacity <= 0)
            return Result<TableDto>.Failure("La capacidad debe ser mayor a cero");

        var table = await _tableRepository.GetByIdAsync(id, cancellationToken);
        if (table == null)
            return Result<TableDto>.Failure("Mesa no encontrada");

        // Verificar que no exista otra mesa con el mismo número (excepto la actual)
        var existing = await _tableRepository.GetByNumberAsync(request.Number, cancellationToken);
        if (existing != null && existing.Id != id)
            return Result<TableDto>.Failure($"Ya existe otra mesa con el número '{request.Number}'");

        table.UpdateFromRequest(request);
        var updated = await _tableRepository.UpdateAsync(table, cancellationToken);

        return Result<TableDto>.Success(updated.ToDto());
    }

    public async Task<Result<TableDto>> UpdateStatusAsync(Guid id, UpdateTableStatusRequest request, CancellationToken cancellationToken = default)
    {
        var table = await _tableRepository.GetByIdAsync(id, cancellationToken);
        if (table == null)
            return Result<TableDto>.Failure("Mesa no encontrada");

        table.UpdateStatus(request);
        var updated = await _tableRepository.UpdateAsync(table, cancellationToken);

        return Result<TableDto>.Success(updated.ToDto());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var table = await _tableRepository.GetByIdAsync(id, cancellationToken);
        if (table == null)
            return Result<bool>.Failure("Mesa no encontrada");

        // No permitir eliminar mesas ocupadas
        if (table.Status == TableStatus.Occupied)
            return Result<bool>.Failure("No se puede eliminar una mesa ocupada");

        await _tableRepository.DeleteAsync(id, cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<TableLayoutDto>> GetLayoutAsync(CancellationToken cancellationToken = default)
    {
        var tables = await _tableRepository.GetAllAsync(cancellationToken);
        var zones = await _tableRepository.GetZonesAsync(cancellationToken);

        var layout = new TableLayoutDto
        {
            Tables = tables.Select(t => t.ToDto()).ToList(),
            Zones = zones.ToList(),
            AvailableCount = tables.Count(t => t.Status == TableStatus.Available),
            OccupiedCount = tables.Count(t => t.Status == TableStatus.Occupied),
            ReservedCount = tables.Count(t => t.Status == TableStatus.Reserved),
            TotalCapacity = tables.Sum(t => t.Capacity)
        };

        return Result<TableLayoutDto>.Success(layout);
    }

    public async Task<Result<TableCustomerProfileDto?>> GetCustomerProfileAsync(
        Guid tableId,
        CancellationToken cancellationToken = default)
    {
        // Verificar que la mesa existe
        var table = await _tableRepository.GetByIdAsync(tableId, cancellationToken);
        if (table == null)
            return Result<TableCustomerProfileDto?>.Failure("Mesa no encontrada");

        // Obtener pedidos activos en esta mesa
        var tableOrders = await _orderRepository.GetByTableIdAsync(tableId, cancellationToken);
        var activeOrder = tableOrders
            .Where(o => o.Status == OrderStatus.Pending
                     || o.Status == OrderStatus.Confirmed
                     || o.Status == OrderStatus.Preparing
                     || o.Status == OrderStatus.Ready)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefault();

        if (activeOrder?.CustomerId is null)
            return Result<TableCustomerProfileDto?>.Success(null);

        // Obtener perfil del cliente
        var customer = await _customerRepository.GetByIdAsync(activeOrder.CustomerId.Value, cancellationToken);
        if (customer is null)
            return Result<TableCustomerProfileDto?>.Success(null);

        // Calcular total de pedidos del cliente en esta mesa (pedidos completados)
        var totalOrders = tableOrders.Count(o => o.CustomerId == customer.Id
                                              && o.Status == OrderStatus.Completed);

        var dto = new TableCustomerProfileDto(
            customer.Id,
            customer.FullName,
            customer.Phone,
            customer.Email,
            totalOrders
        );

        return Result<TableCustomerProfileDto?>.Success(dto);
    }
}
