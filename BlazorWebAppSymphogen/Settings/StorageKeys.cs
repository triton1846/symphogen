namespace BlazorWebAppSymphogen.Settings;

public static class StorageKeys
{
    public const string MimerEnvironment = "app_mimerEnvironment";
    public const string RemoveInvalidDataAutomatically = "app_removeInvalidDataAutomatically";

    public static class Testing
    {
        public static class User
        {
            public const string NumberOfUsers = "testing_user_numberOfUsers";
            public static class Delay
            {
                public const string Get = "testing_user_delay_get";
                public const string Save = "testing_user_delay_save";
                public const string Delete = "testing_user_delay_delete";
            }

            public static class Duplicate
            {
                public const string Users = "testing_user_duplicate_users";
                public const string SuperUsers = "testing_user_duplicate_superUsers";
            }

            public static class Unknown
            {
                public const string Users = "testing_user_unknown_users";
                public const string SuperUsers = "testing_user_unknown_superUsers";
            }
        }

        public static class Team
        {
            public const string NumberOfTeams = "testing_team_numberOfTeams";
            public static class Delay
            {
                public const string Get = "testing_team_delay_get";
                public const string Save = "testing_team_delay_save";
                public const string Delete = "testing_team_delay_delete";
            }

            public static class Duplicate
            {
                public const string Teams = "testing_team_duplicate_teams";
            }

            public static class Unknown
            {
                public const string Teams = "testing_team_unknown_teams";
            }
        }

        public static class WorkflowConfiguration
        {
            public const string NumberOfWorkflowConfigurations = "testing_workflowConfiguration_numberOfWorkflowConfigurations";
            public static class Delay
            {
                public const string Get = "testing_workflowConfiguration_delay_get";
                public const string Save = "testing_workflowConfiguration_delay_save";
                public const string Delete = "testing_workflowConfiguration_delay_delete";
            }

            public static class Duplicate
            {
                public const string WorkflowConfigurations = "testing_workflowConfiguration_duplicate_workflowConfigurations";
            }

            public static class Unknown
            {
                public const string WorkflowConfigurations = "testing_workflowConfiguration_unknown_workflowConfigurations";
            }
        }
    }
}
