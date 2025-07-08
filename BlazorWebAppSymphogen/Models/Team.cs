using FluentValidation.Results;
using Newtonsoft.Json;

namespace BlazorWebAppSymphogen.Models;

[Serializable]
public class Team // TODO: Consider DTO classes for interaction with the API. This class has UI related properties...
{
    public required string Id { get; set; }

    public string? Name { get; set; }

    public IEnumerable<string> UserIds { get; set; } = [];

    private List<User> _users = [];
    public List<User> Users
    {
        get
        {
            return _users;
        }
        set
        {
            _users = value ?? [];
            UserIds = _users.Select(u => u.Id);
        }
    }

    [JsonIgnore]// This property is used for validation purposes only and should not be serialized
    public User? ValidationUsers { get; set; }

    public IEnumerable<string> SuperUserIds { get; set; } = [];

    private List<User> _superUsers = [];
    public List<User> SuperUsers
    {
        get
        {
            return _superUsers;
        }
        set
        {
            _superUsers = value ?? [];
            SuperUserIds = _superUsers.Select(u => u.Id);
        }
    }

    [JsonIgnore]// This property is used for validation purposes only and should not be serialized
    public User? ValidationSuperUsers { get; set; }

    public IEnumerable<string>? WorkflowConfigurationIds { get; set; }

    private List<WorkflowConfiguration> _workflowConfigurations = [];
    public List<WorkflowConfiguration> WorkflowConfigurations
    {
        get
        {
            return _workflowConfigurations;
        }
        set
        {
            _workflowConfigurations = value ?? [];
            WorkflowConfigurationIds = _workflowConfigurations.Select(wc => wc.Id);
        }
    }

    [JsonIgnore]// This property is used for validation purposes only and should not be serialized
    public Team? ValidationWorkflowConfigurations { get; set; }

    public ValidationResult? ValidationResult { get; set; }

    public bool TeamExists { get; set; } = true;

    public bool ShowUsers { get; set; } = false;

    public bool ShowSuperUsers { get; set; } = false;
}
