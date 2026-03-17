using FluentAssertions;
using Moq;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Services;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly Mock<ICategoryRepository> _categoryRepoMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly ProductService _sut;

    private static readonly Guid TenantId = Guid.NewGuid();
    private static readonly Guid CategoryId = Guid.NewGuid();

    public ProductServiceTests()
    {
        _productRepoMock = new Mock<IProductRepository>();
        _categoryRepoMock = new Mock<ICategoryRepository>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _currentUserMock.Setup(c => c.TenantId).Returns(TenantId);
        _currentUserMock.Setup(c => c.Email).Returns("test@demo.com");

        _sut = new ProductService(
            _productRepoMock.Object,
            _categoryRepoMock.Object,
            _currentUserMock.Object);
    }

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_ReturnsProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            CreateProduct("Hamburguesa"),
            CreateProduct("Pizza")
        };
        _productRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsProduct()
    {
        // Arrange
        var product = CreateProduct("Hamburguesa");
        _productRepoMock.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _sut.GetByIdAsync(product.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("Hamburguesa");
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsFailure()
    {
        // Arrange
        _productRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrado");
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var category = new Category { Id = CategoryId, TenantId = TenantId, Name = "Comidas" };
        var request = new CreateProductRequest
        {
            Name = "Hamburguesa",
            Price = 9.99m,
            CategoryId = CategoryId,
            IsActive = true,
            IsAvailable = true
        };

        _categoryRepoMock.Setup(r => r.GetByIdAsync(CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _productRepoMock.Setup(r => r.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken _) => p);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("Hamburguesa");
        result.Data.Price.Should().Be(9.99m);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidCategoryId_ReturnsFailure()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Hamburguesa",
            Price = 9.99m,
            CategoryId = Guid.NewGuid()
        };

        _categoryRepoMock.Setup(r => r.GetByIdAsync(request.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("categoría");
    }

    [Fact]
    public async Task CreateAsync_WithEmptyName_ReturnsFailure()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "",
            Price = 9.99m,
            CategoryId = CategoryId
        };

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("nombre");
    }

    [Fact]
    public async Task CreateAsync_WithNegativePrice_ReturnsFailure()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Hamburguesa",
            Price = -5m,
            CategoryId = CategoryId
        };

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("precio");
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var product = CreateProduct("Hamburguesa");
        var category = new Category { Id = CategoryId, TenantId = TenantId, Name = "Comidas" };
        var request = new UpdateProductRequest
        {
            Name = "Hamburguesa Deluxe",
            Price = 14.99m,
            CategoryId = CategoryId,
            IsActive = true,
            IsAvailable = true
        };

        _productRepoMock.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _categoryRepoMock.Setup(r => r.GetByIdAsync(CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _productRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken _) => p);

        // Act
        var result = await _sut.UpdateAsync(product.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("Hamburguesa Deluxe");
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_WhenExists_ReturnsSuccess()
    {
        // Arrange
        var product = CreateProduct("Hamburguesa");
        _productRepoMock.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _productRepoMock.Setup(r => r.DeleteAsync(product.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteAsync(product.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    #endregion

    #region Helpers

    private static Product CreateProduct(string name)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            Name = name,
            Price = 10m,
            CategoryId = CategoryId,
            Category = new Category { Id = CategoryId, Name = "Comidas" },
            IsActive = true,
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    #endregion
}
