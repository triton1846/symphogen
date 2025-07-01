using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Abstractions;
using System.Text.Json;

namespace BlazorWebAppSymphogen.Auth;

public class RequireDomainHandler(
    ILogger<RequireDomainHandler> logger,
    IDownstreamApi downstreamApi)
    : AuthorizationHandler<RequireDomainRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RequireDomainRequirement requirement)
    {
        var result = await downstreamApi.CallApiForUserAsync("GraphApi", options =>
        {
            options.RelativePath = "me?$select=mail";
        });

        logger.LogInformation("GraphApi call ({RequestUri}) result: {StatusCode}", result.RequestMessage?.RequestUri, result.StatusCode);

        if (!result.IsSuccessStatusCode)
            return;

        var json = await result.Content.ReadAsStringAsync();
        var user = JsonDocument.Parse(json);
        var email = user.RootElement.GetProperty("mail").GetString();

        if (email != null && email.EndsWith($"@{requirement.RequiredDomain}", StringComparison.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }
    }
}
