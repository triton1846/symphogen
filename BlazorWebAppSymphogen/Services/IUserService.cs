using BlazorWebAppSymphogen.Models;

namespace BlazorWebAppSymphogen.Services;

public interface IUserService
{
    Task<List<User>> GetUsersAsync(
        MimerEnvironment mimerEnvironment,
        Func<IQueryable<User>, IQueryable<User>>? filterExpression = null);

    Task SaveUserAsync(
        MimerEnvironment mimerEnvironment,
        User user);

    Task DeleteUserAsync(
        MimerEnvironment mimerEnvironment,
        string userId);
}
