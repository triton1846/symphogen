using FluentValidation.Results;

namespace BlazorWebAppSymphogen.Models;

public record Team : DTOs.TeamDTO
{
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

    public User? ValidationUsers { get; set; }

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

    public User? ValidationSuperUsers { get; set; }

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

    public Team? ValidationWorkflowConfigurations { get; set; }

    public ValidationResult? ValidationResult { get; set; }

    public bool TeamExists { get; set; } = true;

    public bool ShowUsers { get; set; } = false;

    public bool ShowSuperUsers { get; set; } = false;
}
