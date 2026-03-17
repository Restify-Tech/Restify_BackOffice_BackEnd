namespace Restify.BackOffice.Application.DTOs;

/// <summary>
/// Sugerencia de mesa individual
/// </summary>
public record TableSuggestionDto(
    Guid Id,
    string Number,
    string? Name,
    int Capacity,
    int MinCapacity,
    string? Zone,
    bool IsRecommended
);

/// <summary>
/// Respuesta con mesas sugeridas para un grupo de personas
/// </summary>
public record TableSuggestionResponse(
    IEnumerable<TableSuggestionDto> Tables,
    int GuestCount,
    bool HasWarning,
    string? WarningMessage
);

/// <summary>
/// Request para asignar una mesa a un pedido
/// </summary>
public record AssignTableRequest(
    Guid OrderId,
    int GuestCount,
    bool ConfirmSuboptimal = false
);
