namespace Restify.Core.Application.DTOs.Common;

/// <summary>
/// Resultado de operación con tipado genérico
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string? Error { get; private set; }
    public List<string> Errors { get; private set; } = new();

    private Result() { }

    public static Result<T> Success(T data) => new()
    {
        IsSuccess = true,
        Data = data
    };

    public static Result<T> Failure(string error) => new()
    {
        IsSuccess = false,
        Error = error,
        Errors = new List<string> { error }
    };

    public static Result<T> Failure(List<string> errors) => new()
    {
        IsSuccess = false,
        Error = errors.FirstOrDefault(),
        Errors = errors
    };
}

/// <summary>
/// Resultado de operación sin datos
/// </summary>
public class Result
{
    public bool IsSuccess { get; private set; }
    public string? Error { get; private set; }
    public List<string> Errors { get; private set; } = new();

    private Result() { }

    public static Result Success() => new() { IsSuccess = true };

    public static Result Failure(string error) => new()
    {
        IsSuccess = false,
        Error = error,
        Errors = new List<string> { error }
    };

    public static Result Failure(List<string> errors) => new()
    {
        IsSuccess = false,
        Error = errors.FirstOrDefault(),
        Errors = errors
    };
}
