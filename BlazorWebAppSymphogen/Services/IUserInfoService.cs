namespace BlazorWebAppSymphogen.Services;

public interface IUserInfoService
{
    Task<string?> GetDisplayNameAsync();
}