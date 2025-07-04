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
        UserServiceMock.Verify(m => m.GetUsersAsync(MimerEnvironment.QA, It.IsAny<Func<IQueryable<User>, IQueryable<User>>?>()), Times.Once);
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

    [Fact]
    public async Task UsersTable_DeletesUser_WhenDeleteButtonClicked()
    {
        // Arrange
        DialogReferenceMock.Setup(m => m.Result).ReturnsAsync(DialogResult.Ok(true));
        await UserPreferences.SetAsync(nameof(UserPreferences.MimerEnvironment), MimerEnvironment.QA);
        var cut = RenderComponent<Users>();
        var tables = cut.FindComponents<MudTable<User>>();
        var table = Assert.Single(tables);
        var userToDelete = DefaultData.UsersQA.First();

        // Act
        await cut.InvokeAsync(() =>
        {
            table.Instance.OnRowClick.InvokeAsync(new TableRowClickEventArgs<User>(null!, null!, userToDelete));
        });

        // Assert
        UserServiceMock.Verify(m => m.DeleteUserAsync(MimerEnvironment.QA, userToDelete.Id), Times.Once);
        Assert.NotNull(table.Instance.Items);
        Assert.NotEmpty(table.Instance.Items);
        Assert.DoesNotContain(userToDelete, table.Instance.Items);
    }

    [Fact]
    public async Task UsersTable_SavesUser_WhenSaveButtonClicked()
    {
        // Arrange
        await UserPreferences.SetAsync(nameof(UserPreferences.MimerEnvironment), MimerEnvironment.QA);
        var cut = RenderComponent<Users>();
        var tables = cut.FindComponents<MudTable<User>>();
        var table = Assert.Single(tables);
        var userToSave = DefaultData.UsersQA.First();
        userToSave.FullName = $"Updated User Name {Guid.NewGuid()}"; // Simulate a change to the user
        DialogReferenceMock.Setup(m => m.Result).ReturnsAsync(DialogResult.Ok(userToSave));

        // Act
        await cut.InvokeAsync(() =>
        {
            table.Instance.OnRowClick.InvokeAsync(new TableRowClickEventArgs<User>(null!, null!, userToSave));
        });

        // Assert
        UserServiceMock.Verify(m => m.SaveUserAsync(MimerEnvironment.QA, userToSave), Times.Once);
        Assert.NotNull(table.Instance.Items);
        Assert.NotEmpty(table.Instance.Items);
        var updatedUser = table.Instance.Items.Single(i => i.Id == userToSave.Id);
        Assert.Equal(userToSave.FullName, updatedUser.FullName);
    }

    [Fact]
    public async Task UsersTable_FilterCorrectly_WhenFilterApplied()
    {
        // Arrange
        await UserPreferences.SetAsync(nameof(UserPreferences.MimerEnvironment), MimerEnvironment.QA);
        var users = Fixture.CreateMany<User>(10).ToList();
        var searchText = Guid.NewGuid().ToString();
        users.ElementAt(0).Department = $"{users.ElementAt(0).Department}_{searchText}";
        users.ElementAt(1).Email = $"{users.ElementAt(1).Email}_{searchText}";
        users.ElementAt(2).FullName = $"{users.ElementAt(2).FullName}_{searchText}";
        users.ElementAt(3).Location = $"{users.ElementAt(3).Location}_{searchText}";
        users.ElementAt(4).Id = searchText;
        UserServiceMock.Setup(m => m.GetUsersAsync(MimerEnvironment.QA, It.IsAny<Func<IQueryable<User>, IQueryable<User>>?>()))
            .ReturnsAsync(users);

        var cut = RenderComponent<Users>();
        var tables = cut.FindComponents<MudTable<User>>();
        var table = Assert.Single(tables);

        // Act
        var mudTextField = cut.FindComponent<MudTextField<string>>();
        // Set the search text to filter the users
        await cut.InvokeAsync(() =>
        {
            mudTextField.Instance.SetText(searchText);
        });

        // Assert
        Assert.NotNull(table.Instance.Items);
        Assert.True(table.Instance.FilteredItems.OrderBy(i => i.Id).SequenceEqual(users.Take(5).OrderBy(i => i.Id)));
    }
}
