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

public class PaymentServiceTests
{
    private readonly Mock<IPaymentRepository> _paymentRepoMock;
    private readonly Mock<IOrderRepository> _orderRepoMock;
    private readonly Mock<IPaymentGatewayFactory> _gatewayFactoryMock;
    private readonly Mock<IPaymentGateway> _gatewayMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly Mock<IOrderNotificationService> _notificationMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly Mock<ILogger<PaymentService>> _loggerMock;
    private readonly PaymentService _sut;

    private static readonly Guid TenantId = Guid.NewGuid();

    public PaymentServiceTests()
    {
        _paymentRepoMock = new Mock<IPaymentRepository>();
        _orderRepoMock = new Mock<IOrderRepository>();
        _gatewayFactoryMock = new Mock<IPaymentGatewayFactory>();
        _gatewayMock = new Mock<IPaymentGateway>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _notificationMock = new Mock<IOrderNotificationService>();
        _eventPublisherMock = new Mock<IEventPublisher>();
        _loggerMock = new Mock<ILogger<PaymentService>>();

        _currentUserMock.Setup(c => c.TenantId).Returns(TenantId);

        // Default: gateway returns success
        _gatewayMock.Setup(g => g.ProcessPaymentAsync(It.IsAny<PaymentGatewayRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentGatewayResult(true, "TXN-001", "Completed", null, "{}"));
        _gatewayMock.Setup(g => g.RefundPaymentAsync(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentGatewayResult(true, "TXN-REF-001", "Refunded", null, "{}"));

        _gatewayFactoryMock.Setup(f => f.GetGateway(It.IsAny<string>())).Returns(_gatewayMock.Object);
        _gatewayFactoryMock.Setup(f => f.GetDefaultGateway()).Returns(_gatewayMock.Object);

        var configuration = new ConfigurationBuilder().Build();

        _sut = new PaymentService(
            _paymentRepoMock.Object,
            _orderRepoMock.Object,
            _gatewayFactoryMock.Object,
            _currentUserMock.Object,
            _notificationMock.Object,
            _eventPublisherMock.Object,
            new NullBenefitHubClient(),
            configuration,
            _loggerMock.Object);
    }

    #region ProcessPaymentAsync

    [Fact]
    public async Task ProcessPaymentAsync_ForValidOrder_ReturnsSuccess()
    {
        // Arrange
        var order = CreateOrder(OrderStatus.Confirmed, PaymentStatus.Unpaid, 100m);
        var request = new ProcessOrderPaymentRequest(
            OrderId: order.Id,
            Amount: 100m,
            Method: (int)PaymentMethodType.Cash,
            PayerName: "Juan",
            PayerIdentification: "123",
            GatewayTransactionId: null);

        _orderRepoMock.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _paymentRepoMock.Setup(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payment p, CancellationToken _) => p);
        _paymentRepoMock.Setup(r => r.GetByOrderIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Payment>
            {
                new() { Id = Guid.NewGuid(), Amount = 100m, Status = PaymentTransactionStatus.Completed }
            });
        _orderRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order o, CancellationToken _) => o);

        // Act
        var result = await _sut.ProcessPaymentAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Amount.Should().Be(100m);
        result.Data.Status.Should().Be(PaymentTransactionStatus.Completed.ToString());
    }

    [Fact]
    public async Task ProcessPaymentAsync_ForAlreadyPaidOrder_ReturnsFailure()
    {
        // Arrange
        var order = CreateOrder(OrderStatus.Confirmed, PaymentStatus.Paid, 100m);
        var request = new ProcessOrderPaymentRequest(
            OrderId: order.Id,
            Amount: 100m,
            Method: (int)PaymentMethodType.Cash,
            PayerName: null,
            PayerIdentification: null,
            GatewayTransactionId: null);

        _orderRepoMock.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _sut.ProcessPaymentAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("completamente pagado");
    }

    [Fact]
    public async Task ProcessPaymentAsync_ForCancelledOrder_ReturnsFailure()
    {
        // Arrange
        var order = CreateOrder(OrderStatus.Cancelled, PaymentStatus.Unpaid, 100m);
        var request = new ProcessOrderPaymentRequest(
            OrderId: order.Id,
            Amount: 100m,
            Method: (int)PaymentMethodType.Cash,
            PayerName: null,
            PayerIdentification: null,
            GatewayTransactionId: null);

        _orderRepoMock.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _sut.ProcessPaymentAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("cancelado");
    }

    [Fact]
    public async Task ProcessPaymentAsync_UpdatesOrderPaymentStatus()
    {
        // Arrange
        var order = CreateOrder(OrderStatus.Pending, PaymentStatus.Unpaid, 50m);
        var request = new ProcessOrderPaymentRequest(
            OrderId: order.Id,
            Amount: 50m,
            Method: (int)PaymentMethodType.Cash,
            PayerName: null,
            PayerIdentification: null,
            GatewayTransactionId: null);

        _orderRepoMock.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _paymentRepoMock.Setup(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payment p, CancellationToken _) => p);
        _paymentRepoMock.Setup(r => r.GetByOrderIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Payment>
            {
                new() { Id = Guid.NewGuid(), Amount = 50m, Status = PaymentTransactionStatus.Completed }
            });
        _orderRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order o, CancellationToken _) => o);

        // Act
        var result = await _sut.ProcessPaymentAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _orderRepoMock.Verify(
            r => r.UpdateAsync(It.Is<Order>(o => o.PaymentStatus == PaymentStatus.Paid), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region RefundPaymentAsync

    [Fact]
    public async Task RefundPaymentAsync_CompletedPayment_ReturnsSuccess()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            OrderId = orderId,
            Amount = 100m,
            Method = PaymentMethodType.Cash,
            Status = PaymentTransactionStatus.Completed,
            GatewayTransactionId = "TXN-001",
            CreatedAt = DateTime.UtcNow,
            Items = new List<PaymentItem>()
        };
        var request = new RefundPaymentRequest("Cliente insatisfecho");

        _paymentRepoMock.Setup(r => r.GetByIdAsync(payment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);
        _paymentRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var order = CreateOrder(OrderStatus.Completed, PaymentStatus.Paid, 100m);
        payment.OrderId = order.Id;
        _orderRepoMock.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _paymentRepoMock.Setup(r => r.GetByOrderIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Payment>());
        _orderRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order o, CancellationToken _) => o);

        // Act
        var result = await _sut.RefundPaymentAsync(payment.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Status.Should().Be(PaymentTransactionStatus.Refunded.ToString());
    }

    [Fact]
    public async Task RefundPaymentAsync_PendingPayment_ReturnsFailure()
    {
        // Arrange
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            OrderId = Guid.NewGuid(),
            Amount = 100m,
            Status = PaymentTransactionStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Items = new List<PaymentItem>()
        };
        var request = new RefundPaymentRequest("Motivo");

        _paymentRepoMock.Setup(r => r.GetByIdAsync(payment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        // Act
        var result = await _sut.RefundPaymentAsync(payment.Id, request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("completados");
    }

    [Fact]
    public async Task RefundPaymentAsync_EmptyReason_ReturnsFailure()
    {
        // Arrange
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            OrderId = Guid.NewGuid(),
            Amount = 100m,
            Status = PaymentTransactionStatus.Completed,
            CreatedAt = DateTime.UtcNow,
            Items = new List<PaymentItem>()
        };
        var request = new RefundPaymentRequest("");

        _paymentRepoMock.Setup(r => r.GetByIdAsync(payment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        // Act
        var result = await _sut.RefundPaymentAsync(payment.Id, request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("motivo");
    }

    #endregion

    #region ProcessSplitPaymentAsync

    [Fact]
    public async Task ProcessSplitPaymentAsync_DistributesAmountsCorrectly()
    {
        // Arrange
        var itemId1 = Guid.NewGuid();
        var itemId2 = Guid.NewGuid();
        var order = CreateOrder(OrderStatus.Confirmed, PaymentStatus.Unpaid, 100m);
        order.Items = new List<OrderItem>
        {
            new() { Id = itemId1, TenantId = TenantId, OrderId = order.Id, Quantity = 1, UnitPrice = 60m, Subtotal = 60m, ProductId = Guid.NewGuid(), Modifiers = new List<OrderItemModifier>(), CreatedAt = DateTime.UtcNow },
            new() { Id = itemId2, TenantId = TenantId, OrderId = order.Id, Quantity = 1, UnitPrice = 40m, Subtotal = 40m, ProductId = Guid.NewGuid(), Modifiers = new List<OrderItemModifier>(), CreatedAt = DateTime.UtcNow }
        };

        var request = new SplitPaymentRequest(
            OrderId: order.Id,
            Method: (int)PaymentMethodType.Cash,
            PayerName: "Juan",
            PayerIdentification: null,
            Items: new List<SplitPaymentItemRequest>
            {
                new(OrderItemId: itemId1, Amount: 60m),
                new(OrderItemId: itemId2, Amount: 40m)
            });

        _orderRepoMock.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _paymentRepoMock.Setup(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payment p, CancellationToken _) => p);
        _paymentRepoMock.Setup(r => r.GetByOrderIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Payment>
            {
                new() { Id = Guid.NewGuid(), Amount = 100m, Status = PaymentTransactionStatus.Completed }
            });
        _orderRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order o, CancellationToken _) => o);

        // Act
        var result = await _sut.ProcessSplitPaymentAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Amount.Should().Be(100m);
        _paymentRepoMock.Verify(
            r => r.AddAsync(It.Is<Payment>(p => p.Amount == 100m && p.Items.Count == 2), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region Helpers

    private static Order CreateOrder(OrderStatus status, PaymentStatus paymentStatus, decimal total)
    {
        return new Order
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            OrderNumber = $"ORD-{Guid.NewGuid().ToString()[..4]}",
            Type = OrderType.DineIn,
            Status = status,
            PaymentStatus = paymentStatus,
            Total = total,
            CreatedAt = DateTime.UtcNow,
            Items = new List<OrderItem>()
        };
    }

    #endregion
}
