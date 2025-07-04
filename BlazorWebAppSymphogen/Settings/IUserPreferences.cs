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

    /// <summary>
    /// Gets the Mimer environment.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether invalid data should be removed automatically when editing data.
    /// If true, invalid data will be removed automatically when editing data.
    /// </summary>
    bool RemoveInvalidDataAutomatically { get; set; }

    #region Test Data preferences

    /// <summary>
    /// Delays for fetching users.
    /// </summary>
    /// <remarks>
    /// This property may be removed in the future as it is primarily used for testing purposes.
    /// </remarks>
    TimeSpan FetchUsersDelay { get; set; }

    /// <summary>
    /// Delays for fetching teams.
    /// </summary>
    /// <remarks>
    /// This property may be removed in the future as it is primarily used for testing purposes.
    /// </remarks>
    TimeSpan FetchTeamsDelay { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This property may be removed in the future as it is primarily used for testing purposes.
    /// </remarks>
    TimeSpan FetchWorkflowConfigurationsDelay { get; set; }

    /// <summary>
    /// Specifies the number of users to create for testing purposes.
    /// </summary>
    /// <remarks>
    /// This property may be removed in the future as it is primarily used for testing purposes.
    /// </remarks>
    int TestDataNumberOfUsers { get; set; }

    /// <summary>
    /// Creates unknown users as team members for testing purposes.
    /// </summary>
    /// <remarks>
    /// This property may be removed in the future as it is primarily used for testing purposes.
    /// </remarks>
    bool TestDataCreateUnknownUsersAsTeamMembers { get; set; }

    /// <summary>
    /// Creates duplicate team memberships for users for testing purposes.
    /// </summary>
    /// <remarks>
    /// This property may be removed in the future as it is primarily used for testing purposes.
    /// </remarks>
    bool TestDataCreateDuplicateTeamMembershipsForUsers { get; set; }

    /// <summary>
    /// Creates unknown super users as team members for testing purposes.
    /// </summary>
    /// <remarks>
    /// This property may be removed in the future as it is primarily used for testing purposes.
    /// </remarks>
    bool TestDataCreateUnknownSuperUsersAsTeamMembers { get; set; }

    /// <summary>
    /// Creates duplicate team memberships for super users for testing purposes.
    /// </summary>
    /// <remarks>
    /// This property may be removed in the future as it is primarily used for testing purposes.
    /// </remarks>
    bool TestDataCreateDuplicateTeamMembershipsForSuperUsers { get; set; }

    /// <summary>
    /// Creates unknown teams for testing purposes.
    /// </summary>
    /// <remarks>
    /// This property may be removed in the future as it is primarily used for testing purposes.
    /// </remarks>
    bool TestDataCreateUnknownTeams { get; set; }

    /// <summary>
    /// Creates duplicate teams for testing purposes.
    /// </summary>
    /// <remarks>
    /// This property may be removed in the future as it is primarily used for testing purposes.
    /// </remarks>
    bool TestDataCreateDuplicateTeams { get; set; }

    #endregion Test Data preferences
}
