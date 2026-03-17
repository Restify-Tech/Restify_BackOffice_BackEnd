using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restify.BackOffice.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGlobalModifiers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GlobalModifiers",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DefaultPriceAdjustment = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalModifiers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GlobalModifierProducts",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GlobalModifierId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomPriceAdjustment = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalModifierProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GlobalModifierProducts_GlobalModifiers_GlobalModifierId",
                        column: x => x.GlobalModifierId,
                        principalSchema: "backoffice",
                        principalTable: "GlobalModifiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GlobalModifierProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "backoffice",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GlobalModifierProducts_GlobalModifierId",
                schema: "backoffice",
                table: "GlobalModifierProducts",
                column: "GlobalModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalModifierProducts_GlobalModifierId_ProductId",
                schema: "backoffice",
                table: "GlobalModifierProducts",
                columns: new[] { "GlobalModifierId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlobalModifierProducts_ProductId",
                schema: "backoffice",
                table: "GlobalModifierProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalModifierProducts_TenantId",
                schema: "backoffice",
                table: "GlobalModifierProducts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalModifiers_TenantId",
                schema: "backoffice",
                table: "GlobalModifiers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalModifiers_TenantId_IsActive",
                schema: "backoffice",
                table: "GlobalModifiers",
                columns: new[] { "TenantId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_GlobalModifiers_TenantId_Type",
                schema: "backoffice",
                table: "GlobalModifiers",
                columns: new[] { "TenantId", "Type" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GlobalModifierProducts",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "GlobalModifiers",
                schema: "backoffice");
        }
    }
}
