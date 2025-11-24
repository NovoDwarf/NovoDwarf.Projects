namespace NovoUtils.Web;

public static class Result
{
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);
    public static Result<T> Failure<T>(string error, int httpStatusCode = 500) => Result<T>.Failure(error, httpStatusCode);

    public static Result<object?> Success() => Result<object?>.Success(null);
    public static Result<object?> Failure(string error, int httpStatusCode = 500) => Result<object?>.Failure(error, httpStatusCode);
}

public sealed class Result<T>
{
    private Result(T? value)
    {
        Value = value;
        IsSuccess = true;
        Error = string.Empty;
        HttpStatusCode = 200;
    }

    private Result(string error, int httpStatusCode)
    {
        Value = default;
        IsSuccess = false;
        Error = error;
        HttpStatusCode = httpStatusCode;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }
    public int HttpStatusCode { get; }

    public T Value => IsSuccess
        ? field!
        : throw new InvalidOperationException("Cannot access Value of a failed result");

    public static Result<T> Success(T? value) => new(value);
    public static Result<T> Failure(string error, int httpStatusCode = 500) => new(error, httpStatusCode);
}
