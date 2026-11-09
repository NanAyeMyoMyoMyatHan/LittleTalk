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
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            try
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    var parsed = JsonSerializer.Deserialize<Result<T>>(content, JsonOptions);
                    if (parsed != null) return parsed;
                }
            }
            catch
            {
                // Fall back to clean unauthorized result
            }

            return Result<T>.Unauthorized("Your session has expired or you are not authorized. Please log in again.");
        }

        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            return Result<T>.Unauthorized("You do not have permission to access this resource.");
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            try
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    var parsed = JsonSerializer.Deserialize<Result<T>>(content, JsonOptions);
                    if (parsed != null) return parsed;
                }
            }
            catch
            {
                // Fall back to clean not found result
            }

            return Result<T>.NotFound("The requested resource was not found.");
        }

        try
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(content))
            {
                if (response.IsSuccessStatusCode)
                {
                    return Result<T>.Success(default!, "Operation completed successfully.");
                }

                return Result<T>.SystemError($"Server returned HTTP {(int)response.StatusCode} with empty response body.");
            }

            var result = JsonSerializer.Deserialize<Result<T>>(content, JsonOptions);
            return result ?? Result<T>.SystemError("Received empty response from server.");
        }
        catch (Exception ex)
        {
            return Result<T>.SystemError($"Failed to parse response: {ex.Message} (HTTP {(int)response.StatusCode})");
        }
    }

    private static async Task<Result> DeserializeResultAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            try
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    var parsed = JsonSerializer.Deserialize<Result>(content, JsonOptions);
                    if (parsed != null) return parsed;
                }
            }
            catch
            {
                // Fall back to clean unauthorized result
            }

            return Result.Unauthorized("Your session has expired or you are not authorized. Please log in again.");
        }

        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            return Result.Unauthorized("You do not have permission to access this resource.");
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            try
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    var parsed = JsonSerializer.Deserialize<Result>(content, JsonOptions);
                    if (parsed != null) return parsed;
                }
            }
            catch
            {
                // Fall back to clean not found result
            }

            return Result.NotFound("The requested resource was not found.");
        }

        try
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(content))
            {
                if (response.IsSuccessStatusCode)
                {
                    return Result.Success("Operation completed successfully.");
                }

                return Result.SystemError($"Server returned HTTP {(int)response.StatusCode} with empty response body.");
            }

            var result = JsonSerializer.Deserialize<Result>(content, JsonOptions);
            return result ?? Result.SystemError("Received empty response from server.");
        }
        catch (Exception ex)
        {
            return Result.SystemError($"Failed to parse response: {ex.Message} (HTTP {(int)response.StatusCode})");
        }
    }
}
