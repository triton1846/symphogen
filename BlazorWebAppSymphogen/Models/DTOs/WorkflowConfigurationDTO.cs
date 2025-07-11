namespace BlazorWebAppSymphogen.Models.DTOs;

[Serializable]
public record WorkflowConfigurationDTO
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Department { get; set; }
    public IEnumerable<StudyType> StudyTypes { get; set; } = [];
    public IEnumerable<string> WorkflowPropertyHeaders { get; set; } = [];
    public string? ParameterIdentifier { get; set; }
    public int ParameterRowCount { get; set; }
    public IEnumerable<string> DatasourceConfigurationIds { get; set; } = [];
    public bool IsActive { get; set; } = true;
}
