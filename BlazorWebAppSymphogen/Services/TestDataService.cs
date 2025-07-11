using BlazorWebAppSymphogen.Models;
using BlazorWebAppSymphogen.Settings;
using System.Diagnostics.CodeAnalysis;

namespace BlazorWebAppSymphogen.Services;

[ExcludeFromCodeCoverage(Justification = "This is temporary test data service for development purposes only.")]
public class TestDataService : ITestDataService
{
    private readonly ILogger<TestDataService> _logger;
    private readonly IUserPreferences _userPreferences;

    private readonly List<User> _users = [];
    private readonly List<Team> _teams = [];
    private readonly List<WorkflowConfiguration> _workflowConfigurations = [];
    private bool _dataCreated = false;

    public TestDataService(
        ILogger<TestDataService> logger,
        IUserPreferences userPreferences)
    {
        _logger = logger;
        _userPreferences = userPreferences;

        CreateData();
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        await Task.Delay(_userPreferences.Users_Delay_Get);
        return _users;
    }

    public async Task SaveUserAsync(User user)
    {
        await Task.Delay(_userPreferences.Users_Delay_Save);

        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }
        // Simulate saving the user by adding or updating in the list
        var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser != null)
        {
            _users.Remove(existingUser);
        }
        _users.Add(user);
        _logger.LogInformation("User {UserId} saved successfully.", user.Id);
    }

    public async Task DeleteUserAsync(string userId)
    {
        await Task.Delay(_userPreferences.Users_Delay_Delete);

        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
        }
        var user = _users.FirstOrDefault(u => u.Id == userId);
        if (user != null)
        {
            _users.Remove(user);
            _logger.LogInformation("User {UserId} deleted successfully.", userId);
        }
        else
        {
            _logger.LogWarning("User {UserId} not found for deletion.", userId);
        }
    }

    public async Task<IEnumerable<Team>> GetTeamsAsync()
    {
        await Task.Delay(_userPreferences.Teams_Delay_Get);
        return _teams;
    }

    public async Task SaveTeamAsync(Team team)
    {
        await Task.Delay(_userPreferences.Teams_Delay_Save);

        if (team == null)
        {
            throw new ArgumentNullException(nameof(team), "Team cannot be null.");
        }
        // Simulate saving the team by adding or updating in the list
        var existingTeam = _teams.FirstOrDefault(t => t.Id == team.Id);
        if (existingTeam != null)
        {
            _teams.Remove(existingTeam);
        }
        _teams.Add(team);
        _logger.LogInformation("Team {TeamId} saved successfully.", team.Id);
    }

    public async Task DeleteTeamAsync(string teamId)
    {
        await Task.Delay(_userPreferences.Teams_Delay_Delete);

        if (string.IsNullOrEmpty(teamId))
        {
            throw new ArgumentException("Team ID cannot be null or empty.", nameof(teamId));
        }
        var team = _teams.FirstOrDefault(t => t.Id == teamId);
        if (team != null)
        {
            _teams.Remove(team);
            _logger.LogInformation("Team {TeamId} deleted successfully.", teamId);
        }
        else
        {
            _logger.LogWarning("Team {TeamId} not found for deletion.", teamId);
        }
    }

    public async Task<IEnumerable<WorkflowConfiguration>> GetWorkflowConfigurationsAsync()
    {
        await Task.Delay(_userPreferences.WorkflowConfigurations_Delay_Get);
        return _workflowConfigurations;
    }

    public async Task SaveWorkflowConfigurationAsync(WorkflowConfiguration workflowConfiguration)
    {
        await Task.Delay(_userPreferences.WorkflowConfigurations_Delay_Save);

        if (workflowConfiguration == null)
        {
            throw new ArgumentNullException(nameof(workflowConfiguration), "Workflow configuration cannot be null.");
        }
        // Simulate saving the workflow configuration by adding or updating in the list
        var existingConfig = _workflowConfigurations.FirstOrDefault(wc => wc.Id == workflowConfiguration.Id);
        if (existingConfig != null)
        {
            _workflowConfigurations.Remove(existingConfig);
        }
        _workflowConfigurations.Add(workflowConfiguration);
        _logger.LogInformation("Workflow configuration {WorkflowConfigurationId} saved successfully.", workflowConfiguration.Id);
    }

    public async Task DeleteWorkflowConfigurationAsync(string workflowConfigurationId)
    {
        await Task.Delay(_userPreferences.WorkflowConfigurations_Delay_Delete);

        if (string.IsNullOrEmpty(workflowConfigurationId))
        {
            throw new ArgumentException("Workflow configuration ID cannot be null or empty.", nameof(workflowConfigurationId));
        }
        var workflowConfiguration = _workflowConfigurations.FirstOrDefault(wc => wc.Id == workflowConfigurationId);
        if (workflowConfiguration != null)
        {
            _workflowConfigurations.Remove(workflowConfiguration);
            _logger.LogInformation("Workflow configuration {WorkflowConfigurationId} deleted successfully.", workflowConfigurationId);
        }
        else
        {
            _logger.LogWarning("Workflow configuration {WorkflowConfigurationId} not found for deletion.", workflowConfigurationId);
        }
    }

    private void CreateInvalidData()
    {
        if (_userPreferences.Users_Unknown_TeamMembership)
        {
            var user = _users.OrderBy(u => u.FullName).Skip(0).FirstOrDefault();
            ArgumentNullException.ThrowIfNull(user, "No user found to create an unknown team membership.");
            user.TeamIds = user.TeamIds.Append(Guid.NewGuid().ToString());
        }

        if (_userPreferences.Users_Duplicate_TeamMembership)
        {
            var user = _users.OrderBy(u => u.FullName).Skip(1).FirstOrDefault();
            ArgumentNullException.ThrowIfNull(user, "No user found to create a duplicate team membership.");
            var teamId = _teams.OrderBy(t => new Random().Next()).First().Id;
            user.TeamIds = user.TeamIds.Append(teamId);
            user.TeamIds = user.TeamIds.Append(teamId);
            user.TeamIds = user.TeamIds.Append(teamId);
        }

        // Side effect: user created not in user list. This is fine as it triggers data validation error
        if (_userPreferences.Teams_Unknown_User)
        {
            var team = _teams.OrderBy(t => t.Name).Skip(0).FirstOrDefault();
            ArgumentNullException.ThrowIfNull(team, "No team found to create an unknown user membership.");
            team.UserIds = team.UserIds.Append(Guid.NewGuid().ToString());
        }

        // Side effect: super user created not in team list for user. This is fine as it triggers data validation error
        if (_userPreferences.Teams_Unknown_SuperUser)
        {
            var team = _teams.OrderBy(t => t.Name).Skip(1).First();
            team.SuperUserIds = team.SuperUserIds.Append(Guid.NewGuid().ToString());
        }

        if (_userPreferences.Teams_Duplicate_User)
        {
            var team = _teams.OrderBy(t => t.Name).Skip(2).FirstOrDefault();
            ArgumentNullException.ThrowIfNull(team, "No team found to create a duplicate user membership.");
            var userId = _users.OrderBy(u => new Random().Next()).First().Id;
            team.UserIds = team.UserIds.Append(userId);
            team.UserIds = team.UserIds.Append(userId);
            team.UserIds = team.UserIds.Append(userId);
        }

        if (_userPreferences.Teams_Duplicate_SuperUser)
        {
            var team = _teams.OrderBy(t => t.Name).Skip(3).FirstOrDefault();
            ArgumentNullException.ThrowIfNull(team, "No team found to create a duplicate super user membership.");
            var superUserId = _users.OrderBy(u => new Random().Next()).First().Id;
            team.SuperUserIds = team.SuperUserIds.Append(superUserId);
            team.SuperUserIds = team.SuperUserIds.Append(superUserId);
            team.SuperUserIds = team.SuperUserIds.Append(superUserId);
        }
    }

    private void CreateData()
    {
        if (_dataCreated)
        {
            _logger.LogInformation("Test data already created. Skipping data creation.");
            return;
        }

        var random = new Random();

        // Create basic data
        CreateTeams();
        CreateUsers();
        CreateWorkflowConfigurations();

        // Make users and teams fit together
        foreach (var team in _teams)
        {
            var teamUsers = _users.OrderBy(u => random.Next()).Take(random.Next(1, Math.Min(5, _users.Count))).ToList();
            team.UserIds = [.. teamUsers.Select(u => u.Id)];
            // Assign super users. Use team.UserIds to ensure that super users are also team members
            team.SuperUserIds = [.. team.UserIds.OrderBy(_ => random.Next()).Take(random.Next(1, team.UserIds.Count() + 1))];

            // Assign teams to users
            foreach (var userId in team.UserIds)//TODO: invalid data would be super user not in team.UserIds
            {
                var user = _users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    user.TeamIds = user.TeamIds.Append(team.Id);
                }
            }
        }
        // Ensure that at least one team has no users
        if (_teams.Count > 0 && _teams.Any(t => !t.UserIds.Any()))
        {
            var teamWithoutUsers = _teams.FirstOrDefault(t => !t.UserIds.Any());
            if (teamWithoutUsers != null)
            {
                foreach (var user in _users.Where(u => u.TeamIds.Contains(teamWithoutUsers.Id)))
                {
                    user.TeamIds = [.. user.TeamIds.Where(tid => tid != teamWithoutUsers.Id)];
                }
                teamWithoutUsers.UserIds = [];
                teamWithoutUsers.SuperUserIds = [];
            }
        }
        // Ensure that at least one user has no teams
        if (_users.Count > 0 && _users.Any(u => !u.TeamIds.Any()))
        {
            var userWithoutTeams = _users.FirstOrDefault(u => !u.TeamIds.Any());
            if (userWithoutTeams != null)
            {
                foreach (var team in _teams.Where(t => t.UserIds.Contains(userWithoutTeams.Id)))
                {
                    team.UserIds = [.. team.UserIds.Where(uid => uid != userWithoutTeams.Id)];
                }
                userWithoutTeams.TeamIds = [];
            }
        }

        // Assign workflow configurations to teams
        foreach (var team in _teams)
        {
            var teamWorkflowConfigurations = _workflowConfigurations.OrderBy(wc => random.Next()).Take(random.Next(0, Math.Min(5, _workflowConfigurations.Count))).ToList();
            team.WorkflowConfigurationIds = [.. teamWorkflowConfigurations.Select(wc => wc.Id)]; // TODO: Is this valid? Should configs only be one single team?
        }
        // Make sure at least one team has no workflow configurations
        if (_teams.Count > 0 && _teams.Any(t => !t.WorkflowConfigurationIds.Any()))
        {
            var teamWithoutConfigs = _teams.FirstOrDefault(t => !t.WorkflowConfigurationIds.Any());
            if (teamWithoutConfigs != null)
            {
                teamWithoutConfigs.WorkflowConfigurationIds = [];
            }
        }

        // Create invalid data
        CreateInvalidData();

        _dataCreated = true;
    }

    private void CreateTeams()
    {
        var numberOfTeams = Math.Max(5, _userPreferences.Teams_NumberOf);
        for (int i = 0; i < numberOfTeams; i++)
        {
            var team = new Bogus.Faker<Team>()
                .RuleFor(t => t.Id, f => Guid.NewGuid().ToString())
                .RuleFor(t => t.Name, f => f.Commerce.Department())
                .RuleFor(t => t.UserIds, f => [])
                .RuleFor(t => t.WorkflowConfigurationIds, f => [])
                .Generate();

            // Ensure that names are unique
            while (_teams.Any(t => t.Name == team.Name))
            {
                team.Name = new Bogus.Faker().Commerce.Department();
            }
            _teams.Add(team);
        }
    }

    private void CreateUsers()
    {
        var numberOfUsers = Math.Max(10, _userPreferences.Users_NumberOf);
        for (int i = 0; i < numberOfUsers; i++)
        {
            var user = new Bogus.Faker<User>()
                .RuleFor(u => u.Id, f => Guid.NewGuid().ToString())
                .RuleFor(u => u.FullName, f => f.Name.FullName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Department, f => f.Commerce.Department())
                .RuleFor(u => u.Location, f => f.Address.City())
                .RuleFor(u => u.Favorites, f => f.Make(3, () => f.Commerce.ProductName()))
                .RuleFor(u => u.Initials, f => $"{f.Name.FirstName()[0]}{f.Name.LastName()[0]}")
                .RuleFor(u => u.JobTitle, f => f.Name.JobTitle())
                .RuleFor(u => u.OfficePhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.TeamIds, f => []).Generate();

            _users.Add(user);
        }
    }

    private void CreateWorkflowConfigurations()
    {
        var random = new Random();
        var numberOfWorkflowConfigurations = Math.Max(15, _userPreferences.WorkflowConfigurations_NumberOf);
        var studyTypeKeys = new[] { "labbook", "external development", "N/A", "gmp_batch_no" };
        var studyTypeInputTypes = new[] { "text", "number", "date", "select" };
        var parameterIdentifier = new[] {
                                "batch_id", "bioreactor_id", "drug_product_id", "id", "mimer_id", "ngs_sample_id",
                                "plasmid_prep_id", "plate_well_id", "purification_id", "sample_id", "seed_train_id",
                                "single_cell_id", "stability_id", "testing_id", "transfection_id" };

        for (int i = 0; i < numberOfWorkflowConfigurations; i++)
        {
            var workflowConfiguration = new Bogus.Faker<WorkflowConfiguration>()
                .RuleFor(t => t.Id, f => Guid.NewGuid().ToString())
                .RuleFor(t => t.Name, f => f.Commerce.ProductName())
                .RuleFor(t => t.Department, f => f.Commerce.Department())
                .RuleFor(t => t.StudyTypes, f =>
                {
                    return [.. f.Make(random.Next(0, 5), () => new StudyType
                    {
                        Key = f.PickRandom(studyTypeKeys),
                        InputType = f.PickRandom(studyTypeInputTypes)
                    })];
                })
                .RuleFor(t => t.ParameterIdentifier, f =>
                {
                    return f.PickRandom(parameterIdentifier);
                })
                .RuleFor(t => t.ParameterRowCount, f => random.Next(1, 1000))
                .RuleFor(t => t.DatasourceConfigurationIds, f =>
                {
                    return [.. f.Make(random.Next(0, 10), () => Guid.NewGuid().ToString()).Distinct()];
                })
                .RuleFor(t => t.IsActive, f => f.Random.Bool())
                .Generate();

            // Ensure that names are unique
            while (_workflowConfigurations.Any(wc => wc.Name == workflowConfiguration.Name))
            {
                workflowConfiguration.Name = new Bogus.Faker().Commerce.ProductName();
            }

            _workflowConfigurations.Add(workflowConfiguration);
        }
    }
}

public interface ITestDataService
{
    Task<IEnumerable<User>> GetUsersAsync();
    Task SaveUserAsync(User user);
    Task DeleteUserAsync(string userId);

    Task<IEnumerable<Team>> GetTeamsAsync();
    Task SaveTeamAsync(Team team);
    Task DeleteTeamAsync(string teamId);

    Task<IEnumerable<WorkflowConfiguration>> GetWorkflowConfigurationsAsync();
    Task SaveWorkflowConfigurationAsync(WorkflowConfiguration workflowConfiguration);
    Task DeleteWorkflowConfigurationAsync(string workflowConfigurationId);
}
