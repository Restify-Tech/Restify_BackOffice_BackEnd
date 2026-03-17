using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Application.Mappings;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class AIImagePromptTemplateService : IAIImagePromptTemplateService
{
    private readonly IAIImagePromptTemplateRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public AIImagePromptTemplateService(
        IAIImagePromptTemplateRepository repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<AIImagePromptTemplateDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var template = await _repository.GetByIdAsync(id, cancellationToken);

        if (template == null)
            return Result<AIImagePromptTemplateDto>.Failure("Plantilla no encontrada");

        return Result<AIImagePromptTemplateDto>.Success(template.ToDto());
    }

    public async Task<Result<IEnumerable<AIImagePromptTemplateDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var templates = await _repository.GetAllAsync(cancellationToken);
        var dtos = templates.Select(t => t.ToDto());

        return Result<IEnumerable<AIImagePromptTemplateDto>>.Success(dtos);
    }

    public async Task<Result<AIImagePromptTemplateDto>> CreateAsync(CreateAIImagePromptTemplateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<AIImagePromptTemplateDto>.Failure("El nombre es requerido");

        if (string.IsNullOrWhiteSpace(request.PromptTemplate))
            return Result<AIImagePromptTemplateDto>.Failure("La plantilla de prompt es requerida");

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var entity = request.ToEntity(tenantId);

        // If this is set as default, unset other defaults
        if (request.IsDefault)
        {
            var currentDefault = await _repository.GetDefaultAsync(cancellationToken);
            if (currentDefault != null)
            {
                currentDefault.IsDefault = false;
                await _repository.UpdateAsync(currentDefault, cancellationToken);
            }
        }

        var created = await _repository.CreateAsync(entity, cancellationToken);

        return Result<AIImagePromptTemplateDto>.Success(created.ToDto());
    }

    public async Task<Result<AIImagePromptTemplateDto>> UpdateAsync(Guid id, UpdateAIImagePromptTemplateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<AIImagePromptTemplateDto>.Failure("El nombre es requerido");

        if (string.IsNullOrWhiteSpace(request.PromptTemplate))
            return Result<AIImagePromptTemplateDto>.Failure("La plantilla de prompt es requerida");

        var template = await _repository.GetByIdAsync(id, cancellationToken);
        if (template == null)
            return Result<AIImagePromptTemplateDto>.Failure("Plantilla no encontrada");

        // If setting as default, unset other defaults
        if (request.IsDefault && !template.IsDefault)
        {
            var currentDefault = await _repository.GetDefaultAsync(cancellationToken);
            if (currentDefault != null && currentDefault.Id != id)
            {
                currentDefault.IsDefault = false;
                await _repository.UpdateAsync(currentDefault, cancellationToken);
            }
        }

        template.UpdateFrom(request);
        var updated = await _repository.UpdateAsync(template, cancellationToken);

        return Result<AIImagePromptTemplateDto>.Success(updated.ToDto());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var template = await _repository.GetByIdAsync(id, cancellationToken);
        if (template == null)
            return Result<bool>.Failure("Plantilla no encontrada");

        await _repository.DeleteAsync(id, cancellationToken);

        return Result<bool>.Success(true);
    }
}
