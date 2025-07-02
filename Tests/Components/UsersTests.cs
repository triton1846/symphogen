using BlazorWebAppSymphogen;
using BlazorWebAppSymphogen.Components.Dialogs;
using BlazorWebAppSymphogen.Components.Pages;
using BlazorWebAppSymphogen.Models;
using Bunit;
using MudBlazor;

namespace Tests.Components;

public class UsersTests : BaseTestContext
{
    [Fact]
    public async Task UsersTable_LoadsCorrectEnvironmentData()
    {
        // Arrange
        await UserPreferences.SetAsync(nameof(UserPreferences.MimerEnvironment), MimerEnvironment.QA);
        var cut = RenderComponent<Users>();

        // Act
        var tables = cut.FindComponents<MudTable<User>>();

        // Assert
        CosmosServiceMock.Verify(m => m.GetUsersAsync(MimerEnvironment.QA, It.IsAny<Func<IQueryable<User>, IQueryable<User>>?>()), Times.Once);
        var table = Assert.Single(tables);
        Assert.NotNull(table.Instance.Items);
        Assert.True(table.Instance.Items.SequenceEqual(DefaultData.UsersQA));
    }

    [Fact]
    public async Task UserRowClick_OpenEditDialogAndPreserveUserData_WhenDialogCancelled()
    {
        // Arrange
        DialogReferenceMock.Setup(m => m.Result).ReturnsAsync(DialogResult.Cancel);
        await UserPreferences.SetAsync(nameof(UserPreferences.MimerEnvironment), MimerEnvironment.QA);
        var cut = RenderComponent<Users>();
        var tables = cut.FindComponents<MudTable<User>>();
        var table = Assert.Single(tables);

        // Act
        // Fake user manipulation so we can verify that the cancelled dialog triggers a reset of the selected user
        var originalUser = DefaultData.UsersQA.First();
        var changedUser = Fixture.Create<User>();
        changedUser.Id = originalUser.Id; // Ensure we are updating an existing user
        await cut.InvokeAsync(() =>
        {
            table.Instance.OnRowClick.InvokeAsync(new TableRowClickEventArgs<User>(null!, null!, changedUser));
        });

        // Assert parameters.Get<User>("")
        DialogServiceMock.Verify(m => m.ShowAsync<UserEditorDialog>(
            It.IsAny<string?>(),
            It.Is<DialogParameters>(p => p.Get<User>(nameof(UserEditorDialog.User)) != null && p.Get<User>(nameof(UserEditorDialog.User))!.Id == originalUser.Id),
            It.IsAny<DialogOptions>()), 
            Times.Once);

        DialogReferenceMock.Verify(m => m.Result, Times.Once);

        // Verify that the user was not updated since the dialog was cancelled
        Assert.NotEqual(originalUser.FullName, changedUser.FullName);
        Assert.NotNull(table.Instance.Items);
        Assert.Equal(originalUser.FullName, table.Instance.Items.Single(i => i.Id == changedUser.Id).FullName);
    }

    // TODO: Create test to test deletion and updating of users
}
