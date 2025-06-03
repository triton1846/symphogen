## Pagestatus: _Draft_
## Introduction
The testing infrastructure supports both standard unit tests and UI-dependent tests that require a UI context to execute. 
The testing framework is built on Microsoft's `Visual Studio Test Tools`, with special accommodations for `WinUI 3` application testing.

This document explains how to write and run tests in the `WinUI` application, with particular focus on UI testing using the `UITestMethod` attribute, `ConditionalUITestAttribute`, and the testing infrastructure we've established.

## Table of Contents
- [Section 1: Testing Types Overview](#section-1%3A-testing-types-overview)
- [Section 2: Test Setup with TestServiceSetup](#section-2%3A-test-setup-with-testservicesetup)
- [Section 3: UI Testing Attributes](#section-3%3A-ui-testing-attributes)
- [Section 4: Running Tests](#section-4%3A-running-tests)
- [Related Articles](#related-articles)
- [Change Log](#change-log)

## Section 1: Testing Types Overview
The application uses two primary types of tests:
1.	Standard Unit Tests - Test business logic and services without UI dependencies
	- Use the `[TestMethod]` attribute
	- Can run on any build agent
2.	UI-Dependent Tests - Test components that interact with the UI framework
	- Use the `[ConditionalUITest]` attribute (preferred) 
	- Require a Windows environment with UI capabilities
	- Always marked with `TestCategory.RequiresDispatcherQueue`
	- (`[UITestMethod]` attribute exists but is used internally by `ConditionalUITest`)

## Section 2: Test Setup with TestServiceSetup
The `TestServiceSetup` class provides a fluent API for configuring test dependencies and creating a consistent testing environment:
```csharp
// Example: Setting up services for testing a ViewModel with mock dependencies
var serviceSetup = new TestServiceSetup()
    .ConfigureDefaultServices()
    .WithSingleton<IWorkflowService>(workflowService.Object)
    .WithSingleton<IUserService>(userService.Object);
serviceSetup.BuildServiceProvider();

var sut = new YourViewModel(
    serviceSetup.GetRequiredService<IWorkflowService>(),
    serviceSetup.GetRequiredService<IUserService>());
```

### Key Features
- `ConfigureDefaultServices()` - Sets up common services with reasonable defaults
- `WithSingleton<T>()` - Registers singleton services with custom configurations, overriding defaults
- `WithTransient<T>()` - Registers transient services, overriding defaults
- `GetRequiredService<T>()` - Retrieves configured services
- `GetMock<T>()` - Retrieves or creates consistent mock objects that can be configured and verified

### Detailed Features
The `TestServiceSetup` class offers several additional features:

1. **Multiple Registration Methods**:
   - `WithSingleton<T>(Action<Mock<T>>?)` - Creates a mock with custom setup actions
   - `WithSingleton<TService, TImplementation>()` - Registers concrete implementations
   - `WithSingleton<TService>(TService instance)` - Registers specific instances
   - Similar methods for transient registrations

2. **Default Service Configuration**:
   - Pre-configures common services like `IModelStateManager`, `IUserService`, etc.
   - Sets up reasonable default mock behaviors
   - Can be overridden by custom configurations

3. **Service Provider Initialization**:
   - `BuildServiceProvider()` - Creates a provider that can be used throughout tests
   - Initializes the `ServiceProviderFactory` for dependency resolution

4. **Mock Persistence**:
   - Mocks are stored internally and reused when requested multiple times
   - The same mock instance is returned each time `GetMock<T>()` is called for a type
   - Ensures consistent verification and setup across test methods

## Section 3: UI Testing Attributes

### ConditionalUITest Attribute
The `ConditionalUITestAttribute` is the preferred way to mark tests that require UI context. It provides smart handling of testing environments:
```csharp
[ConditionalUITest, TestCategory(TestCategories.RequiresDispatcherQueue)]
public void YourUITest_TestScenario_ExpectedResult()
{
    // Test code that requires UI context
}
```

This attribute makes UI tests more robust by:
- Running the test normally in environments with a UI dispatcher
- Marking tests as "Inconclusive" in CI environments without UI capability
- Providing clear diagnostics when tests are skipped due to environment limitations

> [!IMPORTANT]
> All tests using the `ConditionalUITest` attribute **must** be marked with the `TestCategory.RequiresDispatcherQueue` category.

### Handling Async Operations in UI Tests

> [!CAUTION]
> UI tests do not support async methods. Attempting to define tests with async Task will result in:
<br>***System.NotSupportedException: async TestMethod with UITestMethodAttribute are not supported. Either remove async or use TestMethodAttribute.***
<br>Instead, use one of these approaches to call async methods:
> 
> Option 1: Use GetAwaiter().GetResult() (preferred) sut.LoadDataAsync("workflowId").GetAwaiter().GetResult();
> 
> Option 2: Use Wait() sut.LoadDataAsync("workflowId").Wait();
<br>Prefer `.GetAwaiter().GetResult()` as it unwraps exceptions, making debugging easier.

### Behind the Scenes: UITestMethod
The `ConditionalUITest` attribute uses `UITestMethod` internally to run tests. While you should primarily use `ConditionalUITest` in your tests, it's helpful to understand that:

- `UITestMethod` comes from the Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer namespace
- It requires a UI dispatcher to function, provided by the UnitTestApp initialization
- In CI environments without a UI, `UITestMethod` throws exceptions that `ConditionalUITest` handles gracefully

### Test Infrastructure
The UnitTestApp class initializes the testing environment for UI tests:
```csharp
protected override void OnLaunched(LaunchActivatedEventArgs args)
{
    Microsoft.VisualStudio.TestPlatform.TestExecutor.UnitTestClient.CreateDefaultUI();
    m_window = new UnitTestAppWindow();
    m_window.Activate();
    UITestMethodAttribute.DispatcherQueue = m_window.DispatcherQueue;
    Microsoft.VisualStudio.TestPlatform.TestExecutor.UnitTestClient.Run(Environment.CommandLine);
}
```

This initialization ensures that UI tests have access to the UI thread through the DispatcherQueue.

### When to Use UI Testing
Use `ConditionalUITest` when testing components that:
- Use dispatcher operations
- Modify or interact with UI elements
- Rely on UI thread affinity
- Use `WinUI`-specific APIs

For all other cases, prefer regular `TestMethod` tests as they are simpler and run in all environments.

### Writing Effective Tests
1. Use descriptive test names
    - Format: MethodUnderTest_Scenario_ExpectedBehavior
    - Example: LoadDataAsync_ShouldLoadData_WhenWorkflowIdIsSet
2. Follow the [Arrange-Act-Assert pattern](https://learn.microsoft.com/en-us/visualstudio/test/unit-test-basics?view=vs-2022#write-your-tests):
    ```csharp
    // Arrange
    var fixture = new Fixture();
    // Setup test dependencies
   
    // Act
    sut.YourMethodToTest();
   
    // Assert
    Assert.AreEqual(expected, actual);
    ```
3. Use TestServiceSetup consistently
    - Leverage ConfigureDefaultServices() for common dependencies
    - Customize only the services relevant to your test
4. Keep tests focused
    - Test one behavior per test method
    - Use descriptive assertion messages: 
        ```csharp
        Assert.AreEqual(expected, actual, "Message explaining what failed")
        ```

## Section 4: Running Tests
### Running Tests on a Local Developer Machine

To run tests locally, use Visual Studio's built-in tools:

1. **Open the solution in Visual Studio.**
2. **Build the solution** to ensure all dependencies are up to date.
3. **Open the Test Explorer** (__Test > Test Explorer__) to view available tests.
4. **Run or debug tests** by selecting them in Test Explorer and clicking __Run__ or __Debug__.
   - You can filter tests by category using the filter dropdown in Test Explorer
   - UI-dependent tests marked with `[ConditionalUITest]` require a Windows environment with UI capabilities
5. **Review test results** in the Test Explorer window.

You can also run tests from the command line using `dotnet test` or `vstest.console.exe`.

### Running Tests in Azure DevOps Pipeline

Tests are automatically executed as part of the `Azure DevOps build pipeline`. The pipeline is configured to:

- **Discover and run all tests** during the build process
- **Handle UI-dependent tests appropriately**:
  - Standard tests run normally in all environments
  - Tests with `[ConditionalUITest]` attribute will run in UI-enabled environments
  - Tests with `[ConditionalUITest]` will be marked as "Inconclusive" (skipped) in environments without UI capabilities
- **Report test results** to the pipeline summary

#### Configuring the Build for UI Tests

For UI tests to run properly in Azure DevOps pipelines with UI capabilities, set the MSBuild property `/p:EnableMsixToolingForTests=true` when building the test project:
```cmd
dotnet build -p:EnableMsixToolingForTests=true
```

This enables the MSIX tooling required for UI tests to properly initialize the WinUI framework.

> [!NOTE]
> Full UI testing requires a self-hosted Windows agent with the necessary UI capabilities. Tests using `ConditionalUITest` will gracefully handle being run in headless environments by marking themselves as "Inconclusive" rather than failing.

### Troubleshooting Tests

If you encounter issues with tests:

1. **UI Tests Failing with Dispatcher Errors**:
   - Ensure the test is marked with `[ConditionalUITest]` and `[TestCategory(TestCategories.RequiresDispatcherQueue)]`
   - Check that async operations use `.GetAwaiter().GetResult()` rather than `async/await`

2. **Tests Failing in Pipeline but Passing Locally**:
   - UI-dependent code may be running in a headless environment
   - Consider using `ConditionalUITest` to make tests more resilient to different environments

3. **Test Explorer Not Showing All Tests**:
   - Rebuild the solution
   - Use __Test > Run All Tests__ to discover tests not yet shown

Test results are published to the `Azure DevOps` build summary, where you can review passed and failed tests. If any test fails, the build is marked as failed and will not proceed to subsequent stages.

## Related Articles
- [Testing the Mimer Client](https://symphogenteams.visualstudio.com/Development%20and%20Data%20Engineering/_wiki/wikis/Development-and-Data-Engineering.wiki/418/Testing-the-Mimer-Client)

## Change Log
- **2025-05-20:** Initial version. Covers standard unit tests and UI-dependent tests using `UITestMethod`. Describes test setup with `TestServiceSetup`, usage of `TestCategory.RequiresSelfhostedWindowsAgent`, and best practices for writing and running tests in `WinUI 3` applications.
- **2025-06-15:** Added details on `ConditionalUITest` attribute, async handling in UI tests, and troubleshooting tips for common test issues. Updated examples for clarity and consistency.
