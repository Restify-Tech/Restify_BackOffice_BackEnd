using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restify.BackOffice.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    /// <remarks>
    /// Esta migración estaba duplicada con AddOrdersModule.
    /// Las tablas Orders, OrderItems y OrderItemModifiers ya fueron creadas.
    /// Se dejó vacía para mantener la consistencia del historial de migraciones.
    /// </remarks>
    public partial class AddInvoicesAndReportsEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Migración vacía - las tablas ya fueron creadas en AddOrdersModule
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Migración vacía
        }
    }
}
