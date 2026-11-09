using System.Net.Http.Json;
using System.Text.Json;
using LittleTalk.Model.Common;

namespace LittleTalk.Client.Services.Features;

public abstract class BaseFeatureApiClient
{
    protected readonly HttpClient Http;
    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected BaseFeatureApiClient(HttpClient http)
    {
        Http = http;
    }

    protected async Task<Result<T>> SendGetAsync<T>(string uri, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await Http.GetAsync(uri, cancellationToken);
            return await DeserializeResultAsync<T>(response, cancellationToken);
        }
        catch (Exception ex)
        {
            return Result<T>.SystemError($"Communication error: {ex.Message}");
        }
    }

    protected async Task<Result<T>> SendPostAsync<T, TRequest>(string uri, TRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await Http.PostAsJsonAsync(uri, request, JsonOptions, cancellationToken);
            return await DeserializeResultAsync<T>(response, cancellationToken);
        }
        catch (Exception ex)
        {
            return Result<T>.SystemError($"Communication error: {ex.Message}");
        }
    }

    protected async Task<Result<T>> SendPutAsync<T, TRequest>(string uri, TRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await Http.PutAsJsonAsync(uri, request, JsonOptions, cancellationToken);
            return await DeserializeResultAsync<T>(response, cancellationToken);
        }
        catch (Exception ex)
        {
            return Result<T>.SystemError($"Communication error: {ex.Message}");
        }
    }

    protected async Task<Result> SendDeleteAsync(string uri, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await Http.DeleteAsync(uri, cancellationToken);
            return await DeserializeResultAsync(response, cancellationToken);
        }
        catch (Exception ex)
        {
            return Result.SystemError($"Communication error: {ex.Message}");
        }
    }

    private static async Task<Result<T>> DeserializeResultAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            var result = await response.Content.ReadFromJsonAsync<Result<T>>(JsonOptions, cancellationToken);
            return result ?? Result<T>.SystemError("Received empty response from server.");
        }
        catch (Exception ex)
        {
            return Result<T>.SystemError($"Failed to parse response: {ex.Message} (HTTP {(int)response.StatusCode})");
        }
    }

    private static async Task<Result> DeserializeResultAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            var result = await response.Content.ReadFromJsonAsync<Result>(JsonOptions, cancellationToken);
            return result ?? Result.SystemError("Received empty response from server.");
        }
        catch (Exception ex)
        {
            return Result.SystemError($"Failed to parse response: {ex.Message} (HTTP {(int)response.StatusCode})");
        }
    }
}
