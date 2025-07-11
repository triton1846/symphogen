using FluentValidation.Results;
using System.Text.Json.Serialization;

namespace BlazorWebAppSymphogen.Models;

public record User : DTOs.UserDTO
{
    private List<Team> _teams = [];
    public List<Team> Teams
    {
        get => _teams;
        set
        {
            _teams = value ?? [];
            TeamIds = _teams.Select(t => t.Id);
        }
    }
    [JsonIgnore]
    public Team? ValidationTeams { get; set; }
    public bool UserExists { get; set; } = true;
    [JsonIgnore]
    public ValidationResult? ValidationResult { get; set; }
}
