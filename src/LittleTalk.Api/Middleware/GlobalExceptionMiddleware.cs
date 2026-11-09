using System.Net;
using System.Text.Json;
using LittleTalk.Application.Common.Exceptions;
using LittleTalk.Application.Common.Models;

namespace LittleTalk.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        int statusCode;
        string message;
        List<string>? errors = null;

        switch (exception)
        {
            case AppValidationException appValEx:
                statusCode = appValEx.StatusCode;
                message = appValEx.Message;
                errors = appValEx.Errors;
                _logger.LogWarning("Validation failure: {Message} | Errors: {Errors}", message, string.Join(", ", errors ?? new()));
                break;

            case FluentValidation.ValidationException fvEx:
                statusCode = (int)HttpStatusCode.UnprocessableEntity;
                message = "One or more validation failures have occurred.";
                errors = fvEx.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Validation failure: {Message} | Errors: {Errors}", message, string.Join(", ", errors));
                break;

            case NotFoundException notFoundEx:
                statusCode = notFoundEx.StatusCode;
                message = notFoundEx.Message;
                _logger.LogWarning("Resource not found: {Message}", message);
                break;

            case UnauthorizedException unauthorizedEx:
                statusCode = unauthorizedEx.StatusCode;
                message = unauthorizedEx.Message;
                _logger.LogWarning("Unauthorized access: {Message}", message);
                break;

            case AppException appEx:
                statusCode = appEx.StatusCode;
                message = appEx.Message;
                errors = appEx.Errors;
                _logger.LogWarning("Application error: {Message}", message);
                break;

            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = "An unexpected internal server error occurred.";
                _logger.LogError(exception, "Unhandled exception caught by middleware: {Message}", exception.Message);
                break;
        }

        context.Response.StatusCode = statusCode;
        var response = ApiResponse<object>.Fail(message, errors, statusCode);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, jsonOptions);
        await context.Response.WriteAsync(json);
    }
}
