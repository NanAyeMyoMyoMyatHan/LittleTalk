using LittleTalk.Shared; using LittleTalk.Models.Common;
using LittleTalk.Models.Features.Auth;

namespace LittleTalk.App.Services.Features;

public interface IAuthApiClient
{
    Task<Result<LoginResponseModel>?> LoginAsync(LoginRequestModel request);
    Task<Result<UserProfileResponseModel>?> GetCurrentUserAsync();
    Task<Result?> LogoutAsync();
}

