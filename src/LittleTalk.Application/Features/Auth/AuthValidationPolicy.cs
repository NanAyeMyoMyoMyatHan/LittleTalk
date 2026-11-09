using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Auth;

namespace LittleTalk.Application.Features.Auth;

public class AuthValidationPolicy
{
    public Result ValidateLogin(LoginRequestModel request)
    {
        if (request == null)
        {
            return Result.ValidationError("Login request payload cannot be empty.");
        }

        request.UsernameOrEmail = request.UsernameOrEmail?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(request.UsernameOrEmail))
        {
            return Result.ValidationError("Username or Email is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return Result.ValidationError("Password is required.");
        }

        if (request.Password.Length < 6)
        {
            return Result.ValidationError("Password must be at least 6 characters.");
        }

        return Result.Success("Validation succeeded.");
    }
}
