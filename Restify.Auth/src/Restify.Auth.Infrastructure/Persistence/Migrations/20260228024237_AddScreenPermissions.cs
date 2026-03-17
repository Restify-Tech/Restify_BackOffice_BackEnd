using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restify.Auth.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddScreenPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScreenPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScreenCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ScreenName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Module = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Route = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RequiredPermission = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreenPermissions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScreenPermissions_ScreenCode",
                table: "ScreenPermissions",
                column: "ScreenCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScreenPermissions");
        }
    }
}
