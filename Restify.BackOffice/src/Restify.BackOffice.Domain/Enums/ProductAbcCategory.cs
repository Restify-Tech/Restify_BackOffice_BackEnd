namespace Restify.BackOffice.Domain.Enums;

/// <summary>
/// Clasificación ABC de productos según curva de Pareto
/// </summary>
public enum ProductAbcCategory
{
    /// <summary>
    /// Categoría A: ~20% de productos generan ~80% de ventas
    /// </summary>
    A = 1,
    
    /// <summary>
    /// Categoría B: ~30% de productos generan ~15% de ventas
    /// </summary>
    B = 2,
    
    /// <summary>
    /// Categoría C: ~50% de productos generan ~5% de ventas
    /// </summary>
    C = 3
}
