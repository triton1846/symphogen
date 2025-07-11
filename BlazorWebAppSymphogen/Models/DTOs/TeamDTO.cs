namespace BlazorWebAppSymphogen.Models.DTOs;

public record TeamDTO
{
    public required string Id { get; set; }

    public string? Name { get; set; }

    public IEnumerable<string> UserIds { get; set; } = [];

    public IEnumerable<string> SuperUserIds { get; set; } = [];

    public IEnumerable<string> WorkflowConfigurationIds { get; set; } = [];
}
