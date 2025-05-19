## Pagestatus: _Draft_

## Introduction
This document outlines the testing strategy and quality assurance processes for the Mimer ecosystem. 
The testing infrastructure supports both standard unit tests and specialized UI-dependent tests designed for WinUI 3 applications. 
Built on Microsoft's Visual Studio Test Tools, the framework provides robust capabilities for validating business logic, services, and user interface components through automated and manual testing approaches. 
This documentation covers test methodologies, infrastructure setup, execution practices, and integration with Azure DevOps for continuous quality assurance throughout the development lifecycle.

## Table of Contents
- [Section 1: Overview](#section-1-overview)
- [Section 2: Standard Unit Tests](#section-2-standard-unit-tests)
- [Section 3](#section-3)
- [Section 4: Manual Tests](#section-4-manual-tests)
- [Related Articles](#related-articles)
- [Change Log](#change-log)

## Section 1: Overview
The application uses the following types of tests:
1. **Standard Unit Tests**: These tests validate business logic and services. 
   For the Mimer Client unit tests are divided into two categories:
	- Those marked with the `[TestMethod]` attribute => can run on any build agent.
	- Those marked with the `[UITestMethod]` attribute => require a Windows environment with UI capabilities.
2. **Manual Tests**: These tests are conducted by QA teams to validate the application against user stories and acceptance criteria. 
   They are not automated and are typically performed in a controlled environment to ensure the application meets functional and non-functional requirements.

## Section 2: Standard Unit Tests
The standard unit tests are described in [TBD](https://insert-link-here).

## Section 3: UI-Dependent Tests (TODO: Does this section belong here?)

## Section 4: Manual Tests
Manual tests are run by developers or testers and are not part of the CI/CD pipeline. 
Automated tests are faster and more reliable than manual tests, but they require more setup and maintenance. 
Manual tests are easier to write and maintain, but they are slower and less reliable than automated tests.
We've also created a set of manual tests that are run by testers.
All tests, or a subset of them, should be run when a new version of the client is released.
The test plan(s) for Mimer Client are located in Azure DevOps [Test Plans](https://symphogenteams.visualstudio.com/Development%20and%20Data%20Engineering/_testManagement/all).

## Related Articles
- [Related Page 1](link) TBD link to page where unit tests are described
- [Related Page 2](link) TBD link to test plans in Azure DevOps?

## Change Log
- **Date:** Description of changes made.