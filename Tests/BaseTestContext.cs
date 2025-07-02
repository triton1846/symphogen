using AutoFixture.AutoMoq;
using BlazorWebAppSymphogen;
using BlazorWebAppSymphogen.Components.Dialogs;
using BlazorWebAppSymphogen.Services;
using BlazorWebAppSymphogen.Settings;
using Bunit;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MudBlazor;
using MudBlazor.Services;

namespace Tests;

public class BaseTestContext : TestContext
{
    protected readonly Fixture Fixture = new();

    protected Mock<ISnackbar> SnackbarMock = default!;
    protected Mock<IDialogService> DialogServiceMock = default!;
    protected Mock<IDialogReference> DialogReferenceMock = default!;

    protected IUserPreferences UserPreferences = default!;
    protected Mock<ICosmosService> CosmosServiceMock = default!;

    protected DefaultData DefaultData { get; private set; } = new();

    public BaseTestContext()
    {
        // Fixture max recursion depth
        Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => Fixture.Behaviors.Remove(b));
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        // Use AutoMoqCustomization for mocking
        Fixture.Customize(new AutoMoqCustomization());

        // Use Loose mode for JSInterop to avoid strict type checks
        JSInterop.Mode = JSRuntimeMode.Loose;
        // Needed for MudBlazor components to work properly
        Services.AddMudServices();

        // In case snackbar or dialog services are used in components
        AddServiceSnackbar();
        AddServiceDialog();

        // Add null loggers for components that require logging
        AddNullLoggers();

        SetupDefaultData();

        AddCosmosService();

        // Needed for MudBlazor components to work properly
        _ = RenderComponent<MudPopoverProvider>();
    }

    private void AddServiceSnackbar()
    {
        SnackbarMock = new Mock<ISnackbar>();
        SnackbarMock
            .Setup(m => m.Add(It.IsAny<string>(), It.IsAny<Severity>(), It.IsAny<Action<SnackbarOptions>>(), It.IsAny<string?>()))
            .Verifiable();
        Services.AddSingleton<ISnackbar>(service => SnackbarMock.Object);
    }

    private void AddServiceDialog()
    {
        DialogServiceMock = new Mock<IDialogService>();
        DialogReferenceMock = new Mock<IDialogReference>();
        var dialogResult = DialogResult.Ok(true);
        DialogReferenceMock
            .Setup(m => m.Result)
            .ReturnsAsync(dialogResult)
            .Verifiable();
        DialogServiceMock
            //.Setup(m => m.ShowAsync(It.IsAny<Type>(), It.IsAny<string?>(), It.IsAny<DialogParameters>(), It.IsAny<DialogOptions>()))
            .Setup(m => m.ShowAsync<UserEditorDialog>(It.IsAny<string?>(), It.IsAny<DialogParameters>(), It.IsAny<DialogOptions>()))
            .ReturnsAsync(DialogReferenceMock.Object)
            .Verifiable();
        Services.AddSingleton<IDialogService>(service => DialogServiceMock.Object);
    }

    private void AddNullLoggers()
    {
        void AddNullLogger<T>()
        {
            Services.AddSingleton<ILogger<T>>(NullLogger<T>.Instance);
        }

        AddNullLogger<BlazorWebAppSymphogen.Components.Pages.Users>();
    }

    private void SetupUserPreferences()
    {
        UserPreferences = new UserPreferences(
            NullLogger<UserPreferences>.Instance,
            Fixture.Create<ProtectedLocalStorage>());
        Services.AddSingleton<IUserPreferences>(service => UserPreferences);
    }

    private void AddCosmosService()
    {
        CosmosServiceMock = new Mock<ICosmosService>();

        CosmosServiceMock
            .Setup(m => m.GetUsersAsync(It.IsAny<MimerEnvironment>(), It.IsAny<Func<IQueryable<BlazorWebAppSymphogen.Models.User>, IQueryable<BlazorWebAppSymphogen.Models.User>>?>()))
            .ReturnsAsync((MimerEnvironment env, Func<IQueryable<BlazorWebAppSymphogen.Models.User>, IQueryable<BlazorWebAppSymphogen.Models.User>>? filter) =>
            {
                return env switch
                {
                    MimerEnvironment.SB1 => [.. DefaultData.UsersSB1],
                    MimerEnvironment.QA => [.. DefaultData.UsersQA],
                    _ => throw new NotSupportedException($"Environment {env} is not supported.")
                };
            })
            .Verifiable();

        CosmosServiceMock
            .Setup(m => m.GetTeamsAsync(It.IsAny<MimerEnvironment>(), It.IsAny<Func<IQueryable<BlazorWebAppSymphogen.Models.Team>, IQueryable<BlazorWebAppSymphogen.Models.Team>>?>()))
            .ReturnsAsync((MimerEnvironment env, Func<IQueryable<BlazorWebAppSymphogen.Models.Team>, IQueryable<BlazorWebAppSymphogen.Models.Team>>? filter) =>
            {
                return env switch
                {
                    MimerEnvironment.SB1 => [.. DefaultData.TeamsSB1],
                    MimerEnvironment.QA => [.. DefaultData.TeamsQA],
                    _ => throw new NotSupportedException($"Environment {env} is not supported.")
                };
            });

        Services.AddSingleton<ICosmosService>(service => CosmosServiceMock.Object);
    }

    private void SetupDefaultData()
    {
        SetupUserPreferences();

        var teamIds = Fixture.CreateMany<Guid>(15).ToList();// TODO: Use different IDs for environments?
        var random = new Random();

        DefaultData.UsersSB1 = Fixture.CreateMany<BlazorWebAppSymphogen.Models.User>(20)
            .Select(user => Fixture.Build<BlazorWebAppSymphogen.Models.User>()
                .With(u => u.Id, Guid.NewGuid().ToString())
                .With(u => u.TeamIds, teamIds.OrderBy(_ => random.Next()).Take(random.Next(1, 4)).Select(id => id.ToString()))
                .With(u => u.Teams, [])
                .Without(u => u.ValidationTeams)
                .Create())
            .ToList();

        DefaultData.UsersQA = Fixture.CreateMany<BlazorWebAppSymphogen.Models.User>(25)
            .Select(user => Fixture.Build<BlazorWebAppSymphogen.Models.User>()
                .With(u => u.Id, Guid.NewGuid().ToString())
                .With(u => u.TeamIds, teamIds.OrderBy(_ => random.Next()).Take(random.Next(1, 4)).Select(id => id.ToString()))
                .With(u => u.Teams, [])
                .Without(u => u.ValidationTeams)
                .Create())
            .ToList();

        DefaultData.TeamsSB1 = teamIds
            .Select(id => Fixture.Build<BlazorWebAppSymphogen.Models.Team>()
                .With(t => t.Id, id.ToString())
                .With(t => t.Users, [])
                .With(t => t.SuperUsers, [])
                .Without(t => t.ValidationSuperUsers)
                .Without(t => t.ValidationUsers)
                .Without(t => t.ValidationWorkflowConfigurations)
            .Create())
            .ToList();

        DefaultData.TeamsQA = teamIds
            .Select(id => Fixture.Build<BlazorWebAppSymphogen.Models.Team>()
                .With(t => t.Id, id.ToString())
                .With(t => t.Users, [])
                .With(t => t.SuperUsers, [])
                .Without(t => t.ValidationSuperUsers)
                .Without(t => t.ValidationUsers)
                .Without(t => t.ValidationWorkflowConfigurations)
            .Create())
            .ToList();
    }
}
