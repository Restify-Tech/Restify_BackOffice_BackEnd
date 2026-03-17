using FluentAssertions;
using FluentValidation.TestHelper;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Validators;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Tests.Validators;

public class CreateCategoryRequestValidatorTests
{
    private readonly CreateCategoryRequestValidator _validator = new();

    [Fact]
    public void EmptyName_ShouldFail()
    {
        var request = new CreateCategoryRequest { Name = "" };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void ValidRequest_ShouldPass()
    {
        var request = new CreateCategoryRequest { Name = "Bebidas", IsActive = true };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void NameTooLong_ShouldFail()
    {
        var request = new CreateCategoryRequest { Name = new string('A', 101) };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void NegativeDisplayOrder_ShouldFail()
    {
        var request = new CreateCategoryRequest { Name = "Bebidas", DisplayOrder = -1 };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.DisplayOrder);
    }
}

public class CreateProductRequestValidatorTests
{
    private readonly CreateProductRequestValidator _validator = new();

    [Fact]
    public void EmptyName_ShouldFail()
    {
        var request = new CreateProductRequest { Name = "", Price = 10m, CategoryId = Guid.NewGuid() };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void NegativePrice_ShouldFail()
    {
        var request = new CreateProductRequest { Name = "Hamburguesa", Price = -1m, CategoryId = Guid.NewGuid() };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void EmptyCategoryId_ShouldFail()
    {
        var request = new CreateProductRequest { Name = "Hamburguesa", Price = 10m, CategoryId = Guid.Empty };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public void ValidRequest_ShouldPass()
    {
        var request = new CreateProductRequest
        {
            Name = "Hamburguesa",
            Price = 9.99m,
            CategoryId = Guid.NewGuid(),
            IsActive = true,
            IsAvailable = true
        };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ZeroPrice_ShouldPass()
    {
        var request = new CreateProductRequest
        {
            Name = "Agua cortesía",
            Price = 0m,
            CategoryId = Guid.NewGuid()
        };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Price);
    }
}

public class CreateOrderRequestValidatorTests
{
    private readonly CreateOrderRequestValidator _validator = new();

    [Fact]
    public void EmptyItems_ShouldFail()
    {
        var request = new CreateOrderRequest
        {
            Type = OrderType.DineIn,
            Items = new List<CreateOrderItemRequest>()
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    [Fact]
    public void ValidRequest_ShouldPass()
    {
        var request = new CreateOrderRequest
        {
            Type = OrderType.DineIn,
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = Guid.NewGuid(), Quantity = 2 }
            }
        };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void NegativeDiscount_ShouldFail()
    {
        var request = new CreateOrderRequest
        {
            Type = OrderType.DineIn,
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = Guid.NewGuid(), Quantity = 1 }
            },
            Discount = -5m
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Discount);
    }

    [Fact]
    public void ItemWithZeroQuantity_ShouldFail()
    {
        var request = new CreateOrderRequest
        {
            Type = OrderType.DineIn,
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = Guid.NewGuid(), Quantity = 0 }
            }
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor("Items[0].Quantity");
    }
}

public class UpdateOrderStatusRequestValidatorTests
{
    private readonly UpdateOrderStatusRequestValidator _validator = new();

    [Fact]
    public void CancelledWithoutReason_ShouldFail()
    {
        var request = new UpdateOrderStatusRequest
        {
            Status = OrderStatus.Cancelled,
            CancelReason = null
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.CancelReason);
    }

    [Fact]
    public void CancelledWithReason_ShouldPass()
    {
        var request = new UpdateOrderStatusRequest
        {
            Status = OrderStatus.Cancelled,
            CancelReason = "Cliente canceló"
        };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
