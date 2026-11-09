using System.Text.Json;
using Microsoft.JSInterop;

namespace LittleTalk.Client.Services;

public interface ILocalStorageService
{
    ValueTask<T?> GetItemAsync<T>(string key);
    ValueTask SetItemAsync<T>(string key, T value);
    ValueTask RemoveItemAsync(string key);
}

public class LocalStorageService : ILocalStorageService
{
    private readonly IJSRuntime _js;

    public LocalStorageService(IJSRuntime js)
    {
        _js = js;
    }

    public async ValueTask<T?> GetItemAsync<T>(string key)
    {
        try
        {
            var json = await _js.InvokeAsync<string?>("littleTalkStorage.getItem", key);
            if (string.IsNullOrEmpty(json)) return default;
            return JsonSerializer.Deserialize<T>(json);
        }
        catch
        {
            return default;
        }
    }

    public async ValueTask SetItemAsync<T>(string key, T value)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            await _js.InvokeVoidAsync("littleTalkStorage.setItem", key, json);
        }
        catch
        {
            // Ignore during prerender
        }
    }

    public async ValueTask RemoveItemAsync(string key)
    {
        try
        {
            await _js.InvokeVoidAsync("littleTalkStorage.removeItem", key);
        }
        catch
        {
            // Ignore during prerender
        }
    }
}
