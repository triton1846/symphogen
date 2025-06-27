using Newtonsoft.Json;

namespace BlazorWebAppSymphogen.Models;

[Serializable]
public class Team : IEquatable<Team>, ICloneable
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

    public List<WorkflowConfiguration> WorkflowConfigurations { get; set; } = [];

    [JsonIgnore]// This property is used for validation purposes only and should not be serialized
    public Team? ValidationWorkflowConfigurations { get; set; }

    public bool TeamExists { get; set; } = true;

    public object Clone()
    {
        Users.ForEach(user =>
        {
            user.Teams.Clear();
        });

        return new Team
        {
            Id = Id,
            Name = Name,
            UserIds = [.. UserIds],
            Users = [.. Users.Select(u => (User)u.Clone())],
            ValidationUsers = ValidationUsers,
            SuperUserIds = [.. SuperUserIds],
            SuperUsers = [.. SuperUsers.Select(u => (User)u.Clone())],
            ValidationSuperUsers = ValidationSuperUsers,
            WorkflowConfigurationIds = WorkflowConfigurationIds?.ToList(),
            WorkflowConfigurations = [.. WorkflowConfigurations.Select(wc => (WorkflowConfiguration)wc.Clone())],
            ValidationWorkflowConfigurations = ValidationWorkflowConfigurations,
            TeamExists = TeamExists
        };
    }

    public bool Equals(Team? other)
    {
        return Id == other?.Id;
    }
}
