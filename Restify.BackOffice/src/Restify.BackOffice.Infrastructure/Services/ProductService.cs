using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Application.Mappings;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICurrentUserService _currentUserService;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ICurrentUserService currentUserService)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ProductDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);

        if (product == null)
            return Result<ProductDto>.Failure("Producto no encontrado");

        return Result<ProductDto>.Success(product.ToDto());
    }

    public async Task<Result<IEnumerable<ProductDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        var dtos = products.Select(p => p.ToDto());

        return Result<IEnumerable<ProductDto>>.Success(dtos);
    }

    public async Task<Result<IEnumerable<ProductDto>>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        // Verificar que la categoría existe
        var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
        if (category == null)
            return Result<IEnumerable<ProductDto>>.Failure("Categoría no encontrada");

        var products = await _productRepository.GetByCategoryIdAsync(categoryId, cancellationToken);
        var dtos = products.Select(p => p.ToDto());

        return Result<IEnumerable<ProductDto>>.Success(dtos);
    }

    public async Task<Result<ProductDto>> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        // Validaciones
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<ProductDto>.Failure("El nombre del producto es requerido");

        if (request.Price < 0)
            return Result<ProductDto>.Failure("El precio debe ser mayor o igual a cero");

        // Verificar que la categoría existe
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category == null)
            return Result<ProductDto>.Failure("La categoría especificada no existe");

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var product = request.ToEntity(tenantId);

        var created = await _productRepository.CreateAsync(product, cancellationToken);

        return Result<ProductDto>.Success(created.ToDto());
    }

    public async Task<Result<ProductDto>> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        // Validaciones
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<ProductDto>.Failure("El nombre del producto es requerido");

        if (request.Price < 0)
            return Result<ProductDto>.Failure("El precio debe ser mayor o igual a cero");

        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null)
            return Result<ProductDto>.Failure("Producto no encontrado");

        // Verificar que la categoría existe
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category == null)
            return Result<ProductDto>.Failure("La categoría especificada no existe");

        product.UpdateFromRequest(request);
        var updated = await _productRepository.UpdateAsync(product, cancellationToken);

        return Result<ProductDto>.Success(updated.ToDto());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null)
            return Result<bool>.Failure("Producto no encontrado");

        await _productRepository.DeleteAsync(id, cancellationToken);

        return Result<bool>.Success(true);
    }
}
