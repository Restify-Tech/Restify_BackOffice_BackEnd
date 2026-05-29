namespace Restify.BackOffice.Domain.Constants;

/// <summary>
/// Constantes globales del servicio BackOffice. Operaciones del restaurante: menu, pedidos, facturacion, inventario.
/// </summary>
public static class AppConstants
{
    public static class Pagination
    {
        public const int DefaultSize = 20;
        public const int MaxSize = 100;
    }

    public static class Order
    {
        public const int DefaultPreparationMinutes = 15;
        public const int MaxItemsPerOrder = 50;
        public const decimal MinOrderAmount = 0.01m;
    }

    public static class Invoice
    {
        public const decimal DefaultTaxPercentage = 15.0m;
        public const int MaxItemsPerInvoice = 100;
        public const string DefaultCurrency = "USD";
    }

    public static class Inventory
    {
        public const int LowStockThreshold = 10;
        public const int CriticalStockThreshold = 3;
    }

    public static class Delivery
    {
        public const int DefaultEstimatedMinutes = 30;
        public const decimal DefaultCommissionPercentage = 10.0m;
        public const int MaxDeliveryRadiusKm = 20;
    }

    public static class Security
    {
        public const int MinJwtSecretLength = 32;
    }

    public static class CashRegister
    {
        public const decimal MinOpeningBalance = 0m;
        public const decimal MaxOpeningBalance = 9999.99m;
    }
}
