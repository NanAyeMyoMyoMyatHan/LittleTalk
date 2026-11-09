using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Authorization;

namespace LittleTalk.Client.Services;

public class TokenAuthenticationHandler : DelegatingHandler
{
    private readonly AuthenticationStateProvider _authStateProvider;

    public TokenAuthenticationHandler(AuthenticationStateProvider authStateProvider)
    {
        _authStateProvider = authStateProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization == null && _authStateProvider is CustomAuthenticationStateProvider customProvider)
        {
            var token = await customProvider.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
