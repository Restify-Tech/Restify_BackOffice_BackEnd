using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IProductService
{
    Task<Result<ProductDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ProductDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ProductDto>>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<Result<ProductDto>> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<Result<ProductDto>> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
