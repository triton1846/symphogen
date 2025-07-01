namespace BlazorWebAppSymphogen.Settings;

public interface IUserPreferences
{
    bool IsInitialized { get; }
    Task InitializeAsync();
    MimerEnvironment MimerEnvironment { get; }
    Task SetAsync<TValue>(string propertyName, TValue value);
    bool RemoveInvalidDataAutomatically { get; set; }

    #region Test Data preferences
    TimeSpan FetchUsersDelay { get; set; }
    TimeSpan FetchTeamsDelay { get; set; }
    TimeSpan FetchWorkflowConfigurationsDelay { get; set; }
    int TestDataNumberOfUsers { get; set; }
    bool TestDataCreateUnknownUsersAsTeamMembers { get; set; }
    bool TestDataCreateDuplicateTeamMembershipsForUsers { get; set; }
    bool TestDataCreateUnknownSuperUsersAsTeamMembers { get; set; }
    bool TestDataCreateDuplicateTeamMembershipsForSuperUsers { get; set; }
    bool TestDataCreateUnknownTeams { get; set; }
    bool TestDataCreateDuplicateTeams { get; set; }
    #endregion Test Data preferences
}
