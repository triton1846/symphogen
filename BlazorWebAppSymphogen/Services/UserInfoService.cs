using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using System.Text.Json;

namespace BlazorWebAppSymphogen.Services;

public class UserInfoService(
    ILogger<UserInfoService> logger,
    IDownstreamApi downstreamApi,
    IMemoryCache memoryCache,
    IHttpContextAccessor httpContextAccessor) : IUserInfoService
{
    private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

    public async Task<string?> GetDisplayNameAsync()
    {
        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return null; // Can't cache safely without a user ID
        }

        var cacheKey = $"DisplayName:{userId}";

        if (memoryCache.TryGetValue(cacheKey, out string? cachedDisplayName))
        {
            return cachedDisplayName;
        }

        try
        {
            var response = await downstreamApi.CallApiForUserAsync("GraphApi", options =>
            {
                options.RelativePath = "me";
            });

            if (!response.IsSuccessStatusCode)
                return null;

            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);
            var displayName = doc.RootElement.GetProperty("displayName").GetString();

            if (!string.IsNullOrEmpty(displayName))
            {
                memoryCache.Set(cacheKey, displayName, _cacheDuration);
            }

            return displayName;
        }
        catch (MicrosoftIdentityWebChallengeUserException)
        {
            throw; // Let the caller handle this (e.g., reauth or consent)
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve display name for user {UserId}", userId);
            return null;
        }
    }
}
