using BlazorWebAppSymphogen;
using BlazorWebAppSymphogen.Settings;
using Microsoft.AspNetCore.DataProtection;

namespace Tests.Settings;

public class UserPreferencesTests : BaseTestContext
{
    public class TestData_UserPreferences_LoadsStoredValues_WhenInitialized : IEnumerable<object[]>
    {
        private readonly List<object[]> _data =
        [
            [MimerEnvironment.QA, true,
                TimeSpan.MinValue, TimeSpan.MinValue, TimeSpan.MinValue,
                TimeSpan.MinValue, TimeSpan.MinValue, TimeSpan.MinValue,
                TimeSpan.MinValue, TimeSpan.MinValue, TimeSpan.MinValue,
                42, 24, 9, true, true, true, true, true, true],
            [MimerEnvironment.QA, true,
                TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-2), TimeSpan.FromMilliseconds(-3),
                TimeSpan.FromMilliseconds(-4), TimeSpan.FromMilliseconds(-5), TimeSpan.FromMilliseconds(-6),
                TimeSpan.FromMilliseconds(-7), TimeSpan.FromMilliseconds(-8), TimeSpan.FromMilliseconds(-9),
                -42, -24, -9, true, true, true, true, true, true],
            [MimerEnvironment.QA, true,
                TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero,
                TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero,
                TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero,
                0, 0, 0, true, true, true, true, true, true],
            [MimerEnvironment.QA, false,
                TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(2), TimeSpan.FromMilliseconds(3),
                TimeSpan.FromMilliseconds(4), TimeSpan.FromMilliseconds(5), TimeSpan.FromMilliseconds(6),
                TimeSpan.FromMilliseconds(7), TimeSpan.FromMilliseconds(8), TimeSpan.FromMilliseconds(9),
                42, 24, 9, false, false, false, false, false, false],
            [MimerEnvironment.QA, true,
                TimeSpan.MaxValue, TimeSpan.MaxValue, TimeSpan.MaxValue,
                TimeSpan.MaxValue, TimeSpan.MaxValue, TimeSpan.MaxValue,
                TimeSpan.MaxValue, TimeSpan.MaxValue, TimeSpan.MaxValue,
                42, 24, 9, true, true, true, true, true, true],
            [MimerEnvironment.SB1, true, 
                TimeSpan.MinValue, TimeSpan.MinValue, TimeSpan.MinValue, 
                TimeSpan.MinValue, TimeSpan.MinValue, TimeSpan.MinValue,
                TimeSpan.MinValue, TimeSpan.MinValue, TimeSpan.MinValue,
                42, 24, 9, true, true, true, true, true, true],
            [MimerEnvironment.SB1, true, 
                TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-2), TimeSpan.FromMilliseconds(-3), 
                TimeSpan.FromMilliseconds(-4), TimeSpan.FromMilliseconds(-5), TimeSpan.FromMilliseconds(-6),
                TimeSpan.FromMilliseconds(-7), TimeSpan.FromMilliseconds(-8), TimeSpan.FromMilliseconds(-9),
                -42, -24, -9, true, true, true, true, true, true],
            [MimerEnvironment.SB1, true, 
                TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, 
                TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero,
                TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero,
                0, 0, 0, true, true, true, true, true, true],
            [MimerEnvironment.SB1, false, 
                TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(2), TimeSpan.FromMilliseconds(3), 
                TimeSpan.FromMilliseconds(4), TimeSpan.FromMilliseconds(5), TimeSpan.FromMilliseconds(6),
                TimeSpan.FromMilliseconds(7), TimeSpan.FromMilliseconds(8), TimeSpan.FromMilliseconds(9),
                42, 24, 9, false, false, false, false, false, false],
            [MimerEnvironment.SB1, true, 
                TimeSpan.MaxValue, TimeSpan.MaxValue, TimeSpan.MaxValue, 
                TimeSpan.MaxValue, TimeSpan.MaxValue, TimeSpan.MaxValue,
                TimeSpan.MaxValue, TimeSpan.MaxValue, TimeSpan.MaxValue,
                42, 24, 9, true, true, true, true, true, true],
        ];

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(TestData_UserPreferences_LoadsStoredValues_WhenInitialized))]
    public async Task UserPreferences_LoadsStoredValues_WhenInitialized(
        MimerEnvironment mimerEnvironment,
        bool removeInvalidDataAutomatically,
        TimeSpan user_delay_get,
        TimeSpan user_delay_save,
        TimeSpan user_delay_delete,
        TimeSpan team_delay_get,
        TimeSpan team_delay_save,
        TimeSpan team_delay_delete,
        TimeSpan workflowConfiguration_delay_get,
        TimeSpan workflowConfiguration_delay_save,
        TimeSpan workflowConfiguration_delay_delete,
        int user_numberOf,
        int team_numberOf,
        int workflowConfiguration_numberOf,
        bool user_duplicate_teamMemberships,
        bool user_unknown_teamMemberships,
        bool team_unknown_user,
        bool team_unknown_superUser,
        bool team_duplicate_user,
        bool team_duplicate_superUser)
    {
        // Arrange
        SetStorageValue(StorageKeys.MimerEnvironment, mimerEnvironment);
        SetStorageValue(StorageKeys.RemoveInvalidDataAutomatically, removeInvalidDataAutomatically);

        SetStorageValue(StorageKeys.Testing.User.NumberOfUsers, user_numberOf);
        SetStorageValue(StorageKeys.Testing.User.Delay.Get, user_delay_get);
        SetStorageValue(StorageKeys.Testing.User.Delay.Save, user_delay_save);
        SetStorageValue(StorageKeys.Testing.User.Delay.Delete, user_delay_delete);
        SetStorageValue(StorageKeys.Testing.User.Duplicate.TeamMemberships, user_duplicate_teamMemberships);
        SetStorageValue(StorageKeys.Testing.User.Unknown.TeamMemberships, user_unknown_teamMemberships);

        SetStorageValue(StorageKeys.Testing.Team.NumberOfTeams, team_numberOf);
        SetStorageValue(StorageKeys.Testing.Team.Delay.Get, team_delay_get);
        SetStorageValue(StorageKeys.Testing.Team.Delay.Save, team_delay_save);
        SetStorageValue(StorageKeys.Testing.Team.Delay.Delete, team_delay_delete);
        SetStorageValue(StorageKeys.Testing.Team.Unknown.Users, team_unknown_user);
        SetStorageValue(StorageKeys.Testing.Team.Unknown.SuperUsers, team_unknown_superUser);
        SetStorageValue(StorageKeys.Testing.Team.Duplicate.Users, team_duplicate_user);
        SetStorageValue(StorageKeys.Testing.Team.Duplicate.SuperUsers, team_duplicate_superUser);

        SetStorageValue(StorageKeys.Testing.WorkflowConfiguration.NumberOfWorkflowConfigurations, workflowConfiguration_numberOf);
        SetStorageValue(StorageKeys.Testing.WorkflowConfiguration.Delay.Get, workflowConfiguration_delay_get);
        SetStorageValue(StorageKeys.Testing.WorkflowConfiguration.Delay.Save, workflowConfiguration_delay_save);
        SetStorageValue(StorageKeys.Testing.WorkflowConfiguration.Delay.Delete, workflowConfiguration_delay_delete);

        var sut = new UserPreferences(NullLogger<UserPreferences>.Instance, ProtectedLocalStorage);

        // Act
        await sut.InitializeAsync();

        // Assert
        Assert.True(sut.IsInitialized, "");
        Assert.Equal(mimerEnvironment, sut.MimerEnvironment);
        Assert.Equal(removeInvalidDataAutomatically, sut.RemoveInvalidDataAutomatically);
        Assert.Equal(user_numberOf, sut.Users_NumberOf);
        Assert.Equal(user_delay_get, sut.Users_Delay_Get);
        Assert.Equal(user_delay_save, sut.Users_Delay_Save);
        Assert.Equal(user_delay_delete, sut.Users_Delay_Delete);
        Assert.Equal(user_duplicate_teamMemberships, sut.Users_Duplicate_TeamMembership);
        Assert.Equal(user_unknown_teamMemberships, sut.Users_Unknown_TeamMembership);
        Assert.Equal(team_numberOf, sut.Teams_NumberOf);
        Assert.Equal(team_delay_get, sut.Teams_Delay_Get);
        Assert.Equal(team_delay_save, sut.Teams_Delay_Save);
        Assert.Equal(team_delay_delete, sut.Teams_Delay_Delete);
        Assert.Equal(team_unknown_user, sut.Teams_Unknown_User);
        Assert.Equal(team_unknown_superUser, sut.Teams_Unknown_SuperUser);
        Assert.Equal(team_duplicate_user, sut.Teams_Duplicate_User);
        Assert.Equal(team_duplicate_superUser, sut.Teams_Duplicate_SuperUser);
        Assert.Equal(workflowConfiguration_numberOf, sut.WorkflowConfigurations_NumberOf);
        Assert.Equal(workflowConfiguration_delay_get, sut.WorkflowConfigurations_Delay_Get);
        Assert.Equal(workflowConfiguration_delay_save, sut.WorkflowConfigurations_Delay_Save);
        Assert.Equal(workflowConfiguration_delay_delete, sut.WorkflowConfigurations_Delay_Delete);
    }

    [Fact]
    public async Task UserPreferences_InitializeAsync_ThrowsExceptionWhenPropertyDoesntExist()
    {
        // Arrange
        var userPreferences = new UserPreferences(NullLogger<UserPreferences>.Instance, ProtectedLocalStorage);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => userPreferences.SetAsync("NonExistentProperty", "SomeValue"));
    }

    [Fact]
    public async Task UserPreferences_InitializeAsync_DoesNotCallLocalStorage_WhenAlreadyInitialized()
    {
        // Arrange
        var userPreferences = new UserPreferences(NullLogger<UserPreferences>.Instance, ProtectedLocalStorage);
        SetStorageValue(StorageKeys.MimerEnvironment, MimerEnvironment.SB1);

        // Act/Assert
        Assert.False(userPreferences.IsInitialized);
        await userPreferences.InitializeAsync();
        var initialCallCount = DataProtectorMock.Invocations
            .Count(m => m.Method.Name == nameof(IDataProtector.Unprotect));
        Assert.True(userPreferences.IsInitialized);
        await userPreferences.InitializeAsync();
        var afterCount = DataProtectorMock.Invocations
            .Count(m => m.Method.Name == nameof(IDataProtector.Unprotect));
        Assert.Equal(initialCallCount, afterCount);
    }
}
