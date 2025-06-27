using Newtonsoft.Json;

namespace BlazorWebAppSymphogen.Models;

[Serializable]
public class User : ICloneable
{
    public required string Id { get; set; }

    public string? Email { get; set; }

    public string? FullName { get; set; }

    public string? Department { get; set; }

    public string? Location { get; set; }

    public IEnumerable<string>? Favorites { get; set; }

    public string? Initials { get; set; }

    public string? JobTitle { get; set; }

    [JsonProperty(PropertyName = "officephone")]
    public string? OfficePhoneNumber { get; set; }

    public IEnumerable<string> TeamIds { get; set; } = [];

    public List<Team> Teams { get; set; } = [];

    [JsonIgnore]// This property is used for validation purposes only and should not be serialized
    public Team? ValidationTeams { get; set; }

    public bool UserExists { get; set; } = true;

    [JsonIgnore]
    public string? GroupKey => null;

    public object Clone()
    {
        Teams.ForEach(team =>
        {
            team.Users.Clear();
            team.SuperUsers.Clear();
        });

        return new User
        {
            Id = Id,
            Email = Email,
            FullName = FullName,
            Department = Department,
            Location = Location,
            Favorites = Favorites?.ToList(),
            Initials = Initials,
            JobTitle = JobTitle,
            OfficePhoneNumber = OfficePhoneNumber,
            TeamIds = TeamIds?.ToList(),
            Teams = [.. Teams.Select(t => (Team)t.Clone())],
            ValidationTeams = ValidationTeams,
            UserExists = UserExists
        };
    }
}
