namespace LittleTalk.Application.Common.Exceptions;

public class AppException : Exception
{
    public int StatusCode { get; }
    public List<string>? Errors { get; }

    public AppException(string message, int statusCode = 400, List<string>? errors = null) 
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string entityName, object key) 
        : base($"{entityName} with id ({key}) was not found.", 404)
    {
    }

    public NotFoundException(string message) 
        : base(message, 404)
    {
    }
}

public class AppValidationException : AppException
{
    public AppValidationException(List<string> errors) 
        : base("One or more validation failures have occurred.", 422, errors)
    {
    }

    public AppValidationException(string error) 
        : base(error, 422, new List<string> { error })
    {
    }
}

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "You are not authorized to perform this action.") 
        : base(message, 401)
    {
    }
}
