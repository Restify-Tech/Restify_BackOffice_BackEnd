using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restify.BackOffice.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddElectronicInvoicing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerIdType",
                schema: "backoffice",
                table: "Invoices",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CreditNotes",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditNoteNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    CustomerIdNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CustomerIdType = table.Column<int>(type: "integer", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxRate = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false),
                    Tax = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditNotes_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalSchema: "backoffice",
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FiscalConfigurations",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Environment = table.Column<int>(type: "integer", nullable: false),
                    Ruc = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    BusinessName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    TradeName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    MainAddress = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    EstablishmentAddress = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    ObligadoContabilidad = table.Column<bool>(type: "boolean", nullable: false),
                    ContribuyenteEspecial = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Establishment = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    EmissionPoint = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    CertificateData = table.Column<byte[]>(type: "bytea", nullable: true),
                    CertificatePassword = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CertificateExpiration = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NextInvoiceSequential = table.Column<int>(type: "integer", nullable: false),
                    NextCreditNoteSequential = table.Column<int>(type: "integer", nullable: false),
                    NextWithholdingSequential = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    AutoSendOnPayment = table.Column<bool>(type: "boolean", nullable: false),
                    AutoSendEmailOnAuth = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiscalConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WithholdingVouchers",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VoucherNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PurchaseOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplierName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    SupplierRuc = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    SupplierIdType = table.Column<int>(type: "integer", nullable: false),
                    SupportDocType = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    SupportDocNumber = table.Column<string>(type: "character varying(49)", maxLength: 49, nullable: false),
                    SupportDocDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalWithheld = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WithholdingVouchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WithholdingVouchers_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalSchema: "backoffice",
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CreditNoteItems",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditNoteId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxRate = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditNoteItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditNoteItems_CreditNotes_CreditNoteId",
                        column: x => x.CreditNoteId,
                        principalSchema: "backoffice",
                        principalTable: "CreditNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElectronicDocuments",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreditNoteId = table.Column<Guid>(type: "uuid", nullable: true),
                    WithholdingVoucherId = table.Column<Guid>(type: "uuid", nullable: true),
                    DocumentType = table.Column<int>(type: "integer", nullable: false),
                    AccessKey = table.Column<string>(type: "character varying(49)", maxLength: 49, nullable: false),
                    Establishment = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    EmissionPoint = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Sequential = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Environment = table.Column<int>(type: "integer", nullable: false),
                    XmlContent = table.Column<string>(type: "text", nullable: true),
                    SignedXmlContent = table.Column<string>(type: "text", nullable: true),
                    AuthorizationCode = table.Column<string>(type: "character varying(49)", maxLength: 49, nullable: true),
                    AuthorizationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SriResponse = table.Column<string>(type: "jsonb", nullable: true),
                    SriErrors = table.Column<string>(type: "jsonb", nullable: true),
                    SendAttempts = table.Column<int>(type: "integer", nullable: false),
                    LastSendAttempt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RidePdfUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    EmailSent = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectronicDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElectronicDocuments_CreditNotes_CreditNoteId",
                        column: x => x.CreditNoteId,
                        principalSchema: "backoffice",
                        principalTable: "CreditNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ElectronicDocuments_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalSchema: "backoffice",
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ElectronicDocuments_WithholdingVouchers_WithholdingVoucherId",
                        column: x => x.WithholdingVoucherId,
                        principalSchema: "backoffice",
                        principalTable: "WithholdingVouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "WithholdingDetails",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WithholdingVoucherId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaxCode = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    RetentionCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    TaxBase = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    RetentionPercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    RetentionAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WithholdingDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WithholdingDetails_WithholdingVouchers_WithholdingVoucherId",
                        column: x => x.WithholdingVoucherId,
                        principalSchema: "backoffice",
                        principalTable: "WithholdingVouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreditNoteItems_CreditNoteId",
                schema: "backoffice",
                table: "CreditNoteItems",
                column: "CreditNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNoteItems_TenantId",
                schema: "backoffice",
                table: "CreditNoteItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_InvoiceId",
                schema: "backoffice",
                table: "CreditNotes",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_TenantId",
                schema: "backoffice",
                table: "CreditNotes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_TenantId_CreditNoteNumber",
                schema: "backoffice",
                table: "CreditNotes",
                columns: new[] { "TenantId", "CreditNoteNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ElectronicDocuments_AccessKey",
                schema: "backoffice",
                table: "ElectronicDocuments",
                column: "AccessKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ElectronicDocuments_CreditNoteId",
                schema: "backoffice",
                table: "ElectronicDocuments",
                column: "CreditNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectronicDocuments_InvoiceId",
                schema: "backoffice",
                table: "ElectronicDocuments",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectronicDocuments_TenantId_DocumentType_Sequential",
                schema: "backoffice",
                table: "ElectronicDocuments",
                columns: new[] { "TenantId", "DocumentType", "Sequential" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ElectronicDocuments_TenantId_Status",
                schema: "backoffice",
                table: "ElectronicDocuments",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ElectronicDocuments_WithholdingVoucherId",
                schema: "backoffice",
                table: "ElectronicDocuments",
                column: "WithholdingVoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_FiscalConfigurations_TenantId",
                schema: "backoffice",
                table: "FiscalConfigurations",
                column: "TenantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WithholdingDetails_TenantId",
                schema: "backoffice",
                table: "WithholdingDetails",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_WithholdingDetails_WithholdingVoucherId",
                schema: "backoffice",
                table: "WithholdingDetails",
                column: "WithholdingVoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_WithholdingVouchers_PurchaseOrderId",
                schema: "backoffice",
                table: "WithholdingVouchers",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_WithholdingVouchers_TenantId",
                schema: "backoffice",
                table: "WithholdingVouchers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_WithholdingVouchers_TenantId_VoucherNumber",
                schema: "backoffice",
                table: "WithholdingVouchers",
                columns: new[] { "TenantId", "VoucherNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditNoteItems",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "ElectronicDocuments",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "FiscalConfigurations",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "WithholdingDetails",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "CreditNotes",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "WithholdingVouchers",
                schema: "backoffice");

            migrationBuilder.DropColumn(
                name: "CustomerIdType",
                schema: "backoffice",
                table: "Invoices");
        }
    }
}
