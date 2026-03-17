using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restify.BackOffice.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryItems",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentStock = table.Column<decimal>(type: "numeric(18,3)", nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MinimumStock = table.Column<decimal>(type: "numeric(18,3)", nullable: false),
                    MaximumStock = table.Column<decimal>(type: "numeric(18,3)", nullable: true),
                    AverageCost = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    LastPurchaseCost = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    LastPurchaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastSaleDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StorageLocation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TrackStock = table.Column<bool>(type: "boolean", nullable: false),
                    CostMethod = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "backoffice",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TaxId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ContactName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryMovements",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InventoryItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,3)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    PreviousStock = table.Column<decimal>(type: "numeric(18,3)", nullable: false),
                    NewStock = table.Column<decimal>(type: "numeric(18,3)", nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReferenceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    MovementDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RegisteredBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryMovements_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalSchema: "backoffice",
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SupplierId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpectedDeliveryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActualDeliveryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Tax = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    OrderedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReceivedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SupplierInvoiceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalSchema: "backoffice",
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderItems",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PurchaseOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuantityOrdered = table.Column<decimal>(type: "numeric(18,3)", nullable: false),
                    QuantityReceived = table.Column<decimal>(type: "numeric(18,3)", nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UnitCost = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "backoffice",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItems_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalSchema: "backoffice",
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_ProductId",
                schema: "backoffice",
                table: "InventoryItems",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_TenantId",
                schema: "backoffice",
                table: "InventoryItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_InventoryItemId",
                schema: "backoffice",
                table: "InventoryMovements",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_MovementDate",
                schema: "backoffice",
                table: "InventoryMovements",
                column: "MovementDate");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_TenantId",
                schema: "backoffice",
                table: "InventoryMovements",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_Type",
                schema: "backoffice",
                table: "InventoryMovements",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_ProductId",
                schema: "backoffice",
                table: "PurchaseOrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_PurchaseOrderId",
                schema: "backoffice",
                table: "PurchaseOrderItems",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_TenantId",
                schema: "backoffice",
                table: "PurchaseOrderItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_OrderDate",
                schema: "backoffice",
                table: "PurchaseOrders",
                column: "OrderDate");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_OrderNumber",
                schema: "backoffice",
                table: "PurchaseOrders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_Status",
                schema: "backoffice",
                table: "PurchaseOrders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_SupplierId",
                schema: "backoffice",
                table: "PurchaseOrders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_TenantId",
                schema: "backoffice",
                table: "PurchaseOrders",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_Name",
                schema: "backoffice",
                table: "Suppliers",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_TenantId",
                schema: "backoffice",
                table: "Suppliers",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryMovements",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "PurchaseOrderItems",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "InventoryItems",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "PurchaseOrders",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "Suppliers",
                schema: "backoffice");
        }
    }
}
