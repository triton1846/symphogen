using Microsoft.AspNetCore.Authorization;

namespace BlazorWebAppSymphogen.Auth;

public class RequireDomainRequirement : IAuthorizationRequirement
{
    public string RequiredDomain { get; }

    public RequireDomainRequirement(string requiredDomain)
    {
        RequiredDomain = requiredDomain;
    }
}

