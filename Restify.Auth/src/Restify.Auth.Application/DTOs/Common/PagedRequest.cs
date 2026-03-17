namespace Restify.Auth.Application.DTOs.Common;

/// <summary>
/// Request base para paginación
/// </summary>
public record PagedRequest(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    string? OrderBy = null,
    bool Descending = false
);
