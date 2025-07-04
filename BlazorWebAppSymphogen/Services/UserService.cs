using BlazorWebAppSymphogen.Models;
using BlazorWebAppSymphogen.Settings;
using Microsoft.Azure.Cosmos;

namespace BlazorWebAppSymphogen.Services;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IUserPreferences _userPreferences;
    private readonly ICosmosService _cosmosService;
    private readonly ITestDataService _testDataService;

    private readonly Dictionary<MimerEnvironment, string> _connectionStrings = [];
    private readonly Dictionary<MimerEnvironment, CosmosClient> _clients = [];

    private readonly Dictionary<MimerEnvironment, List<Models.User>> _users = [];
    private readonly Dictionary<MimerEnvironment, List<Team>> _teams = [];
    private readonly Dictionary<MimerEnvironment, List<WorkflowConfiguration>> _workflowConfigurations = [];

    public UserService(
        ILogger<UserService> logger,
        IUserPreferences userPreferences,
        ICosmosService cosmosService,
        ITestDataService testDataService,
        string connectionStringSb1,
        string connectionStringQa)
    {
        _logger = logger;
        _userPreferences = userPreferences;
        _cosmosService = cosmosService;
        _testDataService = testDataService;

        _connectionStrings.Add(MimerEnvironment.SB1, connectionStringSb1);
        _connectionStrings.Add(MimerEnvironment.QA, connectionStringQa);

        var cosmosOptions = new CosmosClientOptions
        {
            ConnectionMode = ConnectionMode.Gateway,
        };

        foreach (var kvp in _connectionStrings)
        {
            _logger.LogDebug("Creating Cosmos client for {Environment}", kvp.Key);
            _clients[kvp.Key] = new CosmosClient(kvp.Value, cosmosOptions);
        }
    }    

    public async Task<List<Models.User>> GetUsersAsync(MimerEnvironment mimerEnvironment, Func<IQueryable<Models.User>, IQueryable<Models.User>>? filterExpression = null)
    {
        if (_userPreferences.MimerEnvironment == MimerEnvironment.TestData)
        {
            _logger.LogDebug("Using test data for users in {Environment}", mimerEnvironment);
            var testDataUsers = await _testDataService.GetUsersAsync();
            return [.. testDataUsers];
        }

        if (_users.TryGetValue(mimerEnvironment, out _) && _users[mimerEnvironment].Count != 0)
        {
            _logger.LogDebug("Returning cached users for {Environment}", mimerEnvironment);
            return [.. _users[mimerEnvironment]];
        }

        var users = await _cosmosService.GetItemsAsync<Models.User>(mimerEnvironment, "users", "users_search", filterExpression);
        _users[mimerEnvironment] = users;

        return _users[mimerEnvironment];
    }

    public async Task SaveUserAsync(MimerEnvironment mimerEnvironment, Models.User user)
    {
        if (_userPreferences.MimerEnvironment == MimerEnvironment.TestData)
        {
            _logger.LogDebug("Saving user in test data environment {Environment}", mimerEnvironment);
            await _testDataService.SaveUserAsync(user);
            return;
        }
        //var cosmosClient = _clients[mimerEnvironment] ?? throw new ArgumentException($"Cosmos client for {mimerEnvironment} not found.");
        //var container = cosmosClient.GetContainer("users", "users_search");
        //try
        //{
        //    var response = await container.UpsertItemAsync(user, new PartitionKey(user.Id));
        //    _logger.LogDebug("User {UserId} saved successfully in {ContainerId} of {DatabaseId}", user.Id, container.Id, container.Database.Id);
        //}
        //catch (CosmosException ex)
        //{
        //    _logger.LogError(ex, "An error occurred while saving user {UserId} in {ContainerId} of {DatabaseId}", user.Id, container.Id, container.Database.Id);
        //    throw;
        //}

        // For now just update or add the user to the cache
        if (_users.TryGetValue(mimerEnvironment, out var users))
        {
            var existingUser = users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser != null)
            {
                _logger.LogDebug("Updating existing user {UserId} in cache for {Environment}", user.Id, mimerEnvironment);
                users.Remove(existingUser);
            }
            else
            {
                _logger.LogDebug("Adding new user {UserId} to cache for {Environment}", user.Id, mimerEnvironment);
            }
            users.Add(user);
        }
        else
        {
            _logger.LogDebug("Creating new user list for {Environment} and adding user {UserId}", mimerEnvironment, user.Id);
            _users[mimerEnvironment] = [user];
        }
    }

    public async Task DeleteUserAsync(MimerEnvironment mimerEnvironment, string userId)
    {
        if (_userPreferences.MimerEnvironment == MimerEnvironment.TestData)
        {
            _logger.LogDebug("Deleting user {UserId} in test data environment {Environment}", userId, mimerEnvironment);
            await _testDataService.DeleteUserAsync(userId);
            return;
        }
        //var cosmosClient = _clients[mimerEnvironment] ?? throw new ArgumentException($"Cosmos client for {mimerEnvironment} not found.");
        //var container = cosmosClient.GetContainer("users", "users_search");
        //try
        //{
        //    await container.DeleteItemAsync<Models.User>(userId, new PartitionKey(userId));
        //    _logger.LogDebug("User {UserId} deleted successfully from {ContainerId} of {DatabaseId}", userId, container.Id, container.Database.Id);
        //}
        //catch (CosmosException ex)
        //{
        //    _logger.LogError(ex, "An error occurred while deleting user {UserId} from {ContainerId} of {DatabaseId}", userId, container.Id, container.Database.Id);
        //    throw;
        //}
        // For now just remove the user from the cache
        if (_users.TryGetValue(mimerEnvironment, out var users))
        {
            var existingUser = users.FirstOrDefault(u => u.Id == userId);
            if (existingUser != null)
            {
                _logger.LogDebug("Removing user {UserId} from cache for {Environment}", userId, mimerEnvironment);
                users.Remove(existingUser);
            }
            else
            {
                _logger.LogWarning("User {UserId} not found in cache for {Environment}", userId, mimerEnvironment);
            }
        }
    }
}
