using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface ICategoryService
{
    Task<Result<List<CategoryDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<List<CategoryDto>>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
    Task<Result<CategoryDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<CategoryDto>> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<Result<CategoryDto>> UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PagedResponse<CategoryDto>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
}
