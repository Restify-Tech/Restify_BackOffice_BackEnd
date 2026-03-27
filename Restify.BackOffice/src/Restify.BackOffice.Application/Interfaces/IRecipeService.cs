using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IRecipeService
{
    Task<Result<RecipeDto>> GetByProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<RecipeDto>> CreateAsync(CreateRecipeRequest request, CancellationToken cancellationToken = default);
    Task<Result<RecipeDto>> UpdateAsync(Guid id, CreateRecipeRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<FoodCostReportDto>> GetFoodCostReportAsync(CancellationToken cancellationToken = default);
}
