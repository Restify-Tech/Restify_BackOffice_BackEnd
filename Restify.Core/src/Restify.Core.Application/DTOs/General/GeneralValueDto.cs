namespace Restify.Core.Application.DTOs.General;

/// <summary>
/// DTO completo de GeneralValue
/// </summary>
public class GeneralValueDto
{
    public Guid Id { get; set; }
    public Guid GeneralTableId { get; set; }
    public string GeneralTableCode { get; set; } = string.Empty;
    public string GeneralTableName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public decimal? NumericValue { get; set; }
    public string? Reference1 { get; set; }
    public string? Reference2 { get; set; }
    public string? Reference3 { get; set; }
    public string? Reference4 { get; set; }
    public string? Reference5 { get; set; }
    public string? Icon { get; set; }
    public string? BackgroundColor { get; set; }
    public string? TextColor { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsLocked { get; set; }
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    public Dictionary<string, object>? ExtraConfig { get; set; }
}

/// <summary>
/// DTO simplificado para listas/lookups
/// </summary>
public class GeneralValueListDto
{
    public Guid Id { get; set; }
    public Guid GeneralTableId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public decimal? NumericValue { get; set; }
    public string? Icon { get; set; }
    public string? BackgroundColor { get; set; }
    public string? TextColor { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsLocked { get; set; }
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
}

/// <summary>
/// Request para crear GeneralValue
/// </summary>
public class CreateGeneralValueRequest
{
    public Guid GeneralTableId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public decimal? NumericValue { get; set; }
    public string? Reference1 { get; set; }
    public string? Reference2 { get; set; }
    public string? Reference3 { get; set; }
    public string? Reference4 { get; set; }
    public string? Reference5 { get; set; }
    public string? Icon { get; set; }
    public string? BackgroundColor { get; set; }
    public string? TextColor { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public bool IsLocked { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; } = false;
    public Dictionary<string, object>? ExtraConfig { get; set; }
}

/// <summary>
/// Request para actualizar GeneralValue
/// </summary>
public class UpdateGeneralValueRequest : CreateGeneralValueRequest
{
    public Guid Id { get; set; }
}

/// <summary>
/// Request para obtener valores por código de tabla
/// </summary>
public class GetValuesByTableCodeRequest
{
    public string TableCode { get; set; } = string.Empty;
    public bool? ActiveOnly { get; set; } = true;
}
