namespace BlazorWebAppSymphogen;

// TODO: Per user session state management
public class AppState : IAppState
{
    public bool UseTestData { get; set; }
    public TimeSpan FetchUsersDelay { get; set; } = TimeSpan.FromSeconds(0);
    public TimeSpan FetchTeamsDelay { get; set; } = TimeSpan.FromSeconds(0);
}

public interface IAppState
{
    bool UseTestData { get; set; }
    TimeSpan FetchUsersDelay { get; set; }
    TimeSpan FetchTeamsDelay { get; set; }
}
