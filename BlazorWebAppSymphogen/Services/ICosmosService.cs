using BlazorWebAppSymphogen.Models;

namespace BlazorWebAppSymphogen.Services;

public interface ICosmosService
{
    Task<List<User>> GetUsersAsync(
        MimerEnvironment mimerEnvironment, 
        Func<IQueryable<User>, IQueryable<User>>? filterExpression = null);

    Task SaveUserAsync(
        MimerEnvironment mimerEnvironment, 
        User user);

    Task DeleteUserAsync(
        string userId, 
        MimerEnvironment mimerEnvironment);

    Task<List<Team>> GetTeamsAsync(
        MimerEnvironment mimerEnvironment, 
        Func<IQueryable<Team>, IQueryable<Team>>? filterExpression = null);

    Task<List<WorkflowConfiguration>> GetWorkflowConfigurationsAsync(
        MimerEnvironment mimerEnvironment, 
        Func<IQueryable<WorkflowConfiguration>, IQueryable<WorkflowConfiguration>>? filterExpression = null);

    Task<List<T>> GetItemsAsync<T>(
        MimerEnvironment mimerEnvironment,
        string databaseId,
        string containerId,
        Func<IQueryable<T>, IQueryable<T>>? filterExpression = null);
}
