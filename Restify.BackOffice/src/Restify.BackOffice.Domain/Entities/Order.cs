using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Pedido de un cliente
/// </summary>
public class Order : TenantEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public OrderType Type { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    
    // Cliente
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    // Estado de pago
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

    // Responsable del pedido (para pago directo)
    public string? ResponsiblePersonName { get; set; }
    public string? ResponsiblePersonIdentification { get; set; }

    // Mesa (si es para comer en el restaurante)
    public Guid? TableId { get; set; }
    public Table? Table { get; set; }

    // --- Dine-in / Mesa inteligente (Fase 1) ---

    /// <summary>
    /// Cantidad de personas (para pedidos dine-in)
    /// </summary>
    public int? GuestCount { get; set; }

    /// <summary>
    /// Fecha/hora en que se asignó la mesa
    /// </summary>
    public DateTime? TableAssignedAt { get; set; }

    /// <summary>
    /// Usuario que asignó la mesa
    /// </summary>
    public Guid? TableAssignedByUserId { get; set; }

    /// <summary>
    /// Observaciones sobre la mesa ("Junto a ventana", "Cumpleaños", etc.)
    /// </summary>
    public string? TableNotes { get; set; }

    // Items del pedido
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    
    // Montos
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    
    // Notas especiales
    public string? Notes { get; set; }
    
    // Timestamps
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
    
    // Usuario que tomó el pedido
    public string? TakenBy { get; set; }
}

/// <summary>
/// Item individual de un pedido
/// </summary>
public class OrderItem : TenantEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
    
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    
    // Estado del item (para cocina)
    public OrderItemStatus Status { get; set; } = OrderItemStatus.Pending;
    
    // Notas especiales del item
    public string? Notes { get; set; }
    
    // Modificadores aplicados
    public ICollection<OrderItemModifier> Modifiers { get; set; } = new List<OrderItemModifier>();
}

/// <summary>
/// Modificador aplicado a un item del pedido
/// </summary>
public class OrderItemModifier : BaseEntity
{
    public Guid OrderItemId { get; set; }
    public OrderItem OrderItem { get; set; } = null!;
    
    // Puede ser de un ProductModifier o GlobalModifier
    public string ModifierName { get; set; } = string.Empty;
    public decimal PriceAdjustment { get; set; }
}

/// <summary>
/// Tipo de pedido
/// </summary>
public enum OrderType
{
    DineIn = 1,     // Para comer en el restaurante
    Takeaway = 2,   // Para llevar
    Delivery = 3,   // Delivery
    Pickup = 4      // Para recoger en local
}

/// <summary>
/// Estado del pedido
/// </summary>
public enum OrderStatus
{
    Pending = 1,                    // Pendiente (recién creado)
    Confirmed = 2,                  // Confirmado (pago recibido, enviado a cocina)
    Preparing = 3,                  // En preparación
    Ready = 4,                      // Listo para servir/entregar
    AwaitingDispatchApproval = 5,   // Esperando aprobación de despacho
    Served = 6,                     // Servido
    Completed = 7,                  // Completado
    Cancelled = 8                   // Cancelado
}

/// <summary>
/// Estado de pago del pedido
/// </summary>
public enum PaymentStatus
{
    Unpaid = 1,
    PartiallyPaid = 2,
    Paid = 3
}

/// <summary>
/// Estado de un item individual
/// </summary>
public enum OrderItemStatus
{
    Pending = 1,    // Pendiente
    Preparing = 2,  // En preparación
    Ready = 3,      // Listo
    Served = 4      // Servido
}
