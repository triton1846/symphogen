# Self-hosted agent

Dev Box, definition, project

https://devportal.microsoft.com/

https://learn.microsoft.com/en-us/azure/devops/pipelines/agents/windows-agent?view=azure-devops&tabs=IP-V4#confirm-the-user-has-permission

https://symphogenteams.visualstudio.com/Mimer/_settings/agentqueues?queueId=244&view=security









using microsoft devbox I want to spin up a new devbox after I've built my winui3 application in azure devops pipeline. when the devbox is running I want to install my winui3 client on the devbox making it possible for other users to connect to the devbox to test my application
ChatGPT said:

To achieve your goal of spinning up a Microsoft Dev Box, installing your WinUI 3 client on it after a successful Azure DevOps pipeline build, and enabling access for others to test, follow this streamlined workflow:
✅ Overview Steps

    Build WinUI 3 App in Azure DevOps Pipeline

    Provision a Dev Box (using Dev Center API or Azure CLI)

    Install the App on the Dev Box

    Allow Access for Testers

🔧 Step-by-Step Setup
1. Build WinUI 3 App in Azure DevOps Pipeline

Your pipeline should:

    Build the WinUI 3 app

    Package the app as .msix or .msixbundle

    Publish the build artifact for deployment

Example Azure DevOps YAML snippet:

steps:
- task: VSBuild@1
  inputs:
    solution: '**/*.sln'
    msbuildArgs: '/p:Configuration=Release'

- task: PublishBuildArtifacts@1
  inputs:
    PathtoPublish: '$(Build.ArtifactStagingDirectory)'
    ArtifactName: 'winui3-app'

2. Provision a New Dev Box (Automatically After Build)

Use the Microsoft Dev Box API or Azure CLI in a pipeline release or post-build step.
Option A: Using Azure CLI

First, authenticate:

az login
az account set --subscription <subscription-id>

Then create a Dev Box:

az devcenter dev dev-box create \
  --project-name <project-name> \
  --dev-center <dev-center-name> \
  --pool-name <dev-box-pool> \
  --user-id <aad-user-id>

    You can automate this with a bash or PowerShell step in the release pipeline.

3. Install the WinUI 3 App on the Dev Box

Once the Dev Box is provisioned and accessible (e.g., via RDP or remote scripts):
Option A: Use WinRM or Remote PowerShell

You can remotely copy and install the app:

Copy-Item -Path "\\buildserver\artifacts\winui3-app\MyApp.msixbundle" -Destination "C:\Temp"
Add-AppxPackage -Path "C:\Temp\MyApp.msixbundle"

Option B: Use Azure DevOps Release Pipeline to Deploy

Create a release pipeline that:

    Downloads the build artifact

    Uses remote PowerShell or a VM extension to install on the Dev Box

4. Enable Access for Testers
A. RDP Access

Ensure:

    Dev Box network rules allow RDP (port 3389)

    Add testers to the VM as users (or enable Just-in-Time access)

B. Web-based Access (if app exposes web services)

    Make sure the firewall and app allow external access

    Use Azure Bastion or Remote Desktop Web Client for browser access

🧠 Bonus: Automate with Logic Apps or GitHub Actions

For orchestration:

    Use an Azure Logic App or GitHub Action to:

        Trigger on pipeline success

        Call Dev Center API to create the Dev Box

        Deploy the app to the box

📌 Tips

    WinUI 3 apps need Windows App SDK Runtime installed. Ensure it's part of the Dev Box image or install it during provisioning.

    You can customize your Dev Box image to include the SDKs, runtimes, and tools needed for faster spin-up.

    To make this multi-user, configure remote desktop or provide shared testing credentials (while maintaining security).
