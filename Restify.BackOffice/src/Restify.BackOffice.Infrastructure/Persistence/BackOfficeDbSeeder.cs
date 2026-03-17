using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restify.Core.Domain.Entities;
using Restify.Core.Domain.Enums;
using Restify.Core.Infrastructure.Persistence;

namespace Restify.BackOffice.Infrastructure.Persistence;

/// <summary>
/// Seeder para configuraciones iniciales del BackOffice
/// </summary>
public class BackOfficeDbSeeder
{
    private readonly BackOfficeDbContext _backOfficeContext;
    private readonly CoreDbContext _coreContext;
    private readonly ILogger<BackOfficeDbSeeder> _logger;
    // TenantId por defecto para datos de seed (mismo que en Auth)
    private static readonly Guid DefaultTenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public BackOfficeDbSeeder(
        BackOfficeDbContext backOfficeContext,
        CoreDbContext coreContext,
        ILogger<BackOfficeDbSeeder> logger)
    {
        _backOfficeContext = backOfficeContext;
        _coreContext = coreContext;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await SeedCategoryGridConfigurationAsync();
    }

    private async Task SeedCategoryGridConfigurationAsync()
    {
        // Verificar si ya existe la configuración de grid para Category
        var existingConfig = await _coreContext.GridConfigurations
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.EntityName == "Category");

        if (existingConfig != null)
        {
            _logger.LogInformation("Grid configuration for Category already exists");
            return;
        }

        _logger.LogInformation("Creating grid configuration for Category...");

        var gridConfig = new GridConfiguration
        {
            TenantId = DefaultTenantId,
            EntityName = "Category",
            DisplayName = "Categoría",
            DisplayNamePlural = "Categorías",
            Description = "Categorías de productos del menú",
            Icon = "mdi-folder",
            ApiEndpoint = "/api/categories",
            DefaultPageSize = 10,
            PageSizeOptions = "[10, 25, 50, 100]",
            DefaultSortColumn = "displayOrder",
            DefaultSortDescending = false,
            AllowCreate = true,
            AllowEdit = true,
            AllowDelete = true,
            AllowView = true,
            AllowExport = true,
            ExportFormats = "[\"excel\", \"csv\", \"pdf\"]",
            AllowImport = true,
            AllowSearch = true,
            AllowAdvancedFilter = true,
            AllowInlineEdit = true,
            AllowMultiSelect = true,
            AllowColumnReorder = true,
            AllowColumnResize = true,
            AllowColumnToggle = true,
            FormMode = "modal",
            ShowRowActions = true,
            RowActionsPosition = "end",
            IsActive = true,
            MenuOrder = 1,
            MenuPath = "/menu/categories",
            RequiredPermission = "menu.view"
        };

        // Columnas
        gridConfig.Columns = new List<GridColumn>
        {
            new GridColumn
            {
                FieldName = "id",
                HeaderText = "ID",
                ColumnType = GridColumnType.Guid,
                EditorType = EditorType.Hidden,
                GridOrder = 0,
                FormOrder = 0,
                IsVisibleInGrid = false,
                IsVisibleInCreate = false,
                IsVisibleInEdit = false,
                IsVisibleInView = true,
                IsPrimaryKey = true,
                IsReadOnly = true
            },
            new GridColumn
            {
                FieldName = "name",
                HeaderText = "Nombre",
                FormLabel = "Nombre de la categoría",
                ColumnType = GridColumnType.String,
                EditorType = EditorType.TextBox,
                GridOrder = 1,
                FormOrder = 1,
                Width = "200px",
                IsVisibleInGrid = true,
                IsVisibleInCreate = true,
                IsVisibleInEdit = true,
                IsVisibleInView = true,
                IsSortable = true,
                IsFilterable = true,
                IsSearchable = true,
                IsRequired = true,
                Placeholder = "Ej: Bebidas, Entradas...",
                Validations = new List<GridColumnValidation>
                {
                    new GridColumnValidation
                    {
                        ValidationType = ValidationType.Required,
                        ErrorMessage = "El nombre es requerido",
                        Order = 1
                    },
                    new GridColumnValidation
                    {
                        ValidationType = ValidationType.MaxLength,
                        ValidationValue = "100",
                        ErrorMessage = "El nombre no puede exceder 100 caracteres",
                        Order = 2
                    }
                }
            },
            new GridColumn
            {
                FieldName = "description",
                HeaderText = "Descripción",
                ColumnType = GridColumnType.String,
                EditorType = EditorType.TextArea,
                GridOrder = 2,
                FormOrder = 2,
                Width = "300px",
                IsVisibleInGrid = true,
                IsVisibleInCreate = true,
                IsVisibleInEdit = true,
                IsVisibleInView = true,
                IsSortable = false,
                IsFilterable = false,
                IsSearchable = true,
                Placeholder = "Descripción opcional..."
            },
            new GridColumn
            {
                FieldName = "icon",
                HeaderText = "Icono",
                ColumnType = GridColumnType.String,
                EditorType = EditorType.IconPicker,
                GridOrder = 3,
                FormOrder = 3,
                Width = "100px",
                Alignment = GridColumnAlignment.Center,
                IsVisibleInGrid = true,
                IsVisibleInCreate = true,
                IsVisibleInEdit = true,
                IsVisibleInView = true,
                CellTemplate = "<i class=\"{{value}}\"></i>"
            },
            new GridColumn
            {
                FieldName = "displayOrder",
                HeaderText = "Orden",
                ColumnType = GridColumnType.Integer,
                EditorType = EditorType.NumberBox,
                GridOrder = 4,
                FormOrder = 4,
                Width = "80px",
                Alignment = GridColumnAlignment.Center,
                IsVisibleInGrid = true,
                IsVisibleInCreate = true,
                IsVisibleInEdit = true,
                IsVisibleInView = true,
                IsSortable = true,
                DefaultValue = "0",
                HelpText = "Orden de visualización en el menú"
            },
            new GridColumn
            {
                FieldName = "parentCategoryId",
                HeaderText = "Categoría Padre",
                FormLabel = "Categoría Padre",
                ColumnType = GridColumnType.Lookup,
                EditorType = EditorType.AutoComplete,
                GridOrder = 5,
                FormOrder = 5,
                Width = "180px",
                IsVisibleInGrid = true,
                IsVisibleInCreate = true,
                IsVisibleInEdit = true,
                IsVisibleInView = true,
                IsFilterable = true,
                Lookup = new GridColumnLookup
                {
                    TargetEntity = "Category",
                    ApiEndpoint = "/api/categories/lookup",
                    ValueField = "id",
                    DisplayField = "name",
                    AllowSearch = true,
                    AllowClear = true,
                    PreloadData = true
                }
            },
            new GridColumn
            {
                FieldName = "isActive",
                HeaderText = "Activo",
                ColumnType = GridColumnType.Boolean,
                EditorType = EditorType.Switch,
                GridOrder = 6,
                FormOrder = 6,
                Width = "80px",
                Alignment = GridColumnAlignment.Center,
                IsVisibleInGrid = true,
                IsVisibleInCreate = true,
                IsVisibleInEdit = true,
                IsVisibleInView = true,
                IsSortable = true,
                IsFilterable = true,
                IsInlineEditable = true,
                DefaultValue = "true"
            }
        };

        _coreContext.GridConfigurations.Add(gridConfig);
        await _coreContext.SaveChangesAsync();

        _logger.LogInformation("Grid configuration for Category created successfully");
    }
}
