namespace Restify.BackOffice.Domain.Enums;

/// <summary>
/// Método de cálculo de costo de inventario
/// </summary>
public enum StockCostMethod
{
    /// <summary>
    /// FIFO - First In, First Out (lo primero que entra, primero sale)
    /// </summary>
    FIFO = 1,
    
    /// <summary>
    /// Promedio ponderado
    /// </summary>
    WeightedAverage = 2,
    
    /// <summary>
    /// LIFO - Last In, First Out (lo último que entra, primero sale)
    /// </summary>
    LIFO = 3
}
