namespace BlazorWebAppSymphogen.Services.Interfaces;

public interface ICosmosService
{
    Task<IEnumerable<T>> GetAsync<T>(
        MimerEnvironment mimerEnvironment,
        string databaseId,
        string containerId,
        Func<IQueryable<T>, IQueryable<T>>? filterExpression = null);
}
