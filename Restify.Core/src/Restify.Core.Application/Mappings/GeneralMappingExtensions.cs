using System.Text.Json;
using Restify.Core.Application.DTOs.General;
using Restify.Core.Domain.Entities;

namespace Restify.Core.Application.Mappings;

/// <summary>
/// Extensiones de mapeo para GeneralTable y GeneralValue
/// </summary>
public static class GeneralMappingExtensions
{
    public static GeneralTableDto ToDto(this GeneralTable entity)
    {
        return new GeneralTableDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Description = entity.Description,
            ParentId = entity.ParentId,
            ParentName = entity.Parent?.Name,
            ApplicationCode = entity.ApplicationCode,
            IsInternal = entity.IsInternal,
            IsActive = entity.IsActive,
            DisplayOrder = entity.DisplayOrder,
            Icon = entity.Icon,
            ExtraConfig = ParseJsonObject(entity.ExtraConfig),
            Children = entity.Children?.Select(c => c.ToDto()).ToList() ?? new(),
            Values = entity.Values?.Select(v => v.ToDto()).ToList() ?? new()
        };
    }

    public static GeneralTableListDto ToListDto(this GeneralTable entity)
    {
        return new GeneralTableListDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Description = entity.Description,
            ParentId = entity.ParentId,
            ParentName = entity.Parent?.Name,
            IsInternal = entity.IsInternal,
            IsActive = entity.IsActive,
            DisplayOrder = entity.DisplayOrder,
            Icon = entity.Icon,
            ValuesCount = entity.Values?.Count ?? 0
        };
    }

    public static GeneralValueDto ToDto(this GeneralValue entity)
    {
        return new GeneralValueDto
        {
            Id = entity.Id,
            GeneralTableId = entity.GeneralTableId,
            GeneralTableCode = entity.GeneralTable?.Code ?? string.Empty,
            GeneralTableName = entity.GeneralTable?.Name ?? string.Empty,
            Code = entity.Code,
            Content = entity.Content,
            ShortDescription = entity.ShortDescription,
            NumericValue = entity.NumericValue,
            Reference1 = entity.Reference1,
            Reference2 = entity.Reference2,
            Reference3 = entity.Reference3,
            Reference4 = entity.Reference4,
            Reference5 = entity.Reference5,
            Icon = entity.Icon,
            BackgroundColor = entity.BackgroundColor,
            TextColor = entity.TextColor,
            DisplayOrder = entity.DisplayOrder,
            IsLocked = entity.IsLocked,
            IsActive = entity.IsActive,
            IsDefault = entity.IsDefault,
            ExtraConfig = ParseJsonObject(entity.ExtraConfig)
        };
    }

    public static GeneralValueListDto ToListDto(this GeneralValue entity)
    {
        return new GeneralValueListDto
        {
            Id = entity.Id,
            GeneralTableId = entity.GeneralTableId,
            Code = entity.Code,
            Content = entity.Content,
            ShortDescription = entity.ShortDescription,
            NumericValue = entity.NumericValue,
            Icon = entity.Icon,
            BackgroundColor = entity.BackgroundColor,
            TextColor = entity.TextColor,
            DisplayOrder = entity.DisplayOrder,
            IsLocked = entity.IsLocked,
            IsActive = entity.IsActive,
            IsDefault = entity.IsDefault
        };
    }

    private static Dictionary<string, object>? ParseJsonObject(string? json)
    {
        if (string.IsNullOrEmpty(json)) return null;
        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        }
        catch
        {
            return null;
        }
    }
}
