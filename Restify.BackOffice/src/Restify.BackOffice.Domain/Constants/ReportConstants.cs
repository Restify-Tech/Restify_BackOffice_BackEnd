namespace Restify.BackOffice.Domain.Constants;

/// <summary>
/// Constantes para el modulo de reportes
/// </summary>
public static class ReportConstants
{
    // ========== Mensajes de error ==========
    public const string ErrorSalesReport = "Error al obtener reporte de ventas";
    public const string ErrorProductReport = "Error al obtener reporte de productos";
    public const string ErrorStaffReport = "Error al obtener reporte de personal";
    public const string ErrorDashboardSummary = "Error al obtener resumen del dashboard";
    public const string ErrorExportReport = "Error al exportar reporte";
    public const string ErrorInvalidDateRange = "El rango de fechas no es valido";
    public const string ErrorInvalidReportType = "Tipo de reporte no valido. Valores permitidos: sales, products, staff";
    public const string ErrorInvalidExportFormat = "Formato de exportacion no valido. Valores permitidos: csv, excel, pdf";

    // ========== Tipos de reporte ==========
    public const string ReportTypeSales = "sales";
    public const string ReportTypeProducts = "products";
    public const string ReportTypeStaff = "staff";

    // ========== Formatos de exportacion ==========
    public const string FormatCsv = "csv";
    public const string FormatExcel = "excel";
    public const string FormatPdf = "pdf";

    // ========== CSV ==========
    public const string CsvContentType = "text/csv";
    public const string CsvSalesFileName = "reporte_ventas.csv";
    public const string CsvProductsFileName = "reporte_productos.csv";
    public const string CsvStaffFileName = "reporte_personal.csv";

    // ========== CSV Headers ==========
    public const string CsvSalesHeader = "Fecha,Facturas,Subtotal,Impuesto,Descuento,Total,Pedidos,Ticket Promedio";
    public const string CsvProductsHeader = "Producto,Categoria,Cantidad Vendida,Ingresos,Precio Promedio,Porcentaje";
    public const string CsvStaffHeader = "Empleado,Rol,Pedidos Atendidos,Ventas Totales,Ticket Promedio,Tiempo Promedio (min)";

    // ========== Nombres de dias de la semana ==========
    public static readonly string[] DayNames = { "Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado" };

    // ========== Limites por defecto ==========
    public const int DefaultTopProductsLimit = 20;
    public const int MaxDateRangeDays = 365;
}
