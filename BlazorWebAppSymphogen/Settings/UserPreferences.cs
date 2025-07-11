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

    [StorageKey(StorageKeys.Testing.User.NumberOfUsers)]
    public int Users_NumberOf { get; set; } = 100;

    [StorageKey(StorageKeys.Testing.User.Delay.Get)]
    public TimeSpan Users_Delay_Get { get; set; } = TimeSpan.Zero;

    [StorageKey(StorageKeys.Testing.User.Delay.Save)]
    public TimeSpan Users_Delay_Save { get; set; } = TimeSpan.Zero;

    [StorageKey(StorageKeys.Testing.User.Delay.Delete)]
    public TimeSpan Users_Delay_Delete { get; set; } = TimeSpan.Zero;

    [StorageKey(StorageKeys.Testing.User.Unknown.TeamMemberships)]
    public bool Users_Unknown_TeamMembership { get; set; }

    [StorageKey(StorageKeys.Testing.User.Duplicate.TeamMemberships)]
    public bool Users_Duplicate_TeamMembership { get; set; }



    [StorageKey(StorageKeys.Testing.Team.NumberOfTeams)]
    public int Teams_NumberOf { get; set; } = 10;

    [StorageKey(StorageKeys.Testing.Team.Delay.Get)]
    public TimeSpan Teams_Delay_Get { get; set; } = TimeSpan.Zero;

    [StorageKey(StorageKeys.Testing.Team.Delay.Save)]
    public TimeSpan Teams_Delay_Save { get; set; } = TimeSpan.Zero;

    [StorageKey(StorageKeys.Testing.Team.Delay.Delete)]
    public TimeSpan Teams_Delay_Delete { get; set; } = TimeSpan.Zero;

    [StorageKey(StorageKeys.Testing.Team.Unknown.Users)]
    public bool Teams_Unknown_User { get; set; }

    [StorageKey(StorageKeys.Testing.Team.Unknown.SuperUsers)]
    public bool Teams_Unknown_SuperUser { get; set; }

    [StorageKey(StorageKeys.Testing.Team.Duplicate.Users)]
    public bool Teams_Duplicate_User { get; set; }

    [StorageKey(StorageKeys.Testing.Team.Duplicate.SuperUsers)]
    public bool Teams_Duplicate_SuperUser { get; set; }



    [StorageKey(StorageKeys.Testing.WorkflowConfiguration.NumberOfWorkflowConfigurations)]
    public int WorkflowConfigurations_NumberOf { get; set; } = 35;

    [StorageKey(StorageKeys.Testing.WorkflowConfiguration.Delay.Get)]
    public TimeSpan WorkflowConfigurations_Delay_Get { get; set; } = TimeSpan.Zero;

    [StorageKey(StorageKeys.Testing.WorkflowConfiguration.Delay.Save)]
    public TimeSpan WorkflowConfigurations_Delay_Save { get; set; } = TimeSpan.Zero;

    [StorageKey(StorageKeys.Testing.WorkflowConfiguration.Delay.Delete)]
    public TimeSpan WorkflowConfigurations_Delay_Delete { get; set; } = TimeSpan.Zero;

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

        await InitializeTestDataAsync();

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

    private async Task InitializeTestDataAsync()
    {
        // Users settings
        var getUsersResult = await protectedLocalStorage.GetAsync<int>(StorageKeys.Testing.User.NumberOfUsers);
        if (getUsersResult.Success)
            Users_NumberOf = getUsersResult.Value;

        var getUsersDelayResult = await protectedLocalStorage.GetAsync<TimeSpan>(StorageKeys.Testing.User.Delay.Get);
        if (getUsersDelayResult.Success)
            Users_Delay_Get = getUsersDelayResult.Value;

        var saveUserDelayResult = await protectedLocalStorage.GetAsync<TimeSpan>(StorageKeys.Testing.User.Delay.Save);
        if (saveUserDelayResult.Success)
            Users_Delay_Save = saveUserDelayResult.Value;

        var deleteUserDelayResult = await protectedLocalStorage.GetAsync<TimeSpan>(StorageKeys.Testing.User.Delay.Delete);
        if (deleteUserDelayResult.Success)
            Users_Delay_Delete = deleteUserDelayResult.Value;

        var unknownUsersAsTeamMembersResult = await protectedLocalStorage.GetAsync<bool>(StorageKeys.Testing.User.Unknown.TeamMemberships);
        if (unknownUsersAsTeamMembersResult.Success)
            Users_Unknown_TeamMembership = unknownUsersAsTeamMembersResult.Value;

        var duplicateTeamMembershipsResult = await protectedLocalStorage.GetAsync<bool>(StorageKeys.Testing.User.Duplicate.TeamMemberships);
        if (duplicateTeamMembershipsResult.Success)
            Users_Duplicate_TeamMembership = duplicateTeamMembershipsResult.Value;

        // Teams settings
        var getTeamsResult = await protectedLocalStorage.GetAsync<int>(StorageKeys.Testing.Team.NumberOfTeams);
        if (getTeamsResult.Success)
            Teams_NumberOf = getTeamsResult.Value;

        var getTeamsDelayResult = await protectedLocalStorage.GetAsync<TimeSpan>(StorageKeys.Testing.Team.Delay.Get);
        if (getTeamsDelayResult.Success)
            Teams_Delay_Get = getTeamsDelayResult.Value;

        var saveTeamsDelayResult = await protectedLocalStorage.GetAsync<TimeSpan>(StorageKeys.Testing.Team.Delay.Save);
        if (saveTeamsDelayResult.Success)
            Teams_Delay_Save = saveTeamsDelayResult.Value;

        var deleteTeamsDelayResult = await protectedLocalStorage.GetAsync<TimeSpan>(StorageKeys.Testing.Team.Delay.Delete);
        if (deleteTeamsDelayResult.Success)
            Teams_Delay_Delete = deleteTeamsDelayResult.Value;

        var unknownUsersResult = await protectedLocalStorage.GetAsync<bool>(StorageKeys.Testing.Team.Unknown.Users);
        if (unknownUsersResult.Success)
            Teams_Unknown_User = unknownUsersResult.Value;

        var unknownSuperUsersResult = await protectedLocalStorage.GetAsync<bool>(StorageKeys.Testing.Team.Unknown.SuperUsers);
        if (unknownSuperUsersResult.Success)
            Teams_Unknown_SuperUser = unknownSuperUsersResult.Value;

        var duplicateUsersResult = await protectedLocalStorage.GetAsync<bool>(StorageKeys.Testing.Team.Duplicate.Users);
        if (duplicateUsersResult.Success)
            Teams_Duplicate_User = duplicateUsersResult.Value;

        var duplicateSuperUsersResult = await protectedLocalStorage.GetAsync<bool>(StorageKeys.Testing.Team.Duplicate.SuperUsers);
        if (duplicateSuperUsersResult.Success)
            Teams_Duplicate_SuperUser = duplicateSuperUsersResult.Value;

        // Workflow configurations settings
        var getWorkflowConfigurationsResult = await protectedLocalStorage.GetAsync<int>(StorageKeys.Testing.WorkflowConfiguration.NumberOfWorkflowConfigurations);
        if (getWorkflowConfigurationsResult.Success)
            WorkflowConfigurations_NumberOf = getWorkflowConfigurationsResult.Value;

        var getWorkflowConfigurationsDelayResult = await protectedLocalStorage.GetAsync<TimeSpan>(StorageKeys.Testing.WorkflowConfiguration.Delay.Get);
        if (getWorkflowConfigurationsDelayResult.Success)
            WorkflowConfigurations_Delay_Get = getWorkflowConfigurationsDelayResult.Value;

        var saveWorkflowConfigurationDelayResult = await protectedLocalStorage.GetAsync<TimeSpan>(StorageKeys.Testing.WorkflowConfiguration.Delay.Save);
        if (saveWorkflowConfigurationDelayResult.Success)
            WorkflowConfigurations_Delay_Save = saveWorkflowConfigurationDelayResult.Value;

        var deleteWorkflowConfigurationDelayResult = await protectedLocalStorage.GetAsync<TimeSpan>(StorageKeys.Testing.WorkflowConfiguration.Delay.Delete);
        if (deleteWorkflowConfigurationDelayResult.Success)
            WorkflowConfigurations_Delay_Delete = deleteWorkflowConfigurationDelayResult.Value;
    }
}
