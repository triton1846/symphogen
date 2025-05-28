## Pagestatus: _Draft_
## Introduction
The testing infrastructure supports both standard unit tests and UI-dependent tests that require a UI context to execute. 
The testing framework is built on Microsoft's `Visual Studio Test Tools`, with special accommodations for `WinUI 3` application testing.

This document explains how to write and run tests in the `WinUI` application, with particular focus on UI testing using the `UITestMethod` attribute and the testing infrastructure we've established.

## Table of Contents
- [Section 1: Testing Types Overview](#section-1-testing-types-overview)
- [Section 2: Test Setup with TestServiceSetup](#section-2-test-setup-with-testservicesetup)
- [Section 3: UITestMethod and Windows UI Testing](#section-3-uitestmethod-and-windows-ui-testing)
- [Related Articles](#related-articles)
- [Change Log](#change-log)

## Section 1: Testing Types Overview
The application uses two primary types of tests:
1.	Standard Unit Tests - Test business logic and services without UI dependencies
	- Use the `[TestMethod]` attribute
	- Can run on any build agent
2.	UI-Dependent Tests - Test components that interact with the UI framework
	- Use the `[UITestMethod]` attribute
	- Require a Windows environment with UI capabilities
	- Always marked with `TestCategory.RequiresSelfhostedWindowsAgent`

## Section 2: Test Setup with TestServiceSetup
The `TestServiceSetup` class provides a fluent API for configuring test dependencies and creating a consistent testing environment:
```csharp
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
	- ConfigureDefaultServices() - Sets up common services with reasonable defaults
	- WithSingleton<T>() - Registers singleton services with custom configurations, overriding defaults
	- WithTransient<T>() - Registers transient services, overriding defaults
	- GetRequiredService<T>() - Retrieves configured services
	- GetMock<T>() - Retrieves consistent mock objects for a type

## Section 3: `UITestMethod` and Windows UI Testing
The `UITestMethod` attribute is used to mark tests that require a UI context. 
These tests are executed in a Windows environment and can interact with the UI elements of the application.

For more information on testing `WinUI` functionality, 
see the [official Microsoft documentation on WinUI 3 testing](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/testing/#how-do-i-test-winui-functionality-in-my-app).

The `UITestMethod` attribute comes from the *Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer* namespace and is essential for testing components that require UI thread access:
```csharp
[UITestMethod, TestCategory(TestCategory.RequiresSelfhostedWindowsAgent)]
public void YourTest_TestScenario_ExpectedResult()
{
    // Test code that requires UI context
}
```
The `TestCategory.RequiresSelfhostedWindowsAgent` attribute is used to indicate that the test requires a self-hosted Windows agent to run.
This is important for tests that interact with the UI or require a specific Windows environment.
Running the test in a headless environment will fail - e.g. in `Azure DevOps Pipelines`.

> [!CAUTION]
> Defining tests with async Task will result in:
<br>***System.NotSupportedException: async TestMethod with UITestMethodAttribute are not supported. Either remove async or use TestMethodAttribute.***
<br>Use `.Wait()` or `.GetAwaiter().GetResult()` to call async methods in the test method. 
This is required because `[UITestMethod]` does not support `async Task` test methods. 
Both approaches block the calling thread until the async operation completes. 
Prefer `.GetAwaiter().GetResult()` if you want exceptions to be unwrapped.

### When to Use `UITestMethod`
Use UITestMethod when testing components that:
- Use dispatcher operations
- Modify or interact with UI elements
- Rely on UI thread affinity
- Use `WinUI`-specific APIs

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
This initialization ensures that UITestMethod-attributed tests have access to the UI thread.

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
   - UI-dependent tests (`[UITestMethod]`) require a Windows environment with UI capabilities.
5. **Review test results** in the Test Explorer window.

You can also run tests from the command line using `dotnet test` or `vstest.console.exe`. Note that UI tests require a Windows environment with desktop interaction enabled.

### Running Tests in Azure DevOps Pipeline

Tests are automatically executed as part of the `Azure DevOps build pipeline`. The pipeline is configured to:

- **Discover and run all tests** (both standard unit tests and UI-dependent tests) during the build process.
- **Require a self-hosted Windows agent** for UI-dependent tests marked with `TestCategory.RequiresSelfhostedWindowsAgent`.
- **Fail the build if any test fails.** This ensures that only code passing all tests is considered for deployment or further integration.

Test results are published to the `Azure DevOps` build summary, where you can review passed and failed tests. If any test fails, the build is marked as failed and will not proceed to subsequent stages.

## Related Articles
- [Testing and Quality Assurance](https://symphogenteams.visualstudio.com/Development%20and%20Data%20Engineering/_wiki/wikis/Development-and-Data-Engineering.wiki/311/Testing-and-Quality-Assurance)

## Change Log
- **2025-05-20:** Initial version. Covers standard unit tests and UI-dependent tests using `UITestMethod`. Describes test setup with `TestServiceSetup`, usage of `TestCategory.RequiresSelfhostedWindowsAgent`, and best practices for writing and running tests in `WinUI 3` applications.
