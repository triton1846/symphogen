## Pagestatus: _Draft_
## Introduction
The testing infrastructure supports both standard unit tests and UI-dependent tests that require a UI context to execute. 
The testing framework is built on Microsoft's Visual Studio Test Tools, with special accommodations for WinUI 3 application testing.

This document explains how to write and run tests in the WinUI application, with particular focus on UI testing using the UITestMethod attribute and the testing infrastructure we've established.

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
    serviceSetup.GetRequiredService<IService1>(),
    serviceSetup.GetRequiredService<IService2>());
```

### Key Features
	- ConfigureDefaultServices() - Sets up common services with reasonable defaults
	- WithSingleton<T>() - Registers singleton services with custom configurations
	- WithTransient<T>() - Registers transient services
	- GetRequiredService<T>() - Retrieves configured services
	- GetMock<T>() - Retrieves consistent mock objects for a type

## Section 3: `UITestMethod` and Windows UI Testing
The `UITestMethod` attribute is used to mark tests that require a UI context. 
These tests are executed in a Windows environment and can interact with the UI elements of the application.

The `UITestMethod` attribute comes from the Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer namespace and is essential for testing components that require UI thread access:
```csharp
[UITestMethod, TestCategory(TestCategory.RequiresSelfhostedWindowsAgent)]
public void YourTest_TestScenario_ExpectedResult()
{
    // Test code that requires UI context
}
```
The `TestCategory.RequiresSelfhostedWindowsAgent` attribute is used to indicate that the test requires a self-hosted Windows agent to run.

> [!CAUTION]
> Defining tests with async Task will result in:
<br>***System.NotSupportedException: async TestMethod with UITestMethodAttribute are not supported. Either remove async or use TestMethodAttribute.***
<br>Use `.Wait()` to call async methods in the test method.

### When to Use UITestMethod
Use UITestMethod when testing components that:
- Use dispatcher operations
- Modify or interact with UI elements
- Rely on UI thread affinity
- Use WinUI-specific APIs

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
2. Follow the Arrange-Act-Assert pattern
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
    - Use descriptive assertion messages: Assert.AreEqual(expected, actual, "Message explaining what failed")

## Related Articles
- [Related Page 1](link)
- [Related Page 2](link)

## Change Log
- **Date:** Description of changes made.
