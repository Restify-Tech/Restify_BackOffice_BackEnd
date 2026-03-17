using FluentAssertions;
using Moq;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Services;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Tests.Services;

public class InvoiceServiceTests
{
    private readonly Mock<IInvoiceRepository> _invoiceRepoMock;
    private readonly Mock<IOrderRepository> _orderRepoMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly InvoiceService _sut;

    private static readonly Guid TenantId = Guid.NewGuid();

    public InvoiceServiceTests()
    {
        _invoiceRepoMock = new Mock<IInvoiceRepository>();
        _orderRepoMock = new Mock<IOrderRepository>();
        _currentUserMock = new Mock<ICurrentUserService>();

        _currentUserMock.Setup(c => c.TenantId).Returns(TenantId);
        _currentUserMock.Setup(c => c.Email).Returns("admin@demo.com");

        _sut = new InvoiceService(
            _invoiceRepoMock.Object,
            _orderRepoMock.Object,
            _currentUserMock.Object);
    }

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsInvoice()
    {
        // Arrange
        var invoice = CreateInvoice(InvoiceStatus.Draft);
        _invoiceRepoMock.Setup(r => r.GetByIdAsync(invoice.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        // Act
        var result = await _sut.GetByIdAsync(invoice.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.InvoiceNumber.Should().Be(invoice.InvoiceNumber);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsFailure()
    {
        // Arrange
        _invoiceRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);

        // Act
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_WithValidOrder_ReturnsSuccess()
    {
        // Arrange
        var order = CreateValidOrderForInvoicing();
        var request = new CreateInvoiceRequest
        {
            OrderId = order.Id,
            PaymentMethod = PaymentMethod.Cash,
            CustomerName = "Juan"
        };

        _orderRepoMock.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _invoiceRepoMock.Setup(r => r.GetByOrderIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);
        _invoiceRepoMock.Setup(r => r.GenerateInvoiceNumberAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("FAC-001");
        _invoiceRepoMock.Setup(r => r.CreateAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice i, CancellationToken _) => i);
        _orderRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order o, CancellationToken _) => o);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.InvoiceNumber.Should().Be("FAC-001");
    }

    [Fact]
    public async Task CreateAsync_OrderAlreadyHasInvoice_ReturnsFailure()
    {
        // Arrange
        var order = CreateValidOrderForInvoicing();
        var existingInvoice = CreateInvoice(InvoiceStatus.Draft);
        var request = new CreateInvoiceRequest { OrderId = order.Id, PaymentMethod = PaymentMethod.Cash };

        _orderRepoMock.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _invoiceRepoMock.Setup(r => r.GetByOrderIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingInvoice);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("ya tiene una factura");
    }

    [Fact]
    public async Task CreateAsync_PendingOrder_ReturnsFailure()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            OrderNumber = "ORD-001",
            Status = OrderStatus.Pending,
            Items = new List<OrderItem>
            {
                new() { Id = Guid.NewGuid(), TenantId = TenantId, Quantity = 1, UnitPrice = 10m, Subtotal = 10m, ProductId = Guid.NewGuid(), Modifiers = new List<OrderItemModifier>(), CreatedAt = DateTime.UtcNow }
            },
            CreatedAt = DateTime.UtcNow
        };
        var request = new CreateInvoiceRequest { OrderId = order.Id, PaymentMethod = PaymentMethod.Cash };

        _orderRepoMock.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _invoiceRepoMock.Setup(r => r.GetByOrderIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("listo o servido");
    }

    #endregion

    #region ProcessPaymentAsync

    [Fact]
    public async Task ProcessPaymentAsync_DraftInvoice_ReturnsSuccess()
    {
        // Arrange
        var invoice = CreateInvoice(InvoiceStatus.Draft);
        invoice.Total = 100m;
        var request = new ProcessPaymentRequest { PaymentMethod = PaymentMethod.Cash };

        _invoiceRepoMock.Setup(r => r.GetByIdAsync(invoice.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _invoiceRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice i, CancellationToken _) => i);

        // Act
        var result = await _sut.ProcessPaymentAsync(invoice.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Status.Should().Be(InvoiceStatus.Paid);
    }

    [Fact]
    public async Task ProcessPaymentAsync_AlreadyPaid_ReturnsFailure()
    {
        // Arrange
        var invoice = CreateInvoice(InvoiceStatus.Paid);
        var request = new ProcessPaymentRequest { PaymentMethod = PaymentMethod.Cash };

        _invoiceRepoMock.Setup(r => r.GetByIdAsync(invoice.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        // Act
        var result = await _sut.ProcessPaymentAsync(invoice.Id, request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("ya está pagada");
    }

    #endregion

    #region CancelAsync

    [Fact]
    public async Task CancelAsync_WithReason_ReturnsSuccess()
    {
        // Arrange
        var invoice = CreateInvoice(InvoiceStatus.Draft);
        var request = new CancelInvoiceRequest { Reason = "Error en datos" };

        _invoiceRepoMock.Setup(r => r.GetByIdAsync(invoice.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _invoiceRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice i, CancellationToken _) => i);

        // Act
        var result = await _sut.CancelAsync(invoice.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Status.Should().Be(InvoiceStatus.Cancelled);
    }

    [Fact]
    public async Task CancelAsync_EmptyReason_ReturnsFailure()
    {
        // Arrange
        var request = new CancelInvoiceRequest { Reason = "" };

        // Act
        var result = await _sut.CancelAsync(Guid.NewGuid(), request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("motivo");
    }

    [Fact]
    public async Task CancelAsync_AlreadyCancelled_ReturnsFailure()
    {
        // Arrange
        var invoice = CreateInvoice(InvoiceStatus.Cancelled);
        var request = new CancelInvoiceRequest { Reason = "Motivo" };

        _invoiceRepoMock.Setup(r => r.GetByIdAsync(invoice.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        // Act
        var result = await _sut.CancelAsync(invoice.Id, request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("ya está anulada");
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_DraftInvoice_ReturnsSuccess()
    {
        // Arrange
        var invoice = CreateInvoice(InvoiceStatus.Draft);
        _invoiceRepoMock.Setup(r => r.GetByIdAsync(invoice.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _invoiceRepoMock.Setup(r => r.DeleteAsync(invoice.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.DeleteAsync(invoice.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_PaidInvoice_ReturnsFailure()
    {
        // Arrange
        var invoice = CreateInvoice(InvoiceStatus.Paid);
        _invoiceRepoMock.Setup(r => r.GetByIdAsync(invoice.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        // Act
        var result = await _sut.DeleteAsync(invoice.Id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("No se puede eliminar");
    }

    #endregion

    #region GetStatisticsAsync

    [Fact]
    public async Task GetStatisticsAsync_ReturnsPaidInvoiceStats()
    {
        // Arrange
        var invoices = new List<Invoice>
        {
            CreateInvoice(InvoiceStatus.Paid, 100m),
            CreateInvoice(InvoiceStatus.Paid, 200m),
            CreateInvoice(InvoiceStatus.Cancelled, 50m)
        };
        _invoiceRepoMock.Setup(r => r.GetByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoices);

        // Act
        var result = await _sut.GetStatisticsAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.TotalInvoices.Should().Be(2);
        result.Data.TotalRevenue.Should().Be(300m);
        result.Data.AverageTicket.Should().Be(150m);
    }

    #endregion

    #region Helpers

    private static Invoice CreateInvoice(InvoiceStatus status, decimal total = 100m)
    {
        return new Invoice
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            InvoiceNumber = $"FAC-{Guid.NewGuid().ToString()[..4]}",
            OrderId = Guid.NewGuid(),
            Status = status,
            Total = total,
            PaymentMethod = PaymentMethod.Cash,
            Items = new List<InvoiceItem>(),
            CreatedAt = DateTime.UtcNow
        };
    }

    private static Order CreateValidOrderForInvoicing()
    {
        var productId = Guid.NewGuid();
        return new Order
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            OrderNumber = "ORD-001",
            Status = OrderStatus.Ready,
            Items = new List<OrderItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    TenantId = TenantId,
                    ProductId = productId,
                    Product = new Product { Id = productId, Name = "Hamburguesa", Price = 10m, CategoryId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow },
                    Quantity = 2,
                    UnitPrice = 10m,
                    Subtotal = 20m,
                    Modifiers = new List<OrderItemModifier>(),
                    CreatedAt = DateTime.UtcNow
                }
            },
            CreatedAt = DateTime.UtcNow
        };
    }

    #endregion
}
