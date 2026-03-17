using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restify.Auth.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIsTenantDefaultToPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeliveryOperationMode",
                table: "Tenants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "DeliveryZoneId",
                table: "Tenants",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullAddress",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentificationNumber",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdentificationType",
                table: "Tenants",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Tenants",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Tenants",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OnboardingCompleted",
                table: "Tenants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SignatureUrl",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTenantDefault",
                table: "Permissions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "DeliveryZones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Region = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: false),
                    DefaultCommissionPercentage = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxDeliveryRadiusKm = table.Column<double>(type: "double precision", nullable: false),
                    MinDriverRating = table.Column<decimal>(type: "numeric", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryZones", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_DeliveryZoneId",
                table: "Tenants",
                column: "DeliveryZoneId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_DeliveryZones_DeliveryZoneId",
                table: "Tenants",
                column: "DeliveryZoneId",
                principalTable: "DeliveryZones",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_DeliveryZones_DeliveryZoneId",
                table: "Tenants");

            migrationBuilder.DropTable(
                name: "DeliveryZones");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_DeliveryZoneId",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "DeliveryOperationMode",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "DeliveryZoneId",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "FullAddress",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "IdentificationNumber",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "IdentificationType",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "OnboardingCompleted",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "SignatureUrl",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "IsTenantDefault",
                table: "Permissions");
        }
    }
}
