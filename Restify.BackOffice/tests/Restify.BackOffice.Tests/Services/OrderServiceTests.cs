using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.BenefitHub;
using Restify.BackOffice.Infrastructure.Services;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Tests.Services;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepoMock;
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly Mock<ITableRepository> _tableRepoMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly Mock<IOrderNotificationService> _notificationMock;
    private readonly Mock<IWebhookService> _webhookServiceMock;
    private readonly Mock<ILogger<OrderService>> _loggerMock;
    private readonly OrderService _sut;

    private static readonly Guid TenantId = Guid.NewGuid();

    public OrderServiceTests()
    {
        _orderRepoMock = new Mock<IOrderRepository>();
        _productRepoMock = new Mock<IProductRepository>();
        _tableRepoMock = new Mock<ITableRepository>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _notificationMock = new Mock<IOrderNotificationService>();
        _webhookServiceMock = new Mock<IWebhookService>();
        _loggerMock = new Mock<ILogger<OrderService>>();

        _currentUserMock.Setup(c => c.TenantId).Returns(TenantId);
        _currentUserMock.Setup(c => c.Email).Returns("test@demo.com");

        _webhookServiceMock
            .Setup(w => w.DispatchEventAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()))
            .Returns(Task.CompletedTask);

        var configuration = new ConfigurationBuilder().Build();

        _sut = new OrderService(
            _orderRepoMock.Object,
            _productRepoMock.Object,
            _tableRepoMock.Object,
            _currentUserMock.Object,
            _notificationMock.Object,
            new NullBenefitHubClient(),
            configuration,
            _webhookServiceMock.Object,
            _loggerMock.Object);
    }

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_WithValidItems_ReturnsSuccess()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = CreateProduct(productId, "Hamburguesa", 10m);
        var request = new CreateOrderRequest
        {
            Type = OrderType.DineIn,
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = productId, Quantity = 2 }
            }
        };

        _orderRepoMock.Setup(r => r.GenerateOrderNumberAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("ORD-001");
        _productRepoMock.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _orderRepoMock.Setup(r => r.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order o, CancellationToken _) => o);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.OrderNumber.Should().Be("ORD-001");
        result.Data.Items.Should().HaveCount(1);
        _notificationMock.Verify(
            n => n.NotifyOrderCreatedAsync(TenantId, It.IsAny<OrderDto>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithEmptyItems_ReturnsFailure()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            Type = OrderType.DineIn,
            Items = new List<CreateOrderItemRequest>()
        };

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("al menos un item");
    }

    [Fact]
    public async Task CreateAsync_WithNullItems_ReturnsFailure()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            Type = OrderType.DineIn,
            Items = null!
        };

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("al menos un item");
    }

    [Fact]
    public async Task CreateAsync_WithUnavailableProduct_ReturnsFailure()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = CreateProduct(productId, "Hamburguesa", 10m);
        product.IsAvailable = false;

        var request = new CreateOrderRequest
        {
            Type = OrderType.DineIn,
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = productId, Quantity = 1 }
            }
        };

        _orderRepoMock.Setup(r => r.GenerateOrderNumberAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("ORD-002");
        _productRepoMock.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no disponible");
    }

    #endregion

    #region UpdateStatusAsync

    [Fact]
    public async Task UpdateStatusAsync_ValidTransition_ReturnsSuccess()
    {
        // Arrange
        var order = CreateOrder(OrderStatus.Pending);
        var request = new UpdateOrderStatusRequest { Status = OrderStatus.Confirmed };

        _orderRepoMock.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _orderRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order o, CancellationToken _) => o);

        // Act
        var result = await _sut.UpdateStatusAsync(order.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public async Task UpdateStatusAsync_CompletedOrder_ReturnsFailure()
    {
        // Arrange
        var order = CreateOrder(OrderStatus.Completed);
        var request = new UpdateOrderStatusRequest { Status = OrderStatus.Preparing };

        _orderRepoMock.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _sut.UpdateStatusAsync(order.Id, request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("completado o cancelado");
    }

    [Fact]
    public async Task UpdateStatusAsync_CancelledOrder_ReturnsFailure()
    {
        // Arrange
        var order = CreateOrder(OrderStatus.Cancelled);
        var request = new UpdateOrderStatusRequest { Status = OrderStatus.Preparing };

        _orderRepoMock.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _sut.UpdateStatusAsync(order.Id, request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("completado o cancelado");
    }

    [Fact]
    public async Task UpdateStatusAsync_CancelWithReason_ReturnsSuccess()
    {
        // Arrange
        var order = CreateOrder(OrderStatus.Pending);
        var request = new UpdateOrderStatusRequest
        {
            Status = OrderStatus.Cancelled,
            CancelReason = "Cliente canceló"
        };

        _orderRepoMock.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _orderRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order o, CancellationToken _) => o);

        // Act
        var result = await _sut.UpdateStatusAsync(order.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Status.Should().Be(OrderStatus.Cancelled);
        result.Data.CancelReason.Should().Be("Cliente canceló");
    }

    #endregion

    #region GetByStatusAsync

    [Fact]
    public async Task GetByStatusAsync_ReturnsFilteredOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            CreateOrder(OrderStatus.Pending),
            CreateOrder(OrderStatus.Pending)
        };
        _orderRepoMock.Setup(r => r.GetByStatusAsync(OrderStatus.Pending, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);

        // Act
        var result = await _sut.GetByStatusAsync(OrderStatus.Pending);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_PendingOrder_ReturnsSuccess()
    {
        // Arrange
        var order = CreateOrder(OrderStatus.Pending);
        _orderRepoMock.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _orderRepoMock.Setup(r => r.DeleteAsync(order.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteAsync(order.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_NonPendingOrder_ReturnsFailure()
    {
        // Arrange
        var order = CreateOrder(OrderStatus.Preparing);
        _orderRepoMock.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _sut.DeleteAsync(order.Id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("pendientes");
    }

    #endregion

    #region Helpers

    private static Product CreateProduct(Guid id, string name, decimal price)
    {
        return new Product
        {
            Id = id,
            TenantId = TenantId,
            Name = name,
            Price = price,
            IsAvailable = true,
            IsActive = true,
            CategoryId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };
    }

    private static Order CreateOrder(OrderStatus status)
    {
        return new Order
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            OrderNumber = $"ORD-{Guid.NewGuid().ToString()[..4]}",
            Type = OrderType.DineIn,
            Status = status,
            PaymentStatus = PaymentStatus.Unpaid,
            CreatedAt = DateTime.UtcNow,
            Items = new List<OrderItem>()
        };
    }

    #endregion
}
