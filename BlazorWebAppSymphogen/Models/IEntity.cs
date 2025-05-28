namespace BlazorWebAppSymphogen.Models;

public interface IEntity
{
    string Id { get; set; }
    string? GroupKey { get; }
}
