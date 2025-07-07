using BlazorWebAppSymphogen.Models;

namespace BlazorWebAppSymphogen.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> GetAsync(
        MimerEnvironment mimerEnvironment,
        Func<IQueryable<User>, IQueryable<User>>? filterExpression = null);

    Task SaveAsync(
        MimerEnvironment mimerEnvironment,
        User user);

    Task DeleteAsync(
        MimerEnvironment mimerEnvironment,
        string userId);
}
