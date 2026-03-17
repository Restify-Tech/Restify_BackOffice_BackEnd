using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restify.BackOffice.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialBackOfficeSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "backoffice");

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ParentCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalSchema: "backoffice",
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId",
                schema: "backoffice",
                table: "Categories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_TenantId_Name",
                schema: "backoffice",
                table: "Categories",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_TenantId_ParentCategoryId_DisplayOrder",
                schema: "backoffice",
                table: "Categories",
                columns: new[] { "TenantId", "ParentCategoryId", "DisplayOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories",
                schema: "backoffice");
        }
    }
}
