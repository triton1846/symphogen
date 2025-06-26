using BlazorWebAppSymphogen.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace BlazorWebAppSymphogen.Services;

public class CosmosService : ICosmosService
{
    private readonly ILogger<CosmosService> _logger;
    private readonly IUserPreferences _userPreferences;
    private readonly ITestDataService _testDataService;

    private Dictionary<MimerEnvironment, string> _connectionStrings = [];
    private readonly Dictionary<MimerEnvironment, CosmosClient> _clients = [];

    private readonly Dictionary<MimerEnvironment, List<Models.User>> _users = [];
    private readonly Dictionary<MimerEnvironment, List<Team>> _teams = [];

    public CosmosService(
        ILogger<CosmosService> logger,
        IUserPreferences userPreferences,
        string connectionStringSb1,
        string connectionStringQa,
        ITestDataService testDataService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userPreferences = userPreferences ?? throw new ArgumentNullException(nameof(userPreferences));
        _testDataService = testDataService ?? throw new ArgumentNullException(nameof(testDataService));

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

    public async Task<List<Models.User>> GetUsersAsync(
        MimerEnvironment mimerEnvironment,
        Func<IQueryable<Models.User>, IQueryable<Models.User>>? filterExpression = null)
    {
        if (_userPreferences.MimerEnvironment == MimerEnvironment.TestData)
        {
            _logger.LogDebug("Using test data for users in {Environment}", mimerEnvironment);
            var testDataUsers = await _testDataService.GetUsersAsync();
            return [.. testDataUsers];
        }

        if (_userPreferences.UseCacheData && _users.TryGetValue(mimerEnvironment, out _) && _users[mimerEnvironment].Any())
        {
            _logger.LogDebug("Returning cached users for {Environment}", mimerEnvironment);
            return [.. _users[mimerEnvironment]];
        }

        var users = await GetItemsAsync<Models.User>(mimerEnvironment, "users", "users_search", filterExpression);
        _users[mimerEnvironment] = users;
        return _users[mimerEnvironment];



        //_users[mimerEnvironment] = await GetRandomUsers(mimerEnvironment, _userPreferences.TestDataNumberOfUsers, _userPreferences.FetchUsersDelay); // Simulate a delay for testing purposes

        //// Make sure test data is consistent
        //foreach (var user in _users[mimerEnvironment])
        //{
        //    foreach (var team in user.Teams)
        //    {
        //        if (_teams[mimerEnvironment].All(t => t.Id != team.Id))
        //        {
        //            _teams[mimerEnvironment].Add(team);
        //        }
        //    }
        //}


        //// TODO: Use user preferences to determine if we should simulate data errors
        //// Make data errors to test UI error handling

        //// Add a user with a duplicate team ID to simulate data inconsistency
        //var userWithDuplicateTeamId = _users[mimerEnvironment].OrderBy(u => u.FullName).First(u => u.TeamIds?.Count() > 1);
        //userWithDuplicateTeamId.TeamIds = userWithDuplicateTeamId.TeamIds!.Append(userWithDuplicateTeamId.TeamIds!.First());

        //// Add a user with an unknown team ID to simulate data inconsistency
        //var userWithUnknownTeamId = _users[mimerEnvironment].OrderBy(u => u.FullName).First();
        //userWithUnknownTeamId.TeamIds = userWithUnknownTeamId.TeamIds?.Append("unknown-team-id");

        //return _users[mimerEnvironment];
    }

    public async Task<List<Team>> GetTeamsAsync(
        MimerEnvironment mimerEnvironment,
        Func<IQueryable<Team>, IQueryable<Team>>? filterExpression = null)
    {
        if (_userPreferences.MimerEnvironment == MimerEnvironment.TestData)
        {
            _logger.LogDebug("Using test data for teams in {Environment}", mimerEnvironment);
            var testDataTeams = await _testDataService.GetTeamsAsync();
            return [.. testDataTeams];
        }

        if (_userPreferences.UseCacheData && _teams.TryGetValue(mimerEnvironment, out _) && _teams[mimerEnvironment].Any())
        {
            _logger.LogDebug("Returning cached teams for {Environment}", mimerEnvironment);
            return [.. _teams[mimerEnvironment]];
        }

        var teams = await GetItemsAsync<Team>(mimerEnvironment, "users", "teams", filterExpression);
        _teams[mimerEnvironment] = teams;
        return _teams[mimerEnvironment];



        //_teams[mimerEnvironment] = await GetRandomTeams(mimerEnvironment, _userPreferences.FetchTeamsDelay); // Simulate a delay for testing purposes

        //// TODO: Use user preferences to determine if we should simulate data errors
        //// Add a user duplicate team ID to simulate data inconsistency
        //var teamWithDuplicateUserId = _teams[mimerEnvironment].OrderBy(t => t.Name).First(t => t.UserIds.Count() > 1);
        //teamWithDuplicateUserId.UserIds = teamWithDuplicateUserId.UserIds.Append(teamWithDuplicateUserId.UserIds.First()).ToList();
        //var teamWithDuplicateSuperUserId = _teams[mimerEnvironment].OrderBy(t => t.Name).First(t => t.SuperUserIds.Count() > 1);
        //teamWithDuplicateSuperUserId.SuperUserIds = teamWithDuplicateSuperUserId.SuperUserIds.Append(teamWithDuplicateSuperUserId.SuperUserIds.First()).ToList();

        //return _teams[mimerEnvironment];
    }

    public async Task<List<T>> GetItemsAsync<T>(
        MimerEnvironment mimerEnvironment,
        string databaseId,
        string containerId,
        Func<IQueryable<T>, IQueryable<T>>? filterExpression = null)
    {
        var cosmosClient = _clients[mimerEnvironment] ?? throw new ArgumentException($"Cosmos client for {mimerEnvironment} not found.");
        var container = cosmosClient.GetContainer(databaseId, containerId);
        var queryRequestOptions = new QueryRequestOptions
        {
            MaxItemCount = 50
        };

        var results = new List<T>();

        try
        {
            // Get the base queryable and apply the filter expression
            var cosmosLinqSerializerOptions = new CosmosLinqSerializerOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase };
            var queryable = container.GetItemLinqQueryable<T>(requestOptions: queryRequestOptions, linqSerializerOptions: cosmosLinqSerializerOptions);
            filterExpression ??= q => q;
            var filteredQueryable = filterExpression(queryable);
            var numberOfCalls = 0;

            using var iterator = filteredQueryable.ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange([.. response]);
                numberOfCalls++;
            }
            _logger.LogDebug("Query completed for {ContainerId} in {DatabaseId} with {NumberOfCalls} call(s)", containerId, databaseId, numberOfCalls);
        }
        catch (CosmosException ex)
        {
            _logger.LogError(ex, "An error occurred while querying Cosmos DB for {ContainerId} in {DatabaseId}", containerId, databaseId);
        }

        return results;
    }
}
