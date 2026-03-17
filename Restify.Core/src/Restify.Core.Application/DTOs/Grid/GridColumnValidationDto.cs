namespace Restify.Core.Application.DTOs.Grid;

/// <summary>
/// DTO de validación de columna
/// </summary>
public class GridColumnValidationDto
{
    public Guid Id { get; set; }
    public string ValidationType { get; set; } = string.Empty;
    public string? ValidationValue { get; set; }
    public string? ValidationValue2 { get; set; }
    public string? ErrorMessage { get; set; }
    public int Order { get; set; }
    public bool ClientOnly { get; set; }
    public bool ServerOnly { get; set; }
}

/// <summary>
/// Request para crear validación
/// </summary>
public class CreateGridColumnValidationRequest
{
    public Guid GridColumnId { get; set; }
    public string ValidationType { get; set; } = string.Empty;
    public string? ValidationValue { get; set; }
    public string? ValidationValue2 { get; set; }
    public string? ErrorMessage { get; set; }
    public int Order { get; set; }
}
