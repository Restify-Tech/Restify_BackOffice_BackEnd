using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly BackOfficeDbContext _context;

    public ProductRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Modifiers)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Modifiers)
            .OrderBy(p => p.DisplayOrder)
            .ThenBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Modifiers)
            .Where(p => p.CategoryId == categoryId)
            .OrderBy(p => p.DisplayOrder)
            .ThenBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        // Recargar con navegación
        return (await GetByIdAsync(product.Id, cancellationToken))!;
    }

    public async Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
        
        // Recargar con navegación
        return (await GetByIdAsync(product.Id, cancellationToken))!;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products.FindAsync(new object[] { id }, cancellationToken);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
