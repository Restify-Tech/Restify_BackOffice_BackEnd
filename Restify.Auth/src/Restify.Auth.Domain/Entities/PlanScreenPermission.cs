using Restify.Auth.Domain.Common;

namespace Restify.Auth.Domain.Entities;

/// <summary>
/// Relación entre Plan y ScreenPermission — define qué pantallas incluye cada plan
/// </summary>
public class PlanScreenPermission : BaseEntity
{
    /// <summary>
    /// ID del plan al que pertenece este registro
    /// </summary>
    public Guid PlanId { get; set; }

    public virtual Plan Plan { get; set; } = null!;

    /// <summary>
    /// Código de la pantalla (FK lógica a ScreenPermission.ScreenCode)
    /// </summary>
    public string ScreenCode { get; set; } = string.Empty;

    /// <summary>
    /// Indica si la pantalla está incluida en el plan
    /// </summary>
    public bool IsIncluded { get; set; } = true;
}
