using Microsoft.AspNetCore.Authorization;

namespace BlazorWebAppSymphogen.Auth;

public class RequireDomainRequirement : IAuthorizationRequirement
{
    public List<string> AllowedDomains { get; } = [];

    public RequireDomainRequirement(string requiredDomain)
    {
        AllowedDomains.Add(requiredDomain);
    }

    public RequireDomainRequirement(params string[] requiredDomains)
    {
        AllowedDomains.AddRange(requiredDomains);
    }

    public RequireDomainRequirement(IEnumerable<string> requiredDomains)
    {
        AllowedDomains.AddRange(requiredDomains);
    }
}

