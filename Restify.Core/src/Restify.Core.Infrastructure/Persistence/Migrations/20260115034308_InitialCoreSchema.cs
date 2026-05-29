using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restify.Core.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCoreSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "core");

            migrationBuilder.CreateTable(
                name: "GridConfigurations",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DisplayNamePlural = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ApiEndpoint = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DefaultPageSize = table.Column<int>(type: "integer", nullable: false),
                    PageSizeOptions = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DefaultSortColumn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DefaultSortDescending = table.Column<bool>(type: "boolean", nullable: false),
                    AllowCreate = table.Column<bool>(type: "boolean", nullable: false),
                    AllowEdit = table.Column<bool>(type: "boolean", nullable: false),
                    AllowDelete = table.Column<bool>(type: "boolean", nullable: false),
                    AllowView = table.Column<bool>(type: "boolean", nullable: false),
                    AllowExport = table.Column<bool>(type: "boolean", nullable: false),
                    ExportFormats = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AllowImport = table.Column<bool>(type: "boolean", nullable: false),
                    AllowSearch = table.Column<bool>(type: "boolean", nullable: false),
                    AllowAdvancedFilter = table.Column<bool>(type: "boolean", nullable: false),
                    AllowInlineEdit = table.Column<bool>(type: "boolean", nullable: false),
                    AllowMultiSelect = table.Column<bool>(type: "boolean", nullable: false),
                    AllowColumnReorder = table.Column<bool>(type: "boolean", nullable: false),
                    AllowColumnResize = table.Column<bool>(type: "boolean", nullable: false),
                    AllowColumnToggle = table.Column<bool>(type: "boolean", nullable: false),
                    FormMode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "modal"),
                    ShowRowActions = table.Column<bool>(type: "boolean", nullable: false),
                    RowActionsPosition = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "end"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MenuOrder = table.Column<int>(type: "integer", nullable: false),
                    MenuPath = table.Column<string>(type: "text", nullable: true),
                    RequiredPermission = table.Column<string>(type: "text", nullable: true),
                    ExtraConfig = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GridConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GridColumns",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GridConfigurationId = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    HeaderText = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FormLabel = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    HeaderTooltip = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ColumnType = table.Column<int>(type: "integer", nullable: false),
                    EditorType = table.Column<int>(type: "integer", nullable: false),
                    GridOrder = table.Column<int>(type: "integer", nullable: false),
                    FormOrder = table.Column<int>(type: "integer", nullable: false),
                    FormGroup = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Width = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MinWidth = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MaxWidth = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Alignment = table.Column<int>(type: "integer", nullable: false),
                    IsVisibleInGrid = table.Column<bool>(type: "boolean", nullable: false),
                    IsVisibleInCreate = table.Column<bool>(type: "boolean", nullable: false),
                    IsVisibleInEdit = table.Column<bool>(type: "boolean", nullable: false),
                    IsVisibleInView = table.Column<bool>(type: "boolean", nullable: false),
                    IsExportable = table.Column<bool>(type: "boolean", nullable: false),
                    IsSortable = table.Column<bool>(type: "boolean", nullable: false),
                    IsFilterable = table.Column<bool>(type: "boolean", nullable: false),
                    IsSearchable = table.Column<bool>(type: "boolean", nullable: false),
                    IsInlineEditable = table.Column<bool>(type: "boolean", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    IsPrimaryKey = table.Column<bool>(type: "boolean", nullable: false),
                    IsReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    IsFrozen = table.Column<bool>(type: "boolean", nullable: false),
                    FrozenPosition = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DisplayFormat = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DefaultValue = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Placeholder = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    HelpText = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Prefix = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Suffix = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    FormColumnSpan = table.Column<int>(type: "integer", nullable: false),
                    CssClass = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CellTemplate = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    EditorTemplate = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    SelectOptions = table.Column<string>(type: "jsonb", nullable: true),
                    VisibilityCondition = table.Column<string>(type: "jsonb", nullable: true),
                    EnabledCondition = table.Column<string>(type: "jsonb", nullable: true),
                    ExtraConfig = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GridColumns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GridColumns_GridConfigurations_GridConfigurationId",
                        column: x => x.GridConfigurationId,
                        principalSchema: "core",
                        principalTable: "GridConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GridColumnLookups",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GridColumnId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetEntity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ApiEndpoint = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ValueField = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "id"),
                    DisplayField = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "name"),
                    AdditionalDisplayFields = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DisplayFormat = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AllowSearch = table.Column<bool>(type: "boolean", nullable: false),
                    SearchField = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    MinSearchLength = table.Column<int>(type: "integer", nullable: false),
                    AllowAdd = table.Column<bool>(type: "boolean", nullable: false),
                    AllowClear = table.Column<bool>(type: "boolean", nullable: false),
                    StaticFilter = table.Column<string>(type: "jsonb", nullable: true),
                    OrderBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    MaxItems = table.Column<int>(type: "integer", nullable: true),
                    IsCached = table.Column<bool>(type: "boolean", nullable: false),
                    CacheDuration = table.Column<int>(type: "integer", nullable: false),
                    PreloadData = table.Column<bool>(type: "boolean", nullable: false),
                    IsDependentLookup = table.Column<bool>(type: "boolean", nullable: false),
                    ParentField = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ParentFilterField = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ClearOnParentChange = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GridColumnLookups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GridColumnLookups_GridColumns_GridColumnId",
                        column: x => x.GridColumnId,
                        principalSchema: "core",
                        principalTable: "GridColumns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GridColumnValidations",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GridColumnId = table.Column<Guid>(type: "uuid", nullable: false),
                    ValidationType = table.Column<int>(type: "integer", nullable: false),
                    ValidationValue = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ValidationValue2 = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ClientOnly = table.Column<bool>(type: "boolean", nullable: false),
                    ServerOnly = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GridColumnValidations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GridColumnValidations_GridColumns_GridColumnId",
                        column: x => x.GridColumnId,
                        principalSchema: "core",
                        principalTable: "GridColumns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GridColumnLookups_GridColumnId",
                schema: "core",
                table: "GridColumnLookups",
                column: "GridColumnId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GridColumns_GridConfigurationId_GridOrder",
                schema: "core",
                table: "GridColumns",
                columns: new[] { "GridConfigurationId", "GridOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_GridColumnValidations_GridColumnId_Order",
                schema: "core",
                table: "GridColumnValidations",
                columns: new[] { "GridColumnId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_GridConfigurations_TenantId_EntityName",
                schema: "core",
                table: "GridConfigurations",
                columns: new[] { "TenantId", "EntityName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GridColumnLookups",
                schema: "core");

            migrationBuilder.DropTable(
                name: "GridColumnValidations",
                schema: "core");

            migrationBuilder.DropTable(
                name: "GridColumns",
                schema: "core");

            migrationBuilder.DropTable(
                name: "GridConfigurations",
                schema: "core");
        }
    }
}
