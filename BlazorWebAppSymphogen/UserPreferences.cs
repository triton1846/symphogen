using BlazorWebAppSymphogen.Services;

namespace BlazorWebAppSymphogen;

public class UserPreferences : IUserPreferences
{
    private readonly ILocalStorageService _localStorage;
    //private const string UseCacheDataKey = "app_useCacheData";
    private const string MimerEnvironmentKey = "app_mimerEnvironment";

    #region Test Data Keys
    private const string FetchUsersDelayKey = "app_fetchUsersDelay";
    private const string FetchTeamsDelayKey = "app_fetchTeamsDelay";
    private const string TestDataNumberOfUsersKey = "app_testDataNumberOfUsers";
    private const string TestDataCreateUnknownUsersAsTeamMembersKey = "app_testDataCreateUnknownUsersAsTeamMembers";
    private const string TestDataCreateDuplicateTeamMembershipsForUsersKey = "app_testDataCreateDuplicateTeamMembershipsForUsers";
    //private const string TestDataCreateUnknownSuperUsersAsTeamMembersKey = "app_testDataCreateUnknownSuperUsersAsTeamMembers";
    //private const string TestDataCreateDuplicateSuperUsersKey = "app_testDataCreateDuplicateSuperUsers";
    //private const string TestDataCreateUnknownTeamsKey = "app_testDataCreateUnknownTeams";
    //private const string TestDataCreateDuplicateTeamsKey = "app_testDataCreateDuplicateTeams";
    #endregion Test Data Keys

    private bool _isInitialized = false;
    //private bool _useCacheData = true; // Default to using cache
    private MimerEnvironment _mimerEnvironment = MimerEnvironment.TestData;

    #region Test Data preferences
    private TimeSpan _fetchUsersDelay = TimeSpan.FromSeconds(0);
    private TimeSpan _fetchTeamsDelay = TimeSpan.FromSeconds(0);
    private int _testDataNumberOfUsers = 100; // Default number of users for test data
    private bool _testDataCreateUnknownUsersAsTeamMembers = true;
    private bool _testDataCreateDuplicateTeamMembershipsForUsers = true;
    //private bool _testDataCreateUnknownSuperUsersAsTeamMembers = true;
    //private bool _testDataCreateDuplicateSuperUsers = true;
    //private bool _testDataCreateUnknownTeams = true;
    //private bool _testDataCreateDuplicateTeams = true;
    #endregion Test Data preferences

    public UserPreferences(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    //public bool UseCacheData
    //{
    //    get => _useCacheData;
    //    set
    //    {
    //        _useCacheData = value;
    //        _ = _localStorage.SetItemAsync(UseCacheDataKey, value);
    //    }
    //}

    public MimerEnvironment MimerEnvironment
    {
        get => _mimerEnvironment;
        set
        {
            _mimerEnvironment = value;
            _ = _localStorage.SetItemAsync(MimerEnvironmentKey, value.ToString());
        }
    }

    #region Test Data preferences
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

    public int TestDataNumberOfUsers
    {
        get => _testDataNumberOfUsers;
        set
        {
            _testDataNumberOfUsers = value;
            _ = _localStorage.SetItemAsync(TestDataNumberOfUsersKey, value);
        }
    }

    public bool TestDataCreateUnknownUsersAsTeamMembers
    {
        get => _testDataCreateUnknownUsersAsTeamMembers;
        set
        {
            _testDataCreateUnknownUsersAsTeamMembers = value;
            _ = _localStorage.SetItemAsync(TestDataCreateUnknownUsersAsTeamMembersKey, value);
        }
    }

    public bool TestDataCreateDuplicateTeamMembershipsForUsers
    {
        get => _testDataCreateDuplicateTeamMembershipsForUsers;
        set
        {
            _testDataCreateDuplicateTeamMembershipsForUsers = value;
            _ = _localStorage.SetItemAsync(TestDataCreateDuplicateTeamMembershipsForUsersKey, value);
        }
    }

    //public bool TestDataCreateUnknownSuperUsersAsTeamMembers
    //{
    //    get => _testDataCreateUnknownSuperUsersAsTeamMembers;
    //    set
    //    {
    //        _testDataCreateUnknownSuperUsersAsTeamMembers = value;
    //        _ = _localStorage.SetItemAsync(TestDataCreateUnknownSuperUsersAsTeamMembersKey, value);
    //    }
    //}

    //public bool TestDataCreateDuplicateSuperUsers
    //{
    //    get => _testDataCreateDuplicateSuperUsers;
    //    set
    //    {
    //        _testDataCreateDuplicateSuperUsers = value;
    //        _ = _localStorage.SetItemAsync(TestDataCreateDuplicateSuperUsersKey, value);
    //    }
    //}

    //public bool TestDataCreateUnknownTeams
    //{
    //    get => _testDataCreateUnknownTeams;
    //    set
    //    {
    //        _testDataCreateUnknownTeams = value;
    //        _ = _localStorage.SetItemAsync(TestDataCreateUnknownTeamsKey, value);
    //    }
    //}

    //public bool TestDataCreateDuplicateTeams
    //{
    //    get => _testDataCreateDuplicateTeams;
    //    set
    //    {
    //        _testDataCreateDuplicateTeams = value;
    //        _ = _localStorage.SetItemAsync(TestDataCreateDuplicateTeamsKey, value);
    //    }
    //}
    #endregion Test Data preferences

    // Returns if the state is fully loaded from local storage
    public bool IsInitialized => _isInitialized;

    public async Task InitializeAsync()
    {
        //var useCacheData = await _localStorage.GetItemAsync<bool?>(UseCacheDataKey);
        //if (useCacheData.HasValue)
        //    _useCacheData = useCacheData.Value;

        var mimerEnvironmentString = await _localStorage.GetItemAsync<string?>("app_mimerEnvironment");
        if (!string.IsNullOrEmpty(mimerEnvironmentString) && Enum.TryParse<MimerEnvironment>(mimerEnvironmentString, out var environment))
            _mimerEnvironment = environment;

        #region Test Data preferences
        var fetchUsersDelay = await _localStorage.GetItemAsync<double?>(FetchUsersDelayKey);
        if (fetchUsersDelay.HasValue)
            _fetchUsersDelay = TimeSpan.FromMilliseconds(fetchUsersDelay.Value);

        var fetchTeamsDelay = await _localStorage.GetItemAsync<double?>(FetchTeamsDelayKey);
        if (fetchTeamsDelay.HasValue)
            _fetchTeamsDelay = TimeSpan.FromMilliseconds(fetchTeamsDelay.Value);

        var testDataNumberOfUsers = await _localStorage.GetItemAsync<int?>(TestDataNumberOfUsersKey);
        if (testDataNumberOfUsers.HasValue)
            _testDataNumberOfUsers = testDataNumberOfUsers.Value;

        var testDataCreateUnknownUsersAsTeamMembers = await _localStorage.GetItemAsync<bool?>(TestDataCreateUnknownUsersAsTeamMembersKey);
        if (testDataCreateUnknownUsersAsTeamMembers.HasValue)
            _testDataCreateUnknownUsersAsTeamMembers = testDataCreateUnknownUsersAsTeamMembers.Value;

        var testDataCreateDuplicateTeamMembershipsForUsers = await _localStorage.GetItemAsync<bool?>(TestDataCreateDuplicateTeamMembershipsForUsersKey);
        if (testDataCreateDuplicateTeamMembershipsForUsers.HasValue)
            _testDataCreateDuplicateTeamMembershipsForUsers = testDataCreateDuplicateTeamMembershipsForUsers.Value;

        //var testDataCreateUnknownSuperUsersAsTeamMembers = await _localStorage.GetItemAsync<bool?>(TestDataCreateUnknownSuperUsersAsTeamMembersKey);
        //if (testDataCreateUnknownSuperUsersAsTeamMembers.HasValue)
        //    _testDataCreateUnknownSuperUsersAsTeamMembers = testDataCreateUnknownSuperUsersAsTeamMembers.Value;

        //var testDataCreateDuplicateSuperUsers = await _localStorage.GetItemAsync<bool?>(TestDataCreateDuplicateSuperUsersKey);
        //if (testDataCreateDuplicateSuperUsers.HasValue)
        //    _testDataCreateDuplicateSuperUsers = testDataCreateDuplicateSuperUsers.Value;

        //var testDataCreateUnknownTeams = await _localStorage.GetItemAsync<bool?>(TestDataCreateUnknownTeamsKey);
        //if (testDataCreateUnknownTeams.HasValue)
        //    _testDataCreateUnknownTeams = testDataCreateUnknownTeams.Value;

        //var testDataCreateDuplicateTeams = await _localStorage.GetItemAsync<bool?>(TestDataCreateDuplicateTeamsKey);
        //if (testDataCreateDuplicateTeams.HasValue)
        //    _testDataCreateDuplicateTeams = testDataCreateDuplicateTeams.Value;
        #endregion Test Data preferences

        _isInitialized = true;
    }
}

public interface IUserPreferences
{
    bool IsInitialized { get; }
    Task InitializeAsync();
    //bool UseCacheData { get; set; }
    MimerEnvironment MimerEnvironment { get; set; }

    #region Test Data preferences
    TimeSpan FetchUsersDelay { get; set; }
    TimeSpan FetchTeamsDelay { get; set; }
    
    int TestDataNumberOfUsers { get; set; }
    bool TestDataCreateUnknownUsersAsTeamMembers { get; set; }
    bool TestDataCreateDuplicateTeamMembershipsForUsers { get; set; }
    //bool TestDataCreateUnknownSuperUsersAsTeamMembers { get; set; }
    //bool TestDataCreateDuplicateSuperUsers { get; set; }
    //bool TestDataCreateUnknownTeams { get; set; }
    //bool TestDataCreateDuplicateTeams { get; set; }
    #endregion Test Data preferences
}
