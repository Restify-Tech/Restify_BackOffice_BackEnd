using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class GlobalModifierRepository : IGlobalModifierRepository
{
    private readonly BackOfficeDbContext _context;

    public GlobalModifierRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<GlobalModifier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.GlobalModifiers
            .Include(m => m.Products)
                .ThenInclude(gmp => gmp.Product)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<GlobalModifier>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.GlobalModifiers
            .Include(m => m.Products)
            .OrderBy(m => m.Type)
            .ThenBy(m => m.DisplayOrder)
            .ThenBy(m => m.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<GlobalModifier>> GetByTypeAsync(ModifierType type, CancellationToken cancellationToken = default)
    {
        return await _context.GlobalModifiers
            .Include(m => m.Products)
            .Where(m => m.Type == type)
            .OrderBy(m => m.DisplayOrder)
            .ThenBy(m => m.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<GlobalModifier> CreateAsync(GlobalModifier modifier, CancellationToken cancellationToken = default)
    {
        await _context.GlobalModifiers.AddAsync(modifier, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return (await GetByIdAsync(modifier.Id, cancellationToken))!;
    }

    public async Task<GlobalModifier> UpdateAsync(GlobalModifier modifier, CancellationToken cancellationToken = default)
    {
        _context.GlobalModifiers.Update(modifier);
        await _context.SaveChangesAsync(cancellationToken);
        
        return (await GetByIdAsync(modifier.Id, cancellationToken))!;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var modifier = await _context.GlobalModifiers.FindAsync(new object[] { id }, cancellationToken);
        if (modifier != null)
        {
            _context.GlobalModifiers.Remove(modifier);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<GlobalModifier>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.GlobalModifiers
            .Include(m => m.Products)
            .Where(m => m.Products.Any(gmp => gmp.ProductId == productId))
            .OrderBy(m => m.Type)
            .ThenBy(m => m.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task AssignToProductAsync(Guid modifierId, Guid productId, GlobalModifierProduct assignment, CancellationToken cancellationToken = default)
    {
        await _context.GlobalModifierProducts.AddAsync(assignment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveFromProductAsync(Guid modifierId, Guid productId, CancellationToken cancellationToken = default)
    {
        var assignment = await _context.GlobalModifierProducts
            .FirstOrDefaultAsync(gmp => gmp.GlobalModifierId == modifierId && gmp.ProductId == productId, cancellationToken);

        if (assignment != null)
        {
            _context.GlobalModifierProducts.Remove(assignment);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
