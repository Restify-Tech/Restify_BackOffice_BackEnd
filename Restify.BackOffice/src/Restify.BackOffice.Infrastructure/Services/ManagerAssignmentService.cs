using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class ManagerAssignmentService : IManagerAssignmentService
{
    private readonly BackOfficeDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ManagerAssignmentService(
        BackOfficeDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<IEnumerable<ManagerAssignmentDto>>> GetByBranchAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        // Verificar que la sucursal existe
        var branchExists = await _context.Branches
            .AnyAsync(b => b.Id == branchId, cancellationToken);

        if (!branchExists)
            return Result<IEnumerable<ManagerAssignmentDto>>.Failure("Sucursal no encontrada");

        var assignments = await _context.ManagerAssignments
            .AsNoTracking()
            .Include(ma => ma.Branch)
            .Where(ma => ma.BranchId == branchId)
            .OrderBy(ma => ma.UserName)
            .ToListAsync(cancellationToken);

        var dtos = assignments.Select(MapToDto);

        return Result<IEnumerable<ManagerAssignmentDto>>.Success(dtos);
    }

    public async Task<Result<ManagerAssignmentDto>> CreateAsync(CreateManagerAssignmentRequest request, CancellationToken cancellationToken = default)
    {
        // Verificar que la sucursal existe
        var branch = await _context.Branches
            .FirstOrDefaultAsync(b => b.Id == request.BranchId, cancellationToken);

        if (branch == null)
            return Result<ManagerAssignmentDto>.Failure("Sucursal no encontrada");

        // Verificar que el usuario no ya esta asignado activamente
        var existingActive = await _context.ManagerAssignments
            .AnyAsync(ma => ma.BranchId == request.BranchId
                && ma.UserId == request.UserId
                && ma.IsActive, cancellationToken);

        if (existingActive)
            return Result<ManagerAssignmentDto>.Failure("El usuario ya tiene una asignacion activa en esta sucursal");

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;

        var assignment = new ManagerAssignment
        {
            BranchId = request.BranchId,
            UserId = request.UserId,
            UserName = request.UserName,
            UserEmail = request.UserEmail,
            CanApproveCashClosing = request.CanApproveCashClosing,
            CanVoidOrders = request.CanVoidOrders,
            MaxDiscountPercent = request.MaxDiscountPercent,
            IsActive = true,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            TenantId = tenantId
        };

        _context.ManagerAssignments.Add(assignment);
        await _context.SaveChangesAsync(cancellationToken);

        // Recargar con Branch
        assignment.Branch = branch;

        return Result<ManagerAssignmentDto>.Success(MapToDto(assignment));
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var assignment = await _context.ManagerAssignments
            .FirstOrDefaultAsync(ma => ma.Id == id, cancellationToken);

        if (assignment == null)
            return Result<bool>.Failure("Asignacion no encontrada");

        // Desactivar en lugar de eliminar fisicamente
        assignment.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    private static ManagerAssignmentDto MapToDto(ManagerAssignment ma)
    {
        return new ManagerAssignmentDto(
            Id: ma.Id,
            BranchId: ma.BranchId,
            BranchName: ma.Branch?.Name ?? string.Empty,
            UserId: ma.UserId,
            UserName: ma.UserName,
            UserEmail: ma.UserEmail,
            CanApproveCashClosing: ma.CanApproveCashClosing,
            CanVoidOrders: ma.CanVoidOrders,
            MaxDiscountPercent: ma.MaxDiscountPercent,
            IsActive: ma.IsActive,
            ValidFrom: ma.ValidFrom,
            ValidTo: ma.ValidTo,
            CreatedAt: ma.CreatedAt);
    }
}
