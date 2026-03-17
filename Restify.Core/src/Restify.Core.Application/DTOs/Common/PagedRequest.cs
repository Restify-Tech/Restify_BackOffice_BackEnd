namespace Restify.Core.Application.DTOs.Common;

/// <summary>
/// Request para paginación
/// </summary>
public class PagedRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortColumn { get; set; }
    public bool SortDescending { get; set; } = false;
    public string? SearchTerm { get; set; }
    public Dictionary<string, string>? Filters { get; set; }
}

/// <summary>
/// Respuesta paginada
/// </summary>
public class PagedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
