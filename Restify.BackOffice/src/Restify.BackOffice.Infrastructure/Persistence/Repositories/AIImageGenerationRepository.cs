using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class AIImageGenerationRepository : IAIImageGenerationRepository
{
    private readonly BackOfficeDbContext _context;

    public AIImageGenerationRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<AIImageGeneration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AIImageGenerations
            .Include(x => x.Product)
            .Include(x => x.PromptTemplate)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<AIImageGeneration>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.AIImageGenerations
            .Include(x => x.Product)
            .Include(x => x.PromptTemplate)
            .Where(x => x.ProductId == productId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AIImageGeneration>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AIImageGenerations
            .Include(x => x.Product)
            .Include(x => x.PromptTemplate)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<AIImageGeneration> CreateAsync(AIImageGeneration generation, CancellationToken cancellationToken = default)
    {
        _context.AIImageGenerations.Add(generation);
        await _context.SaveChangesAsync(cancellationToken);
        return generation;
    }

    public async Task<AIImageGeneration> UpdateAsync(AIImageGeneration generation, CancellationToken cancellationToken = default)
    {
        _context.AIImageGenerations.Update(generation);
        await _context.SaveChangesAsync(cancellationToken);
        return generation;
    }
}
