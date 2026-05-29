using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restify.BackOffice.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Phase11_Recipes_Reservations_Promotions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Phase8 (MultiBranch) and Phase9 (CashClosing_Shifts) already applied all structural changes.
            // Phase11 content (Recipes, Reservations, Promotions) is included in Phase12 migration.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op: nothing to undo here.
        }
    }
}
