namespace BlazorWebAppSymphogen.Services.Interfaces;

public interface IUserInfoService
{
    Task<string?> GetDisplayNameAsync();
}