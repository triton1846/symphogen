namespace BlazorWebAppSymphogen.Settings;

public static class StorageKeys
{
    public const string MimerEnvironment = "app_mimerEnvironment";
    public const string RemoveInvalidDataAutomatically = "app_removeInvalidDataAutomatically";
    public const string FetchUsersDelay = "app_fetchUsersDelay";
    public const string FetchTeamsDelay = "app_fetchTeamsDelay";
    public const string FetchWorkflowConfigurationsDelay = "app_fetchWorkflowConfigurationsDelay";

    public static class TestData
    {
        public const string NumberOfUsers = "testData_numberOfUsers";
        public const string CreateUnknownUsersAsTeamMembers = "testData_createUnknownUsersAsTeamMembers";
        public const string CreateDuplicateTeamMembershipsForUsers = "testData_createDuplicateTeamMembershipsForUsers";
        public const string CreateUnknownSuperUsersAsTeamMembers = "testData_createUnknownSuperUsersAsTeamMembers";
        public const string CreateDuplicateTeamMembershipsForSuperUsers = "testData_createDuplicateTeamMembershipsForSuperUsers";
        public const string CreateUnknownTeams = "testData_createUnknownTeams";
        public const string CreateDuplicateTeams = "testData_createDuplicateTeams";
        public const string CreateUnknownSuperUsers = "testData_createUnknownSuperUsers";
        public const string CreateDuplicateSuperUsers = "testData_createDuplicateSuperUsers";
        public const string CreateUnknownTeamsAsTeamMembers = "testData_createUnknownTeamsAsTeamMembers";
        public const string CreateDuplicateTeamMembershipsForTeams = "testData_createDuplicateTeamMembershipsForTeams";
        public const string CreateUnknownSuperTeams = "testData_createUnknownSuperTeams";
        public const string CreateDuplicateSuperTeams = "testData_createDuplicateSuperTeams";
        public const string CreateUnknownSuperTeamsAsTeamMembers = "testData_createUnknownSuperTeamsAsTeamMembers";
        public const string CreateDuplicateSuperTeamsAsTeamMembers = "testData_createDuplicateSuperTeamsAsTeamMembers";
    }
}
