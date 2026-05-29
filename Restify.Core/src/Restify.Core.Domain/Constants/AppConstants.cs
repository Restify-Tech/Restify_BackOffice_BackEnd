namespace Restify.Core.Domain.Constants;

/// <summary>
/// Constantes globales del servicio Core. Parametros de configuracion de grids y tablas generales.
/// </summary>
public static class AppConstants
{
    public static class Pagination
    {
        public const int DefaultSize = 20;
        public const int MaxSize = 100;
        public const int DefaultGridPageSize = 10;
        public const string DefaultPageSizeOptions = "[10, 25, 50, 100]";
    }

    public static class Grid
    {
        public const string DefaultSortDirection = "asc";
        public const string FormModeModal = "modal";
        public const string FormModeDrawer = "drawer";
        public const string FormModeInline = "inline";
        public const string RowActionsPositionEnd = "end";
        public const string RowActionsPositionStart = "start";
    }

    public static class Export
    {
        public const string FormatExcel = "excel";
        public const string FormatCsv = "csv";
        public const string FormatPdf = "pdf";
        public const string DefaultFormats = "[\"excel\", \"csv\", \"pdf\"]";
    }

    public static class Security
    {
        public const int MinJwtSecretLength = 32;
    }
}
