namespace BlazorWebAppSymphogen.Settings;

public interface IUserPreferences
{
    /// <summary>
    /// Returns if the state is fully loaded from local storage.
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Initializes the user preferences by loading data from local storage.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task InitializeAsync();

    MimerEnvironment MimerEnvironment { get; }

    /// <summary>
    /// Updates the specified user preference property with a new value asynchronously.
    /// If the property is decorated with a <see cref="StorageKeyAttribute"/>, the change is also persisted to local storage.
    /// </summary>
    /// <typeparam name="TValue">The type of the user preference value.</typeparam>
    /// <param name="propertyName">The name of the user preference property to update.</param>
    /// <param name="value">The new value to assign to the property.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task completes once the user preference has been updated and, if applicable, persisted to local storage.
    /// </returns>
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
