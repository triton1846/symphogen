using BlazorWebAppSymphogen.Models;

namespace BlazorWebAppSymphogen.Services.Interfaces;

public interface ITeamService
{
    Task<IEnumerable<Team>> GetAsync(
        MimerEnvironment mimerEnvironment,
        Func<IQueryable<Team>, IQueryable<Team>>? filterExpression = null);

    Task SaveAsync(
        MimerEnvironment mimerEnvironment,
        Team team);

    Task DeleteAsync(
        MimerEnvironment mimerEnvironment,
        string teamId);
}
