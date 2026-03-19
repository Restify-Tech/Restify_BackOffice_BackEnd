using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restify.Auth.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantBrandingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccentColor",
                table: "Tenants",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "#F59E0B");

            migrationBuilder.AddColumn<string>(
                name: "CoverImageUrl",
                table: "Tenants",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomCss",
                table: "Tenants",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FaviconUrl",
                table: "Tenants",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FontBody",
                table: "Tenants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FontHeading",
                table: "Tenants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemplateName",
                table: "Tenants",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "elegante");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccentColor",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "CoverImageUrl",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "CustomCss",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "FaviconUrl",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "FontBody",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "FontHeading",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "TemplateName",
                table: "Tenants");
        }
    }
}
