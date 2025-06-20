using BlazorWebAppSymphogen.Services;

namespace BlazorWebAppSymphogen;

public class UserPreferences : IUserPreferences
{
    private readonly ILocalStorageService _localStorage;
    private const string UseTestDataKey = "app_useTestData";
    private const string UseCacheDataKey = "app_useCacheData";
    private const string FetchUsersDelayKey = "app_fetchUsersDelay";
    private const string FetchTeamsDelayKey = "app_fetchTeamsDelay";
    private const string TestDataNumberOfUsersKey = "app_testDataNumberOfUsers";

    private bool _useTestData;
    private bool _useCacheData = true; // Default to using cache
    private TimeSpan _fetchUsersDelay = TimeSpan.FromSeconds(0);
    private TimeSpan _fetchTeamsDelay = TimeSpan.FromSeconds(0);
    private bool _isInitialized = false;
    private MimerEnvironment _mimerEnvironment = MimerEnvironment.SB1;
    private int _testDataNumberOfUsers = 100; // Default number of users for test data

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

    public bool UseCacheData
    {
        get => _useCacheData;
        set
        {
            _useCacheData = value;
            _ = _localStorage.SetItemAsync(UseCacheDataKey, value);
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

    public MimerEnvironment MimerEnvironment
    {
        get => _mimerEnvironment;
        set
        {
            _mimerEnvironment = value;
            _ = _localStorage.SetItemAsync("app_mimerEnvironment", value.ToString());
        }
    }

    public int TestDataNumberOfUsers
    {
        get => _testDataNumberOfUsers;
        set
        {
            _testDataNumberOfUsers = value;
            _ = _localStorage.SetItemAsync(TestDataNumberOfUsersKey, value);
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

        var useCacheData = await _localStorage.GetItemAsync<bool?>(UseCacheDataKey);
        if (useCacheData.HasValue)
            _useCacheData = useCacheData.Value;

        var fetchUsersDelay = await _localStorage.GetItemAsync<double?>(FetchUsersDelayKey);
        if (fetchUsersDelay.HasValue)
            _fetchUsersDelay = TimeSpan.FromMilliseconds(fetchUsersDelay.Value);

        var fetchTeamsDelay = await _localStorage.GetItemAsync<double?>(FetchTeamsDelayKey);
        if (fetchTeamsDelay.HasValue)
            _fetchTeamsDelay = TimeSpan.FromMilliseconds(fetchTeamsDelay.Value);

        var mimerEnvironmentString = await _localStorage.GetItemAsync<string?>("app_mimerEnvironment");
        if (!string.IsNullOrEmpty(mimerEnvironmentString) && Enum.TryParse<MimerEnvironment>(mimerEnvironmentString, out var environment))
            _mimerEnvironment = environment;

        var testDataNumberOfUsers = await _localStorage.GetItemAsync<int?>(TestDataNumberOfUsersKey);
        if (testDataNumberOfUsers.HasValue)
            _testDataNumberOfUsers = testDataNumberOfUsers.Value;

        _isInitialized = true;
    }
}

public interface IUserPreferences
{
    bool IsInitialized { get; }
    Task InitializeAsync();
    bool UseTestData { get; set; }
    bool UseCacheData { get; set; }
    TimeSpan FetchUsersDelay { get; set; }
    TimeSpan FetchTeamsDelay { get; set; }
    MimerEnvironment MimerEnvironment { get; set; }
    int TestDataNumberOfUsers { get; set; }
}
