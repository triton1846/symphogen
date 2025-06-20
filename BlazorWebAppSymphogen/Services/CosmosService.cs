using BlazorWebAppSymphogen.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace BlazorWebAppSymphogen.Services;

public class CosmosService : ICosmosService
{
    private readonly ILogger<CosmosService> _logger;
    private readonly IUserPreferences _userPreferences;

    private Dictionary<MimerEnvironment, string> _connectionStrings = [];
    private readonly Dictionary<MimerEnvironment, CosmosClient> _clients = [];

    private Dictionary<MimerEnvironment, List<string>> _teamIds = [];
    private Dictionary<MimerEnvironment, List<Models.User>> _users = [];
    private Dictionary<MimerEnvironment, List<Team>> _teams = [];

    public CosmosService(ILogger<CosmosService> logger, IUserPreferences userPreferences, string connectionStringSb1, string connectionStringQa)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userPreferences = userPreferences;

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
        if (_userPreferences.MimerEnvironment != MimerEnvironment.TestData)
        {
            var users = await GetItemsAsync<Models.User>(mimerEnvironment, "users", "users_search", filterExpression);
            return [.. users];
        }

        if (_userPreferences.UseCacheData && _users.TryGetValue(mimerEnvironment, out _) && _users[mimerEnvironment].Any())
        {
            _logger.LogDebug("Returning cached users for {Environment}", mimerEnvironment);
            return [.. _users[mimerEnvironment]];
        }

        _users[mimerEnvironment] = await GetRandomUsers(mimerEnvironment, _userPreferences.TestDataNumberOfUsers, _userPreferences.FetchUsersDelay); // Simulate a delay for testing purposes

        // Make data errors to test UI error handling

        // Add a user with a duplicate team ID to simulate data inconsistency
        var userWithDuplicateTeamId = _users[mimerEnvironment].First(u => u.TeamIds?.Count() > 1);
        userWithDuplicateTeamId.TeamIds = userWithDuplicateTeamId.TeamIds!.Append(userWithDuplicateTeamId.TeamIds!.First());

        // Add a user with an unknown team ID to simulate data inconsistency
        var userWithUnknownTeamId = _users[mimerEnvironment].First();
        userWithUnknownTeamId.TeamIds = userWithUnknownTeamId.TeamIds?.Append("unknown-team-id");

        return _users[mimerEnvironment];
    }

    public async Task<List<Team>> GetTeamsAsync(
        MimerEnvironment mimerEnvironment,
        Func<IQueryable<Team>, IQueryable<Team>>? filterExpression = null)
    {
        if (_userPreferences.MimerEnvironment != MimerEnvironment.TestData)
        {
            var teams = await GetItemsAsync<Team>(mimerEnvironment, "users", "teams", filterExpression);
            return [.. teams];
        }

        if (_userPreferences.UseCacheData && _teams.TryGetValue(mimerEnvironment, out _) && _teams[mimerEnvironment].Any())
        {
            _logger.LogDebug("Returning cached teams for {Environment}", mimerEnvironment);
            return [.. _teams[mimerEnvironment]];
        }

        _teams[mimerEnvironment] = await GetRandomTeams(mimerEnvironment, _userPreferences.FetchTeamsDelay); // Simulate a delay for testing purposes

        return _teams[mimerEnvironment];
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

    private async Task<List<Models.User>> GetRandomUsers(MimerEnvironment mimerEnvironment, int numberOfUsers = 10, TimeSpan? delay = null)
    {
        if (delay.HasValue)
        {
            await Task.Delay(delay.Value);
        }
        var random = new Random();
        var randomUsers = new List<Models.User>();

        for (int i = 0; i < numberOfUsers; i++)
        {
            // Use Bogus to generate random user data
            var user = new Bogus.Faker<Models.User>()
            .RuleFor(u => u.Id, f => Guid.NewGuid().ToString())
            .RuleFor(u => u.FullName, f => f.Name.FullName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Department, f => f.Commerce.Department())
            .RuleFor(u => u.Location, f => f.Address.City())
            .RuleFor(u => u.Favorites, f => f.Make(3, () => f.Commerce.ProductName()))
            .RuleFor(u => u.Initials, f => $"{f.Name.FirstName()[0]}{f.Name.LastName()[0]}")
            .RuleFor(u => u.JobTitle, f => f.Name.JobTitle())
            .RuleFor(u => u.OfficePhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(u => u.TeamIds, f =>
            {
                return [.. GetTeamIds(mimerEnvironment).OrderBy(_ => random.Next()).Take(random.Next(1, 8))];
            }).Generate();

            randomUsers.Add(user);
        }

        return randomUsers;
    }

    private async Task<List<Team>> GetRandomTeams(MimerEnvironment mimerEnvironment, TimeSpan? delay = null)
    {
        if (delay.HasValue)
        {
            await Task.Delay(delay.Value);
        }
        var users = await GetRandomUsers(mimerEnvironment); // Generate some random users to assign to teams

        var random = new Random();
        var randomTeams = new List<Team>();
        foreach (var teamId in _teamIds[mimerEnvironment])
        {
            var team = new Bogus.Faker<Team>()
                .RuleFor(t => t.Id, f => teamId)
                .RuleFor(t => t.Name, f => f.Commerce.Department())
                .RuleFor(t => t.UserIds, f =>
                {
                    return users
                        .Where(u => u.TeamIds?.Contains(teamId) == true)
                        .Select(u => u.Id)
                        .ToList();
                }).Generate();

            if (team.UserIds.Any())
            {
                team.SuperUserIds = [.. team.UserIds.OrderBy(_ => random.Next()).Take(random.Next(1, team.UserIds.Count()))];
            }

            randomTeams.Add(team);
        }

        return randomTeams;
    }

    private List<string> GetTeamIds(MimerEnvironment mimerEnvironment, int numberOfTeamIds = 25)
    {
        if (!_teamIds.TryGetValue(mimerEnvironment, out _) || !_teamIds[mimerEnvironment].Any())
        {
            _teamIds[mimerEnvironment] = [.. Enumerable.Range(1, numberOfTeamIds).Select(i => Guid.NewGuid().ToString())];
        }

        return _teamIds[mimerEnvironment];
    }
}
