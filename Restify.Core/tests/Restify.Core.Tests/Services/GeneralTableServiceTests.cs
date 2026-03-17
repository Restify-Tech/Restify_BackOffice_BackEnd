using FluentAssertions;
using Moq;
using Restify.Core.Application.DTOs.General;
using Restify.Core.Application.Interfaces;
using Restify.Core.Domain.Entities;
using Restify.Core.Infrastructure.Services;

namespace Restify.Core.Tests.Services;

public class GeneralTableServiceTests
{
    private readonly Mock<IGeneralTableRepository> _tableRepoMock;
    private readonly GeneralTableService _service;

    public GeneralTableServiceTests()
    {
        _tableRepoMock = new Mock<IGeneralTableRepository>();
        _service = new GeneralTableService(_tableRepoMock.Object);
    }

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_ReturnsAllTables()
    {
        // Arrange
        var tables = new List<GeneralTable>
        {
            CreateGeneralTable("ESTADOS_PEDIDO", "Estados de Pedido"),
            CreateGeneralTable("TIPOS_PAGO", "Tipos de Pago")
        };
        _tableRepoMock.Setup(r => r.GetAllAsync(true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tables);

        // Act
        var result = await _service.GetAllAsync(true);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data![0].Code.Should().Be("ESTADOS_PEDIDO");
        result.Data[1].Code.Should().Be("TIPOS_PAGO");
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ReturnsEmptyList()
    {
        // Arrange
        _tableRepoMock.Setup(r => r.GetAllAsync(true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GeneralTable>());

        // Act
        var result = await _service.GetAllAsync(true);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    #endregion

    #region GetByCodeAsync

    [Fact]
    public async Task GetByCodeAsync_WhenExists_ReturnsTableWithValues()
    {
        // Arrange
        var table = CreateGeneralTable("ESTADOS_PEDIDO", "Estados de Pedido");
        table.Values = new List<GeneralValue>
        {
            new GeneralValue
            {
                Id = Guid.NewGuid(),
                GeneralTableId = table.Id,
                Code = "PENDING",
                Content = "Pendiente",
                IsActive = true
            }
        };

        _tableRepoMock.Setup(r => r.GetByCodeAsync("ESTADOS_PEDIDO", true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(table);

        // Act
        var result = await _service.GetByCodeAsync("ESTADOS_PEDIDO", true);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Code.Should().Be("ESTADOS_PEDIDO");
        result.Data.Name.Should().Be("Estados de Pedido");
    }

    [Fact]
    public async Task GetByCodeAsync_WhenNotFound_ReturnsFailure()
    {
        // Arrange
        _tableRepoMock.Setup(r => r.GetByCodeAsync("NONEXISTENT", false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeneralTable?)null);

        // Act
        var result = await _service.GetByCodeAsync("NONEXISTENT", false);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("NONEXISTENT");
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsTable()
    {
        // Arrange
        var id = Guid.NewGuid();
        var table = CreateGeneralTable("TIPOS_PAGO", "Tipos de Pago");
        table.Id = id;

        _tableRepoMock.Setup(r => r.GetByIdAsync(id, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(table);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Id.Should().Be(id);
        result.Data.Code.Should().Be("TIPOS_PAGO");
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        _tableRepoMock.Setup(r => r.GetByIdAsync(id, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeneralTable?)null);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    #endregion

    #region GetByApplicationCodeAsync

    [Fact]
    public async Task GetByApplicationCodeAsync_ReturnsFilteredTables()
    {
        // Arrange
        var tables = new List<GeneralTable>
        {
            CreateGeneralTable("TIPO_DOC", "Tipos de Documento")
        };
        _tableRepoMock.Setup(r => r.GetByApplicationCodeAsync("AUTH", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tables);

        // Act
        var result = await _service.GetByApplicationCodeAsync("AUTH");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        result.Data![0].Code.Should().Be("TIPO_DOC");
    }

    #endregion

    #region GetChildrenAsync

    [Fact]
    public async Task GetChildrenAsync_ReturnsChildTables()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var children = new List<GeneralTable>
        {
            CreateGeneralTable("CHILD1", "Hijo 1"),
            CreateGeneralTable("CHILD2", "Hijo 2")
        };
        _tableRepoMock.Setup(r => r.GetChildrenAsync(parentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(children);

        // Act
        var result = await _service.GetChildrenAsync(parentId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    #endregion

    #region GetTreeAsync

    [Fact]
    public async Task GetTreeAsync_ReturnsRootTables()
    {
        // Arrange
        var roots = new List<GeneralTable>
        {
            CreateGeneralTable("ROOT1", "Raiz 1"),
            CreateGeneralTable("ROOT2", "Raiz 2")
        };
        _tableRepoMock.Setup(r => r.GetRootTablesAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(roots);

        // Act
        var result = await _service.GetTreeAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetTreeAsync_WithApplicationCode_FiltersResults()
    {
        // Arrange
        var roots = new List<GeneralTable>
        {
            CreateGeneralTable("ROOT1", "Raiz 1")
        };
        _tableRepoMock.Setup(r => r.GetRootTablesAsync("BACKOFFICE", It.IsAny<CancellationToken>()))
            .ReturnsAsync(roots);

        // Act
        var result = await _service.GetTreeAsync("BACKOFFICE");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateGeneralTableRequest
        {
            Code = "TIPOS_PAGO",
            Name = "Tipos de Pago",
            Description = "Metodos de pago aceptados",
            IsActive = true
        };

        _tableRepoMock.Setup(r => r.ExistsAsync("TIPOS_PAGO", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _tableRepoMock.Setup(r => r.AddAsync(It.IsAny<GeneralTable>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeneralTable e, CancellationToken _) => e);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Code.Should().Be("TIPOS_PAGO");
        result.Data.Name.Should().Be("Tipos de Pago");
        _tableRepoMock.Verify(r => r.AddAsync(It.IsAny<GeneralTable>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateCode_ReturnsFailure()
    {
        // Arrange
        var request = new CreateGeneralTableRequest
        {
            Code = "TIPOS_PAGO",
            Name = "Tipos de Pago"
        };

        _tableRepoMock.Setup(r => r.ExistsAsync("TIPOS_PAGO", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("TIPOS_PAGO");
        _tableRepoMock.Verify(r => r.AddAsync(It.IsAny<GeneralTable>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidParentId_ReturnsFailure()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var request = new CreateGeneralTableRequest
        {
            Code = "SUBTIPO",
            Name = "Sub Tipo",
            ParentId = parentId
        };

        _tableRepoMock.Setup(r => r.ExistsAsync("SUBTIPO", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _tableRepoMock.Setup(r => r.GetByIdAsync(parentId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeneralTable?)null);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("padre");
    }

    [Fact]
    public async Task CreateAsync_WithValidParentId_ReturnsSuccess()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var parent = CreateGeneralTable("PARENT", "Padre");
        parent.Id = parentId;

        var request = new CreateGeneralTableRequest
        {
            Code = "CHILD",
            Name = "Hijo",
            ParentId = parentId
        };

        _tableRepoMock.Setup(r => r.ExistsAsync("CHILD", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _tableRepoMock.Setup(r => r.GetByIdAsync(parentId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parent);
        _tableRepoMock.Setup(r => r.AddAsync(It.IsAny<GeneralTable>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeneralTable e, CancellationToken _) => e);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.ParentId.Should().Be(parentId);
    }

    [Fact]
    public async Task CreateAsync_WithExtraConfig_SerializesToJson()
    {
        // Arrange
        var request = new CreateGeneralTableRequest
        {
            Code = "WITH_CONFIG",
            Name = "Con Config",
            ExtraConfig = new Dictionary<string, object> { { "key", "value" } }
        };

        _tableRepoMock.Setup(r => r.ExistsAsync("WITH_CONFIG", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _tableRepoMock.Setup(r => r.AddAsync(It.IsAny<GeneralTable>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeneralTable e, CancellationToken _) => e);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _tableRepoMock.Verify(r => r.AddAsync(
            It.Is<GeneralTable>(t => t.ExtraConfig != null && t.ExtraConfig.Contains("key")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_WhenExists_ReturnsSuccess()
    {
        // Arrange
        var existingId = Guid.NewGuid();
        var existing = CreateGeneralTable("TIPOS_PAGO", "Tipos de Pago");
        existing.Id = existingId;

        var request = new UpdateGeneralTableRequest
        {
            Id = existingId,
            Code = "TIPOS_PAGO",
            Name = "Tipos de Pago Actualizados",
            IsActive = true
        };

        _tableRepoMock.Setup(r => r.GetByIdAsync(existingId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _service.UpdateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("Tipos de Pago Actualizados");
        _tableRepoMock.Verify(r => r.UpdateAsync(It.IsAny<GeneralTable>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotFound_ReturnsFailure()
    {
        // Arrange
        var request = new UpdateGeneralTableRequest { Id = Guid.NewGuid(), Code = "X", Name = "X" };
        _tableRepoMock.Setup(r => r.GetByIdAsync(request.Id, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeneralTable?)null);

        // Act
        var result = await _service.UpdateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    [Fact]
    public async Task UpdateAsync_WhenChangingCodeToDuplicate_ReturnsFailure()
    {
        // Arrange
        var existingId = Guid.NewGuid();
        var existing = CreateGeneralTable("ORIGINAL_CODE", "Original");
        existing.Id = existingId;

        var request = new UpdateGeneralTableRequest
        {
            Id = existingId,
            Code = "DUPLICATE_CODE",
            Name = "Updated"
        };

        _tableRepoMock.Setup(r => r.GetByIdAsync(existingId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _tableRepoMock.Setup(r => r.ExistsAsync("DUPLICATE_CODE", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.UpdateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("DUPLICATE_CODE");
    }

    [Fact]
    public async Task UpdateAsync_WhenSelfReferenceAsParent_ReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existing = CreateGeneralTable("TABLE", "Tabla");
        existing.Id = id;

        var request = new UpdateGeneralTableRequest
        {
            Id = id,
            Code = "TABLE",
            Name = "Tabla",
            ParentId = id
        };

        _tableRepoMock.Setup(r => r.GetByIdAsync(id, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _service.UpdateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("propio padre");
    }

    [Fact]
    public async Task UpdateAsync_WhenChangingParentToInvalid_ReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        var invalidParentId = Guid.NewGuid();
        var existing = CreateGeneralTable("TABLE", "Tabla");
        existing.Id = id;

        var request = new UpdateGeneralTableRequest
        {
            Id = id,
            Code = "TABLE",
            Name = "Tabla",
            ParentId = invalidParentId
        };

        _tableRepoMock.Setup(r => r.GetByIdAsync(id, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _tableRepoMock.Setup(r => r.GetByIdAsync(invalidParentId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeneralTable?)null);

        // Act
        var result = await _service.UpdateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("padre no existe");
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_WhenExists_ReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existing = CreateGeneralTable("TIPOS_PAGO", "Tipos de Pago");
        existing.Id = id;
        existing.IsInternal = false;

        _tableRepoMock.Setup(r => r.GetByIdAsync(id, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _tableRepoMock.Setup(r => r.HasChildrenAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _tableRepoMock.Setup(r => r.HasValuesAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
        _tableRepoMock.Verify(r => r.DeleteAsync(existing, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotFound_ReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        _tableRepoMock.Setup(r => r.GetByIdAsync(id, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeneralTable?)null);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    [Fact]
    public async Task DeleteAsync_WhenInternal_ReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existing = CreateGeneralTable("INTERNAL_TABLE", "Tabla Interna");
        existing.Id = id;
        existing.IsInternal = true;

        _tableRepoMock.Setup(r => r.GetByIdAsync(id, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("interna");
    }

    [Fact]
    public async Task DeleteAsync_WhenHasChildren_ReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existing = CreateGeneralTable("PARENT", "Tabla Padre");
        existing.Id = id;

        _tableRepoMock.Setup(r => r.GetByIdAsync(id, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _tableRepoMock.Setup(r => r.HasChildrenAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("hijas");
    }

    [Fact]
    public async Task DeleteAsync_WhenHasValues_ReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existing = CreateGeneralTable("WITH_VALUES", "Con Valores");
        existing.Id = id;

        _tableRepoMock.Setup(r => r.GetByIdAsync(id, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _tableRepoMock.Setup(r => r.HasChildrenAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _tableRepoMock.Setup(r => r.HasValuesAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("valores");
    }

    #endregion

    #region GetLookupAsync

    [Fact]
    public async Task GetLookupAsync_ReturnsLookupItems()
    {
        // Arrange
        var tables = new List<GeneralTable>
        {
            CreateGeneralTable("T1", "Tabla 1"),
            CreateGeneralTable("T2", "Tabla 2")
        };
        _tableRepoMock.Setup(r => r.GetAllAsync(true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tables);

        // Act
        var result = await _service.GetLookupAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data![0].Name.Should().Be("Tabla 1");
        result.Data![0].Code.Should().Be("T1");
    }

    #endregion

    #region Helpers

    private static GeneralTable CreateGeneralTable(string code, string name)
    {
        return new GeneralTable
        {
            Id = Guid.NewGuid(),
            Code = code,
            Name = name,
            IsActive = true,
            IsInternal = false,
            TenantId = Guid.NewGuid(),
            Children = new List<GeneralTable>(),
            Values = new List<GeneralValue>()
        };
    }

    #endregion
}
