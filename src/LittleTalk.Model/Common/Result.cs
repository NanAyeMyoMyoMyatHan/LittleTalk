namespace LittleTalk.Model.Common;

public class Result
{
    public bool IsSuccess => ResultType == ResultType.Success;
    public ResultType ResultType { get; set; } = ResultType.Success;
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();

    public static Result Success(string message = "Operation completed successfully.")
        => new() { ResultType = ResultType.Success, Message = message };

    public static Result ValidationError(List<string> errors, string message = "Validation failed.")
        => new() { ResultType = ResultType.ValidationError, Errors = errors, Message = message };

    public static Result ValidationError(string error)
        => new() { ResultType = ResultType.ValidationError, Errors = new() { error }, Message = error };

    public static Result NotFound(string message = "Requested resource was not found.")
        => new() { ResultType = ResultType.NotFound, Message = message };

    public static Result Duplicate(string message = "A duplicate record already exists.")
        => new() { ResultType = ResultType.Duplicate, Message = message };

    public static Result Unauthorized(string message = "Unauthorized access.")
        => new() { ResultType = ResultType.Unauthorized, Message = message };

    public static Result SystemError(string message = "An internal error occurred.")
        => new() { ResultType = ResultType.SystemError, Message = message };
}

public class Result<T> : Result
{
    public T? Data { get; set; }

    public static Result<T> Success(T data, string message = "Operation completed successfully.")
        => new() { ResultType = ResultType.Success, Data = data, Message = message };

    public static new Result<T> ValidationError(List<string> errors, string message = "Validation failed.")
        => new() { ResultType = ResultType.ValidationError, Errors = errors, Message = message };

    public static new Result<T> ValidationError(string error)
        => new() { ResultType = ResultType.ValidationError, Errors = new() { error }, Message = error };

    public static new Result<T> NotFound(string message = "Requested resource was not found.")
        => new() { ResultType = ResultType.NotFound, Message = message };

    public static new Result<T> Duplicate(string message = "A duplicate record already exists.")
        => new() { ResultType = ResultType.Duplicate, Message = message };

    public static new Result<T> Unauthorized(string message = "Unauthorized access.")
        => new() { ResultType = ResultType.Unauthorized, Message = message };

    public static new Result<T> SystemError(string message = "An internal error occurred.")
        => new() { ResultType = ResultType.SystemError, Message = message };
}
