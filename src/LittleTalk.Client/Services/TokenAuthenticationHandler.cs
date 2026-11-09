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
        if (_authStateProvider is CustomAuthenticationStateProvider customProvider &&
            !string.IsNullOrEmpty(customProvider.CurrentToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", customProvider.CurrentToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
