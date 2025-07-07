using BlazorWebAppSymphogen.Models;
using BlazorWebAppSymphogen.Services.Interfaces;
using BlazorWebAppSymphogen.Settings;

namespace BlazorWebAppSymphogen.Services;

public class TeamService : ITeamService
{
    private readonly ILogger<TeamService> _logger;
    private readonly IUserPreferences _userPreferences;
    private readonly ICosmosService _cosmosService;
    private readonly ITestDataService _testDataService;

    private readonly Dictionary<MimerEnvironment, List<Team>> _teams = [];

    public TeamService(
        ILogger<TeamService> logger,
        IUserPreferences userPreferences,
        ICosmosService cosmosService,
        ITestDataService testDataService)
    {
        _logger = logger;
        _userPreferences = userPreferences;
        _cosmosService = cosmosService;
        _testDataService = testDataService;
    }

    public async Task<IEnumerable<Team>> GetAsync(
        MimerEnvironment mimerEnvironment,
        Func<IQueryable<Team>, IQueryable<Team>>? filterExpression = null)
    {
        if (_userPreferences.MimerEnvironment == MimerEnvironment.TestData)
        {
            _logger.LogDebug("Using test data for teams in {Environment}", mimerEnvironment);
            var testDataTeams = await _testDataService.GetTeamsAsync();
            return [.. testDataTeams];
        }

        if (_teams.TryGetValue(mimerEnvironment, out _) && _teams[mimerEnvironment].Count != 0)
        {
            _logger.LogDebug("Returning cached teams for {Environment}", mimerEnvironment);
            return [.. _teams[mimerEnvironment]];
        }

        var teams = await _cosmosService.GetAsync<Team>(mimerEnvironment, "users", "teams", filterExpression);
        _teams[mimerEnvironment] = [.. teams];

        return _teams[mimerEnvironment];
    }

    public async Task SaveAsync(MimerEnvironment mimerEnvironment, Team team)
    {
        if (_userPreferences.MimerEnvironment == MimerEnvironment.TestData)
        {
            await _testDataService.SaveTeamAsync(team);
            return;
        }

        // Call web API or Cosmos to save
        // For now just update or add the team to the cache
        if (_teams.TryGetValue(mimerEnvironment, out var teams))
        {
            var existingTeam = teams.FirstOrDefault(u => u.Id == team.Id);
            if (existingTeam != null)
            {
                _logger.LogDebug("Updating existing team {TeamId} in cache for {Environment}", team.Id, mimerEnvironment);
                teams.Remove(existingTeam);
            }
            else
            {
                _logger.LogDebug("Updating existing team {TeamId} in cache for {Environment}", team.Id, mimerEnvironment);
            }
            teams.Add(team);
        }
        else
        {
            _logger.LogDebug("Creating new team list for {Environment} and adding team {TeamId}", mimerEnvironment, team.Id);
            _teams[mimerEnvironment] = [team];
        }
    }

    public async Task DeleteAsync(MimerEnvironment mimerEnvironment, string teamId)
    {
        if (_userPreferences.MimerEnvironment == MimerEnvironment.TestData)
        {
            await _testDataService.DeleteTeamAsync(teamId);
            return;
        }

        // Call web API or Cosmos to delete
        // For now just remove the team from the cache
        if (_teams.TryGetValue(mimerEnvironment, out var teams))
        {
            var existingTeam = teams.FirstOrDefault(u => u.Id == teamId);
            if (existingTeam != null)
            {
                _logger.LogDebug("Removing team {TeamId} from cache for {Environment}", teamId, mimerEnvironment);
                teams.Remove(existingTeam);
            }
            else
            {
                _logger.LogWarning("Removing team {TeamId} from cache for {Environment}", teamId, mimerEnvironment);
            }
        }
    }
}
