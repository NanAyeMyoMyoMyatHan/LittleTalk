using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Auth;

namespace LittleTalk.Client.Services.Features;

public class AuthApiClient : BaseFeatureApiClient, IAuthApiClient
{
    public AuthApiClient(HttpClient http) : base(http)
    {
    }

    public async Task<Result<LoginResponseModel>?> LoginAsync(LoginRequestModel request)
    {
        return await SendPostAsync<LoginResponseModel, LoginRequestModel>("api/auth/login", request);
    }

    public async Task<Result<UserProfileResponseModel>?> GetCurrentUserAsync()
    {
        return await SendGetAsync<UserProfileResponseModel>("api/auth/me");
    }

    public async Task<Result?> LogoutAsync()
    {
        return await SendPostAsync<object, object>("api/auth/logout", new { });
    }
}
