namespace Restify.Auth.Application.DTOs.Common;

/// <summary>
/// Response paginada genérica
/// </summary>
public record PagedResponse<T>(
    IEnumerable<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
)
{
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
