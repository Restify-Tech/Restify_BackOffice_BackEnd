using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Category?> GetByIdWithSubCategoriesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetSubCategoriesAsync(Guid parentId, CancellationToken cancellationToken = default);
    Task<Category> AddAsync(Category entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Category entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Category entity, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<int> GetMaxDisplayOrderAsync(Guid? parentId, CancellationToken cancellationToken = default);
}
