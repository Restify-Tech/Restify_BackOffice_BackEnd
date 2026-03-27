using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restify.BackOffice.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Phase9_CashClosing_Shifts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // BranchId columns, branches table and manager_assignments were already created in Phase8_MultiBranch

            migrationBuilder.CreateTable(
                name: "cash_closings",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CashRegisterSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClosedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ClosedByName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    SupervisedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    SupervisedByName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    denominations_json = table.Column<string>(type: "text", nullable: true),
                    TotalCounted = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalExpected = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Difference = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DifferenceReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    BankDepositAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    DepositVoucherUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BankName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    BankReference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ReportZUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ApprovedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RejectionReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ClosingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cash_closings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cash_closings_CashRegisterSessions_CashRegisterSessionId",
                        column: x => x.CashRegisterSessionId,
                        principalSchema: "backoffice",
                        principalTable: "CashRegisterSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "shift_templates",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    DaysOfWeek = table.Column<string>(type: "text", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shift_templates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "manager_assignments",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UserEmail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CanApproveCashClosing = table.Column<bool>(type: "boolean", nullable: false),
                    CanVoidOrders = table.Column<bool>(type: "boolean", nullable: false),
                    MaxDiscountPercent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_manager_assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_manager_assignments_branches_BranchId",
                        column: x => x.BranchId,
                        principalSchema: "backoffice",
                        principalTable: "branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shift_assignments",
                schema: "backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShiftTemplateId = table.Column<Guid>(type: "uuid", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ScheduledStart = table.Column<TimeSpan>(type: "interval", nullable: true),
                    ScheduledEnd = table.Column<TimeSpan>(type: "interval", nullable: true),
                    ActualClockIn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActualClockOut = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ClockInMethod = table.Column<int>(type: "integer", nullable: false),
                    ClockInLocation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ApprovedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    HoursWorked = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shift_assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_shift_assignments_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "backoffice",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_shift_assignments_shift_templates_ShiftTemplateId",
                        column: x => x.ShiftTemplateId,
                        principalSchema: "backoffice",
                        principalTable: "shift_templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cash_closings_CashRegisterSessionId",
                schema: "backoffice",
                table: "cash_closings",
                column: "CashRegisterSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_cash_closings_TenantId",
                schema: "backoffice",
                table: "cash_closings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_cash_closings_TenantId_ClosingDate",
                schema: "backoffice",
                table: "cash_closings",
                columns: new[] { "TenantId", "ClosingDate" });

            migrationBuilder.CreateIndex(
                name: "IX_cash_closings_TenantId_Status",
                schema: "backoffice",
                table: "cash_closings",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_manager_assignments_BranchId",
                schema: "backoffice",
                table: "manager_assignments",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_manager_assignments_BranchId_UserId",
                schema: "backoffice",
                table: "manager_assignments",
                columns: new[] { "BranchId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_manager_assignments_TenantId",
                schema: "backoffice",
                table: "manager_assignments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_manager_assignments_TenantId_IsActive",
                schema: "backoffice",
                table: "manager_assignments",
                columns: new[] { "TenantId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_manager_assignments_UserId",
                schema: "backoffice",
                table: "manager_assignments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_shift_assignments_EmployeeId",
                schema: "backoffice",
                table: "shift_assignments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_shift_assignments_ShiftTemplateId",
                schema: "backoffice",
                table: "shift_assignments",
                column: "ShiftTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_shift_assignments_TenantId",
                schema: "backoffice",
                table: "shift_assignments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_shift_assignments_TenantId_Date",
                schema: "backoffice",
                table: "shift_assignments",
                columns: new[] { "TenantId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_shift_assignments_TenantId_EmployeeId_Date",
                schema: "backoffice",
                table: "shift_assignments",
                columns: new[] { "TenantId", "EmployeeId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_shift_assignments_TenantId_Status",
                schema: "backoffice",
                table: "shift_assignments",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_shift_templates_TenantId",
                schema: "backoffice",
                table: "shift_templates",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_shift_templates_TenantId_BranchId",
                schema: "backoffice",
                table: "shift_templates",
                columns: new[] { "TenantId", "BranchId" });

            migrationBuilder.CreateIndex(
                name: "IX_shift_templates_TenantId_IsActive",
                schema: "backoffice",
                table: "shift_templates",
                columns: new[] { "TenantId", "IsActive" });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cash_closings",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "shift_assignments",
                schema: "backoffice");

            migrationBuilder.DropTable(
                name: "shift_templates",
                schema: "backoffice");
        }
    }
}
