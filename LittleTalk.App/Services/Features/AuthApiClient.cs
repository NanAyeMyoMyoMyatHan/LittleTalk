using LittleTalk.Shared; using LittleTalk.Models.Common;
using LittleTalk.Models.Features.Auth;

namespace LittleTalk.App.Services.Features;

public class AuthApiClient : BaseFeatureApiClient, IAuthApiClient
{
    public AuthApiClient(HttpClient http, CustomAuthenticationStateProvider authStateProvider) 
        : base(http, authStateProvider)
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

