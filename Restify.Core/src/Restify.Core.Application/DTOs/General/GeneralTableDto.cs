namespace Restify.Core.Application.DTOs.General;

/// <summary>
/// DTO completo de GeneralTable
/// </summary>
public class GeneralTableDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
    public string? ParentName { get; set; }
    public string? ApplicationCode { get; set; }
    public bool IsInternal { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public Dictionary<string, object>? ExtraConfig { get; set; }

    // Navegación
    public List<GeneralTableDto> Children { get; set; } = new();
    public List<GeneralValueDto> Values { get; set; } = new();
}

/// <summary>
/// DTO simplificado para listas/lookups
/// </summary>
public class GeneralTableListDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
    public string? ParentName { get; set; }
    public bool IsInternal { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public int ValuesCount { get; set; }
}

/// <summary>
/// Request para crear GeneralTable
/// </summary>
public class CreateGeneralTableRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
    public string? ApplicationCode { get; set; }
    public bool IsInternal { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;
    public string? Icon { get; set; }
    public Dictionary<string, object>? ExtraConfig { get; set; }
}

/// <summary>
/// Request para actualizar GeneralTable
/// </summary>
public class UpdateGeneralTableRequest : CreateGeneralTableRequest
{
    public Guid Id { get; set; }
}
