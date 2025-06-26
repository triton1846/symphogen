using Newtonsoft.Json;

namespace BlazorWebAppSymphogen.Models;

public class Team : IEquatable<Team>
{
    public required string Id { get; set; }
    public string? Name { get; set; }
    public IEnumerable<string> UserIds { get; set; } = [];
    public List<User> Users { get; set; } = [];
    [JsonIgnore]// This property is used for validation purposes only and should not be serialized
    public User? ValidationUsers { get; set; }

    public IEnumerable<string> SuperUserIds { get; set; } = [];
    public List<User> SuperUsers { get; set; } = [];
    [JsonIgnore]// This property is used for validation purposes only and should not be serialized
    public User? ValidationSuperUsers { get; set; }

    public IEnumerable<string>? WorkflowConfigurationIds { get; set; }

    [JsonIgnore]
    public List<WorkflowConfiguration> WorkflowConfigurations { get; set; } = [];

    [JsonIgnore]// This property is used for validation purposes only and should not be serialized
    public Team? ValidationWorkflowConfigurations { get; set; }

    public bool TeamExists { get; set; } = true;

    public bool Equals(Team? other)
    {
        return Id == other?.Id;
    }
}
