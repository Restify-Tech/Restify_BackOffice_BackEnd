using FluentAssertions;
using Moq;
using Restify.Core.Application.DTOs.Grid;
using Restify.Core.Application.Interfaces;
using Restify.Core.Domain.Entities;
using Restify.Core.Domain.Enums;
using Restify.Core.Infrastructure.Services;

namespace Restify.Core.Tests.Services;

public class GridConfigurationServiceTests
{
    private readonly Mock<IGridConfigurationRepository> _configRepoMock;
    private readonly Mock<IGridColumnRepository> _columnRepoMock;
    private readonly Mock<IGridColumnValidationRepository> _validationRepoMock;
    private readonly Mock<IGridColumnLookupRepository> _lookupRepoMock;
    private readonly GridConfigurationService _service;

    public GridConfigurationServiceTests()
    {
        _configRepoMock = new Mock<IGridConfigurationRepository>();
        _columnRepoMock = new Mock<IGridColumnRepository>();
        _validationRepoMock = new Mock<IGridColumnValidationRepository>();
        _lookupRepoMock = new Mock<IGridColumnLookupRepository>();

        _service = new GridConfigurationService(
            _configRepoMock.Object,
            _columnRepoMock.Object,
            _validationRepoMock.Object,
            _lookupRepoMock.Object);
    }

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_ReturnsAllGridConfigurations()
    {
        // Arrange
        var configs = new List<GridConfiguration>
        {
            CreateGridConfiguration("Category", "Categoria"),
            CreateGridConfiguration("Product", "Producto")
        };
        _configRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(configs);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data![0].EntityName.Should().Be("Category");
        result.Data[1].EntityName.Should().Be("Product");
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ReturnsEmptyList()
    {
        // Arrange
        _configRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GridConfiguration>());

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    #endregion

    #region GetByEntityNameAsync

    [Fact]
    public async Task GetByEntityNameAsync_WhenExists_ReturnsConfig()
    {
        // Arrange
        var config = CreateGridConfiguration("Category", "Categoria");
        _configRepoMock.Setup(r => r.GetByEntityNameAsync("Category", It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);

        // Act
        var result = await _service.GetByEntityNameAsync("Category");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.EntityName.Should().Be("Category");
        result.Data.DisplayName.Should().Be("Categoria");
    }

    [Fact]
    public async Task GetByEntityNameAsync_WhenNotFound_ReturnsFailure()
    {
        // Arrange
        _configRepoMock.Setup(r => r.GetByEntityNameAsync("NonExistent", It.IsAny<CancellationToken>()))
            .ReturnsAsync((GridConfiguration?)null);

        // Act
        var result = await _service.GetByEntityNameAsync("NonExistent");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("NonExistent");
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsConfig()
    {
        // Arrange
        var id = Guid.NewGuid();
        var config = CreateGridConfiguration("Category", "Categoria");
        config.Id = id;
        _configRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Id.Should().Be(id);
        result.Data.EntityName.Should().Be("Category");
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        _configRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GridConfiguration?)null);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateGridConfigurationRequest
        {
            EntityName = "Category",
            DisplayName = "Categoria",
            DisplayNamePlural = "Categorias",
            ApiEndpoint = "/api/categories",
            DefaultPageSize = 10
        };

        _configRepoMock.Setup(r => r.ExistsAsync("Category", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _configRepoMock.Setup(r => r.AddAsync(It.IsAny<GridConfiguration>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GridConfiguration e, CancellationToken _) => e);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.EntityName.Should().Be("Category");
        result.Data.DisplayName.Should().Be("Categoria");
        result.Data.ApiEndpoint.Should().Be("/api/categories");
        result.Data.AllowCreate.Should().BeTrue();
        result.Data.AllowEdit.Should().BeTrue();
        result.Data.AllowDelete.Should().BeTrue();
        _configRepoMock.Verify(r => r.AddAsync(It.IsAny<GridConfiguration>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateEntityName_ReturnsFailure()
    {
        // Arrange
        var request = new CreateGridConfigurationRequest
        {
            EntityName = "Category",
            DisplayName = "Categoria",
            DisplayNamePlural = "Categorias",
            ApiEndpoint = "/api/categories"
        };

        _configRepoMock.Setup(r => r.ExistsAsync("Category", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Category");
        _configRepoMock.Verify(r => r.AddAsync(It.IsAny<GridConfiguration>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_WhenExists_ReturnsSuccess()
    {
        // Arrange
        var existingId = Guid.NewGuid();
        var existing = CreateGridConfiguration("Category", "Categoria");
        existing.Id = existingId;

        var request = new UpdateGridConfigurationRequest
        {
            Id = existingId,
            EntityName = "Category",
            DisplayName = "Categoria Actualizada",
            DisplayNamePlural = "Categorias",
            ApiEndpoint = "/api/categories",
            DefaultPageSize = 25
        };

        _configRepoMock.Setup(r => r.GetByIdAsync(existingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _service.UpdateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.DisplayName.Should().Be("Categoria Actualizada");
        result.Data.DefaultPageSize.Should().Be(25);
        _configRepoMock.Verify(r => r.UpdateAsync(It.IsAny<GridConfiguration>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotFound_ReturnsFailure()
    {
        // Arrange
        var request = new UpdateGridConfigurationRequest { Id = Guid.NewGuid() };
        _configRepoMock.Setup(r => r.GetByIdAsync(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GridConfiguration?)null);

        // Act
        var result = await _service.UpdateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_WhenExists_ReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existing = CreateGridConfiguration("Category", "Categoria");
        existing.Id = id;

        _configRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
        _configRepoMock.Verify(r => r.DeleteAsync(existing, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotFound_ReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        _configRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GridConfiguration?)null);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    #endregion

    #region AddColumnAsync

    [Fact]
    public async Task AddColumnAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var configId = Guid.NewGuid();
        var config = CreateGridConfiguration("Category", "Categoria");
        config.Id = configId;

        var request = new CreateGridColumnRequest
        {
            GridConfigurationId = configId,
            FieldName = "name",
            HeaderText = "Nombre",
            ColumnType = GridColumnType.String,
            EditorType = EditorType.TextBox,
            GridOrder = 1,
            FormOrder = 1,
            IsRequired = true,
            IsVisibleInGrid = true,
            IsVisibleInCreate = true,
            IsVisibleInEdit = true
        };

        _configRepoMock.Setup(r => r.GetByIdAsync(configId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);
        _columnRepoMock.Setup(r => r.AddAsync(It.IsAny<GridColumn>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GridColumn e, CancellationToken _) => e);

        // Act
        var result = await _service.AddColumnAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.FieldName.Should().Be("name");
        result.Data.HeaderText.Should().Be("Nombre");
        result.Data.IsRequired.Should().BeTrue();
        _columnRepoMock.Verify(r => r.AddAsync(It.IsAny<GridColumn>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddColumnAsync_WhenConfigNotFound_ReturnsFailure()
    {
        // Arrange
        var configId = Guid.NewGuid();
        var request = new CreateGridColumnRequest
        {
            GridConfigurationId = configId,
            FieldName = "name",
            HeaderText = "Nombre"
        };

        _configRepoMock.Setup(r => r.GetByIdAsync(configId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GridConfiguration?)null);

        // Act
        var result = await _service.AddColumnAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
        _columnRepoMock.Verify(r => r.AddAsync(It.IsAny<GridColumn>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddColumnAsync_WithoutFormLabel_UsesHeaderText()
    {
        // Arrange
        var configId = Guid.NewGuid();
        var config = CreateGridConfiguration("Category", "Categoria");
        config.Id = configId;

        var request = new CreateGridColumnRequest
        {
            GridConfigurationId = configId,
            FieldName = "name",
            HeaderText = "Nombre",
            FormLabel = null
        };

        _configRepoMock.Setup(r => r.GetByIdAsync(configId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);
        _columnRepoMock.Setup(r => r.AddAsync(It.IsAny<GridColumn>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GridColumn e, CancellationToken _) => e);

        // Act
        var result = await _service.AddColumnAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.FormLabel.Should().Be("Nombre");
    }

    #endregion

    #region UpdateColumnAsync

    [Fact]
    public async Task UpdateColumnAsync_WhenExists_ReturnsSuccess()
    {
        // Arrange
        var columnId = Guid.NewGuid();
        var column = CreateGridColumn(columnId, "name", "Nombre");

        var request = new CreateGridColumnRequest
        {
            GridConfigurationId = Guid.NewGuid(),
            FieldName = "name",
            HeaderText = "Nombre Actualizado",
            ColumnType = GridColumnType.String,
            EditorType = EditorType.TextBox,
            GridOrder = 2,
            FormOrder = 2,
            IsRequired = false
        };

        _columnRepoMock.Setup(r => r.GetByIdAsync(columnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(column);

        // Act
        var result = await _service.UpdateColumnAsync(columnId, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.HeaderText.Should().Be("Nombre Actualizado");
        result.Data.GridOrder.Should().Be(2);
        _columnRepoMock.Verify(r => r.UpdateAsync(It.IsAny<GridColumn>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateColumnAsync_WhenNotFound_ReturnsFailure()
    {
        // Arrange
        var columnId = Guid.NewGuid();
        _columnRepoMock.Setup(r => r.GetByIdAsync(columnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GridColumn?)null);

        var request = new CreateGridColumnRequest { FieldName = "x", HeaderText = "X" };

        // Act
        var result = await _service.UpdateColumnAsync(columnId, request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    #endregion

    #region DeleteColumnAsync

    [Fact]
    public async Task DeleteColumnAsync_WhenExists_ReturnsSuccess()
    {
        // Arrange
        var columnId = Guid.NewGuid();
        var column = CreateGridColumn(columnId, "name", "Nombre");

        _columnRepoMock.Setup(r => r.GetByIdAsync(columnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(column);

        // Act
        var result = await _service.DeleteColumnAsync(columnId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _columnRepoMock.Verify(r => r.DeleteAsync(column, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteColumnAsync_WhenNotFound_ReturnsFailure()
    {
        // Arrange
        var columnId = Guid.NewGuid();
        _columnRepoMock.Setup(r => r.GetByIdAsync(columnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GridColumn?)null);

        // Act
        var result = await _service.DeleteColumnAsync(columnId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    #endregion

    #region AddValidationAsync

    [Fact]
    public async Task AddValidationAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var columnId = Guid.NewGuid();
        var column = CreateGridColumn(columnId, "email", "Email");

        var request = new CreateGridColumnValidationRequest
        {
            GridColumnId = columnId,
            ValidationType = "Required",
            ErrorMessage = "El email es requerido",
            Order = 0
        };

        _columnRepoMock.Setup(r => r.GetByIdAsync(columnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(column);
        _validationRepoMock.Setup(r => r.AddAsync(It.IsAny<GridColumnValidation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GridColumnValidation e, CancellationToken _) => e);

        // Act
        var result = await _service.AddValidationAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.ValidationType.Should().Be("required");
        result.Data.ErrorMessage.Should().Be("El email es requerido");
    }

    [Fact]
    public async Task AddValidationAsync_WhenColumnNotFound_ReturnsFailure()
    {
        // Arrange
        var columnId = Guid.NewGuid();
        var request = new CreateGridColumnValidationRequest
        {
            GridColumnId = columnId,
            ValidationType = "Required"
        };

        _columnRepoMock.Setup(r => r.GetByIdAsync(columnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GridColumn?)null);

        // Act
        var result = await _service.AddValidationAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    [Fact]
    public async Task AddValidationAsync_WithInvalidType_ReturnsFailure()
    {
        // Arrange
        var columnId = Guid.NewGuid();
        var column = CreateGridColumn(columnId, "name", "Nombre");

        var request = new CreateGridColumnValidationRequest
        {
            GridColumnId = columnId,
            ValidationType = "InvalidType"
        };

        _columnRepoMock.Setup(r => r.GetByIdAsync(columnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(column);

        // Act
        var result = await _service.AddValidationAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no válido");
    }

    [Theory]
    [InlineData("Required")]
    [InlineData("MinLength")]
    [InlineData("MaxLength")]
    [InlineData("Email")]
    [InlineData("Regex")]
    public async Task AddValidationAsync_WithValidTypes_ReturnsSuccess(string validationType)
    {
        // Arrange
        var columnId = Guid.NewGuid();
        var column = CreateGridColumn(columnId, "field", "Field");

        var request = new CreateGridColumnValidationRequest
        {
            GridColumnId = columnId,
            ValidationType = validationType,
            ValidationValue = "5",
            Order = 0
        };

        _columnRepoMock.Setup(r => r.GetByIdAsync(columnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(column);
        _validationRepoMock.Setup(r => r.AddAsync(It.IsAny<GridColumnValidation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GridColumnValidation e, CancellationToken _) => e);

        // Act
        var result = await _service.AddValidationAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    #endregion

    #region DeleteValidationAsync

    [Fact]
    public async Task DeleteValidationAsync_WhenExists_ReturnsSuccess()
    {
        // Arrange
        var validationId = Guid.NewGuid();
        var validation = new GridColumnValidation
        {
            Id = validationId,
            GridColumnId = Guid.NewGuid(),
            ValidationType = ValidationType.Required
        };

        _validationRepoMock.Setup(r => r.GetByIdAsync(validationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validation);

        // Act
        var result = await _service.DeleteValidationAsync(validationId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _validationRepoMock.Verify(r => r.DeleteAsync(validation, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteValidationAsync_WhenNotFound_ReturnsFailure()
    {
        // Arrange
        var validationId = Guid.NewGuid();
        _validationRepoMock.Setup(r => r.GetByIdAsync(validationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GridColumnValidation?)null);

        // Act
        var result = await _service.DeleteValidationAsync(validationId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    #endregion

    #region SetLookupAsync

    [Fact]
    public async Task SetLookupAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var columnId = Guid.NewGuid();
        var column = CreateGridColumn(columnId, "categoryId", "Categoria");

        var request = new CreateGridColumnLookupRequest
        {
            GridColumnId = columnId,
            TargetEntity = "Category",
            ApiEndpoint = "/api/categories",
            ValueField = "id",
            DisplayField = "name",
            AllowSearch = true,
            PreloadData = false
        };

        _columnRepoMock.Setup(r => r.GetByIdAsync(columnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(column);
        _lookupRepoMock.Setup(r => r.AddAsync(It.IsAny<GridColumnLookup>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GridColumnLookup e, CancellationToken _) => e);

        // Act
        var result = await _service.SetLookupAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.TargetEntity.Should().Be("Category");
        result.Data.ApiEndpoint.Should().Be("/api/categories");
        result.Data.ValueField.Should().Be("id");
        result.Data.DisplayField.Should().Be("name");
        _lookupRepoMock.Verify(r => r.DeleteByColumnIdAsync(columnId, It.IsAny<CancellationToken>()), Times.Once);
        _lookupRepoMock.Verify(r => r.AddAsync(It.IsAny<GridColumnLookup>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SetLookupAsync_WhenColumnNotFound_ReturnsFailure()
    {
        // Arrange
        var columnId = Guid.NewGuid();
        var request = new CreateGridColumnLookupRequest
        {
            GridColumnId = columnId,
            TargetEntity = "Category",
            ApiEndpoint = "/api/categories"
        };

        _columnRepoMock.Setup(r => r.GetByIdAsync(columnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GridColumn?)null);

        // Act
        var result = await _service.SetLookupAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no encontrada");
    }

    #endregion

    #region RemoveLookupAsync

    [Fact]
    public async Task RemoveLookupAsync_ReturnsSuccess()
    {
        // Arrange
        var columnId = Guid.NewGuid();

        // Act
        var result = await _service.RemoveLookupAsync(columnId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _lookupRepoMock.Verify(r => r.DeleteByColumnIdAsync(columnId, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Helpers

    private static GridConfiguration CreateGridConfiguration(string entityName, string displayName)
    {
        return new GridConfiguration
        {
            Id = Guid.NewGuid(),
            EntityName = entityName,
            DisplayName = displayName,
            DisplayNamePlural = displayName + "s",
            ApiEndpoint = $"/api/{entityName.ToLower()}s",
            DefaultPageSize = 10,
            TenantId = Guid.NewGuid(),
            Columns = new List<GridColumn>()
        };
    }

    private static GridColumn CreateGridColumn(Guid id, string fieldName, string headerText)
    {
        return new GridColumn
        {
            Id = id,
            GridConfigurationId = Guid.NewGuid(),
            FieldName = fieldName,
            HeaderText = headerText,
            FormLabel = headerText,
            ColumnType = GridColumnType.String,
            EditorType = EditorType.TextBox,
            GridOrder = 0,
            FormOrder = 0,
            Validations = new List<GridColumnValidation>()
        };
    }

    #endregion
}
