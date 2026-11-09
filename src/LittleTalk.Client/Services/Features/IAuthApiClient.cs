using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Auth;

namespace LittleTalk.Client.Services.Features;

public interface IAuthApiClient
{
    Task<Result<LoginResponseModel>?> LoginAsync(LoginRequestModel request);
    Task<Result<UserProfileResponseModel>?> GetCurrentUserAsync();
    Task<Result?> LogoutAsync();
}
