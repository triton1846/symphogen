using BlazorWebAppSymphogen.Models;

namespace BlazorWebAppSymphogen.Services;

public class TestDataService : ITestDataService
{
    private readonly ILogger<TestDataService> _logger;
    private readonly IUserPreferences _userPreferences;

    private List<User> _users = [];
    private List<Team> _teams = [];
    private List<string> _teamIds = [];
    private List<WorkflowConfiguration> _workflowConfigurations = [];
    private bool _createdUnknownUsersAsTeamMembers = false;
    private bool _createdUnknownSuperUsersAsTeamMembers = false;
    private bool _createdDuplicateUsersAsTeamMembers = false;
    private bool _createdDuplicateSuperUsersAsTeamMembers = false;
    private bool _createdUnknownTeams = false;
    private bool _createdDuplicateTeams = false;

    public TestDataService(
        ILogger<TestDataService> logger,
        IUserPreferences userPreferences)
    {
        _logger = logger;
        _userPreferences = userPreferences;
    }

    public async Task<IEnumerable<Team>> GetTeamsAsync()
    {
        await Task.Delay(_userPreferences.FetchTeamsDelay);

        if (_teams.Count != 0)
        {
            return _teams;
        }

        _teams = CreateRandomTeams();

        return _teams;
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        await Task.Delay(_userPreferences.FetchUsersDelay);

        if (_users.Count != 0)
        {
            return _users;
        }

        _users = GetRandomUsers();
        CreateInvalidData();

        return _users;
    }

    public async Task<IEnumerable<WorkflowConfiguration>> GetWorkflowConfigurationsAsync()
    {
        await Task.Delay(_userPreferences.FetchWorkflowConfigurationsDelay);

        if (_workflowConfigurations.Count != 0)
        {
            return _workflowConfigurations;
        }

        _workflowConfigurations = await GetRandomWorkflowConfigurations();

        return _workflowConfigurations;
    }

    private List<User> GetRandomUsers()
    {
        if (_users.Count > 0)
        {
            return _users;
        }

        var random = new Random();
        var randomUsers = new List<User>();

        for (int i = 0; i < _userPreferences.TestDataNumberOfUsers; i++)
        {
            // Use Bogus to generate random user data
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
            .RuleFor(u => u.TeamIds, f =>
            {
                return [.. GetTeamIds().OrderBy(_ => random.Next()).Take(random.Next(1, 8))];
            }).Generate();

            randomUsers.Add(user);
        }

        _logger.LogInformation("Generated {Count} random users.", randomUsers.Count);

        return randomUsers;
    }

    public void CreateInvalidData()
    {
        // OK
        if (_userPreferences.TestDataCreateUnknownUsersAsTeamMembers && !_createdUnknownUsersAsTeamMembers)
        {
            var numberOfUnknownUsers = new Random().Next(1, 4);
            for (int i = 0; i < numberOfUnknownUsers; i++)
            {
                var user = _users.OrderBy(u => u.FullName).Skip(i).FirstOrDefault(u => u.TeamIds != null);
                ArgumentNullException.ThrowIfNull(user, "No user with a team ID found to add the unknown user to.");
                user.TeamIds = user.TeamIds!.Append(Guid.NewGuid().ToString());
            }
            _createdUnknownUsersAsTeamMembers = true;
        }

        // OK
        if (_userPreferences.TestDataCreateDuplicateTeamMembershipsForUsers && !_createdDuplicateUsersAsTeamMembers)
        {
            var user = _users.OrderBy(u => u.FullName).FirstOrDefault(u => u.TeamIds?.Count() > 1);
            ArgumentNullException.ThrowIfNull(user, "No user with multiple team IDs found to create a duplicate membership.");
            ArgumentNullException.ThrowIfNull(user.TeamIds, "User's TeamIds cannot be null.");
            user.TeamIds = user.TeamIds.Append(user.TeamIds.First());
            user.TeamIds = user.TeamIds.Append(user.TeamIds.First());
            user.TeamIds = user.TeamIds.Append(user.TeamIds.First());
            _createdDuplicateUsersAsTeamMembers = true;
        }

        // Needs testing
        if (_userPreferences.TestDataCreateUnknownSuperUsersAsTeamMembers && !_createdUnknownSuperUsersAsTeamMembers)
        {
            var numberOfUnknownSuperUsers = new Random().Next(1, 4);
            for (int i = 0; i < numberOfUnknownSuperUsers; i++)
            {
                var superUser = _users.OrderBy(u => u.FullName).Skip(i).FirstOrDefault(u => u.TeamIds != null);
                ArgumentNullException.ThrowIfNull(superUser, "No user with a team ID found to add the unknown super user to.");
                superUser.TeamIds = superUser.TeamIds!.Append(Guid.NewGuid().ToString());
            }
            _createdUnknownSuperUsersAsTeamMembers = true;
        }

        // Needs testing
        if (_userPreferences.TestDataCreateDuplicateTeamMembershipsForSuperUsers && !_createdDuplicateSuperUsersAsTeamMembers)
        {
            var superUser = _users.OrderBy(u => u.FullName).FirstOrDefault(u => u.TeamIds?.Count() > 1);
            ArgumentNullException.ThrowIfNull(superUser, "No user with multiple team IDs found to create a duplicate super user membership.");
            ArgumentNullException.ThrowIfNull(superUser.TeamIds, "User's TeamIds cannot be null.");
            superUser.TeamIds = superUser.TeamIds.Append(superUser.TeamIds.First());
            superUser.TeamIds = superUser.TeamIds.Append(superUser.TeamIds.First());
            superUser.TeamIds = superUser.TeamIds.Append(superUser.TeamIds.First());
            _createdDuplicateSuperUsersAsTeamMembers = true;
        }

        // Needs testing
        if (_userPreferences.TestDataCreateUnknownTeams && !_createdUnknownTeams)
        {
            var unknownTeam = new Team
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Unknown Team",
                UserIds = [],
                SuperUserIds = [],
                WorkflowConfigurationIds = []
            };
            _teams.Add(unknownTeam);
            _createdUnknownTeams = true;
        }

        // Needs testing
        if (_userPreferences.TestDataCreateDuplicateTeams && !_createdDuplicateTeams)
        {
            var duplicateTeam = _teams.OrderBy(t => t.Name).FirstOrDefault();
            ArgumentNullException.ThrowIfNull(duplicateTeam, "No team found to create a duplicate.");
            _teams.Add(duplicateTeam);
            _createdDuplicateTeams = true;
        }
    }

    private List<Team> CreateRandomTeams()
    {
        var users = GetRandomUsers(); // Generate some random users to assign to teams

        var random = new Random();
        var randomTeams = new List<Team>();
        foreach (var teamId in GetTeamIds())
        {
            var team = new Bogus.Faker<Team>()
                .RuleFor(t => t.Id, f => teamId)
                .RuleFor(t => t.Name, f => f.Commerce.Department())
                .RuleFor(t => t.UserIds, f =>
                {
                    return users
                        .Where(u => u.TeamIds?.Contains(teamId) == true)
                        .Select(u => u.Id)
                        .ToList();
                })
                .RuleFor(t => t.WorkflowConfigurationIds, f =>
                {
                    return [.. f.Make(random.Next(0, 4), () => Guid.NewGuid().ToString()).Distinct()];
                })
                .Generate();

            // Ensure that names are unique
            while (randomTeams.Any(t => t.Name == team.Name))
            {
                team.Name = new Bogus.Faker().Commerce.Department();
            }

            if (team.UserIds.Any())
            {
                team.SuperUserIds = [.. team.UserIds.OrderBy(_ => random.Next()).Take(random.Next(1, team.UserIds.Count()))];
            }

            randomTeams.Add(team);
        }

        _logger.LogInformation("Generated {Count} random teams.", randomTeams.Count);

        return randomTeams;
    }

    private List<string> GetTeamIds()
    {
        if (_teamIds.Count == 0 && _teams.Count > 0)
        {
            _teamIds = [.. _teams.Select(t => t.Id).Distinct()];
        }
        else if (_teamIds.Count == 0)
        {
            // If no teams are available, generate a default set of team IDs
            var numberOfTeamIds = Math.Max(10, _userPreferences.TestDataNumberOfUsers / 10); // Default to 10% of users
            _teamIds = [.. Enumerable.Range(1, numberOfTeamIds).Select(i => Guid.NewGuid().ToString())];
        }

        return _teamIds;
    }

    private async Task<List<WorkflowConfiguration>> GetRandomWorkflowConfigurations()
    {
        var teams = await GetTeamsAsync(); // Base workflow configurations on teams

        var studyTypeKeys = new[] { "labbook", "external development", "N/A", "gmp_batch_no" };
        var studyTypeInputTypes = new[] { "text", "number", "date", "select" };

        var random = new Random();
        var randomWorkflowConfigurations = new List<WorkflowConfiguration>();
        foreach (var team in teams)
        {
            foreach (var workflowConfigurationId in team.WorkflowConfigurationIds ?? [])
            {
                var workflowConfiguration = new Bogus.Faker<WorkflowConfiguration>()
                    .RuleFor(t => t.Id, f => workflowConfigurationId)
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
                        var identifiers = new[] {
                            "batch_id", "bioreactor_id", "drug_product_id", "id", "mimer_id", "ngs_sample_id",
                            "plasmid_prep_id", "plate_well_id", "purification_id", "sample_id", "seed_train_id",
                            "single_cell_id", "stability_id", "testing_id", "transfection_id" };
                        return f.PickRandom(identifiers);
                    })
                    .RuleFor(t => t.ParameterRowCount, f => random.Next(1, 1000))
                    .RuleFor(t => t.DatasourceConfigurationIds, f =>
                    {
                        return [.. f.Make(random.Next(0, 10), () => Guid.NewGuid().ToString()).Distinct()];
                    })
                    .RuleFor(t => t.IsActive, f => f.Random.Bool())
                    .Generate();

                // Ensure that names are unique
                while (randomWorkflowConfigurations.Any(wc => wc.Name == workflowConfiguration.Name))
                {
                    workflowConfiguration.Name = new Bogus.Faker().Commerce.ProductName();
                }

                randomWorkflowConfigurations.Add(workflowConfiguration);
            }

            _logger.LogInformation("Generated {Count} random workflow configurations.", randomWorkflowConfigurations.Count);
        }

        return randomWorkflowConfigurations;
    }
}

public interface ITestDataService
{
    Task<IEnumerable<User>> GetUsersAsync();
    Task<IEnumerable<Team>> GetTeamsAsync();
    Task<IEnumerable<WorkflowConfiguration>> GetWorkflowConfigurationsAsync();
    void CreateInvalidData();
}
