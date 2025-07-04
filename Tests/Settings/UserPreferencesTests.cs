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
            [MimerEnvironment.QA, true, TimeSpan.MinValue, TimeSpan.MinValue, TimeSpan.MinValue, 42, true, true, true, true, true, true],
            [MimerEnvironment.QA, true, TimeSpan.FromMilliseconds(-123), TimeSpan.FromMilliseconds(-456), TimeSpan.FromMilliseconds(-789), -42, true, true, true, true, true, true],
            [MimerEnvironment.QA, true, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, 0, true, true, true, true, true, true],
            [MimerEnvironment.QA, false, TimeSpan.FromMilliseconds(123), TimeSpan.FromMilliseconds(456), TimeSpan.FromMilliseconds(789), 42, false, false, false, false, false, false],
            [MimerEnvironment.QA, true, TimeSpan.MaxValue, TimeSpan.MaxValue, TimeSpan.MaxValue, 42, true, true, true, true, true, true],
            [MimerEnvironment.SB1, true, TimeSpan.MinValue, TimeSpan.MinValue, TimeSpan.MinValue, 42, true, true, true, true, true, true],
            [MimerEnvironment.SB1, true, TimeSpan.FromMilliseconds(-123), TimeSpan.FromMilliseconds(-456), TimeSpan.FromMilliseconds(-789), -42, true, true, true, true, true, true],
            [MimerEnvironment.SB1, true, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, 0, true, true, true, true, true, true],
            [MimerEnvironment.SB1, false, TimeSpan.FromMilliseconds(123), TimeSpan.FromMilliseconds(456), TimeSpan.FromMilliseconds(789), 42, false, false, false, false, false, false],
            [MimerEnvironment.SB1, true, TimeSpan.MaxValue, TimeSpan.MaxValue, TimeSpan.MaxValue, 42, true, true, true, true, true, true],
        ];

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(TestData_UserPreferences_LoadsStoredValues_WhenInitialized))]
    public async Task UserPreferences_LoadsStoredValues_WhenInitialized(
        MimerEnvironment mimerEnvironment,
        bool removeInvalidDataAutomatically,
        TimeSpan fetchUsersDelay,
        TimeSpan fetchTeamsDelay,
        TimeSpan fetchWorkflowConfigurationsDelay,
        int numberOfUsers,
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
        SetStorageValue(StorageKeys.FetchUsersDelay, fetchUsersDelay);
        SetStorageValue(StorageKeys.FetchTeamsDelay, fetchTeamsDelay);
        SetStorageValue(StorageKeys.FetchWorkflowConfigurationsDelay, fetchWorkflowConfigurationsDelay);
        SetStorageValue(StorageKeys.TestData.NumberOfUsers, numberOfUsers);
        SetStorageValue(StorageKeys.TestData.CreateUnknownUsersAsTeamMembers, createUnknownUsersAsTeamMembers);
        SetStorageValue(StorageKeys.TestData.CreateDuplicateTeamMembershipsForUsers, createDuplicateTeamMembershipsForUsers);
        SetStorageValue(StorageKeys.TestData.CreateUnknownSuperUsersAsTeamMembers, createUnknownSuperUsersAsTeamMembers);
        SetStorageValue(StorageKeys.TestData.CreateDuplicateTeamMembershipsForSuperUsers, createDuplicateTeamMembershipsForSuperUsers);
        SetStorageValue(StorageKeys.TestData.CreateUnknownTeams, createUnknownTeams);
        SetStorageValue(StorageKeys.TestData.CreateDuplicateTeams, createDuplicateTeams);

        var sut = new UserPreferences(NullLogger<UserPreferences>.Instance, ProtectedLocalStorage);

        // Act
        await sut.InitializeAsync();

        // Assert
        Assert.True(sut.IsInitialized);
        Assert.Equal(mimerEnvironment, sut.MimerEnvironment);
        Assert.Equal(removeInvalidDataAutomatically, sut.RemoveInvalidDataAutomatically);
        Assert.Equal(fetchUsersDelay, sut.FetchUsersDelay);
        Assert.Equal(fetchTeamsDelay, sut.FetchTeamsDelay);
        Assert.Equal(fetchWorkflowConfigurationsDelay, sut.FetchWorkflowConfigurationsDelay);
        Assert.Equal(numberOfUsers, sut.TestDataNumberOfUsers);
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
