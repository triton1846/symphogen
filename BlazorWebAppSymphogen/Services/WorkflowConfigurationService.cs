using BlazorWebAppSymphogen.Models;
using BlazorWebAppSymphogen.Services.Interfaces;
using BlazorWebAppSymphogen.Settings;

namespace BlazorWebAppSymphogen.Services;

public class WorkflowConfigurationService : IWorkflowConfigurationService
{
    private readonly ILogger<WorkflowConfigurationService> _logger;
    private readonly IUserPreferences _userPreferences;
    private readonly ICosmosService _cosmosService;
    private readonly ITestDataService _testDataService;

    private readonly Dictionary<MimerEnvironment, List<WorkflowConfiguration>> _workflowConfigurations = [];

    public WorkflowConfigurationService(
        ILogger<WorkflowConfigurationService> logger,
        IUserPreferences userPreferences,
        ICosmosService cosmosService,
        ITestDataService testDataService)
    {
        _logger = logger;
        _userPreferences = userPreferences;
        _cosmosService = cosmosService;
        _testDataService = testDataService;
    }

    public async Task<IEnumerable<WorkflowConfiguration>> GetAsync(
        MimerEnvironment mimerEnvironment,
        Func<IQueryable<WorkflowConfiguration>, IQueryable<WorkflowConfiguration>>? filterExpression = null)
    {
        if (_userPreferences.MimerEnvironment == MimerEnvironment.TestData)
        {
            _logger.LogDebug("Using test data for workflow configurations in {Environment}", mimerEnvironment);
            var testDataWorkflowConfigurations = await _testDataService.GetWorkflowConfigurationsAsync();
            return [.. testDataWorkflowConfigurations];
        }

        if (_workflowConfigurations.TryGetValue(mimerEnvironment, out _) && _workflowConfigurations[mimerEnvironment].Count != 0)
        {
            _logger.LogDebug("Returning cached workflow configurations for {Environment}", mimerEnvironment);
            return [.. _workflowConfigurations[mimerEnvironment]];
        }

        var workflowConfigurations = await _cosmosService.GetAsync<WorkflowConfiguration>(mimerEnvironment, "workflows", "workflowConfigurations", filterExpression);
        _workflowConfigurations[mimerEnvironment] = [.. workflowConfigurations];

        return _workflowConfigurations[mimerEnvironment];
    }

    public async Task SaveAsync(MimerEnvironment mimerEnvironment, WorkflowConfiguration workflowConfiguration)
    {
        if (_userPreferences.MimerEnvironment == MimerEnvironment.TestData)
        {
            await _testDataService.SaveWorkflowConfigurationAsync(workflowConfiguration);
            return;
        }

        // Call web API or Cosmos to save
        // For now just update or add the workflow configuration to the cache
        if (_workflowConfigurations.TryGetValue(mimerEnvironment, out var workflowConfigurations))
        {
            var existingWorkflowConfiguration = workflowConfigurations.FirstOrDefault(u => u.Id == workflowConfiguration.Id);
            if (existingWorkflowConfiguration != null)
            {
                _logger.LogDebug("Updating existing WorkflowConfiguration {WorkflowConfigurationId} in cache for {Environment}", workflowConfiguration.Id, mimerEnvironment);
                workflowConfigurations.Remove(existingWorkflowConfiguration);
            }
            else
            {
                _logger.LogDebug("Updating existing WorkflowConfiguration {WorkflowConfigurationId} in cache for {Environment}", workflowConfiguration.Id, mimerEnvironment);
            }
            workflowConfigurations.Add(workflowConfiguration);
        }
        else
        {
            _logger.LogDebug("Creating new WorkflowConfiguration list for {Environment} and adding WorkflowConfiguration {WorkflowConfigurationId}", mimerEnvironment, workflowConfiguration.Id);
            _workflowConfigurations[mimerEnvironment] = [workflowConfiguration];
        }
    }

    public async Task DeleteAsync(MimerEnvironment mimerEnvironment, string workflowConfigurationId)
    {
        if (_userPreferences.MimerEnvironment == MimerEnvironment.TestData)
        {
            await _testDataService.DeleteWorkflowConfigurationAsync(workflowConfigurationId);
            return;
        }

        // Call web API or Cosmos to delete
        // For now just remove the WorkflowConfiguration from the cache
        if (_workflowConfigurations.TryGetValue(mimerEnvironment, out var workflowConfigurations))
        {
            var existingWorkflowConfiguration = workflowConfigurations.FirstOrDefault(u => u.Id == workflowConfigurationId);
            if (existingWorkflowConfiguration != null)
            {
                _logger.LogDebug("Removing WorkflowConfiguration {WorkflowConfigurationId} from cache for {Environment}", workflowConfigurationId, mimerEnvironment);
                workflowConfigurations.Remove(existingWorkflowConfiguration);
            }
            else
            {
                _logger.LogWarning("Removing WorkflowConfiguration {WorkflowConfigurationId} from cache for {Environment}", workflowConfigurationId, mimerEnvironment);
            }
        }
    }
}
