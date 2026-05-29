using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restify.Core.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGeneralTablesAndValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeneralTables",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApplicationCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsInternal = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    Icon = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ExtraConfig = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralTables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneralTables_GeneralTables_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "core",
                        principalTable: "GeneralTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GeneralValues",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GeneralTableId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ShortDescription = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    NumericValue = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    Reference1 = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Reference2 = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Reference3 = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Reference4 = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Reference5 = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Icon = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BackgroundColor = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    TextColor = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsLocked = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    ExtraConfig = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneralValues_GeneralTables_GeneralTableId",
                        column: x => x.GeneralTableId,
                        principalSchema: "core",
                        principalTable: "GeneralTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GeneralTables_ParentId",
                schema: "core",
                table: "GeneralTables",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralTables_TenantId_Code",
                schema: "core",
                table: "GeneralTables",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GeneralValues_GeneralTableId",
                schema: "core",
                table: "GeneralValues",
                column: "GeneralTableId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralValues_TenantId_GeneralTableId_Code",
                schema: "core",
                table: "GeneralValues",
                columns: new[] { "TenantId", "GeneralTableId", "Code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneralValues",
                schema: "core");

            migrationBuilder.DropTable(
                name: "GeneralTables",
                schema: "core");
        }
    }
}
