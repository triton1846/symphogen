using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace BlazorWebAppSymphogen.Settings;

public class UserPreferences(
    ILogger<UserPreferences> logger, 
    ProtectedLocalStorage protectedLocalStorage) : IUserPreferences
{
    private bool _isInitialized = false;

    [StorageKey(StorageKeys.MimerEnvironment)]
    public MimerEnvironment MimerEnvironment { get; set; } = MimerEnvironment.TestData;

    [StorageKey(StorageKeys.RemoveInvalidDataAutomatically)]
    public bool RemoveInvalidDataAutomatically { get; set; } = false;

    #region Test Data preferences

    [StorageKey(StorageKeys.FetchUsersDelay)]
    public TimeSpan FetchUsersDelay { get; set; } = TimeSpan.Zero;

    [StorageKey(StorageKeys.FetchTeamsDelay)]
    public TimeSpan FetchTeamsDelay { get; set; } = TimeSpan.Zero;

    [StorageKey(StorageKeys.FetchWorkflowConfigurationsDelay)]
    public TimeSpan FetchWorkflowConfigurationsDelay { get; set; } = TimeSpan.Zero;

    [StorageKey(StorageKeys.TestData.NumberOfUsers)]
    public int TestDataNumberOfUsers { get; set; } = 100;

    [StorageKey(StorageKeys.TestData.CreateUnknownUsersAsTeamMembers)]
    public bool TestDataCreateUnknownUsersAsTeamMembers { get; set; } = true;

    [StorageKey(StorageKeys.TestData.CreateDuplicateTeamMembershipsForUsers)]
    public bool TestDataCreateDuplicateTeamMembershipsForUsers { get; set; } = true;

    [StorageKey(StorageKeys.TestData.CreateUnknownSuperUsersAsTeamMembers)]
    public bool TestDataCreateUnknownSuperUsersAsTeamMembers { get; set; } = true;

    [StorageKey(StorageKeys.TestData.CreateDuplicateTeamMembershipsForSuperUsers)]
    public bool TestDataCreateDuplicateTeamMembershipsForSuperUsers { get; set; } = true;

    [StorageKey(StorageKeys.TestData.CreateUnknownTeams)]
    public bool TestDataCreateUnknownTeams { get; set; } = true;

    [StorageKey(StorageKeys.TestData.CreateDuplicateTeams)]
    public bool TestDataCreateDuplicateTeams { get; set; } = true;

    #endregion Test Data preferences

    // Returns if the state is fully loaded from local storage
    public bool IsInitialized => _isInitialized;

    public async Task InitializeAsync()
    {
        if (_isInitialized)
            return;

        var mimerEnvironmentResult = await protectedLocalStorage.GetAsync<MimerEnvironment>(StorageKeys.MimerEnvironment);
        if (mimerEnvironmentResult.Success)
            MimerEnvironment = mimerEnvironmentResult.Value;

        var removeInvalidDataResult = await protectedLocalStorage.GetAsync<bool>(StorageKeys.RemoveInvalidDataAutomatically);
        if (removeInvalidDataResult.Success)
            RemoveInvalidDataAutomatically = removeInvalidDataResult.Value;

        // Load delay settings
        var fetchUsersDelayResult = await protectedLocalStorage.GetAsync<TimeSpan>(StorageKeys.FetchUsersDelay);
        if (fetchUsersDelayResult.Success)
            FetchUsersDelay = fetchUsersDelayResult.Value;

        var fetchTeamsDelayResult = await protectedLocalStorage.GetAsync<TimeSpan>(StorageKeys.FetchTeamsDelay);
        if (fetchTeamsDelayResult.Success)
            FetchTeamsDelay = fetchTeamsDelayResult.Value;

        var fetchWorkflowConfigurationsDelayResult = await protectedLocalStorage.GetAsync<TimeSpan>(StorageKeys.FetchWorkflowConfigurationsDelay);
        if (fetchWorkflowConfigurationsDelayResult.Success)
            FetchWorkflowConfigurationsDelay = fetchWorkflowConfigurationsDelayResult.Value;

        // Load test data settings
        var testDataNumberOfUsersResult = await protectedLocalStorage.GetAsync<int>(StorageKeys.TestData.NumberOfUsers);
        if (testDataNumberOfUsersResult.Success)
            TestDataNumberOfUsers = testDataNumberOfUsersResult.Value;

        var testDataCreateUnknownUsersAsTeamMembersResult = await protectedLocalStorage.GetAsync<bool>(StorageKeys.TestData.CreateUnknownUsersAsTeamMembers);
        if (testDataCreateUnknownUsersAsTeamMembersResult.Success)
            TestDataCreateUnknownUsersAsTeamMembers = testDataCreateUnknownUsersAsTeamMembersResult.Value;

        var testDataCreateDuplicateTeamMembershipsForUsersResult = await protectedLocalStorage.GetAsync<bool>(StorageKeys.TestData.CreateDuplicateTeamMembershipsForUsers);
        if (testDataCreateDuplicateTeamMembershipsForUsersResult.Success)
            TestDataCreateDuplicateTeamMembershipsForUsers = testDataCreateDuplicateTeamMembershipsForUsersResult.Value;

        var testDataCreateUnknownSuperUsersAsTeamMembersResult = await protectedLocalStorage.GetAsync<bool>(StorageKeys.TestData.CreateUnknownSuperUsersAsTeamMembers);
        if (testDataCreateUnknownSuperUsersAsTeamMembersResult.Success)
            TestDataCreateUnknownSuperUsersAsTeamMembers = testDataCreateUnknownSuperUsersAsTeamMembersResult.Value;

        var testDataCreateDuplicateTeamMembershipsForSuperUsersResult = await protectedLocalStorage.GetAsync<bool>(StorageKeys.TestData.CreateDuplicateTeamMembershipsForSuperUsers);
        if (testDataCreateDuplicateTeamMembershipsForSuperUsersResult.Success)
            TestDataCreateDuplicateTeamMembershipsForSuperUsers = testDataCreateDuplicateTeamMembershipsForSuperUsersResult.Value;

        var testDataCreateUnknownTeamsResult = await protectedLocalStorage.GetAsync<bool>(StorageKeys.TestData.CreateUnknownTeams);
        if (testDataCreateUnknownTeamsResult.Success)
            TestDataCreateUnknownTeams = testDataCreateUnknownTeamsResult.Value;

        var testDataCreateDuplicateTeamsResult = await protectedLocalStorage.GetAsync<bool>(StorageKeys.TestData.CreateDuplicateTeams);
        if (testDataCreateDuplicateTeamsResult.Success)
            TestDataCreateDuplicateTeams = testDataCreateDuplicateTeamsResult.Value;

        _isInitialized = true;
    }

    public async Task SetAsync<TValue>(string propertyName, TValue value)
    {
        var property = GetType().GetProperty(propertyName);
        if (property != null)
        {
            GetType().GetProperty(propertyName)?.SetValue(this, value);
            if (property.GetCustomAttributes(typeof(StorageKeyAttribute), false).SingleOrDefault() is StorageKeyAttribute storageKeyAttribute)
            {
                await protectedLocalStorage.SetAsync(storageKeyAttribute.Key, value!);
            }
            else
            {
                logger.LogWarning("Property '{PropertyName}' does not have a StorageKeyAttribute defined. Value will not be saved to local storage.", propertyName);
            }
        }
        else
        {
            throw new ArgumentException($"Property '{propertyName}' not found on {nameof(UserPreferences)}.");
        }
    }
}
