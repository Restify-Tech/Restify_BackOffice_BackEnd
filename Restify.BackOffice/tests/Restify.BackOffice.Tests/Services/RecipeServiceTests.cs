using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.BackOffice.Infrastructure.Services;

namespace Restify.BackOffice.Tests.Services;

public class RecipeServiceTests : IDisposable
{
    private readonly BackOfficeDbContext _context;
    private readonly RecipeService _sut;

    private static readonly Guid TenantId = Guid.NewGuid();

    public RecipeServiceTests()
    {
        var options = new DbContextOptionsBuilder<BackOfficeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new BackOfficeDbContext(options);
        _sut = new RecipeService(_context);
    }

    public void Dispose() => _context.Dispose();

    #region GetByProductAsync

    [Fact]
    public async Task GetByProductAsync_ReturnsRecipe_WhenExists()
    {
        // Arrange
        var product = CreateProduct("Hamburguesa", 15m);
        _context.Products.Add(product);

        var recipe = CreateRecipe(product);
        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetByProductAsync(product.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.ProductId.Should().Be(product.Id);
        result.Data.ProductName.Should().Be("Hamburguesa");
    }

    [Fact]
    public async Task GetByProductAsync_ReturnsFailure_WhenNotFound()
    {
        // Act
        var result = await _sut.GetByProductAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_CreatesRecipe_WhenProductExists()
    {
        // Arrange
        var product = CreateProduct("Pizza", 20m);
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var request = new CreateRecipeRequest(
            ProductId: product.Id,
            Instructions: "Hornear 20 minutos a 200°C",
            PreparationMinutes: 30,
            Ingredients: new List<CreateRecipeIngredientRequest>());

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.ProductId.Should().Be(product.Id);
        _context.Recipes.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFailure_WhenProductNotFound()
    {
        // Arrange
        var request = new CreateRecipeRequest(
            ProductId: Guid.NewGuid(),
            Instructions: null,
            PreparationMinutes: 15,
            Ingredients: new List<CreateRecipeIngredientRequest>());

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrado");
    }

    [Fact]
    public async Task CreateAsync_ReturnsFailure_WhenRecipeAlreadyExists()
    {
        // Arrange
        var product = CreateProduct("Ensalada", 10m);
        _context.Products.Add(product);

        var existing = CreateRecipe(product);
        _context.Recipes.Add(existing);
        await _context.SaveChangesAsync();

        var request = new CreateRecipeRequest(
            ProductId: product.Id,
            Instructions: null,
            PreparationMinutes: 5,
            Ingredients: new List<CreateRecipeIngredientRequest>());

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Ya existe una receta");
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_DeletesRecipe_WhenFound()
    {
        // Arrange
        var product = CreateProduct("Sopa", 8m);
        _context.Products.Add(product);

        var recipe = CreateRecipe(product);
        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.DeleteAsync(recipe.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
        _context.Recipes.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFailure_WhenNotFound()
    {
        // Act
        var result = await _sut.DeleteAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    #endregion

    #region GetFoodCostReportAsync

    [Fact]
    public async Task GetFoodCostReportAsync_ReturnsReport_WithEmptyRecipes()
    {
        // Act
        var result = await _sut.GetFoodCostReportAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Products.Should().BeEmpty();
        result.Data.AvgFoodCostPercentage.Should().Be(0);
    }

    #endregion

    #region Helpers

    private static Product CreateProduct(string name, decimal price) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = TenantId,
        Name = name,
        Price = price,
        IsAvailable = true,
        IsActive = true,
        CategoryId = Guid.NewGuid(),
        CreatedAt = DateTime.UtcNow
    };

    private static Recipe CreateRecipe(Product product) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = TenantId,
        ProductId = product.Id,
        Product = product,
        Instructions = "Instrucciones de prueba",
        PreparationMinutes = 15,
        CreatedAt = DateTime.UtcNow,
        Ingredients = new List<RecipeIngredient>()
    };

    #endregion
}
