using BlazorWebAppSymphogen.Services.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace BlazorWebAppSymphogen.Services;

public class CosmosService : ICosmosService
{
    private readonly ILogger<CosmosService> _logger;

    private readonly Dictionary<MimerEnvironment, string> _connectionStrings = [];
    private readonly Dictionary<MimerEnvironment, CosmosClient> _clients = [];

    public CosmosService(
        ILogger<CosmosService> logger,
        string connectionStringSb1,
        string connectionStringQa)
    {
        _logger = logger;

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

    public async Task<IEnumerable<T>> GetAsync<T>(
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
