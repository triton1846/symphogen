using System.Text.Json.Serialization;

namespace BlazorWebAppSymphogen.Models.DTOs;

public record UserDTO
{
    public required string Id { get; set; }

    public string? Email { get; set; }

    public string? FullName { get; set; }

    public string? Department { get; set; }

    public string? Location { get; set; }

    public IEnumerable<string> Favorites { get; set; } = [];

    public DateTime Created { get; set; }

    public DateTime Updated { get; set; }

    public bool Deleted { get; set; } = false;

    public string? Company { get; set; }

    public string? Initials { get; set; }

    public string? JobTitle { get; set; }

    public string? Manager { get; set; }

    [JsonPropertyName("officephone")]
    public string? OfficePhoneNumber { get; set; }

    public string? OfficeLocation { get; set; }

    [JsonIgnore]
    public string? GroupKey => null;

    public IEnumerable<string> TeamIds { get; set; } = [];
}
