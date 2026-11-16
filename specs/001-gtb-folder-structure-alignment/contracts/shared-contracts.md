# Shared Library Contract (`LittleTalk.Shared`)

## 1. Unified Result Envelope

All API endpoints and domain services must return the standard `Result<T>` envelope:

```csharp
namespace LittleTalk.Shared;

public class Result<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public ResultType ResultType { get; set; }

    public static Result<T> Success(T data, string? message = null);
    public static Result<T> Failure(string message, ResultType type = ResultType.BadRequest);
    public static Result<T> NotFound(string message = "Resource not found");
    public static Result<T> Unauthorized(string message = "Unauthorized access");
}
```

## 2. DevCode Shared Utilities Contract

`DevCode.cs` must reside in `LittleTalk.Shared` and provide centralized pagination, validation, and extension helpers:

```csharp
namespace LittleTalk.Shared;

public static class DevCode
{
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 10;
    public const int MaximumPageSize = 100;

    public static readonly int[] PageSizeOptions = [10, 25, 50, 100];

    public static int NormalizePageSize(int requestedPageSize);
    public static int NormalizePageNumber(int requestedPageNumber);
    
    // String & Entity helper extensions
    public static bool IsNullOrWhiteSpace(this string? value);
}
```

## 3. Base API Controller Contract

`BaseApiController` in `LittleTalk.Shared` provides standardized response mapping:

```csharp
namespace LittleTalk.Shared;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result);
}
```
