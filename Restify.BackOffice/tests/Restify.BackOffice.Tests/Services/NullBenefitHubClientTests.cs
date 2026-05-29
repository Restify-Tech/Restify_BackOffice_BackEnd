using FluentAssertions;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Infrastructure.BenefitHub;

namespace Restify.BackOffice.Tests.Services;

public class NullBenefitHubClientTests
{
    private readonly NullBenefitHubClient _sut = new();

    #region SimulateAsync

    [Fact]
    public async Task SimulateAsync_ReturnsSuccessWithEmptyBenefits()
    {
        // Arrange
        var request = CreateBenefitRequest();

        // Act
        var result = await _sut.SimulateAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Benefits.Should().BeEmpty();
        result.TotalSaving.Should().Be(0);
    }

    [Fact]
    public async Task SimulateAsync_DoesNotThrow_WithEmptyItems()
    {
        // Arrange
        var request = new BenefitRequest(
            TenantSourceId: "tenant-001",
            ExternalCustomerId: "cust-001",
            ExternalTransactionId: "txn-001",
            Items: new List<BenefitItem>(),
            TotalAmount: 0,
            CouponCode: null);

        // Act
        var act = async () => await _sut.SimulateAsync(request);

        // Assert
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region ApplyAsync

    [Fact]
    public async Task ApplyAsync_ReturnsSuccess_WithoutCallingExternalService()
    {
        // Arrange
        var request = CreateBenefitRequest();

        // Act
        var result = await _sut.ApplyAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.TotalSaving.Should().Be(0);
        result.PointsEarned.Should().Be(0);
    }

    [Fact]
    public async Task ApplyAsync_ReturnsNullRedemptionId()
    {
        // Arrange
        var request = CreateBenefitRequest();

        // Act
        var result = await _sut.ApplyAsync(request);

        // Assert
        result.RedemptionId.Should().BeNull();
    }

    #endregion

    #region ReverseAsync

    [Fact]
    public async Task ReverseAsync_ReturnsTrue_WithoutCallingExternalService()
    {
        // Act
        var result = await _sut.ReverseAsync("txn-001", "tenant-001", "Pedido cancelado");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ReverseAsync_ReturnsTrue_WhenReasonIsNull()
    {
        // Act
        var result = await _sut.ReverseAsync("txn-002", "tenant-001", null);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region GetCustomerBenefitsAsync

    [Fact]
    public async Task GetCustomerBenefitsAsync_ReturnsSuccessWithZeroPoints()
    {
        // Act
        var result = await _sut.GetCustomerBenefitsAsync("cust-001", "tenant-001");

        // Assert
        result.Success.Should().BeTrue();
        result.Points.Should().Be(0);
        result.Benefits.Should().BeEmpty();
    }

    #endregion

    #region Helpers

    private static BenefitRequest CreateBenefitRequest() => new(
        TenantSourceId: "tenant-001",
        ExternalCustomerId: "cust-001",
        ExternalTransactionId: "txn-001",
        Items: new List<BenefitItem>
        {
            new("prod-001", "Hamburguesa", 10m, 2, "cat-001")
        },
        TotalAmount: 20m,
        CouponCode: null);

    #endregion
}
