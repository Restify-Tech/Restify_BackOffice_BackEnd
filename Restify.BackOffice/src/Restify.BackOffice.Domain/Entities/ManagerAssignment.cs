using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Asignacion de un encargado/gerente a una sucursal
/// </summary>
public class ManagerAssignment : TenantEntity
{
    public Guid BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    /// <summary>
    /// FK al usuario en el servicio Auth (sin navegacion directa)
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Nombre del usuario (denormalizado para display)
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    public string UserEmail { get; set; } = string.Empty;

    public bool CanApproveCashClosing { get; set; } = true;
    public bool CanVoidOrders { get; set; } = false;
    public decimal MaxDiscountPercent { get; set; } = 10;
    public bool IsActive { get; set; } = true;
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}
