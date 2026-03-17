using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly BackOfficeDbContext _context;

    public CategoryRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(x => x.ParentCategory)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Category?> GetByIdWithSubCategoriesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(x => x.ParentCategory)
            .Include(x => x.SubCategories.OrderBy(s => s.DisplayOrder))
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(x => x.ParentCategory)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Where(x => x.ParentCategoryId == null)
            .Include(x => x.SubCategories.OrderBy(s => s.DisplayOrder))
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetSubCategoriesAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Where(x => x.ParentCategoryId == parentId)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category> AddAsync(Category entity, CancellationToken cancellationToken = default)
    {
        await _context.Categories.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Category entity, CancellationToken cancellationToken = default)
    {
        _context.Categories.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Category entity, CancellationToken cancellationToken = default)
    {
        _context.Categories.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AnyAsync(x => x.Name == name && (excludeId == null || x.Id != excludeId), cancellationToken);
    }

    public async Task<int> GetMaxDisplayOrderAsync(Guid? parentId, CancellationToken cancellationToken = default)
    {
        var query = _context.Categories.Where(x => x.ParentCategoryId == parentId);
        return await query.AnyAsync(cancellationToken)
            ? await query.MaxAsync(x => x.DisplayOrder, cancellationToken)
            : 0;
    }
}
