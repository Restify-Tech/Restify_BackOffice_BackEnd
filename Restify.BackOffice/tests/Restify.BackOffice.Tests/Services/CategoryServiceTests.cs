using FluentAssertions;
using Moq;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Services;

namespace Restify.BackOffice.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _repositoryMock;
    private readonly CategoryService _sut;

    private static readonly Guid TenantId = Guid.NewGuid();

    public CategoryServiceTests()
    {
        _repositoryMock = new Mock<ICategoryRepository>();
        _sut = new CategoryService(_repositoryMock.Object);
    }

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_ReturnsCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            CreateCategory("Bebidas"),
            CreateCategory("Comidas")
        };
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data![0].Name.Should().Be("Bebidas");
        result.Data[1].Name.Should().Be("Comidas");
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsCategory()
    {
        // Arrange
        var category = CreateCategory("Bebidas");
        _repositoryMock.Setup(r => r.GetByIdWithSubCategoriesAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _sut.GetByIdAsync(category.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("Bebidas");
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsFailure()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdWithSubCategoriesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "Bebidas", IsActive = true };
        _repositoryMock.Setup(r => r.ExistsAsync(request.Name, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _repositoryMock.Setup(r => r.GetMaxDisplayOrderAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category c, CancellationToken _) => c);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("Bebidas");
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ReturnsFailure()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "Bebidas" };
        _repositoryMock.Setup(r => r.ExistsAsync(request.Name, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Ya existe");
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var existing = CreateCategory("Bebidas");
        var request = new UpdateCategoryRequest { Id = existing.Id, Name = "Bebidas Actualizadas", IsActive = true };

        _repositoryMock.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _repositoryMock.Setup(r => r.ExistsAsync(request.Name, request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.UpdateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("Bebidas Actualizadas");
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_WhenNoSubCategories_ReturnsSuccess()
    {
        // Arrange
        var category = CreateCategory("Bebidas");
        _repositoryMock.Setup(r => r.GetByIdWithSubCategoriesAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _repositoryMock.Setup(r => r.DeleteAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteAsync(category.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_WithSubCategories_ReturnsFailure()
    {
        // Arrange
        var category = CreateCategory("Bebidas");
        category.SubCategories.Add(CreateCategory("Jugos"));

        _repositoryMock.Setup(r => r.GetByIdWithSubCategoriesAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _sut.DeleteAsync(category.Id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("subcategorías");
    }

    #endregion

    #region GetRootCategoriesAsync

    [Fact]
    public async Task GetRootCategoriesAsync_ReturnsOnlyTopLevel()
    {
        // Arrange
        var root1 = CreateCategory("Bebidas");
        var root2 = CreateCategory("Comidas");
        _repositoryMock.Setup(r => r.GetRootCategoriesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category> { root1, root2 });

        // Act
        var result = await _sut.GetRootCategoriesAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    #endregion

    #region Helpers

    private static Category CreateCategory(string name)
    {
        return new Category
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            Name = name,
            IsActive = true,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow
        };
    }

    #endregion
}
