namespace BlazorWebAppSymphogen.Models;

[Serializable]
public class WorkflowConfiguration : ICloneable
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

    public object Clone()
    {
        return new WorkflowConfiguration
        {
            Id = Id,
            Name = Name,
            Department = Department,
            StudyTypes = [.. StudyTypes.Select(st => (StudyType)st.Clone())],
            WorkflowPropertyHeaders = [.. WorkflowPropertyHeaders],
            ParameterIdentifier = ParameterIdentifier,
            ParameterRowCount = ParameterRowCount,
            DatasourceConfigurationIds = [.. DatasourceConfigurationIds],
            IsActive = IsActive
        };
    }
}
