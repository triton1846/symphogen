using Microsoft.JSInterop;
using System.Text.Json;

namespace BlazorWebAppSymphogen.Services;

public class LocalStorageService(IJSRuntime jsRuntime) : ILocalStorageService
{
    public async Task<T?> GetItemAsync<T>(string key)
    {
        var json = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);

        if (string.IsNullOrEmpty(json))
            return default;

        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value));
    }

    public async Task RemoveItemAsync(string key)
    {
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
    }

    public async Task ClearAsync()
    {
        await jsRuntime.InvokeVoidAsync("localStorage.clear");
    }
}
