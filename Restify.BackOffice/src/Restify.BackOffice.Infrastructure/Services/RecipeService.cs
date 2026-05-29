using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Infrastructure.Services;

public class RecipeService : IRecipeService
{
    private readonly BackOfficeDbContext _context;

    public RecipeService(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<Result<RecipeDto>> GetByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var recipe = await _context.Recipes
            .Include(r => r.Product)
            .Include(r => r.Ingredients)
                .ThenInclude(i => i.InventoryItem)
                    .ThenInclude(inv => inv.Product)
            .FirstOrDefaultAsync(r => r.ProductId == productId, cancellationToken);

        if (recipe == null)
            return Result<RecipeDto>.Failure("Receta no encontrada para este producto");

        return Result<RecipeDto>.Success(MapToDto(recipe));
    }

    public async Task<Result<RecipeDto>> CreateAsync(CreateRecipeRequest request, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products.FindAsync([request.ProductId], cancellationToken);
        if (product == null)
            return Result<RecipeDto>.Failure("Producto no encontrado");

        var existing = await _context.Recipes.FirstOrDefaultAsync(r => r.ProductId == request.ProductId, cancellationToken);
        if (existing != null)
            return Result<RecipeDto>.Failure("Ya existe una receta para este producto. Use el endpoint de actualizacion.");

        var recipe = new Recipe
        {
            ProductId = request.ProductId,
            Instructions = request.Instructions,
            PreparationMinutes = request.PreparationMinutes
        };

        foreach (var ing in request.Ingredients)
        {
            recipe.Ingredients.Add(new RecipeIngredient
            {
                InventoryItemId = ing.InventoryItemId,
                Quantity = ing.Quantity,
                Unit = ing.Unit,
                WasteFactor = ing.WasteFactor
            });
        }

        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync(cancellationToken);

        // Reload with navigation
        return await GetByProductAsync(request.ProductId, cancellationToken);
    }

    public async Task<Result<RecipeDto>> UpdateAsync(Guid id, CreateRecipeRequest request, CancellationToken cancellationToken = default)
    {
        var recipe = await _context.Recipes
            .Include(r => r.Ingredients)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (recipe == null)
            return Result<RecipeDto>.Failure("Receta no encontrada");

        recipe.Instructions = request.Instructions;
        recipe.PreparationMinutes = request.PreparationMinutes;

        // Reemplazar ingredientes
        _context.RecipeIngredients.RemoveRange(recipe.Ingredients);
        recipe.Ingredients.Clear();

        foreach (var ing in request.Ingredients)
        {
            recipe.Ingredients.Add(new RecipeIngredient
            {
                InventoryItemId = ing.InventoryItemId,
                Quantity = ing.Quantity,
                Unit = ing.Unit,
                WasteFactor = ing.WasteFactor
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        return await GetByProductAsync(recipe.ProductId, cancellationToken);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var recipe = await _context.Recipes.FindAsync([id], cancellationToken);
        if (recipe == null)
            return Result<bool>.Failure("Receta no encontrada");

        _context.Recipes.Remove(recipe);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<FoodCostReportDto>> GetFoodCostReportAsync(CancellationToken cancellationToken = default)
    {
        var recipes = await _context.Recipes
            .Include(r => r.Product)
            .Include(r => r.Ingredients)
                .ThenInclude(i => i.InventoryItem)
            .ToListAsync(cancellationToken);

        var productCosts = new List<ProductCostDto>();

        foreach (var recipe in recipes)
        {
            var product = recipe.Product;
            var costPrice = CalculateCostPrice(recipe);
            var margin = product.Price - costPrice;
            var marginPct = product.Price > 0 ? (margin / product.Price) * 100 : 0;

            var status = marginPct switch
            {
                > 60 => "OK",
                > 40 => "WARNING",
                _ => "CRITICAL"
            };

            productCosts.Add(new ProductCostDto(
                product.Id,
                product.Name,
                product.Price,
                costPrice,
                margin,
                Math.Round(marginPct, 2),
                status
            ));
        }

        var avgFoodCostPct = productCosts.Count > 0
            ? productCosts.Average(p => p.CostPrice > 0 ? (p.CostPrice / p.SellingPrice) * 100 : 0)
            : 0;

        return Result<FoodCostReportDto>.Success(new FoodCostReportDto(
            productCosts,
            Math.Round(avgFoodCostPct, 2)
        ));
    }

    private static decimal CalculateCostPrice(Recipe recipe)
    {
        return recipe.Ingredients.Sum(ing =>
            ing.Quantity * (1 + ing.WasteFactor) * (ing.InventoryItem?.LastPurchaseCost ?? ing.InventoryItem?.AverageCost ?? 0)
        );
    }

    private static RecipeDto MapToDto(Recipe recipe)
    {
        var costPrice = CalculateCostPrice(recipe);
        var sellingPrice = recipe.Product.Price;
        var margin = sellingPrice - costPrice;

        var ingredients = recipe.Ingredients.Select(ing =>
        {
            var unitCost = ing.InventoryItem?.LastPurchaseCost ?? ing.InventoryItem?.AverageCost ?? 0;
            var totalCost = ing.Quantity * (1 + ing.WasteFactor) * unitCost;
            return new RecipeIngredientDto(
                ing.Id,
                ing.InventoryItemId,
                ing.InventoryItem?.Product?.Name ?? "Ingrediente",
                ing.Quantity,
                ing.Unit,
                ing.WasteFactor,
                unitCost,
                totalCost
            );
        });

        return new RecipeDto(
            recipe.Id,
            recipe.ProductId,
            recipe.Product.Name,
            recipe.Instructions,
            recipe.PreparationMinutes,
            recipe.EstimatedCostOverride > 0 ? recipe.EstimatedCostOverride : costPrice,
            costPrice,
            margin,
            ingredients
        );
    }
}
