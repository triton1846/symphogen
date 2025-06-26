using BlazorWebAppSymphogen.Models;

namespace BlazorWebAppSymphogen.Services;

public class TestDataService : ITestDataService
{
    private readonly ILogger<TestDataService> _logger;
    private readonly IUserPreferences _userPreferences;

    private List<User> _users = [];
    private List<Team> _teams = [];
    private List<string> _teamIds = [];
    private bool _createdUnknownUsersAsTeamMembers = false;
    //private bool _createdUnknownSuperUsersAsTeamMembers = false;
    private bool _createdDuplicateUsersAsTeamMembers = false;
    //private bool _createdDuplicateSuperUsers = false;
    //private bool _createdUnknownTeams = false;
    //private bool _createdDuplicateTeams = false;

    public TestDataService(
        ILogger<TestDataService> logger,
        IUserPreferences userPreferences)
    {
        _logger = logger;
        _userPreferences = userPreferences;

        if (_userPreferences.MimerEnvironment != MimerEnvironment.TestData)
        {
            _logger.LogError("CreateInvalidData should only be called in TestData environment. Current environment: {Environment}", _userPreferences.MimerEnvironment);
            throw new InvalidOperationException("TestDataService should only be used in TestData environment.");
        }
    }

    public async Task<IEnumerable<Team>> GetTeamsAsync()
    {
        await Task.Delay(_userPreferences.FetchTeamsDelay);

        if (_userPreferences.UseCacheData && _teams.Count != 0)
        {
            return _teams;
        }

        _teams = CreateRandomTeams();

        return _teams;
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        await Task.Delay(_userPreferences.FetchUsersDelay);

        if (_userPreferences.UseCacheData && _users.Count != 0)
        {
            return _users;
        }

        _users = GetRandomUsers();
        CreateInvalidData();

        return _users;
    }

    private List<User> GetRandomUsers()
    {
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

        return randomUsers;
    }

    public void CreateInvalidData()
    {
        if (_userPreferences.TestDataCreateUnknownUsersAsTeamMembers && !_createdUnknownUsersAsTeamMembers)
        {
            var numberOfUnknownUsers = 3; // TODO: Move to user preferences?
            for (int i = 0; i < numberOfUnknownUsers; i++)
            {
                var user = _users.OrderBy(u => u.FullName).Skip(i).FirstOrDefault(u => u.TeamIds != null);
                ArgumentNullException.ThrowIfNull(user, "No user with a team ID found to add the unknown user to.");
                user.TeamIds = user.TeamIds!.Append(Guid.NewGuid().ToString());
            }
            _createdUnknownUsersAsTeamMembers = true;
        }

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

        //if (_userPreferences.TestDataCreateUnknownSuperUsersAsTeamMembers && !_createdUnknownSuperUsersAsTeamMembers)
        //{
        //    //var superUser = _users.OrderBy(u => u.FullName).FirstOrDefault(u => u.TeamIds != null);
        //    //ArgumentNullException.ThrowIfNull(user, "No user with a team ID found to add the unknown user to.");
        //    //user.TeamIds = user.TeamIds!.Append(Guid.NewGuid().ToString());
        //    //_createdUnknownSuperUsersAsTeamMembers = true;
        //}

        //if (_userPreferences.TestDataCreateUnknownTeams && !_createdUnknownTeams)
        //{
        //    var unknownTeam = new Team
        //    {
        //        Id = Guid.NewGuid().ToString(),
        //        Name = "Unknown Team",
        //        UserIds = [],
        //        SuperUserIds = [],
        //        WorkflowConfigurationIds = []
        //    };
        //    _teams.Add(unknownTeam);
        //    _createdUnknownTeams = true;
        //}



        //// Make sure test data is consistent
        //foreach (var user in _users[mimerEnvironment])
        //{
        //    foreach (var team in user.Teams)
        //    {
        //        if (_teams[mimerEnvironment].All(t => t.Id != team.Id))
        //        {
        //            _teams[mimerEnvironment].Add(team);
        //        }
        //    }
        //}


        //// TODO: Use user preferences to determine if we should simulate data errors
        //// Make data errors to test UI error handling

        //// Add a user with a duplicate team ID to simulate data inconsistency
        //var userWithDuplicateTeamId = _users[mimerEnvironment].OrderBy(u => u.FullName).First(u => u.TeamIds?.Count() > 1);
        //userWithDuplicateTeamId.TeamIds = userWithDuplicateTeamId.TeamIds!.Append(userWithDuplicateTeamId.TeamIds!.First());

        //// Add a user with an unknown team ID to simulate data inconsistency
        //var userWithUnknownTeamId = _users[mimerEnvironment].OrderBy(u => u.FullName).First();
        //userWithUnknownTeamId.TeamIds = userWithUnknownTeamId.TeamIds?.Append("unknown-team-id");
    }

    private List<Team> CreateRandomTeams()
    {
        var users = GetRandomUsers(); // Generate some random users to assign to teams

        var random = new Random();
        var randomTeams = new List<Team>();
        foreach (var teamId in GetTeamIds())//_teamIds[mimerEnvironment])
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
                }).Generate();

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

        return randomTeams;
    }

    private List<string> GetTeamIds()
    {
        //if (!_teamIds.TryGetValue(mimerEnvironment, out _) || !_teamIds[mimerEnvironment].Any())
        //{
        //    _teamIds[mimerEnvironment] = [.. Enumerable.Range(1, numberOfTeamIds).Select(i => Guid.NewGuid().ToString())];
        //}

        //return _teamIds[mimerEnvironment];

        //return [.. _users.SelectMany(u => u.TeamIds ?? [])];

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
}

public interface ITestDataService
{
    Task<IEnumerable<User>> GetUsersAsync();
    Task<IEnumerable<Team>> GetTeamsAsync();
    void CreateInvalidData();
}
