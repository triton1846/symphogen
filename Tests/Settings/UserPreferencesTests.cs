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
                42, 24, true, true, true, true, true, true],
            [MimerEnvironment.QA, true,
                TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-2), TimeSpan.FromMilliseconds(-3),
                TimeSpan.FromMilliseconds(-4), TimeSpan.FromMilliseconds(-5), TimeSpan.FromMilliseconds(-6),
                TimeSpan.FromMilliseconds(-7), TimeSpan.FromMilliseconds(-8), TimeSpan.FromMilliseconds(-9),
                -42, -24, true, true, true, true, true, true],
            [MimerEnvironment.QA, true,
                TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero,
                TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero,
                TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero,
                0, 0, true, true, true, true, true, true],
            [MimerEnvironment.QA, false,
                TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(2), TimeSpan.FromMilliseconds(3),
                TimeSpan.FromMilliseconds(4), TimeSpan.FromMilliseconds(5), TimeSpan.FromMilliseconds(6),
                TimeSpan.FromMilliseconds(7), TimeSpan.FromMilliseconds(8), TimeSpan.FromMilliseconds(9),
                42, 24, false, false, false, false, false, false],
            [MimerEnvironment.QA, true,
                TimeSpan.MaxValue, TimeSpan.MaxValue, TimeSpan.MaxValue,
                TimeSpan.MaxValue, TimeSpan.MaxValue, TimeSpan.MaxValue,
                TimeSpan.MaxValue, TimeSpan.MaxValue, TimeSpan.MaxValue,
                42, 24, true, true, true, true, true, true],
            [MimerEnvironment.SB1, true, 
                TimeSpan.MinValue, TimeSpan.MinValue, TimeSpan.MinValue, 
                TimeSpan.MinValue, TimeSpan.MinValue, TimeSpan.MinValue,
                TimeSpan.MinValue, TimeSpan.MinValue, TimeSpan.MinValue,
                42, 24, true, true, true, true, true, true],
            [MimerEnvironment.SB1, true, 
                TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-2), TimeSpan.FromMilliseconds(-3), 
                TimeSpan.FromMilliseconds(-4), TimeSpan.FromMilliseconds(-5), TimeSpan.FromMilliseconds(-6),
                TimeSpan.FromMilliseconds(-7), TimeSpan.FromMilliseconds(-8), TimeSpan.FromMilliseconds(-9),
                -42, -24, true, true, true, true, true, true],
            [MimerEnvironment.SB1, true, 
                TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, 
                TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero,
                TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero,
                0, 0, true, true, true, true, true, true],
            [MimerEnvironment.SB1, false, 
                TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(2), TimeSpan.FromMilliseconds(3), 
                TimeSpan.FromMilliseconds(4), TimeSpan.FromMilliseconds(5), TimeSpan.FromMilliseconds(6),
                TimeSpan.FromMilliseconds(7), TimeSpan.FromMilliseconds(8), TimeSpan.FromMilliseconds(9),
                42, 24, false, false, false, false, false, false],
            [MimerEnvironment.SB1, true, 
                TimeSpan.MaxValue, TimeSpan.MaxValue, TimeSpan.MaxValue, 
                TimeSpan.MaxValue, TimeSpan.MaxValue, TimeSpan.MaxValue,
                TimeSpan.MaxValue, TimeSpan.MaxValue, TimeSpan.MaxValue,
                42, 24, true, true, true, true, true, true],
        ];

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(TestData_UserPreferences_LoadsStoredValues_WhenInitialized))]
    public async Task UserPreferences_LoadsStoredValues_WhenInitialized(
        MimerEnvironment mimerEnvironment,
        bool removeInvalidDataAutomatically,
        TimeSpan getUsersDelay,
        TimeSpan saveUserDelay,
        TimeSpan deleteUserDelay,
        TimeSpan getTeamsDelay,
        TimeSpan saveTeamDelay,
        TimeSpan deleteTeamsDelay,
        TimeSpan getWorkflowConfigurationsDelay,
        TimeSpan saveWorkflowConfigurationDelay,
        TimeSpan deleteWorkflowConfigurationDelay,
        int numberOfUsers,
        int numberOfTeams,
        bool createUnknownUsersAsTeamMembers,
        bool createDuplicateTeamMembershipsForUsers,
        bool createUnknownSuperUsersAsTeamMembers,
        bool createDuplicateTeamMembershipsForSuperUsers,
        bool createUnknownTeams,
        bool createDuplicateTeams)
    {
        // Arrange
        SetStorageValue(StorageKeys.MimerEnvironment, mimerEnvironment);
        SetStorageValue(StorageKeys.RemoveInvalidDataAutomatically, removeInvalidDataAutomatically);
        SetStorageValue(StorageKeys.Testing.User.Delay.Get, getUsersDelay);
        SetStorageValue(StorageKeys.Testing.User.Delay.Save, saveUserDelay);
        SetStorageValue(StorageKeys.Testing.User.Delay.Delete, deleteUserDelay);
        SetStorageValue(StorageKeys.Testing.Team.Delay.Get, getTeamsDelay);
        SetStorageValue(StorageKeys.Testing.Team.Delay.Save, saveTeamDelay);
        SetStorageValue(StorageKeys.Testing.Team.Delay.Delete, deleteTeamsDelay);
        SetStorageValue(StorageKeys.Testing.WorkflowConfiguration.Delay.Get, getWorkflowConfigurationsDelay);
        SetStorageValue(StorageKeys.Testing.WorkflowConfiguration.Delay.Save, saveWorkflowConfigurationDelay);
        SetStorageValue(StorageKeys.Testing.WorkflowConfiguration.Delay.Delete, deleteWorkflowConfigurationDelay);
        SetStorageValue(StorageKeys.Testing.User.NumberOfUsers, numberOfUsers);
        SetStorageValue(StorageKeys.Testing.Team.NumberOfTeams, numberOfTeams);
        SetStorageValue(StorageKeys.Testing.User.Unknown.Users, createUnknownUsersAsTeamMembers);
        SetStorageValue(StorageKeys.Testing.User.Duplicate.Users, createDuplicateTeamMembershipsForUsers);
        SetStorageValue(StorageKeys.Testing.User.Unknown.SuperUsers, createUnknownSuperUsersAsTeamMembers);
        SetStorageValue(StorageKeys.Testing.User.Duplicate.SuperUsers, createDuplicateTeamMembershipsForSuperUsers);
        SetStorageValue(StorageKeys.Testing.Team.Unknown.Teams, createUnknownTeams);
        SetStorageValue(StorageKeys.Testing.Team.Duplicate.Teams, createDuplicateTeams);

        var sut = new UserPreferences(NullLogger<UserPreferences>.Instance, ProtectedLocalStorage);

        // Act
        await sut.InitializeAsync();

        // Assert
        Assert.True(sut.IsInitialized);
        Assert.Equal(mimerEnvironment, sut.MimerEnvironment);
        Assert.Equal(removeInvalidDataAutomatically, sut.RemoveInvalidDataAutomatically);
        Assert.Equal(getUsersDelay, sut.GetUsersDelay);
        Assert.Equal(saveUserDelay, sut.SaveUserDelay);
        Assert.Equal(deleteUserDelay, sut.DeleteUserDelay);
        Assert.Equal(getTeamsDelay, sut.GetTeamsDelay);
        Assert.Equal(saveTeamDelay, sut.SaveTeamDelay);
        Assert.Equal(deleteTeamsDelay, sut.DeleteTeamDelay);
        Assert.Equal(getWorkflowConfigurationsDelay, sut.GetWorkflowConfigurationsDelay);
        Assert.Equal(saveWorkflowConfigurationDelay, sut.SaveWorkflowConfigurationDelay);
        Assert.Equal(deleteWorkflowConfigurationDelay, sut.DeleteWorkflowConfigurationDelay);
        Assert.Equal(numberOfUsers, sut.TestDataNumberOfUsers);
        Assert.Equal(numberOfTeams, sut.TestDataNumberOfTeams);
        Assert.Equal(createUnknownUsersAsTeamMembers, sut.TestDataCreateUnknownUsersAsTeamMembers);
        Assert.Equal(createDuplicateTeamMembershipsForUsers, sut.TestDataCreateDuplicateTeamMembershipsForUsers);
        Assert.Equal(createUnknownSuperUsersAsTeamMembers, sut.TestDataCreateUnknownSuperUsersAsTeamMembers);
        Assert.Equal(createDuplicateTeamMembershipsForSuperUsers, sut.TestDataCreateDuplicateTeamMembershipsForSuperUsers);
        Assert.Equal(createUnknownTeams, sut.TestDataCreateUnknownTeams);
        Assert.Equal(createDuplicateTeams, sut.TestDataCreateDuplicateTeams);
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
