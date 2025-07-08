using FluentValidation.Results;
using Newtonsoft.Json;

namespace BlazorWebAppSymphogen.Models;

[Serializable]
public class User
{
    public required string Id { get; set; }

    public string? Email { get; set; }

    public string? FullName { get; set; }

    public string? Department { get; set; }

    public string? Location { get; set; }

    public IEnumerable<string> Favorites { get; set; } = [];

    public string? Initials { get; set; }

    public string? JobTitle { get; set; }

    [JsonProperty(PropertyName = "officephone")]
    public string? OfficePhoneNumber { get; set; }

    [JsonIgnore]
    public string? GroupKey => null;

    public IEnumerable<string> TeamIds { get; set; } = [];

    private List<Team> _teams = [];
    public List<Team> Teams
    {
        get
        {
            return _teams;
        }
        set
        {
            _teams = value ?? [];
            TeamIds = _teams.Select(t => t.Id);
        }
    }

    [JsonIgnore]// This property is used for validation purposes only and should not be serialized
    public Team? ValidationTeams { get; set; }

    public bool UserExists { get; set; } = true;

    public ValidationResult? ValidationResult { get; set; }
}
