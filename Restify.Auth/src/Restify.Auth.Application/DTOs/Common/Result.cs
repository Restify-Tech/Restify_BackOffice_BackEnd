namespace Restify.Auth.Application.DTOs.Common;

/// <summary>
/// Resultado de operación genérico
/// </summary>
public class Result
{
    public bool IsSuccess { get; protected set; }
    public string? Error { get; protected set; }
    public IEnumerable<string> Errors { get; protected set; } = [];

    protected Result(bool isSuccess, string? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    protected Result(bool isSuccess, IEnumerable<string> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
        Error = errors.FirstOrDefault();
    }

    public static Result Success() => new(true);
    public static Result Failure(string error) => new(false, error);
    public static Result Failure(IEnumerable<string> errors) => new(false, errors);

    public static Result<T> Success<T>(T data) => Result<T>.Success(data);
    public static Result<T> Failure<T>(string error) => Result<T>.Failure(error);
}

/// <summary>
/// Resultado de operación con datos
/// </summary>
public class Result<T> : Result
{
    public T? Data { get; private set; }

    private Result(bool isSuccess, T? data, string? error = null)
        : base(isSuccess, error)
    {
        Data = data;
    }

    private Result(bool isSuccess, IEnumerable<string> errors)
        : base(isSuccess, errors)
    {
    }

    public static Result<T> Success(T data) => new(true, data);
    public new static Result<T> Failure(string error) => new(false, default, error);
    public new static Result<T> Failure(IEnumerable<string> errors) => new(false, errors);
}
