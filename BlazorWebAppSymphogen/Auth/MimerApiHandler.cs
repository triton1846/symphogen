namespace BlazorWebAppSymphogen.Auth;

public class MimerApiHandler(IHttpContextAccessor httpContextAccessor, ILogger<MimerApiHandler> logger) : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ILogger<MimerApiHandler> _logger = logger;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Get current user ID from claims
        var user = _httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            request.Headers.Add("x-ms-mimer-userid", userId);
            _logger.LogTrace("Added x-ms-mimer-userid header with value: {UserId}", userId);
        }
        else
        {
            _logger.LogWarning("No user ID found in claims, x-ms-mimer-userid header not added");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
