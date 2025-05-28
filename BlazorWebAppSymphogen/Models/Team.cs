namespace BlazorWebAppSymphogen.Models;

public class Team : IEquatable<Team>
{
    public required string Id { get; set; }
    public string? Name { get; set; }
    public IEnumerable<string> UserIds { get; set; } = [];
    public IEnumerable<string> SuperUserIds { get; set; } = [];
    public IEnumerable<string>? WorkflowConfigurationIds { get; set; }

    public bool TeamExists { get; set; } = true;

    public bool Equals(Team? other)
    {
        return Id == other?.Id;
    }
}
