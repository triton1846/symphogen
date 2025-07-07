using BlazorWebAppSymphogen.Models;

namespace BlazorWebAppSymphogen.Services.Interfaces;

public interface IWorkflowConfigurationService
{
    Task<IEnumerable<WorkflowConfiguration>> GetAsync(
        MimerEnvironment mimerEnvironment,
        Func<IQueryable<WorkflowConfiguration>, IQueryable<WorkflowConfiguration>>? filterExpression = null);

    Task SaveAsync(
        MimerEnvironment mimerEnvironment,
        WorkflowConfiguration workflowConfiguration);

    Task DeleteAsync(
        MimerEnvironment mimerEnvironment,
        string teamId);
}
