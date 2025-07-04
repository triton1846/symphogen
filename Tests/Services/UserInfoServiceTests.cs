using BlazorWebAppSymphogen.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using System.Net;
using System.Security.Claims;

namespace Tests.Services;

public class UserInfoServiceTests
{
    [Fact]
    public async Task GetUserInfoAsync_ReturnsUserInfo_WhenUserExists()
    {
        // Arrange
        var fixture = new Fixture();
        var downstreamApiMock = new Mock<IDownstreamApi>();
        downstreamApiMock.Setup(x => x.CallApiForUserAsync("GraphApi", It.IsAny<Action<DownstreamApiOptions>?>(), It.IsAny<ClaimsPrincipal?>(), It.IsAny<HttpContent?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"displayName\": \"Test User\"}")
            });

        var memoryCacheMock = new Mock<IMemoryCache>();
        var cacheEntry = Mock.Of<ICacheEntry>();
        memoryCacheMock
            .Setup(m => m.CreateEntry(It.IsAny<object>()))
            .Returns(cacheEntry);

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var user = fixture.Create<ClaimsPrincipal>();
        user.AddIdentity(new ClaimsIdentity(
        [
            new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", "user-id-123")
        ]));
        httpContextAccessorMock.Setup(x => x.HttpContext!.User).Returns(user);
        var sut = new UserInfoService(NullLogger<UserInfoService>.Instance, downstreamApiMock.Object, memoryCacheMock.Object, httpContextAccessorMock.Object);

        // Act
        var displayName = await sut.GetDisplayNameAsync();

        // Assert
        memoryCacheMock.Verify(x => x.TryGetValue(It.IsAny<object>(), out It.Ref<object?>.IsAny), Times.Once);
    }

    [Fact]
    public async Task GetDisplayNameAsync_ReturnsCachedValue_WhenAvailable()
    {
        // Arrange
        var userId = "user-id-123";
        var cachedDisplayName = "Cached User";
        var cacheKey = $"DisplayName:{userId}";

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var user = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", userId)
        ]));
        httpContextAccessorMock.Setup(x => x.HttpContext!.User).Returns(user);

        var memoryCacheMock = new Mock<IMemoryCache>();
        object? returnValue = cachedDisplayName;
        memoryCacheMock
            .Setup(m => m.TryGetValue(cacheKey, out returnValue))
            .Returns(true);

        var downstreamApiMock = new Mock<IDownstreamApi>();
        var sut = new UserInfoService(NullLogger<UserInfoService>.Instance, downstreamApiMock.Object, memoryCacheMock.Object, httpContextAccessorMock.Object);

        // Act
        var result = await sut.GetDisplayNameAsync();

        // Assert
        Assert.Equal(cachedDisplayName, result);
        downstreamApiMock.Verify(
            x => x.CallApiForUserAsync(
                It.IsAny<string>(),
                It.IsAny<Action<DownstreamApiOptions>?>(),
                It.IsAny<ClaimsPrincipal?>(),
                It.IsAny<HttpContent?>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetDisplayNameAsync_ReturnsNull_WhenNoUserContext()
    {
        // Arrange
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext)null!);

        var memoryCacheMock = new Mock<IMemoryCache>();
        var downstreamApiMock = new Mock<IDownstreamApi>();
        var sut = new UserInfoService(NullLogger<UserInfoService>.Instance, downstreamApiMock.Object, memoryCacheMock.Object, httpContextAccessorMock.Object);

        // Act
        var result = await sut.GetDisplayNameAsync();

        // Assert
        Assert.Null(result);
        downstreamApiMock.Verify(
            x => x.CallApiForUserAsync(
                It.IsAny<string>(),
                It.IsAny<Action<DownstreamApiOptions>?>(),
                It.IsAny<ClaimsPrincipal?>(),
                It.IsAny<HttpContent?>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetDisplayNameAsync_ReturnsNull_WhenUserIdMissing()
    {
        // Arrange
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var userWithoutId = new ClaimsPrincipal(new ClaimsIdentity([]));
        httpContextAccessorMock.Setup(x => x.HttpContext!.User).Returns(userWithoutId);

        var memoryCacheMock = new Mock<IMemoryCache>();
        var downstreamApiMock = new Mock<IDownstreamApi>();
        var sut = new UserInfoService(NullLogger<UserInfoService>.Instance, downstreamApiMock.Object, memoryCacheMock.Object, httpContextAccessorMock.Object);

        // Act
        var result = await sut.GetDisplayNameAsync();

        // Assert
        Assert.Null(result);
        downstreamApiMock.Verify(
            x => x.CallApiForUserAsync(
                It.IsAny<string>(),
                It.IsAny<Action<DownstreamApiOptions>?>(),
                It.IsAny<ClaimsPrincipal?>(),
                It.IsAny<HttpContent?>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetDisplayNameAsync_ReturnsNull_WhenApiReturnsError()
    {
        // Arrange
        var userId = "user-id-123";
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var user = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", userId)
        ]));
        httpContextAccessorMock.Setup(x => x.HttpContext!.User).Returns(user);

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var downstreamApiMock = new Mock<IDownstreamApi>();
        downstreamApiMock.Setup(x => x.CallApiForUserAsync(
                "GraphApi",
                It.IsAny<Action<DownstreamApiOptions>?>(),
                It.IsAny<ClaimsPrincipal?>(),
                It.IsAny<HttpContent?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });

        var sut = new UserInfoService(NullLogger<UserInfoService>.Instance, downstreamApiMock.Object, memoryCache, httpContextAccessorMock.Object);

        // Act
        var result = await sut.GetDisplayNameAsync();

        // Assert
        Assert.Null(result);
        Assert.False(memoryCache.TryGetValue($"DisplayName:{userId}", out _));
    }

    [Fact]
    public async Task GetDisplayNameAsync_LogsError_WhenExceptionOccurs()
    {
        // Arrange
        var userId = "user-id-123";
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var user = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", userId)
        ]));
        httpContextAccessorMock.Setup(x => x.HttpContext!.User).Returns(user);

        var memoryCacheMock = new Mock<IMemoryCache>();
        object? returnValue = null;
        memoryCacheMock.Setup(m => m.TryGetValue(It.IsAny<object>(), out returnValue)).Returns(false);

        var downstreamApiMock = new Mock<IDownstreamApi>();
        downstreamApiMock.Setup(x => x.CallApiForUserAsync(
                "GraphApi",
                It.IsAny<Action<DownstreamApiOptions>?>(),
                It.IsAny<ClaimsPrincipal?>(),
                It.IsAny<HttpContent?>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("API error"));

        var loggerMock = new Mock<ILogger<UserInfoService>>();

        var sut = new UserInfoService(loggerMock.Object, downstreamApiMock.Object, memoryCacheMock.Object, httpContextAccessorMock.Object);

        // Act
        var result = await sut.GetDisplayNameAsync();

        // Assert
        Assert.Null(result);
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetDisplayNameAsync_PropagatesChallenge_WhenIdentityChallengeExceptionThrown()
    {
        // Arrange
        var userId = "user-id-123";
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var user = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", userId)
        ]));
        httpContextAccessorMock.Setup(x => x.HttpContext!.User).Returns(user);

        var memoryCacheMock = new Mock<IMemoryCache>();
        object? returnValue = null;
        memoryCacheMock.Setup(m => m.TryGetValue(It.IsAny<object>(), out returnValue)).Returns(false);

        var downstreamApiMock = new Mock<IDownstreamApi>();
        downstreamApiMock.Setup(x => x.CallApiForUserAsync(
                "GraphApi",
                It.IsAny<Action<DownstreamApiOptions>?>(),
                It.IsAny<ClaimsPrincipal?>(),
                It.IsAny<HttpContent?>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new MicrosoftIdentityWebChallengeUserException(new Microsoft.Identity.Client.MsalUiRequiredException("error code", "error message"), []));

        var sut = new UserInfoService(NullLogger<UserInfoService>.Instance, downstreamApiMock.Object, memoryCacheMock.Object, httpContextAccessorMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<MicrosoftIdentityWebChallengeUserException>(() => sut.GetDisplayNameAsync());
    }

    [Fact]
    public async Task GetDisplayNameAsync_CachesDisplayName_WhenApiReturnsValue()
    {
        // Arrange
        var userId = "user-id-123";
        var expectedDisplayName = "Test User";
        var cacheKey = $"DisplayName:{userId}";

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var user = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", userId)
        ]));
        httpContextAccessorMock.Setup(x => x.HttpContext!.User).Returns(user);

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var downstreamApiMock = new Mock<IDownstreamApi>();
        downstreamApiMock.Setup(x => x.CallApiForUserAsync(
                "GraphApi",
                It.IsAny<Action<DownstreamApiOptions>?>(),
                It.IsAny<ClaimsPrincipal?>(),
                It.IsAny<HttpContent?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent($"{{\"displayName\": \"{expectedDisplayName}\"}}")
            });

        var sut = new UserInfoService(NullLogger<UserInfoService>.Instance, downstreamApiMock.Object, memoryCache, httpContextAccessorMock.Object);

        // Act
        var result = await sut.GetDisplayNameAsync();

        // Assert
        Assert.Equal(expectedDisplayName, result);
        Assert.True(memoryCache.TryGetValue(cacheKey, out var cachedValue));
    }
}
