---
name: dotnet-best-practices
description: Buenas practicas C#/.NET para el ecosistema Taku Soft Tech (Clean Architecture, Result<T>, FluentValidation, xUnit, Serilog, multi-tenancy). Usalo al revisar o escribir codigo .NET. Keywords: buenas practicas, clean code, SOLID, async, testing, logging, validacion, seguridad, xUnit, FluentValidation.
allowed-tools: Read, Grep, Glob
---

# .NET Best Practices — Taku Soft Tech

## Arquitectura (Clean Architecture)

Dependencias unidireccionales:
```
Domain ← Application ← Infrastructure ← Api
```

| Capa | Contiene | Prohibido |
|------|----------|-----------|
| **Domain** | Entidades, enums, interfaces de dominio | EF Core, HTTP, logging frameworks |
| **Application** | DTOs, interfaces repos/servicios, validators | DbContext, implementaciones concretas |
| **Infrastructure** | Repos, DbContext, clientes HTTP, messaging | Logica de negocio |
| **Api** | Controllers, middleware, Program.cs | Logica de negocio directa |

## Result<T> Pattern (obligatorio)

```csharp
// CORRECTO — flujos de negocio via Result<T>
public async Task<Result<ProductDto>> GetByIdAsync(Guid id)
{
    var product = await _repo.GetByIdAsync(id);
    if (product is null) return Result<ProductDto>.Failure("Producto no encontrado");
    return Result<ProductDto>.Success(product.ToDto());
}

// INCORRECTO — no lanzar excepciones para flujos esperados
throw new NotFoundException("Producto no encontrado"); // ❌
```

Controllers mapean Result a HTTP:
```csharp
return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
```

## FluentValidation

```csharp
// Un validator por cada Request/Command
public class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("El nombre es requerido");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("El precio debe ser mayor a 0");
        RuleFor(x => x.TenantId).NotEmpty().WithMessage("TenantId es requerido");
    }
}
```

- Mensajes en **espanol**
- Registrar con `AddValidatorsFromAssembly`
- Activar con `AddFluentValidationAutoValidation()`
- NO usar `[Required]` / DataAnnotations para logica de negocio

## Multi-tenancy

```csharp
// Query Filter en DbContext
modelBuilder.Entity<Product>().HasQueryFilter(p => p.TenantId == _tenantId);

// TenantId del JWT — NO del body del request
var tenantId = Guid.Parse(httpContext.User.FindFirst("tenant_id")!.Value);
```

- SuperAdmin usa `TenantId = Guid.Empty` para ver todos los tenants
- NUNCA consultar sin filtro de tenant en entidades multi-tenant

## Async/Await

```csharp
// CORRECTO
public async Task<Result<T>> GetAsync(CancellationToken ct = default)
{
    return await _repo.GetAsync(ct);
}

// INCORRECTO — bloquea el thread
var result = _service.GetAsync().Result; // ❌
_service.GetAsync().Wait(); // ❌
```

- Todos los metodos de I/O deben ser `async Task<T>`
- NO usar `.Result`, `.Wait()`, ni `async void` (salvo event handlers)
- Pasar `CancellationToken` en operaciones largas

## Inyeccion de Dependencias

```csharp
// Primary constructor syntax (.NET 10)
public class ProductService(IProductRepository repo, ILogger<ProductService> logger)
{
    private readonly IProductRepository _repo = repo;
    private readonly ILogger<ProductService> _logger = logger;
}
```

- Lifetimes: Repositories = Scoped, DbContext = Scoped, Factories = Singleton
- Inyectar interfaces, NO implementaciones concretas

## Logging (Serilog)

```csharp
// CORRECTO — structured logging con message templates
_logger.LogInformation("Producto creado {ProductId} para tenant {TenantId}", id, tenantId);

// INCORRECTO — string interpolation pierde los properties estructurados
_logger.LogInformation($"Producto {id} creado"); // ❌
```

- Usar Serilog con `AddSerilog()` en Program.cs
- Nivel minimo: Information en app, Warning para Microsoft/System
- Incluir contexto: TenantId, EntityId en logs de operaciones criticas

## Testing (xUnit + Moq)

```csharp
[Fact]
public async Task GetById_WhenProductExists_ReturnsSuccess()
{
    // Arrange
    var repoMock = new Mock<IProductRepository>();
    repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Product { Id = Guid.NewGuid(), Name = "Test" });
    var service = new ProductService(repoMock.Object, Mock.Of<ILogger<ProductService>>());

    // Act
    var result = await service.GetByIdAsync(Guid.NewGuid());

    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
}

[Fact]
public async Task GetById_WhenProductNotFound_ReturnsFailure()
{
    // Arrange
    var repoMock = new Mock<IProductRepository>();
    repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Product?)null);
    var service = new ProductService(repoMock.Object, Mock.Of<ILogger<ProductService>>());

    // Act
    var result = await service.GetByIdAsync(Guid.NewGuid());

    // Assert
    Assert.False(result.IsSuccess);
    Assert.NotEmpty(result.Error);
}
```

- Framework: **xUnit** (NO MSTest)
- Mocking: **Moq**
- Patron: AAA (Arrange, Act, Assert)
- Testear: exito, fallo, casos borde (null, empty, invalid)

## Null Safety

```csharp
// Operadores null-safe
var name = product?.Name ?? "Sin nombre";
if (product is null) return Result<ProductDto>.Failure("No encontrado");

// Habilitar en csproj
<Nullable>enable</Nullable>
```

## Seguridad

- NUNCA hardcodear secretos — appsettings.json con valores placeholder
- NUNCA exponer entidades de dominio en responses — siempre DTOs
- BCrypt con factor 12 para passwords
- JWT SecretKey minimo 32 caracteres
- Parameterizar queries (EF Core lo hace automaticamente; en MongoDB usar filtros tipados)

## SOLID

| Principio | Ejemplo en el ecosistema |
|-----------|--------------------------|
| **SRP** | `ProductService` solo gestiona productos, `OrderService` solo ordenes |
| **OCP** | Nuevos providers (pais, gateway) sin modificar clases existentes |
| **LSP** | `ManualGatewayProvider` y `DeunaGatewayProvider` intercambiables via `IGatewayProvider` |
| **ISP** | `IProductRepository`, `IOrderRepository` separados (no una mega-interfaz) |
| **DIP** | Controllers y servicios dependen de interfaces, no implementaciones |

## Checklist de Revision

- [ ] Operaciones de I/O son `async Task<T>`
- [ ] Servicios retornan `Result<T>` o `Result`
- [ ] Cada Request tiene su validator FluentValidation
- [ ] Mensajes de validacion en espanol
- [ ] TenantId filtrado en queries multi-tenant
- [ ] Logs con Serilog message templates (no string interpolation)
- [ ] Tests con xUnit + Moq + patron AAA
- [ ] Sin secretos hardcodeados
- [ ] DTOs no exponen entidades de dominio
- [ ] Interfaces inyectadas (no implementaciones concretas)
