using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restify.BackOffice.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MinCapacity",
                schema: "backoffice",
                table: "Tables",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GuestCount",
                schema: "backoffice",
                table: "Orders",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TableAssignedAt",
                schema: "backoffice",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TableAssignedByUserId",
                schema: "backoffice",
                table: "Orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TableNotes",
                schema: "backoffice",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CurrentLatitude",
                schema: "backoffice",
                table: "DeliveryDrivers",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CurrentLongitude",
                schema: "backoffice",
                table: "DeliveryDrivers",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeliveryZoneId",
                schema: "backoffice",
                table: "DeliveryDrivers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPoolDriver",
                schema: "backoffice",
                table: "DeliveryDrivers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLocationUpdateAt",
                schema: "backoffice",
                table: "DeliveryDrivers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                schema: "backoffice",
                table: "DeliveryDrivers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleBrand",
                schema: "backoffice",
                table: "DeliveryDrivers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleColor",
                schema: "backoffice",
                table: "DeliveryDrivers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleModel",
                schema: "backoffice",
                table: "DeliveryDrivers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VehicleType",
                schema: "backoffice",
                table: "DeliveryDrivers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VehicleYear",
                schema: "backoffice",
                table: "DeliveryDrivers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VerificationStatus",
                schema: "backoffice",
                table: "DeliveryDrivers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                schema: "backoffice",
                table: "DeliveryDrivers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VerifiedByUserId",
                schema: "backoffice",
                table: "DeliveryDrivers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeliveryZoneId",
                schema: "backoffice",
                table: "Deliveries",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPoolDelivery",
                schema: "backoffice",
                table: "Deliveries",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "PoolCommissionAmount",
                schema: "backoffice",
                table: "Deliveries",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PoolCommissionPercentage",
                schema: "backoffice",
                table: "Deliveries",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RestaurantLatitude",
                schema: "backoffice",
                table: "Deliveries",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RestaurantLongitude",
                schema: "backoffice",
                table: "Deliveries",
                type: "double precision",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AccountingAccounts",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AccountType = table.Column<int>(type: "integer", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    AcceptsEntries = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingAccounts_AccountingAccounts_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "backoffice",
                        principalTable: "AccountingAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountingPeriods",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ClosedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AIImagePromptTemplates",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PromptTemplate = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Style = table.Column<int>(type: "integer", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIImagePromptTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeductionTypes",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CalculationType = table.Column<int>(type: "integer", nullable: false),
                    DefaultValue = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    AppliesTo = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeductionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DriverDocuments",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DriverId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentType = table.Column<int>(type: "integer", nullable: false),
                    FileUrl = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    RejectionReason = table.Column<string>(type: "text", nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriverDocuments_DeliveryDrivers_DriverId",
                        column: x => x.DriverId,
                        principalSchema: "backoffice",
                        principalTable: "DeliveryDrivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IdentificationNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Position = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Department = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    HireDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TerminationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BaseSalary = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    EmploymentType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    BankName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    BankAccountNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SocialSecurityNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayrollPeriods",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    PeriodType = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TotalGross = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalNet = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ApprovedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaidBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PoolDeliveryCommissions",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeliveryId = table.Column<Guid>(type: "uuid", nullable: false),
                    DriverId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    DeliveryFee = table.Column<decimal>(type: "numeric", nullable: false),
                    CommissionPercentage = table.Column<decimal>(type: "numeric", nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoolDeliveryCommissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PoolDeliveryCommissions_Deliveries_DeliveryId",
                        column: x => x.DeliveryId,
                        principalSchema: "backoffice",
                        principalTable: "Deliveries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PoolDeliveryCommissions_DeliveryDrivers_DriverId",
                        column: x => x.DriverId,
                        principalSchema: "backoffice",
                        principalTable: "DeliveryDrivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JournalEntries",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntryNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PeriodId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    EntryType = table.Column<int>(type: "integer", nullable: false),
                    SourceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PostedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PostedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReversedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ReversedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalEntries_AccountingPeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalSchema: "backoffice",
                        principalTable: "AccountingPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AIImageGenerations",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    PromptTemplateId = table.Column<Guid>(type: "uuid", nullable: true),
                    FinalPrompt = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    GeneratedImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ProviderUsed = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CostUsd = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIImageGenerations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIImageGenerations_AIImagePromptTemplates_PromptTemplateId",
                        column: x => x.PromptTemplateId,
                        principalSchema: "backoffice",
                        principalTable: "AIImagePromptTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AIImageGenerations_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "backoffice",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PayrollEntries",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PayrollPeriodId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    BaseSalary = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    WorkedDays = table.Column<int>(type: "integer", nullable: false),
                    OvertimeHours = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    GrossPay = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    SocialSecurityEmployee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    SocialSecurityEmployer = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IncomeTax = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    OtherDeductions = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    OtherBenefits = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    NetPay = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollEntries_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "backoffice",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PayrollEntries_PayrollPeriods_PayrollPeriodId",
                        column: x => x.PayrollPeriodId,
                        principalSchema: "backoffice",
                        principalTable: "PayrollPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JournalEntryLines",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    JournalEntryId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Debit = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Credit = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntryLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalEntryLines_AccountingAccounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "backoffice",
                        principalTable: "AccountingAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JournalEntryLines_JournalEntries_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalSchema: "backoffice",
                        principalTable: "JournalEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingAccounts_ParentId",
                schema: "backoffice",
                table: "AccountingAccounts",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingAccounts_TenantId",
                schema: "backoffice",
                table: "AccountingAccounts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingAccounts_TenantId_Code",
                schema: "backoffice",
                table: "AccountingAccounts",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingPeriods_TenantId",
                schema: "backoffice",
                table: "AccountingPeriods",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingPeriods_TenantId_Year_Month",
                schema: "backoffice",
                table: "AccountingPeriods",
                columns: new[] { "TenantId", "Year", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AIImageGenerations_ProductId",
                schema: "backoffice",
                table: "AIImageGenerations",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AIImageGenerations_PromptTemplateId",
                schema: "backoffice",
                table: "AIImageGenerations",
                column: "PromptTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_AIImageGenerations_TenantId",
                schema: "backoffice",
                table: "AIImageGenerations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AIImageGenerations_TenantId_Status",
                schema: "backoffice",
                table: "AIImageGenerations",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AIImagePromptTemplates_TenantId_Name",
                schema: "backoffice",
                table: "AIImagePromptTemplates",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeductionTypes_TenantId",
                schema: "backoffice",
                table: "DeductionTypes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DeductionTypes_TenantId_Name",
                schema: "backoffice",
                table: "DeductionTypes",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DriverDocuments_DriverId",
                schema: "backoffice",
                table: "DriverDocuments",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_TenantId",
                schema: "backoffice",
                table: "Employees",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_TenantId_Email",
                schema: "backoffice",
                table: "Employees",
                columns: new[] { "TenantId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_TenantId_IdentificationNumber",
                schema: "backoffice",
                table: "Employees",
                columns: new[] { "TenantId", "IdentificationNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_PeriodId",
                schema: "backoffice",
                table: "JournalEntries",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_TenantId",
                schema: "backoffice",
                table: "JournalEntries",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_TenantId_EntryNumber",
                schema: "backoffice",
                table: "JournalEntries",
                columns: new[] { "TenantId", "EntryNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_TenantId_Status",
                schema: "backoffice",
                table: "JournalEntries",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_AccountId",
                schema: "backoffice",
                table: "JournalEntryLines",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_JournalEntryId",
                schema: "backoffice",
                table: "JournalEntryLines",
                column: "JournalEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollEntries_EmployeeId",
                schema: "backoffice",
                table: "PayrollEntries",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollEntries_PayrollPeriodId",
                schema: "backoffice",
                table: "PayrollEntries",
                column: "PayrollPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollEntries_PayrollPeriodId_EmployeeId",
                schema: "backoffice",
                table: "PayrollEntries",
                columns: new[] { "PayrollPeriodId", "EmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PayrollPeriods_TenantId",
                schema: "backoffice",
                table: "PayrollPeriods",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollPeriods_TenantId_Year_Month_PeriodType",
                schema: "backoffice",
                table: "PayrollPeriods",
                columns: new[] { "TenantId", "Year", "Month", "PeriodType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PoolDeliveryCommissions_DeliveryId",
                schema: "backoffice",
                table: "PoolDeliveryCommissions",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_PoolDeliveryCommissions_DriverId",
                schema: "backoffice",
                table: "PoolDeliveryCommissions",
                column: "DriverId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIImageGenerations",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "DeductionTypes",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "DriverDocuments",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "JournalEntryLines",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "PayrollEntries",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "PoolDeliveryCommissions",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "AIImagePromptTemplates",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "AccountingAccounts",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "JournalEntries",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "Employees",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "PayrollPeriods",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "AccountingPeriods",
                schema: "backoffice");

            migrationBuilder.DropColumn(
                name: "MinCapacity",
                schema: "backoffice",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "GuestCount",
                schema: "backoffice",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TableAssignedAt",
                schema: "backoffice",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TableAssignedByUserId",
                schema: "backoffice",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TableNotes",
                schema: "backoffice",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CurrentLatitude",
                schema: "backoffice",
                table: "DeliveryDrivers");

            migrationBuilder.DropColumn(
                name: "CurrentLongitude",
                schema: "backoffice",
                table: "DeliveryDrivers");

            migrationBuilder.DropColumn(
                name: "DeliveryZoneId",
                schema: "backoffice",
                table: "DeliveryDrivers");

            migrationBuilder.DropColumn(
                name: "IsPoolDriver",
                schema: "backoffice",
                table: "DeliveryDrivers");

            migrationBuilder.DropColumn(
                name: "LastLocationUpdateAt",
                schema: "backoffice",
                table: "DeliveryDrivers");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                schema: "backoffice",
                table: "DeliveryDrivers");

            migrationBuilder.DropColumn(
                name: "VehicleBrand",
                schema: "backoffice",
                table: "DeliveryDrivers");

            migrationBuilder.DropColumn(
                name: "VehicleColor",
                schema: "backoffice",
                table: "DeliveryDrivers");

            migrationBuilder.DropColumn(
                name: "VehicleModel",
                schema: "backoffice",
                table: "DeliveryDrivers");

            migrationBuilder.DropColumn(
                name: "VehicleType",
                schema: "backoffice",
                table: "DeliveryDrivers");

            migrationBuilder.DropColumn(
                name: "VehicleYear",
                schema: "backoffice",
                table: "DeliveryDrivers");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                schema: "backoffice",
                table: "DeliveryDrivers");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                schema: "backoffice",
                table: "DeliveryDrivers");

            migrationBuilder.DropColumn(
                name: "VerifiedByUserId",
                schema: "backoffice",
                table: "DeliveryDrivers");

            migrationBuilder.DropColumn(
                name: "DeliveryZoneId",
                schema: "backoffice",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "IsPoolDelivery",
                schema: "backoffice",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "PoolCommissionAmount",
                schema: "backoffice",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "PoolCommissionPercentage",
                schema: "backoffice",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "RestaurantLatitude",
                schema: "backoffice",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "RestaurantLongitude",
                schema: "backoffice",
                table: "Deliveries");
        }
    }
}
