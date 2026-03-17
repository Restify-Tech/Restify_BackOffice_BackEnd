using FluentAssertions;
using Moq;
using Restify.Core.Application.DTOs.General;
using Restify.Core.Application.Interfaces;
using Restify.Core.Domain.Entities;
using Restify.Core.Infrastructure.Services;

namespace Restify.Core.Tests.Services;

public class GeneralValueServiceTests
{
    private readonly Mock<IGeneralValueRepository> _valueRepoMock;
    private readonly Mock<IGeneralTableRepository> _tableRepoMock;
    private readonly GeneralValueService _service;

    public GeneralValueServiceTests()
    {
        _valueRepoMock = new Mock<IGeneralValueRepository>();
        _tableRepoMock = new Mock<IGeneralTableRepository>();
        _service = new GeneralValueService(_valueRepoMock.Object, _tableRepoMock.Object);
    }

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsValue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var value = CreateGeneralValue(id, "PENDING", "Pendiente");

        _valueRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(value);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Code.Should().Be("PENDING");
        result.Data.Content.Should().Be("Pendiente");
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        _valueRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeneralValue?)null);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrado");
    }

    #endregion

    #region GetByCodeAsync

    [Fact]
    public async Task GetByCodeAsync_WhenTableAndValueExist_ReturnsValue()
    {
        // Arrange
        var tableId = Guid.NewGuid();
        var table = CreateGeneralTable(tableId, "ESTADOS_PEDIDO", "Estados");
        var value = CreateGeneralValue(Guid.NewGuid(), "PENDING", "Pendiente", tableId);

        _tableRepoMock.Setup(r => r.GetByCodeAsync("ESTADOS_PEDIDO", false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(table);
        _valueRepoMock.Setup(r => r.GetByCodeAsync(tableId, "PENDING", It.IsAny<CancellationToken>()))
            .ReturnsAsync(value);

        // Act
        var result = await _service.GetByCodeAsync("ESTADOS_PEDIDO", "PENDING");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Code.Should().Be("PENDING");
    }

    [Fact]
    public async Task GetByCodeAsync_WhenTableNotFound_ReturnsFailure()
    {
        // Arrange
        _tableRepoMock.Setup(r => r.GetByCodeAsync("NONEXISTENT", false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeneralTable?)null);

        // Act
        var result = await _service.GetByCodeAsync("NONEXISTENT", "CODE");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("NONEXISTENT");
    }

    [Fact]
    public async Task GetByCodeAsync_WhenValueNotFound_ReturnsFailure()
    {
        // Arrange
        var tableId = Guid.NewGuid();
        var table = CreateGeneralTable(tableId, "ESTADOS", "Estados");

        _tableRepoMock.Setup(r => r.GetByCodeAsync("ESTADOS", false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(table);
        _valueRepoMock.Setup(r => r.GetByCodeAsync(tableId, "MISSING", It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeneralValue?)null);

        // Act
        var result = await _service.GetByCodeAsync("ESTADOS", "MISSING");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("MISSING");
    }

    #endregion

    #region GetByTableIdAsync

    [Fact]
    public async Task GetByTableIdAsync_ReturnsValues()
    {
        // Arrange
        var tableId = Guid.NewGuid();
        var values = new List<GeneralValue>
        {
            CreateGeneralValue(Guid.NewGuid(), "V1", "Valor 1", tableId),
            CreateGeneralValue(Guid.NewGuid(), "V2", "Valor 2", tableId)
        };

        _valueRepoMock.Setup(r => r.GetByTableIdAsync(tableId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(values);

        // Act
        var result = await _service.GetByTableIdAsync(tableId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    #endregion

    #region GetByTableCodeAsync

    [Fact]
    public async Task GetByTableCodeAsync_WhenTableExists_ReturnsValues()
    {
        // Arrange
        var tableId = Guid.NewGuid();
        var table = CreateGeneralTable(tableId, "TIPOS_PAGO", "Tipos");
        var values = new List<GeneralValue>
        {
            CreateGeneralValue(Guid.NewGuid(), "CASH", "Efectivo", tableId)
        };

        _tableRepoMock.Setup(r => r.GetByCodeAsync("TIPOS_PAGO", false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(table);
        _valueRepoMock.Setup(r => r.GetByTableIdAsync(tableId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(values);

        // Act
        var result = await _service.GetByTableCodeAsync("TIPOS_PAGO");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        result.Data![0].Code.Should().Be("CASH");
    }

    [Fact]
    public async Task GetByTableCodeAsync_WhenTableNotFound_ReturnsFailure()
    {
        // Arrange
        _tableRepoMock.Setup(r => r.GetByCodeAsync("MISSING", false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeneralTable?)null);

        // Act
        var result = await _service.GetByTableCodeAsync("MISSING");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("MISSING");
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var tableId = Guid.NewGuid();
        var table = CreateGeneralTable(tableId, "ESTADOS", "Estados");

        var request = new CreateGeneralValueRequest
        {
            GeneralTableId = tableId,
            Code = "ACTIVE",
            Content = "Activo",
            IsActive = true,
            DisplayOrder = 0
        };

        _tableRepoMock.Setup(r => r.GetByIdAsync(tableId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(table);
        _valueRepoMock.Setup(r => r.ExistsAsync(tableId, "ACTIVE", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _valueRepoMock.Setup(r => r.AddAsync(It.IsAny<GeneralValue>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeneralValue e, CancellationToken _) => e);
        _valueRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) =>
            {
                var v = CreateGeneralValue(id, "ACTIVE", "Activo", tableId);
                v.GeneralTable = table;
                return v;
            });

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Code.Should().Be("ACTIVE");
        result.Data.Content.Should().Be("Activo");
        _valueRepoMock.Verify(r => r.AddAsync(It.IsAny<GeneralValue>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenTableNotFound_ReturnsFailure()
    {
        // Arrange
        var tableId = Guid.NewGuid();
        var request = new CreateGeneralValueRequest
        {
            GeneralTableId = tableId,
            Code = "V1",
            Content = "Valor"
        };

        _tableRepoMock.Setup(r => r.GetByIdAsync(tableId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeneralTable?)null);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("tabla no existe");
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateCode_ReturnsFailure()
    {
        // Arrange
        var tableId = Guid.NewGuid();
        var table = CreateGeneralTable(tableId, "ESTADOS", "Estados");

        var request = new CreateGeneralValueRequest
        {
            GeneralTableId = tableId,
            Code = "DUPLICATE",
            Content = "Duplicado"
        };

        _tableRepoMock.Setup(r => r.GetByIdAsync(tableId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(table);
        _valueRepoMock.Setup(r => r.ExistsAsync(tableId, "DUPLICATE", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("DUPLICATE");
    }

    [Fact]
    public async Task CreateAsync_WhenIsDefault_ClearsOtherDefaults()
    {
        // Arrange
        var tableId = Guid.NewGuid();
        var table = CreateGeneralTable(tableId, "ESTADOS", "Estados");

        var request = new CreateGeneralValueRequest
        {
            GeneralTableId = tableId,
            Code = "DEFAULT_VAL",
            Content = "Valor Default",
            IsDefault = true
        };

        _tableRepoMock.Setup(r => r.GetByIdAsync(tableId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(table);
        _valueRepoMock.Setup(r => r.ExistsAsync(tableId, "DEFAULT_VAL", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _valueRepoMock.Setup(r => r.AddAsync(It.IsAny<GeneralValue>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeneralValue e, CancellationToken _) => e);
        _valueRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) =>
            {
                var v = CreateGeneralValue(id, "DEFAULT_VAL", "Valor Default", tableId);
                v.GeneralTable = table;
                v.IsDefault = true;
                return v;
            });

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _valueRepoMock.Verify(r => r.ClearDefaultAsync(tableId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenNotDefault_DoesNotClearDefaults()
    {
        // Arrange
        var tableId = Guid.NewGuid();
        var table = CreateGeneralTable(tableId, "ESTADOS", "Estados");

        var request = new CreateGeneralValueRequest
        {
            GeneralTableId = tableId,
            Code = "NON_DEFAULT",
            Content = "No default",
            IsDefault = false
        };

        _tableRepoMock.Setup(r => r.GetByIdAsync(tableId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(table);
        _valueRepoMock.Setup(r => r.ExistsAsync(tableId, "NON_DEFAULT", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _valueRepoMock.Setup(r => r.AddAsync(It.IsAny<GeneralValue>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeneralValue e, CancellationToken _) => e);
        _valueRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) =>
            {
                var v = CreateGeneralValue(id, "NON_DEFAULT", "No default", tableId);
                v.GeneralTable = table;
                return v;
            });

        // Act
        await _service.CreateAsync(request);

        // Assert
        _valueRepoMock.Verify(r => r.ClearDefaultAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_WhenExists_ReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tableId = Guid.NewGuid();
        var existing = CreateGeneralValue(id, "ACTIVE", "Activo", tableId);
        existing.IsLocked = false;

        var request = new UpdateGeneralValueRequest
        {
            Id = id,
            GeneralTableId = tableId,
            Code = "ACTIVE",
            Content = "Activo Actualizado"
        };

        _valueRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _service.UpdateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Content.Should().Be("Activo Actualizado");
        _valueRepoMock.Verify(r => r.UpdateAsync(It.IsAny<GeneralValue>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotFound_ReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        _valueRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeneralValue?)null);

        var request = new UpdateGeneralValueRequest { Id = id, Code = "X", Content = "X" };

        // Act
        var result = await _service.UpdateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrado");
    }

    [Fact]
    public async Task UpdateAsync_WhenLocked_ReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existing = CreateGeneralValue(id, "LOCKED", "Bloqueado");
        existing.IsLocked = true;

        _valueRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var request = new UpdateGeneralValueRequest { Id = id, Code = "LOCKED", Content = "Nuevo" };

        // Act
        var result = await _service.UpdateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("bloqueado");
    }

    [Fact]
    public async Task UpdateAsync_WhenChangingCodeToDuplicate_ReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tableId = Guid.NewGuid();
        var existing = CreateGeneralValue(id, "ORIGINAL", "Original", tableId);

        _valueRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _valueRepoMock.Setup(r => r.ExistsAsync(tableId, "DUPLICATE", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new UpdateGeneralValueRequest
        {
            Id = id,
            GeneralTableId = tableId,
            Code = "DUPLICATE",
            Content = "Updated"
        };

        // Act
        var result = await _service.UpdateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("DUPLICATE");
    }

    [Fact]
    public async Task UpdateAsync_WhenSettingAsDefault_ClearsOtherDefaults()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tableId = Guid.NewGuid();
        var existing = CreateGeneralValue(id, "VAL", "Valor", tableId);
        existing.IsDefault = false;

        _valueRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var request = new UpdateGeneralValueRequest
        {
            Id = id,
            GeneralTableId = tableId,
            Code = "VAL",
            Content = "Valor",
            IsDefault = true
        };

        // Act
        await _service.UpdateAsync(request);

        // Assert
        _valueRepoMock.Verify(r => r.ClearDefaultAsync(tableId, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_WhenExists_ReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existing = CreateGeneralValue(id, "VAL", "Valor");
        existing.IsLocked = false;

        _valueRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _valueRepoMock.Verify(r => r.DeleteAsync(existing, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotFound_ReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        _valueRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeneralValue?)null);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrado");
    }

    [Fact]
    public async Task DeleteAsync_WhenLocked_ReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existing = CreateGeneralValue(id, "LOCKED", "Bloqueado");
        existing.IsLocked = true;

        _valueRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("bloqueado");
    }

    #endregion

    #region SetDefaultAsync

    [Fact]
    public async Task SetDefaultAsync_WhenExists_ClearsOthersAndSetsDefault()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tableId = Guid.NewGuid();
        var existing = CreateGeneralValue(id, "VAL", "Valor", tableId);
        existing.IsDefault = false;

        _valueRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _service.SetDefaultAsync(id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _valueRepoMock.Verify(r => r.ClearDefaultAsync(tableId, It.IsAny<CancellationToken>()), Times.Once);
        _valueRepoMock.Verify(r => r.UpdateAsync(
            It.Is<GeneralValue>(v => v.IsDefault == true),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SetDefaultAsync_WhenNotFound_ReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        _valueRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeneralValue?)null);

        // Act
        var result = await _service.SetDefaultAsync(id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrado");
    }

    #endregion

    #region ReorderAsync

    [Fact]
    public async Task ReorderAsync_UpdatesDisplayOrder()
    {
        // Arrange
        var tableId = Guid.NewGuid();
        var v1 = CreateGeneralValue(Guid.NewGuid(), "V1", "Valor 1", tableId);
        var v2 = CreateGeneralValue(Guid.NewGuid(), "V2", "Valor 2", tableId);
        var v3 = CreateGeneralValue(Guid.NewGuid(), "V3", "Valor 3", tableId);

        var values = new List<GeneralValue> { v1, v2, v3 };

        _valueRepoMock.Setup(r => r.GetByTableIdAsync(tableId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(values);

        // Reorder: v3, v1, v2
        var orderedIds = new List<Guid> { v3.Id, v1.Id, v2.Id };

        // Act
        var result = await _service.ReorderAsync(tableId, orderedIds);

        // Assert
        result.IsSuccess.Should().BeTrue();
        v3.DisplayOrder.Should().Be(0);
        v1.DisplayOrder.Should().Be(1);
        v2.DisplayOrder.Should().Be(2);
        _valueRepoMock.Verify(r => r.UpdateRangeAsync(values, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetLookupAsync

    [Fact]
    public async Task GetLookupAsync_ReturnsLookupItems()
    {
        // Arrange
        var tableId = Guid.NewGuid();
        var values = new List<GeneralValue>
        {
            CreateGeneralValue(Guid.NewGuid(), "V1", "Valor 1", tableId),
            CreateGeneralValue(Guid.NewGuid(), "V2", "Valor 2", tableId)
        };

        _valueRepoMock.Setup(r => r.GetByTableIdAsync(tableId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(values);

        // Act
        var result = await _service.GetLookupAsync(tableId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data![0].Name.Should().Be("Valor 1");
        result.Data![0].Code.Should().Be("V1");
    }

    #endregion

    #region GetLookupByTableCodeAsync

    [Fact]
    public async Task GetLookupByTableCodeAsync_WhenTableExists_ReturnsItems()
    {
        // Arrange
        var tableId = Guid.NewGuid();
        var table = CreateGeneralTable(tableId, "ESTADOS", "Estados");
        var values = new List<GeneralValue>
        {
            CreateGeneralValue(Guid.NewGuid(), "V1", "Valor 1", tableId)
        };

        _tableRepoMock.Setup(r => r.GetByCodeAsync("ESTADOS", false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(table);
        _valueRepoMock.Setup(r => r.GetByTableIdAsync(tableId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(values);

        // Act
        var result = await _service.GetLookupByTableCodeAsync("ESTADOS");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetLookupByTableCodeAsync_WhenTableNotFound_ReturnsFailure()
    {
        // Arrange
        _tableRepoMock.Setup(r => r.GetByCodeAsync("MISSING", false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeneralTable?)null);

        // Act
        var result = await _service.GetLookupByTableCodeAsync("MISSING");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("MISSING");
    }

    #endregion

    #region Helpers

    private static GeneralValue CreateGeneralValue(Guid id, string code, string content, Guid? tableId = null)
    {
        var tId = tableId ?? Guid.NewGuid();
        return new GeneralValue
        {
            Id = id,
            GeneralTableId = tId,
            Code = code,
            Content = content,
            IsActive = true,
            IsLocked = false,
            IsDefault = false,
            DisplayOrder = 0,
            TenantId = Guid.NewGuid(),
            GeneralTable = new GeneralTable
            {
                Id = tId,
                Code = "TABLE",
                Name = "Tabla",
                TenantId = Guid.NewGuid()
            }
        };
    }

    private static GeneralTable CreateGeneralTable(Guid id, string code, string name)
    {
        return new GeneralTable
        {
            Id = id,
            Code = code,
            Name = name,
            IsActive = true,
            TenantId = Guid.NewGuid(),
            Children = new List<GeneralTable>(),
            Values = new List<GeneralValue>()
        };
    }

    #endregion
}
