namespace Tests;

public class DefaultData
{
    public IEnumerable<BlazorWebAppSymphogen.Models.User> UsersSB1 { get; set; } = [];
    public IEnumerable<BlazorWebAppSymphogen.Models.User> UsersQA { get; set; } = [];

    public IEnumerable<BlazorWebAppSymphogen.Models.Team> TeamsSB1 { get; set; } = [];
    public IEnumerable<BlazorWebAppSymphogen.Models.Team> TeamsQA { get; set; } = [];
}
