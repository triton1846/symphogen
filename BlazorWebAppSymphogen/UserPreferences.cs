using BlazorWebAppSymphogen.Services;

namespace BlazorWebAppSymphogen;

// TODO: Per user session state management
public class UserPreferences : IUserPreferences
{
    private readonly ILocalStorageService _localStorage;
    private const string UseTestDataKey = "app_useTestData";
    private const string FetchUsersDelayKey = "app_fetchUsersDelay";
    private const string FetchTeamsDelayKey = "app_fetchTeamsDelay";

    private bool _useTestData;
    private TimeSpan _fetchUsersDelay = TimeSpan.FromSeconds(0);
    private TimeSpan _fetchTeamsDelay = TimeSpan.FromSeconds(0);
    private bool _isInitialized = false;

    public UserPreferences(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public bool UseTestData
    {
        get => _useTestData;
        set
        {
            _useTestData = value;
            _ = _localStorage.SetItemAsync(UseTestDataKey, value);
        }
    }

    public TimeSpan FetchUsersDelay
    {
        get => _fetchUsersDelay;
        set
        {
            _fetchUsersDelay = value;
            _ = _localStorage.SetItemAsync(FetchUsersDelayKey, value.TotalMilliseconds);
        }
    }

    public TimeSpan FetchTeamsDelay
    {
        get => _fetchTeamsDelay;
        set
        {
            _fetchTeamsDelay = value;
            _ = _localStorage.SetItemAsync(FetchTeamsDelayKey, value.TotalMilliseconds);
        }
    }

    // Returns if the state is fully loaded from local storage
    public bool IsInitialized => _isInitialized;

    public async Task InitializeAsync()
    {
        // Load values from local storage
        var useTestData = await _localStorage.GetItemAsync<bool?>(UseTestDataKey);
        if (useTestData.HasValue)
            _useTestData = useTestData.Value;

        var fetchUsersDelay = await _localStorage.GetItemAsync<double?>(FetchUsersDelayKey);
        if (fetchUsersDelay.HasValue)
            _fetchUsersDelay = TimeSpan.FromMilliseconds(fetchUsersDelay.Value);

        var fetchTeamsDelay = await _localStorage.GetItemAsync<double?>(FetchTeamsDelayKey);
        if (fetchTeamsDelay.HasValue)
            _fetchTeamsDelay = TimeSpan.FromMilliseconds(fetchTeamsDelay.Value);

        _isInitialized = true;
    }
}

public interface IUserPreferences
{
    bool IsInitialized { get; }
    Task InitializeAsync();
    bool UseTestData { get; set; }
    TimeSpan FetchUsersDelay { get; set; }
    TimeSpan FetchTeamsDelay { get; set; }
}
