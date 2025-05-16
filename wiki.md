read https://symphogenteams.visualstudio.com/Development%20and%20Data%20Engineering/_wiki/wikis/Development-and-Data-Engineering.wiki/323/Client-Architecture (readme in repo used?)

use [Wiki doc template](https://symphogenteams.visualstudio.com/Development%20and%20Data%20Engineering/_wiki/wikis/Development-and-Data-Engineering.wiki/330/Wiki-Template) for new docs!

[new documentation](https://symphogenteams.visualstudio.com/Development%20and%20Data%20Engineering/_wiki/wikis/Development-and-Data-Engineering.wiki/311/Testing-and-Quality-Assurance) for:
- test
  - unit testing, ui
    - writing unit tests (UITestMethod with link to Microsoft docs)
    - running unit tests in azure pipeline
  - dev box
    - client installed on devbox with link to test plan in azure devops
- change request process

# Manual testing (draft)
## Using DevBox for Testing a New Client Version
### Overview
DevBox provides an isolated, consistent environment for testing the new client version according to the test plan defined in Azure DevOps. This document guides testers through the process of setting up a DevBox instance and executing the test plan.
### Prerequisites
•	Access to Azure DevOps with appropriate permissions to view test plans
•	DevBox access credentials
•	Basic understanding of the client application functionality
### Setting Up Your DevBox Environment
1.	Access the DevBox Portal
•	Navigate to the DevBox management portal at https://devbox.microsoft.com (or your organization's specific URL)
•	Sign in with your organizational credentials
2.	Create a New DevBox Instance
•	Select "Create New DevBox"
•	Choose the "Client Testing" template from the available configurations
•	Name your DevBox following the convention: TEST-ClientVX.X-[YourInitials] (replace X.X with the client version number)
3.	Initial Configuration
•	Once your DevBox is ready, connect to it using Remote Desktop
•	Wait for the automatic setup to complete (this may take 5-10 minutes)
•	Verify that all required tools are installed:
•	The client application under test
•	Required database connections
•	Test utilities and monitoring tools
### Accessing the Test Plan
1.	Navigate to Azure DevOps Test Plans
•	Open Azure DevOps in your browser
•	Go to the project containing the test plan
•	Select "Test Plans" from the left navigation menu
2.	Locate the Current Test Plan
•	Open the test plan titled "Client vX.X Acceptance Testing"
•	Review the test plan overview and objectives
•	Familiarize yourself with the test cases and required outcomes
3.	Prepare Test Plan Execution
•	Click "Run" to start executing the test plan
•	Select the iteration/sprint you're testing against
•	Choose "Run with options" to customize your test run if needed
### Executing Test Cases
1.	Step-by-Step Execution
•	Work through each test case methodically
•	Document all observations in the Azure DevOps test runner
•	For each test step:
•	Perform the action exactly as described
•	Verify the expected result
•	Mark as Pass/Fail accordingly
•	Capture screenshots for failures or unexpected behaviors
2.	Test Evidence Collection
•	For failed tests, collect detailed evidence:
•	Screenshots showing the issue
•	Application logs (located at C:\ProgramData\ClientApp\logs\)
•	Error messages or exception details
•	Attach all evidence to the test case in Azure DevOps
3.	Regression Testing Focus
•	Pay special attention to:
•	Workflow approval processes
•	Data import functionality
•	Settings configuration
•	User permissions and access controls
•	Integration with external systems
### Reporting Issues
1.	Creating Bug Reports
•	For failed tests, create a bug directly from the test case
•	Include detailed reproduction steps
•	Link the bug to the relevant test case
•	Set severity according to impact guidelines
2.	Required Bug Information
•	Version of the client being tested
•	DevBox environment details
•	Complete reproduction steps
•	Expected vs. actual results
•	Attachments (screenshots, logs, etc.)
### Completing the Test Cycle
1.	Test Plan Summary
•	Once all test cases are executed, review the overall results
•	Document any patterns in failures or concerns
•	Highlight critical issues that should block release
2.	Clean Up
•	Save all important test artifacts to the shared test results repository
•	Complete the test plan execution in Azure DevOps
•	Either suspend your DevBox (if more testing is needed later) or delete it (if testing is complete)
### Additional Resources
•	link-to-internal-guide
•	link-to-test-strategy
•	Azure DevOps Test Plans Documentation
