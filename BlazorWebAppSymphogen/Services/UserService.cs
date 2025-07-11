using BlazorWebAppSymphogen.Services.Interfaces;
using BlazorWebAppSymphogen.Settings;

namespace BlazorWebAppSymphogen.Services;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IUserPreferences _userPreferences;
    private readonly ICosmosService _cosmosService;
    private readonly ITestDataService _testDataService;

    private readonly Dictionary<MimerEnvironment, List<Models.User>> _users = [];

    public UserService(
        ILogger<UserService> logger,
        IUserPreferences userPreferences,
        ICosmosService cosmosService,
        ITestDataService testDataService)
    {
        _logger = logger;
        _userPreferences = userPreferences;
        _cosmosService = cosmosService;
        _testDataService = testDataService;
    }

    public async Task<IEnumerable<Models.User>> GetAsync(MimerEnvironment mimerEnvironment, Func<IQueryable<Models.User>, IQueryable<Models.User>>? filterExpression = null)
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

        var users = await _cosmosService.GetAsync<Models.User>(mimerEnvironment, "users", "users_search", filterExpression);
        _users[mimerEnvironment] = [.. users];

        return _users[mimerEnvironment];
    }

    public async Task SaveAsync(MimerEnvironment mimerEnvironment, Models.User user)
    {
        if (_userPreferences.MimerEnvironment == MimerEnvironment.TestData)
        {
            await _testDataService.SaveUserAsync(user);
            return;
        }

        //Models.DTOs.UserDTO d2 = user; // This ensures that only the properties defined in UserDTO are serialized, not the entire User object
        // For now using SimpleAPI
        //var httpClient = new HttpClient();
        //httpClient.BaseAddress = new Uri("https://localhost:7133/");
        //httpClient.DefaultRequestHeaders.Add("X-API-KEY", "1234");
        //var response = await httpClient.PostAsJsonAsync("weatherforecast/?key=1234", d2);

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

    public async Task DeleteAsync(MimerEnvironment mimerEnvironment, string userId)
    {
        if (_userPreferences.MimerEnvironment == MimerEnvironment.TestData)
        {
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
