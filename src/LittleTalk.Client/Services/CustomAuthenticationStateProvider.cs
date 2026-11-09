using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using LittleTalk.Model.Features.Auth;

namespace LittleTalk.Client.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());
    private UserProfileResponseModel? _currentUser;
    private string? _currentToken;

    public CustomAuthenticationStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public UserProfileResponseModel? CurrentUser => _currentUser;
    public string? CurrentToken => _currentToken;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("lt_token");
            var user = await _localStorage.GetItemAsync<UserProfileResponseModel>("lt_user");

            if (string.IsNullOrEmpty(token) || user == null)
            {
                _currentUser = null;
                _currentToken = null;
                return new AuthenticationState(_anonymous);
            }

            _currentToken = token;
            _currentUser = user;

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.GivenName, user.FullName),
                new Claim("userId", user.UserId.ToString()),
                new Claim("role", user.Role),
                new Claim("fullName", user.FullName)
            }, "jwtAuth");

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch
        {
            return new AuthenticationState(_anonymous);
        }
    }

    public async Task MarkUserAsAuthenticated(string token, UserProfileResponseModel user)
    {
        await _localStorage.SetItemAsync("lt_token", token);
        await _localStorage.SetItemAsync("lt_user", user);

        _currentToken = token;
        _currentUser = user;

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.GivenName, user.FullName),
            new Claim("userId", user.UserId.ToString()),
            new Claim("role", user.Role),
            new Claim("fullName", user.FullName)
        }, "jwtAuth");

        var userPrincipal = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(userPrincipal)));
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _localStorage.RemoveItemAsync("lt_token");
        await _localStorage.RemoveItemAsync("lt_user");

        _currentToken = null;
        _currentUser = null;

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }
}
