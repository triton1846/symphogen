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

    [StorageKey(StorageKeys.Testing.User.Delay.Get)]
    public TimeSpan GetUsersDelay { get; set; } = TimeSpan.Zero;

    [StorageKey(StorageKeys.Testing.User.Delay.Save)]
    public TimeSpan SaveUserDelay { get; set; } = TimeSpan.Zero;

    [StorageKey(StorageKeys.Testing.User.Delay.Delete)]
    public TimeSpan DeleteUserDelay { get; set; } = TimeSpan.Zero;

    [StorageKey(StorageKeys.Testing.Team.Delay.Get)]
    public TimeSpan GetTeamsDelay { get; set; } = TimeSpan.Zero;

    [StorageKey(StorageKeys.Testing.Team.Delay.Save)]
    public TimeSpan SaveTeamDelay { get; set; } = TimeSpan.Zero;

    [StorageKey(StorageKeys.Testing.Team.Delay.Delete)]
    public TimeSpan DeleteTeamDelay { get; set; } = TimeSpan.Zero;

    [StorageKey(StorageKeys.Testing.WorkflowConfiguration.Delay.Get)]
    public TimeSpan GetWorkflowConfigurationsDelay { get; set; } = TimeSpan.Zero;

    [StorageKey(StorageKeys.Testing.WorkflowConfiguration.Delay.Save)]
    public TimeSpan SaveWorkflowConfigurationDelay { get; set; } = TimeSpan.Zero;

    [StorageKey(StorageKeys.Testing.WorkflowConfiguration.Delay.Delete)]
    public TimeSpan DeleteWorkflowConfigurationDelay { get; set; } = TimeSpan.Zero;

    [StorageKey(StorageKeys.Testing.User.NumberOfUsers)]
    public int TestDataNumberOfUsers { get; set; } = 100;

    [StorageKey(StorageKeys.Testing.Team.NumberOfTeams)]
    public int TestDataNumberOfTeams { get; set; } = 10;

    [StorageKey(StorageKeys.Testing.User.Unknown.Users)]
    public bool TestDataCreateUnknownUsersAsTeamMembers { get; set; } = true;

    [StorageKey(StorageKeys.Testing.User.Duplicate.Users)]
    public bool TestDataCreateDuplicateTeamMembershipsForUsers { get; set; } = true;

    [StorageKey(StorageKeys.Testing.User.Unknown.SuperUsers)]
    public bool TestDataCreateUnknownSuperUsersAsTeamMembers { get; set; } = true;

    [StorageKey(StorageKeys.Testing.User.Duplicate.SuperUsers)]
    public bool TestDataCreateDuplicateTeamMembershipsForSuperUsers { get; set; } = true;

    [StorageKey(StorageKeys.Testing.Team.Unknown.Teams)]
    public bool TestDataCreateUnknownTeams { get; set; } = true;

    [StorageKey(StorageKeys.Testing.Team.Duplicate.Teams)]
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
        var getUsersDelayResult = await protectedLocalStorage.GetAsync<TimeSpan>(StorageKeys.Testing.User.Delay.Get);
        if (getUsersDelayResult.Success)
            GetUsersDelay = getUsersDelayResult.Value;

        var saveUserDelayResult = await protectedLocalStorage.GetAsync<TimeSpan>(StorageKeys.Testing.User.Delay.Save);
        if (saveUserDelayResult.Success)
            SaveUserDelay = saveUserDelayResult.Value;

        var deleteUserDelayResult = await protectedLocalStorage.GetAsync<TimeSpan>(StorageKeys.Testing.User.Delay.Delete);
        if (deleteUserDelayResult.Success)
            DeleteUserDelay = deleteUserDelayResult.Value;

        var getTeamsDelayResult = await protectedLocalStorage.GetAsync<TimeSpan>(StorageKeys.Testing.Team.Delay.Get);
        if (getTeamsDelayResult.Success)
            GetTeamsDelay = getTeamsDelayResult.Value;

        var saveTeamsDelayResult = await protectedLocalStorage.GetAsync<TimeSpan>(StorageKeys.Testing.Team.Delay.Save);
        if (saveTeamsDelayResult.Success)
            SaveTeamDelay = saveTeamsDelayResult.Value;

        var deleteTeamsDelayResult = await protectedLocalStorage.GetAsync<TimeSpan>(StorageKeys.Testing.Team.Delay.Delete);
        if (deleteTeamsDelayResult.Success)
            DeleteTeamDelay = deleteTeamsDelayResult.Value;

        var getWorkflowConfigurationsDelayResult = await protectedLocalStorage.GetAsync<TimeSpan>(StorageKeys.Testing.WorkflowConfiguration.Delay.Get);
        if (getWorkflowConfigurationsDelayResult.Success)
            GetWorkflowConfigurationsDelay = getWorkflowConfigurationsDelayResult.Value;

        var saveWorkflowConfigurationDelayResult = await protectedLocalStorage.GetAsync<TimeSpan>(StorageKeys.Testing.WorkflowConfiguration.Delay.Save);
        if (saveWorkflowConfigurationDelayResult.Success)
            SaveWorkflowConfigurationDelay = saveWorkflowConfigurationDelayResult.Value;

        var deleteWorkflowConfigurationDelayResult = await protectedLocalStorage.GetAsync<TimeSpan>(StorageKeys.Testing.WorkflowConfiguration.Delay.Delete);
        if (deleteWorkflowConfigurationDelayResult.Success)
            DeleteWorkflowConfigurationDelay = deleteWorkflowConfigurationDelayResult.Value;

        // Load test data settings
        var testDataNumberOfUsersResult = await protectedLocalStorage.GetAsync<int>(StorageKeys.Testing.User.NumberOfUsers);
        if (testDataNumberOfUsersResult.Success)
            TestDataNumberOfUsers = testDataNumberOfUsersResult.Value;

        var testDataNumberOfTeamsResult = await protectedLocalStorage.GetAsync<int>(StorageKeys.Testing.Team.NumberOfTeams);
        if (testDataNumberOfTeamsResult.Success)
            TestDataNumberOfTeams = testDataNumberOfTeamsResult.Value;

        var testDataCreateUnknownUsersAsTeamMembersResult = await protectedLocalStorage.GetAsync<bool>(StorageKeys.Testing.User.Unknown.Users);
        if (testDataCreateUnknownUsersAsTeamMembersResult.Success)
            TestDataCreateUnknownUsersAsTeamMembers = testDataCreateUnknownUsersAsTeamMembersResult.Value;

        var testDataCreateDuplicateTeamMembershipsForUsersResult = await protectedLocalStorage.GetAsync<bool>(StorageKeys.Testing.User.Duplicate.Users);
        if (testDataCreateDuplicateTeamMembershipsForUsersResult.Success)
            TestDataCreateDuplicateTeamMembershipsForUsers = testDataCreateDuplicateTeamMembershipsForUsersResult.Value;

        var testDataCreateUnknownSuperUsersAsTeamMembersResult = await protectedLocalStorage.GetAsync<bool>(StorageKeys.Testing.User.Unknown.SuperUsers);
        if (testDataCreateUnknownSuperUsersAsTeamMembersResult.Success)
            TestDataCreateUnknownSuperUsersAsTeamMembers = testDataCreateUnknownSuperUsersAsTeamMembersResult.Value;

        var testDataCreateDuplicateTeamMembershipsForSuperUsersResult = await protectedLocalStorage.GetAsync<bool>(StorageKeys.Testing.User.Duplicate.SuperUsers);
        if (testDataCreateDuplicateTeamMembershipsForSuperUsersResult.Success)
            TestDataCreateDuplicateTeamMembershipsForSuperUsers = testDataCreateDuplicateTeamMembershipsForSuperUsersResult.Value;

        var testDataCreateUnknownTeamsResult = await protectedLocalStorage.GetAsync<bool>(StorageKeys.Testing.Team.Unknown.Teams);
        if (testDataCreateUnknownTeamsResult.Success)
            TestDataCreateUnknownTeams = testDataCreateUnknownTeamsResult.Value;

        var testDataCreateDuplicateTeamsResult = await protectedLocalStorage.GetAsync<bool>(StorageKeys.Testing.Team.Duplicate.Teams);
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
