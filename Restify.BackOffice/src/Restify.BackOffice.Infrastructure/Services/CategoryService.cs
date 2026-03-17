using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Application.Mappings;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<CategoryDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        var dtos = entities.Select(e => e.ToDto()).ToList();
        return Result<List<CategoryDto>>.Success(dtos);
    }

    public async Task<Result<List<CategoryDto>>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetRootCategoriesAsync(cancellationToken);
        var dtos = entities.Select(e => e.ToDto(includeSubCategories: true)).ToList();
        return Result<List<CategoryDto>>.Success(dtos);
    }

    public async Task<Result<CategoryDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdWithSubCategoriesAsync(id, cancellationToken);
        if (entity == null)
            return Result<CategoryDto>.Failure("Categoría no encontrada");

        return Result<CategoryDto>.Success(entity.ToDto(includeSubCategories: true));
    }

    public async Task<Result<CategoryDto>> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        if (await _repository.ExistsAsync(request.Name, cancellationToken: cancellationToken))
            return Result<CategoryDto>.Failure($"Ya existe una categoría con el nombre '{request.Name}'");

        var entity = request.ToEntity();

        // Auto-asignar orden si no se especifica
        if (entity.DisplayOrder == 0)
        {
            entity.DisplayOrder = await _repository.GetMaxDisplayOrderAsync(entity.ParentCategoryId, cancellationToken) + 1;
        }

        await _repository.AddAsync(entity, cancellationToken);

        return Result<CategoryDto>.Success(entity.ToDto());
    }

    public async Task<Result<CategoryDto>> UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity == null)
            return Result<CategoryDto>.Failure("Categoría no encontrada");

        if (await _repository.ExistsAsync(request.Name, request.Id, cancellationToken))
            return Result<CategoryDto>.Failure($"Ya existe otra categoría con el nombre '{request.Name}'");

        entity.UpdateFrom(request);
        await _repository.UpdateAsync(entity, cancellationToken);

        return Result<CategoryDto>.Success(entity.ToDto());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdWithSubCategoriesAsync(id, cancellationToken);
        if (entity == null)
            return Result<bool>.Failure("Categoría no encontrada");

        if (entity.SubCategories?.Count > 0)
            return Result<bool>.Failure("No se puede eliminar una categoría que tiene subcategorías");

        await _repository.DeleteAsync(entity, cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<PagedResponse<CategoryDto>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);

        // Aplicar filtros
        var query = entities.AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var search = request.SearchTerm.ToLower();
            query = query.Where(x =>
                x.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (x.Description != null && x.Description.Contains(search, StringComparison.OrdinalIgnoreCase)));
        }

        // Ordenar
        if (!string.IsNullOrEmpty(request.SortColumn))
        {
            query = request.SortColumn.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
                "displayorder" => request.SortDescending ? query.OrderByDescending(x => x.DisplayOrder) : query.OrderBy(x => x.DisplayOrder),
                "isactive" => request.SortDescending ? query.OrderByDescending(x => x.IsActive) : query.OrderBy(x => x.IsActive),
                _ => query.OrderBy(x => x.DisplayOrder)
            };
        }
        else
        {
            query = query.OrderBy(x => x.DisplayOrder);
        }

        var totalCount = query.Count();
        var items = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => e.ToDto())
            .ToList();

        var response = new PagedResponse<CategoryDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };

        return Result<PagedResponse<CategoryDto>>.Success(response);
    }
}
