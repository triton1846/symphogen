using Newtonsoft.Json;

namespace BlazorWebAppSymphogen.Models;

public class User : IEntity
{
    [JsonProperty(PropertyName = "id")]
    public required string Id { get; set; }

    [JsonProperty(PropertyName = "email")]
    public string? Email { get; set; }

    [JsonProperty(PropertyName = "fullName")]
    public string? FullName { get; set; }

    [JsonProperty(PropertyName = "department")]
    public string? Department { get; set; }

    [JsonProperty(PropertyName = "location")]
    public string? Location { get; set; }

    [JsonProperty(PropertyName = "favorites")]
    public IEnumerable<string>? Favorites { get; set; }

    [JsonProperty(PropertyName = "initials")]
    public string? Initials { get; set; }

    [JsonProperty(PropertyName = "jobtitle")]
    public string? JobTitle { get; set; }

    [JsonProperty(PropertyName = "officephone")]
    public string? OfficePhoneNumber { get; set; }

    [JsonProperty(PropertyName = "teamIds")]
    public IEnumerable<string>? TeamIds { get; set; }

    public List<Team> Teams { get; set; } = [];

    [JsonIgnore]// This property is used for validation purposes only and should not be serialized
    public Team? ValidationTeams { get; set; }

    public bool UserExists { get; set; } = true;

    [JsonIgnore]
    public string? GroupKey => null;
}
