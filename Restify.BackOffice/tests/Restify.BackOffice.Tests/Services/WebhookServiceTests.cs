using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.BackOffice.Infrastructure.Services;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Tests.Services;

public class WebhookServiceTests : IDisposable
{
    private readonly BackOfficeDbContext _context;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<ILogger<WebhookService>> _loggerMock;
    private readonly WebhookService _sut;

    private static readonly Guid TenantId = Guid.NewGuid();

    public WebhookServiceTests()
    {
        var options = new DbContextOptionsBuilder<BackOfficeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new BackOfficeDbContext(options);
        _currentUserMock = new Mock<ICurrentUserService>();
        _currentUserMock.Setup(c => c.TenantId).Returns(TenantId);
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _loggerMock = new Mock<ILogger<WebhookService>>();

        _sut = new WebhookService(_context, _currentUserMock.Object, _httpClientFactoryMock.Object, _loggerMock.Object);
    }

    public void Dispose() => _context.Dispose();

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_ReturnsAllWebhooks()
    {
        // Arrange
        _context.WebhookConfigs.AddRange(
            CreateWebhookConfig("Webhook A", "https://example.com/a", new List<string> { "order.created" }),
            CreateWebhookConfig("Webhook B", "https://example.com/b", new List<string> { "invoice.created" }));
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_CreatesWebhook_WithValidData()
    {
        // Arrange
        var request = new CreateWebhookRequest(
            Name: "Mi Webhook",
            Url: "https://myapp.com/webhooks",
            Secret: "mi-secreto",
            Events: new List<string> { "order.created", "order.completed" },
            TimeoutMs: 5000);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("Mi Webhook");
        result.Data.IsActive.Should().BeTrue();
        _context.WebhookConfigs.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFailure_WhenNameIsEmpty()
    {
        // Arrange
        var request = new CreateWebhookRequest(
            Name: "",
            Url: "https://example.com/webhook",
            Secret: null,
            Events: new List<string>(),
            TimeoutMs: null);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("nombre");
    }

    [Fact]
    public async Task CreateAsync_ReturnsFailure_WhenUrlIsEmpty()
    {
        // Arrange
        var request = new CreateWebhookRequest(
            Name: "Webhook sin URL",
            Url: "",
            Secret: null,
            Events: new List<string>(),
            TimeoutMs: null);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("URL");
    }

    #endregion

    #region ToggleActiveAsync

    [Fact]
    public async Task ToggleActiveAsync_DeactivatesWebhook_WhenActive()
    {
        // Arrange
        var webhook = CreateWebhookConfig("Toggle Test", "https://example.com/toggle", new List<string>());
        webhook.IsActive = true;
        _context.WebhookConfigs.Add(webhook);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.ToggleActiveAsync(webhook.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeFalse();
    }

    [Fact]
    public async Task ToggleActiveAsync_ReturnsFailure_WhenNotFound()
    {
        // Act
        var result = await _sut.ToggleActiveAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrado");
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_DeletesWebhook_WhenFound()
    {
        // Arrange
        var webhook = CreateWebhookConfig("A Eliminar", "https://example.com/del", new List<string>());
        _context.WebhookConfigs.Add(webhook);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.DeleteAsync(webhook.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _context.WebhookConfigs.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFailure_WhenNotFound()
    {
        // Act
        var result = await _sut.DeleteAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrado");
    }

    #endregion

    #region DispatchEventAsync

    [Fact]
    public async Task DispatchEventAsync_CompletesWithoutException_WhenNoWebhooks()
    {
        // Act
        var act = async () => await _sut.DispatchEventAsync(TenantId.ToString(), "order.created", new { orderId = "123" });

        // Assert — no debe lanzar excepcion aunque no haya webhooks
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DispatchEventAsync_CompletesWithoutException_WhenTenantIdIsInvalid()
    {
        // Act
        var act = async () => await _sut.DispatchEventAsync("no-es-guid-valido", "order.created", new { });

        // Assert — ID invalido no debe lanzar excepcion
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region GetDeliveriesAsync

    [Fact]
    public async Task GetDeliveriesAsync_ReturnsEmptyList_WhenNoDeliveries()
    {
        // Arrange
        var webhook = CreateWebhookConfig("Sin Deliveries", "https://example.com/nd", new List<string>());
        _context.WebhookConfigs.Add(webhook);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetDeliveriesAsync(webhook.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    #endregion

    #region Helpers

    private WebhookConfig CreateWebhookConfig(string name, string url, List<string> events) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = TenantId,
        Name = name,
        Url = url,
        Events = events,
        IsActive = true,
        TimeoutMs = 5000,
        CreatedAt = DateTime.UtcNow
    };

    #endregion
}
