namespace PharmacyBilling.Application.Common.Models;

public class Result
{
    public bool IsSuccess { get; protected init; }
    public string? Error { get; protected init; }
    public int StatusCode { get; protected init; }

    protected Result() { }

    public static Result Success() => new() { IsSuccess = true, StatusCode = 200 };
    public static Result Failure(string error, int statusCode = 400) => new() { IsSuccess = false, Error = error, StatusCode = statusCode };
    public static Result NotFound(string error) => Failure(error, 404);
    public static Result Unauthorized(string error) => Failure(error, 401);
    public static Result Forbidden(string error) => Failure(error, 403);
    public static Result Conflict(string error) => Failure(error, 409);
}

public class Result<T> : Result
{
    public T? Data { get; private init; }

    public static Result<T> Success(T data) => new() { IsSuccess = true, Data = data, StatusCode = 200 };
    public static new Result<T> Failure(string error, int statusCode = 400) => new() { IsSuccess = false, Error = error, StatusCode = statusCode };
    public static new Result<T> NotFound(string error) => Failure(error, 404);
    public static new Result<T> Unauthorized(string error) => Failure(error, 401);
    public static new Result<T> Forbidden(string error) => Failure(error, 403);
    public static new Result<T> Conflict(string error) => Failure(error, 409);
}
