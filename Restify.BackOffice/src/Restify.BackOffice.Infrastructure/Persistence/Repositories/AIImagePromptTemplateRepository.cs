using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class AIImagePromptTemplateRepository : IAIImagePromptTemplateRepository
{
    private readonly BackOfficeDbContext _context;

    public AIImagePromptTemplateRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<AIImagePromptTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AIImagePromptTemplates
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<AIImagePromptTemplate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AIImagePromptTemplates
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<AIImagePromptTemplate?> GetDefaultAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AIImagePromptTemplates
            .FirstOrDefaultAsync(x => x.IsDefault && x.IsActive, cancellationToken);
    }

    public async Task<AIImagePromptTemplate> CreateAsync(AIImagePromptTemplate template, CancellationToken cancellationToken = default)
    {
        _context.AIImagePromptTemplates.Add(template);
        await _context.SaveChangesAsync(cancellationToken);
        return template;
    }

    public async Task<AIImagePromptTemplate> UpdateAsync(AIImagePromptTemplate template, CancellationToken cancellationToken = default)
    {
        _context.AIImagePromptTemplates.Update(template);
        await _context.SaveChangesAsync(cancellationToken);
        return template;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var template = await _context.AIImagePromptTemplates
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (template != null)
        {
            _context.AIImagePromptTemplates.Remove(template);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
