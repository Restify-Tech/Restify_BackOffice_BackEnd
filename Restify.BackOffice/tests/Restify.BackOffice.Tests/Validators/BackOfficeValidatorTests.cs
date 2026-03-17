using FluentAssertions;
using FluentValidation.TestHelper;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Validators;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Tests.Validators;

public class BackOfficeValidatorTests
{
    #region CreateCategoryRequestValidator

    public class CreateCategoryRequestValidatorTests
    {
        private readonly CreateCategoryRequestValidator _validator = new();

        [Fact]
        public void Should_Pass_When_ValidRequest()
        {
            var request = new CreateCategoryRequest
            {
                Name = "Bebidas",
                Description = "Todas las bebidas",
                DisplayOrder = 1,
                IsActive = true
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Fail_When_NameIsEmpty()
        {
            var request = new CreateCategoryRequest { Name = "" };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("El nombre es requerido");
        }

        [Fact]
        public void Should_Fail_When_NameTooLong()
        {
            var request = new CreateCategoryRequest { Name = new string('A', 101) };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("El nombre no puede exceder 100 caracteres");
        }

        [Fact]
        public void Should_Fail_When_DescriptionTooLong()
        {
            var request = new CreateCategoryRequest
            {
                Name = "Test",
                Description = new string('A', 501)
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Fail_When_DisplayOrderNegative()
        {
            var request = new CreateCategoryRequest { Name = "Test", DisplayOrder = -1 };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.DisplayOrder);
        }
    }

    public class UpdateCategoryRequestValidatorTests
    {
        private readonly UpdateCategoryRequestValidator _validator = new();

        [Fact]
        public void Should_Fail_When_IdIsEmpty()
        {
            var request = new UpdateCategoryRequest { Id = Guid.Empty, Name = "Test" };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Pass_When_ValidRequest()
        {
            var request = new UpdateCategoryRequest
            {
                Id = Guid.NewGuid(),
                Name = "Bebidas",
                DisplayOrder = 0
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region CreateProductRequestValidator

    public class CreateProductRequestValidatorTests
    {
        private readonly CreateProductRequestValidator _validator = new();

        [Fact]
        public void Should_Pass_When_ValidRequest()
        {
            var request = new CreateProductRequest
            {
                Name = "Hamburguesa",
                Price = 9.99m,
                CategoryId = Guid.NewGuid(),
                DisplayOrder = 1
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Fail_When_NameIsEmpty()
        {
            var request = new CreateProductRequest { Name = "", Price = 5m, CategoryId = Guid.NewGuid() };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Fail_When_NameTooLong()
        {
            var request = new CreateProductRequest
            {
                Name = new string('A', 201),
                Price = 5m,
                CategoryId = Guid.NewGuid()
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Fail_When_PriceNegative()
        {
            var request = new CreateProductRequest
            {
                Name = "Test",
                Price = -1m,
                CategoryId = Guid.NewGuid()
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Price);
        }

        [Fact]
        public void Should_Pass_When_PriceIsZero()
        {
            var request = new CreateProductRequest
            {
                Name = "Cortesia",
                Price = 0m,
                CategoryId = Guid.NewGuid()
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(x => x.Price);
        }

        [Fact]
        public void Should_Fail_When_CategoryIdEmpty()
        {
            var request = new CreateProductRequest { Name = "Test", Price = 5m, CategoryId = Guid.Empty };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.CategoryId);
        }

        [Fact]
        public void Should_Fail_When_SkuTooLong()
        {
            var request = new CreateProductRequest
            {
                Name = "Test",
                Price = 5m,
                CategoryId = Guid.NewGuid(),
                Sku = new string('A', 51)
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Sku);
        }

        [Fact]
        public void Should_Fail_When_ModifierNameEmpty()
        {
            var request = new CreateProductRequest
            {
                Name = "Test",
                Price = 5m,
                CategoryId = Guid.NewGuid(),
                Modifiers = new List<CreateProductModifierRequest>
                {
                    new() { Name = "" }
                }
            };

            var result = _validator.TestValidate(request);

            result.IsValid.Should().BeFalse();
        }
    }

    #endregion

    #region CreateOrderRequestValidator

    public class CreateOrderRequestValidatorTests
    {
        private readonly CreateOrderRequestValidator _validator = new();

        [Fact]
        public void Should_Pass_When_ValidRequest()
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
        public void Should_Fail_When_ItemsEmpty()
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
        public void Should_Fail_When_DiscountNegative()
        {
            var request = new CreateOrderRequest
            {
                Type = OrderType.DineIn,
                Discount = -5m,
                Items = new List<CreateOrderItemRequest>
                {
                    new() { ProductId = Guid.NewGuid(), Quantity = 1 }
                }
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Discount);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Fail_When_ItemQuantityInvalid(int quantity)
        {
            var request = new CreateOrderRequest
            {
                Type = OrderType.DineIn,
                Items = new List<CreateOrderItemRequest>
                {
                    new() { ProductId = Guid.NewGuid(), Quantity = quantity }
                }
            };

            var result = _validator.TestValidate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Should_Fail_When_ItemProductIdEmpty()
        {
            var request = new CreateOrderRequest
            {
                Type = OrderType.DineIn,
                Items = new List<CreateOrderItemRequest>
                {
                    new() { ProductId = Guid.Empty, Quantity = 1 }
                }
            };

            var result = _validator.TestValidate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Should_Fail_When_CustomerNameTooLong()
        {
            var request = new CreateOrderRequest
            {
                Type = OrderType.DineIn,
                CustomerName = new string('A', 201),
                Items = new List<CreateOrderItemRequest>
                {
                    new() { ProductId = Guid.NewGuid(), Quantity = 1 }
                }
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.CustomerName);
        }
    }

    public class UpdateOrderStatusRequestValidatorTests
    {
        private readonly UpdateOrderStatusRequestValidator _validator = new();

        [Fact]
        public void Should_Pass_When_ValidStatus()
        {
            var request = new UpdateOrderStatusRequest { Status = OrderStatus.Confirmed };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Fail_When_CancelledWithoutReason()
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
        public void Should_Pass_When_CancelledWithReason()
        {
            var request = new UpdateOrderStatusRequest
            {
                Status = OrderStatus.Cancelled,
                CancelReason = "Cliente cambio de opinion"
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Fail_When_CancelReasonTooLong()
        {
            var request = new UpdateOrderStatusRequest
            {
                Status = OrderStatus.Cancelled,
                CancelReason = new string('A', 501)
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.CancelReason);
        }
    }

    #endregion

    #region CreateTableRequestValidator

    public class CreateTableRequestValidatorTests
    {
        private readonly CreateTableRequestValidator _validator = new();

        [Fact]
        public void Should_Pass_When_ValidRequest()
        {
            var request = new CreateTableRequest
            {
                Number = "1",
                Capacity = 4,
                Zone = "Interior"
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Fail_When_NumberEmpty()
        {
            var request = new CreateTableRequest { Number = "", Capacity = 4 };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Number);
        }

        [Fact]
        public void Should_Fail_When_NumberTooLong()
        {
            var request = new CreateTableRequest { Number = new string('1', 11), Capacity = 4 };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Number);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Fail_When_CapacityInvalid(int capacity)
        {
            var request = new CreateTableRequest { Number = "1", Capacity = capacity };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Capacity);
        }

        [Fact]
        public void Should_Fail_When_CapacityExceedsMax()
        {
            var request = new CreateTableRequest { Number = "1", Capacity = 51 };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Capacity);
        }

        [Fact]
        public void Should_Fail_When_NameTooLong()
        {
            var request = new CreateTableRequest
            {
                Number = "1",
                Capacity = 4,
                Name = new string('A', 51)
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Fail_When_ZoneTooLong()
        {
            var request = new CreateTableRequest
            {
                Number = "1",
                Capacity = 4,
                Zone = new string('A', 51)
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Zone);
        }
    }

    public class UpdateTableStatusRequestValidatorTests
    {
        private readonly UpdateTableStatusRequestValidator _validator = new();

        [Fact]
        public void Should_Pass_When_ValidStatus()
        {
            var request = new UpdateTableStatusRequest { Status = TableStatus.Available };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Fail_When_InvalidStatus()
        {
            var request = new UpdateTableStatusRequest { Status = (TableStatus)99 };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Status);
        }
    }

    #endregion
}
