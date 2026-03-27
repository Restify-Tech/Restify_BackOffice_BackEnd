namespace Restify.BackOffice.Application.DTOs;

public record RecipeIngredientDto(
    Guid Id,
    Guid InventoryItemId,
    string IngredientName,
    decimal Quantity,
    string Unit,
    decimal WasteFactor,
    decimal UnitCost,
    decimal TotalCost
);

public record RecipeDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string? Instructions,
    int PreparationMinutes,
    decimal EstimatedCost,
    decimal ActualCost,
    decimal Margin,
    IEnumerable<RecipeIngredientDto> Ingredients
);

public record CreateRecipeIngredientRequest(
    Guid InventoryItemId,
    decimal Quantity,
    string Unit,
    decimal WasteFactor
);

public record CreateRecipeRequest(
    Guid ProductId,
    string? Instructions,
    int PreparationMinutes,
    List<CreateRecipeIngredientRequest> Ingredients
);

public record ProductCostDto(
    Guid ProductId,
    string ProductName,
    decimal SellingPrice,
    decimal CostPrice,
    decimal Margin,
    decimal MarginPercentage,
    string CostStatus // "OK" | "WARNING" | "CRITICAL"
);

public record FoodCostReportDto(
    IEnumerable<ProductCostDto> Products,
    decimal AvgFoodCostPercentage
);
