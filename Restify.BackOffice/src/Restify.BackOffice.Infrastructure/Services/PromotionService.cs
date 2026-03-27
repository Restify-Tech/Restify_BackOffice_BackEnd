using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Infrastructure.Services;

public class PromotionService : IPromotionService
{
    private readonly BackOfficeDbContext _context;

    public PromotionService(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<PromotionDto>>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var promotions = await _context.Promotions
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<PromotionDto>>.Success(promotions.Select(MapToDto));
    }

    public async Task<Result<PromotionDto>> CreateAsync(CreatePromotionRequest request, CancellationToken cancellationToken = default)
    {
        var promotion = new Promotion
        {
            Name = request.Name,
            Description = request.Description,
            Type = request.Type,
            DiscountValue = request.DiscountValue,
            ConditionsJson = request.ConditionsJson,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            MaxUsageCount = request.MaxUsageCount,
            IsActive = true
        };

        _context.Promotions.Add(promotion);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<PromotionDto>.Success(MapToDto(promotion));
    }

    public async Task<Result<PromotionDto>> UpdateAsync(Guid id, CreatePromotionRequest request, CancellationToken cancellationToken = default)
    {
        var promotion = await _context.Promotions.FindAsync([id], cancellationToken);
        if (promotion == null)
            return Result<PromotionDto>.Failure("Promocion no encontrada");

        promotion.Name = request.Name;
        promotion.Description = request.Description;
        promotion.Type = request.Type;
        promotion.DiscountValue = request.DiscountValue;
        promotion.ConditionsJson = request.ConditionsJson;
        promotion.ValidFrom = request.ValidFrom;
        promotion.ValidTo = request.ValidTo;
        promotion.MaxUsageCount = request.MaxUsageCount;

        await _context.SaveChangesAsync(cancellationToken);
        return Result<PromotionDto>.Success(MapToDto(promotion));
    }

    public async Task<Result<bool>> ToggleActiveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var promotion = await _context.Promotions.FindAsync([id], cancellationToken);
        if (promotion == null)
            return Result<bool>.Failure("Promocion no encontrada");

        promotion.IsActive = !promotion.IsActive;
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(promotion.IsActive);
    }

    public async Task<Result<IEnumerable<ApplicablePromotionDto>>> EvaluateForOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order == null)
            return Result<IEnumerable<ApplicablePromotionDto>>.Failure("Pedido no encontrado");

        var now = DateTime.UtcNow;
        var promotions = await _context.Promotions
            .Where(p => p.IsActive)
            .Where(p => p.ValidFrom == null || p.ValidFrom <= now)
            .Where(p => p.ValidTo == null || p.ValidTo >= now)
            .Where(p => p.MaxUsageCount == null || p.CurrentUsageCount < p.MaxUsageCount)
            .ToListAsync(cancellationToken);

        var applicablePromotions = new List<ApplicablePromotionDto>();

        foreach (var promo in promotions)
        {
            var result = EvaluatePromotion(promo, order);
            if (result != null)
                applicablePromotions.Add(result);
        }

        return Result<IEnumerable<ApplicablePromotionDto>>.Success(applicablePromotions);
    }

    private static ApplicablePromotionDto? EvaluatePromotion(Promotion promo, Order order)
    {
        // Evaluar condiciones del JSON
        if (!string.IsNullOrEmpty(promo.ConditionsJson))
        {
            try
            {
                var conditions = JsonSerializer.Deserialize<JsonElement>(promo.ConditionsJson);

                // Verificar minAmount
                if (conditions.TryGetProperty("minAmount", out var minAmountEl))
                {
                    var minAmount = minAmountEl.GetDecimal();
                    if (order.Total < minAmount)
                        return null;
                }

                // Verificar dia de semana
                if (conditions.TryGetProperty("dayOfWeek", out var dayEl) && dayEl.ValueKind == JsonValueKind.Array)
                {
                    var allowedDays = dayEl.EnumerateArray().Select(d => d.GetInt32()).ToList();
                    var today = (int)DateTime.UtcNow.DayOfWeek;
                    if (!allowedDays.Contains(today))
                        return null;
                }
            }
            catch
            {
                // Si hay error al parsear el JSON de condiciones, ignorar condicion
            }
        }

        // Calcular descuento
        var discount = promo.Type switch
        {
            PromotionType.PercentageDiscount => order.Total * (promo.DiscountValue / 100),
            PromotionType.FixedDiscount => promo.DiscountValue,
            PromotionType.HappyHour => order.Total * (promo.DiscountValue / 100),
            _ => promo.DiscountValue
        };

        return new ApplicablePromotionDto(
            promo.Id,
            promo.Name,
            promo.Type,
            promo.DiscountValue,
            Math.Round(discount, 2),
            promo.Description ?? promo.Name
        );
    }

    private static PromotionDto MapToDto(Promotion p) => new(
        p.Id,
        p.Name,
        p.Description,
        p.Type,
        p.Type.ToString(),
        p.DiscountValue,
        p.ConditionsJson,
        p.ValidFrom,
        p.ValidTo,
        p.IsActive,
        p.MaxUsageCount,
        p.CurrentUsageCount,
        p.CreatedAt
    );
}
