namespace BlazorWebAppSymphogen.Models;

public record WorkflowConfiguration : DTOs.WorkflowConfigurationDTO
{
    public bool WorkflowConfigurationExists { get; set; } = true;
}
