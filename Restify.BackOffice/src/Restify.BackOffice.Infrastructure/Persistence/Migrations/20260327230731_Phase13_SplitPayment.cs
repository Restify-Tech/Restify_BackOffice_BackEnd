using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restify.BackOffice.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Phase13_SplitPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "split_payments",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    SplitCount = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_split_payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_split_payments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "backoffice",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "split_payment_items",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SplitPaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    PaidBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_split_payment_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_split_payment_items_split_payments_SplitPaymentId",
                        column: x => x.SplitPaymentId,
                        principalSchema: "backoffice",
                        principalTable: "split_payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_split_payment_items_SplitPaymentId",
                schema: "backoffice",
                table: "split_payment_items",
                column: "SplitPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_split_payment_items_Status",
                schema: "backoffice",
                table: "split_payment_items",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_split_payments_OrderId",
                schema: "backoffice",
                table: "split_payments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_split_payments_TenantId",
                schema: "backoffice",
                table: "split_payments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_split_payments_TenantId_OrderId",
                schema: "backoffice",
                table: "split_payments",
                columns: new[] { "TenantId", "OrderId" });

            migrationBuilder.CreateIndex(
                name: "IX_split_payments_TenantId_Status",
                schema: "backoffice",
                table: "split_payments",
                columns: new[] { "TenantId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "split_payment_items",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "split_payments",
                schema: "backoffice");
        }
    }
}
