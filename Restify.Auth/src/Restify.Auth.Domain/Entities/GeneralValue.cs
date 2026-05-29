namespace Restify.Auth.Domain.Entities;

/// <summary>
/// Tabla de parametros generales del sistema.
/// Estructura recursiva padre-hijo para organizar valores por categoria.
/// Reemplaza valores hardcodeados en codigo por parametros configurables.
/// </summary>
public class GeneralValue
{
    public Guid Id { get; private set; }
    public Guid? ParentId { get; private set; }
    public GeneralValue? Parent { get; private set; }
    public ICollection<GeneralValue> Children { get; private set; } = new List<GeneralValue>();

    public string Key { get; private set; } = string.Empty;           // Identificador unico: "MAX_LOGIN_ATTEMPTS"
    public string DisplayName { get; private set; } = string.Empty;   // Nombre legible: "Intentos Maximos de Login"
    public string? Value { get; private set; }                        // Valor almacenado como string
    public GeneralValueType ValueType { get; private set; }           // Tipo del valor
    public string Category { get; private set; } = string.Empty;      // Agrupacion: "CONFIG", "SECURITY", "LIMITS"
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsEditable { get; private set; }      // false = solo lectura desde UI
    public int SortOrder { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private GeneralValue() { }  // EF Core

    public static GeneralValue Create(
        string key,
        string displayName,
        string? value,
        GeneralValueType valueType,
        string category,
        string? description = null,
        Guid? parentId = null,
        bool isEditable = true,
        int sortOrder = 0)
    {
        return new GeneralValue
        {
            Id = Guid.NewGuid(),
            Key = key.ToUpperInvariant(),
            DisplayName = displayName,
            Value = value,
            ValueType = valueType,
            Category = category.ToUpperInvariant(),
            Description = description,
            ParentId = parentId,
            IsActive = true,
            IsEditable = isEditable,
            SortOrder = sortOrder,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public void UpdateValue(string? newValue)
    {
        if (!IsEditable)
            throw new InvalidOperationException($"El parametro '{Key}' no es editable.");
        Value = newValue;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetActive(bool active) { IsActive = active; UpdatedAt = DateTime.UtcNow; }

    // Helpers de conversion tipada
    public string? AsString() => Value;
    public decimal? AsDecimal() => decimal.TryParse(Value, out var d) ? d : null;
    public int? AsInt() => int.TryParse(Value, out var i) ? i : null;
    public bool? AsBool() => bool.TryParse(Value, out var b) ? b : null;
    public T? AsJson<T>() where T : class
    {
        if (string.IsNullOrWhiteSpace(Value)) return null;
        return System.Text.Json.JsonSerializer.Deserialize<T>(Value);
    }
}

public enum GeneralValueType
{
    String,
    Number,
    Decimal,
    Boolean,
    Json,
    Encrypted  // Valor encriptado — no mostrar en UI sin descifrar
}
