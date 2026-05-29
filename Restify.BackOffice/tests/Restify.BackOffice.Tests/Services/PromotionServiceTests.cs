using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.BackOffice.Infrastructure.Services;

namespace Restify.BackOffice.Tests.Services;

public class PromotionServiceTests : IDisposable
{
    private readonly BackOfficeDbContext _context;
    private readonly PromotionService _sut;

    private static readonly Guid TenantId = Guid.NewGuid();

    public PromotionServiceTests()
    {
        var options = new DbContextOptionsBuilder<BackOfficeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new BackOfficeDbContext(options);
        _sut = new PromotionService(_context);
    }

    public void Dispose() => _context.Dispose();

    #region GetActiveAsync

    [Fact]
    public async Task GetActiveAsync_ReturnsAllPromotions()
    {
        // Arrange
        _context.Promotions.AddRange(
            CreatePromotion("2x1 Bebidas", PromotionType.BuyXGetY, 10m),
            CreatePromotion("10% Descuento", PromotionType.PercentageDiscount, 10m));
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetActiveAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetActiveAsync_ReturnsEmpty_WhenNoPromotions()
    {
        // Act
        var result = await _sut.GetActiveAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_CreatesPromotion()
    {
        // Arrange
        var request = new CreatePromotionRequest(
            Name: "Happy Hour 20%",
            Description: "Descuento happy hour",
            Type: PromotionType.HappyHour,
            DiscountValue: 20m,
            ConditionsJson: null,
            ValidFrom: null,
            ValidTo: null,
            MaxUsageCount: null);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("Happy Hour 20%");
        result.Data.Type.Should().Be(PromotionType.HappyHour);
        result.Data.DiscountValue.Should().Be(20m);
        _context.Promotions.Should().HaveCount(1);
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_UpdatesPromotion_WhenFound()
    {
        // Arrange
        var promotion = CreatePromotion("Descuento Viejo", PromotionType.FixedDiscount, 5m);
        _context.Promotions.Add(promotion);
        await _context.SaveChangesAsync();

        var request = new CreatePromotionRequest(
            Name: "Descuento Nuevo",
            Description: null,
            Type: PromotionType.FixedDiscount,
            DiscountValue: 10m,
            ConditionsJson: null,
            ValidFrom: null,
            ValidTo: null,
            MaxUsageCount: null);

        // Act
        var result = await _sut.UpdateAsync(promotion.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("Descuento Nuevo");
        result.Data.DiscountValue.Should().Be(10m);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFailure_WhenNotFound()
    {
        // Arrange
        var request = new CreatePromotionRequest(
            Name: "No Existe",
            Description: null,
            Type: PromotionType.FixedDiscount,
            DiscountValue: 5m,
            ConditionsJson: null,
            ValidFrom: null,
            ValidTo: null,
            MaxUsageCount: null);

        // Act
        var result = await _sut.UpdateAsync(Guid.NewGuid(), request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    #endregion

    #region ToggleActiveAsync

    [Fact]
    public async Task ToggleActiveAsync_TogglesPromotion_WhenFound()
    {
        // Arrange
        var promotion = CreatePromotion("Promo Toggle", PromotionType.PercentageDiscount, 15m);
        promotion.IsActive = true;
        _context.Promotions.Add(promotion);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.ToggleActiveAsync(promotion.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeFalse(); // Paso de true a false
    }

    [Fact]
    public async Task ToggleActiveAsync_ReturnsFailure_WhenNotFound()
    {
        // Act
        var result = await _sut.ToggleActiveAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    #endregion

    #region EvaluateForOrderAsync

    [Fact]
    public async Task EvaluateForOrderAsync_ReturnsFailure_WhenOrderNotFound()
    {
        // Act
        var result = await _sut.EvaluateForOrderAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrado");
    }

    [Fact]
    public async Task EvaluateForOrderAsync_ReturnsApplicablePromotions_ForValidOrder()
    {
        // Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            Name = "Combo Especial",
            Price = 25m,
            IsAvailable = true,
            IsActive = true,
            CategoryId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };
        _context.Products.Add(product);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            OrderNumber = "ORD-TEST",
            Type = OrderType.DineIn,
            Status = OrderStatus.Pending,
            PaymentStatus = PaymentStatus.Unpaid,
            Total = 50m,
            CreatedAt = DateTime.UtcNow,
            Items = new List<OrderItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    TenantId = TenantId,
                    ProductId = product.Id,
                    Product = product,
                    Quantity = 2,
                    UnitPrice = 25m,
                    Subtotal = 50m,
                    CreatedAt = DateTime.UtcNow
                }
            }
        };
        _context.Orders.Add(order);

        // Promocion que aplica (sin condiciones y activa)
        var promo = CreatePromotion("Descuento General 10%", PromotionType.PercentageDiscount, 10m);
        promo.ConditionsJson = null;
        promo.IsActive = true;
        _context.Promotions.Add(promo);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.EvaluateForOrderAsync(order.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        result.Data!.First().CalculatedDiscount.Should().Be(5m); // 10% de 50
    }

    #endregion

    #region Helpers

    private static Promotion CreatePromotion(string name, PromotionType type, decimal discountValue) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = TenantId,
        Name = name,
        Type = type,
        DiscountValue = discountValue,
        IsActive = true,
        CurrentUsageCount = 0,
        CreatedAt = DateTime.UtcNow
    };

    #endregion
}
