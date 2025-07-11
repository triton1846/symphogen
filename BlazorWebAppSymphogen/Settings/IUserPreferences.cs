namespace BlazorWebAppSymphogen.Settings;

public interface IUserPreferences : ITestDataPreferences
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
}

public interface ITestDataPreferences : ITestDataUsers, ITestDataTeams, ITestDataWorkflowConfigurations
{
}

public interface ITestDataUsers
{
    int Users_NumberOf { get; set; }
    TimeSpan Users_Delay_Get { get; set; }
    TimeSpan Users_Delay_Save { get; set; }
    TimeSpan Users_Delay_Delete { get; set; }
    bool Users_Unknown_TeamMembership { get; set; }
    bool Users_Duplicate_TeamMembership { get; set; }
}

public interface ITestDataTeams
{
    int Teams_NumberOf { get; set; }
    TimeSpan Teams_Delay_Get { get; set; }
    TimeSpan Teams_Delay_Save { get; set; }
    TimeSpan Teams_Delay_Delete { get; set; }
    bool Teams_Unknown_User { get; set; }
    bool Teams_Unknown_SuperUser { get; set; }
    bool Teams_Duplicate_User { get; set; }
    bool Teams_Duplicate_SuperUser { get; set; }
    bool Teams_Duplicate_WorkflowConfigurations { get; set; }
    bool Teams_Unknown_WorkflowConfigurations { get; set; }
}

public interface ITestDataWorkflowConfigurations
{
    int WorkflowConfigurations_NumberOf { get; set; }
    TimeSpan WorkflowConfigurations_Delay_Get { get; set; }
    TimeSpan WorkflowConfigurations_Delay_Save { get; set; }
    TimeSpan WorkflowConfigurations_Delay_Delete { get; set; }
}
