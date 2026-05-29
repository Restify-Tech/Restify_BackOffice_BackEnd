using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class FranchiseService : IFranchiseService
{
    private readonly BackOfficeDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<FranchiseService> _logger;

    public FranchiseService(
        BackOfficeDbContext context,
        ICurrentUserService currentUserService,
        ILogger<FranchiseService> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result<FranchiseConfigDto?>> GetMyFranchiseAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = _currentUserService.TenantId ?? Guid.Empty;

        var config = await _context.FranchiseConfigs
            .Include(f => f.Franchisees)
            .FirstOrDefaultAsync(f => f.TenantId == tenantId, cancellationToken);

        if (config == null)
            return Result<FranchiseConfigDto?>.Success(null);

        return Result<FranchiseConfigDto?>.Success(MapToDto(config));
    }

    public async Task<Result<FranchiseConfigDto>> CreateAsync(CreateFranchiseConfigRequest request, CancellationToken cancellationToken = default)
    {
        var tenantId = _currentUserService.TenantId ?? Guid.Empty;

        var existing = await _context.FranchiseConfigs
            .FirstOrDefaultAsync(f => f.TenantId == tenantId, cancellationToken);

        if (existing != null)
            return Result<FranchiseConfigDto>.Failure("Ya existe una configuracion de franquicia para este tenant");

        var config = new FranchiseConfig
        {
            TenantId = tenantId,
            FranchiseName = request.FranchiseName,
            Description = request.Description,
            AllowLocalMenuOverrides = request.AllowLocalMenuOverrides,
            AllowLocalPromotions = request.AllowLocalPromotions,
            SyncMenuAutomatically = request.SyncMenuAutomatically,
            ContactEmail = request.ContactEmail
        };

        _context.FranchiseConfigs.Add(config);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Franquicia creada: {FranchiseName} para tenant {TenantId}", config.FranchiseName, tenantId);

        return Result<FranchiseConfigDto>.Success(MapToDto(config));
    }

    public async Task<Result<FranchiseConfigDto>> UpdateAsync(Guid id, CreateFranchiseConfigRequest request, CancellationToken cancellationToken = default)
    {
        var config = await _context.FranchiseConfigs
            .Include(f => f.Franchisees)
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

        if (config == null)
            return Result<FranchiseConfigDto>.Failure("Configuracion de franquicia no encontrada");

        config.FranchiseName = request.FranchiseName;
        config.Description = request.Description;
        config.AllowLocalMenuOverrides = request.AllowLocalMenuOverrides;
        config.AllowLocalPromotions = request.AllowLocalPromotions;
        config.SyncMenuAutomatically = request.SyncMenuAutomatically;
        config.ContactEmail = request.ContactEmail;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<FranchiseConfigDto>.Success(MapToDto(config));
    }

    public async Task<Result<IEnumerable<FranchiseeRelationDto>>> GetFranchiseesAsync(Guid franchiseConfigId, CancellationToken cancellationToken = default)
    {
        var relations = await _context.FranchiseeRelations
            .Where(r => r.FranchiseConfigId == franchiseConfigId)
            .ToListAsync(cancellationToken);

        var dtos = relations.Select(MapRelationToDto);
        return Result<IEnumerable<FranchiseeRelationDto>>.Success(dtos);
    }

    public async Task<Result<FranchiseeRelationDto>> AddFranchiseeAsync(Guid franchiseConfigId, AddFranchiseeRequest request, CancellationToken cancellationToken = default)
    {
        var config = await _context.FranchiseConfigs
            .FirstOrDefaultAsync(f => f.Id == franchiseConfigId, cancellationToken);

        if (config == null)
            return Result<FranchiseeRelationDto>.Failure("Configuracion de franquicia no encontrada");

        var existing = await _context.FranchiseeRelations
            .FirstOrDefaultAsync(r => r.FranchiseConfigId == franchiseConfigId && r.FranchiseeTenantId == request.FranchiseeTenantId, cancellationToken);

        if (existing != null)
            return Result<FranchiseeRelationDto>.Failure("Este franquiciado ya esta registrado en la franquicia");

        var relation = new FranchiseeRelation
        {
            TenantId = config.TenantId,
            FranchiseConfigId = franchiseConfigId,
            FranchiseeTenantId = request.FranchiseeTenantId,
            FranchiseeName = request.FranchiseeName,
            FranchiseeCity = request.FranchiseeCity,
            RoyaltyPercentage = request.RoyaltyPercentage,
            IsActive = true,
            JoinedAt = DateTime.UtcNow
        };

        _context.FranchiseeRelations.Add(relation);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Franquiciado agregado: {FranchiseeName} a franquicia {FranchiseConfigId}", relation.FranchiseeName, franchiseConfigId);

        return Result<FranchiseeRelationDto>.Success(MapRelationToDto(relation));
    }

    public async Task<Result<bool>> RemoveFranchiseeAsync(Guid franchiseConfigId, Guid franchiseeId, CancellationToken cancellationToken = default)
    {
        var relation = await _context.FranchiseeRelations
            .FirstOrDefaultAsync(r => r.FranchiseConfigId == franchiseConfigId && r.Id == franchiseeId, cancellationToken);

        if (relation == null)
            return Result<bool>.Failure("Franquiciado no encontrado");

        _context.FranchiseeRelations.Remove(relation);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<FranchiseConsolidatedReportDto>> GetConsolidatedReportAsync(Guid franchiseConfigId, DateTime? from, DateTime? to, CancellationToken cancellationToken = default)
    {
        var config = await _context.FranchiseConfigs
            .Include(f => f.Franchisees)
            .FirstOrDefaultAsync(f => f.Id == franchiseConfigId, cancellationToken);

        if (config == null)
            return Result<FranchiseConsolidatedReportDto>.Failure("Configuracion de franquicia no encontrada");

        _logger.LogWarning("Cross-tenant reporting pendiente de implementacion real. Retornando datos mock para FranchiseConfig {Id}", franchiseConfigId);

        var rng = new Random(42);
        var franchiseeStats = config.Franchisees.Select(f =>
        {
            var orders = rng.Next(50, 500);
            var revenue = Math.Round((decimal)(orders * rng.NextDouble() * 30 + 500), 2);
            return new FranchiseeStatsDto(
                TenantId: f.FranchiseeTenantId,
                FranchiseeName: f.FranchiseeName,
                City: f.FranchiseeCity ?? "Sin ciudad",
                Revenue: revenue,
                Orders: orders,
                AvgTicket: orders > 0 ? Math.Round(revenue / orders, 2) : 0,
                IsActive: f.IsActive
            );
        }).ToList();

        var totalRevenue = franchiseeStats.Sum(s => s.Revenue);
        var totalOrders = franchiseeStats.Sum(s => s.Orders);

        var report = new FranchiseConsolidatedReportDto(
            FranchiseName: config.FranchiseName,
            Franchisees: franchiseeStats,
            TotalRevenue: totalRevenue,
            TotalOrders: totalOrders
        );

        return Result<FranchiseConsolidatedReportDto>.Success(report);
    }

    private static FranchiseConfigDto MapToDto(FranchiseConfig config) => new(
        Id: config.Id,
        FranchiseName: config.FranchiseName,
        Description: config.Description,
        AllowLocalMenuOverrides: config.AllowLocalMenuOverrides,
        AllowLocalPromotions: config.AllowLocalPromotions,
        SyncMenuAutomatically: config.SyncMenuAutomatically,
        ContactEmail: config.ContactEmail,
        LogoUrl: config.LogoUrl,
        FranchiseeCount: config.Franchisees.Count
    );

    private static FranchiseeRelationDto MapRelationToDto(FranchiseeRelation r) => new(
        Id: r.Id,
        FranchiseeTenantId: r.FranchiseeTenantId,
        FranchiseeName: r.FranchiseeName,
        FranchiseeCity: r.FranchiseeCity,
        IsActive: r.IsActive,
        JoinedAt: r.JoinedAt,
        RoyaltyPercentage: r.RoyaltyPercentage
    );
}
