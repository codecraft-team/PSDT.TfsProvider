# PSDT.TfsProvider

The PSDT.TfsProvider module enables PowerShell users to easily interact with Team Foundation Server by using a PSDrive. For supported cmdlets please read the documentation below.

## Development Environment

1. Visual Studio 2015 with .NET Framework 4.6.1 or higher.
1. Running scripts must be enabled on the system. Open the Package Manager Console and execute:

   ```powershell
   Set-ExecutionPolicy RemoteSigned
   ```
1. An access token is required to connect to Team Foundation Server services. You can create one by navigating to the *Personal access tokens* page. The *Personal access tokens* page can be reached through the web interface, by clicking on your account and selecting the *Security* option from the popup list.
1. Set your debugging option preferences for the project TfsProvider, and use the project as startup project in debugging session. Example configuration (open the project properties, activate the Debug tab page):

    Start external program:
    ```string
    c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe
    ```

    Arguments:
    ```powershell
    -noexit -file "R:\Source\PSDT.TfsProvider\Source\TfsProvider\InitializeProvider.ps1" "https://youraccount.visualstudio.com" "[enter your access token (see previous point)]"
    ```

## Supported Cmdlets

### Initialize-TfsPSDrive

Initializes a new TeamFoundationServer PSDrive by using the TfsCmdletProvider PSProvider.

### Get-ChildItem

Executing ```Get-ChildItem``` in the PSDrive root will list the available project collections on the Team Foundaton Server.

```powershell
[https://youraccount.visualstudio.com] PS TFS:\> Get-ChildItem

Name        Id                                   Url                                                                                            
----        --                                   ---                                                                                            
youraccount 7760eb13-5232-424c-8b86-0de9fce8b464 https://youraccount.visualstudio.com
```

Executing ```Get-ChildItem``` under a specific project collection will list the available projects within the current project collection.

```powershell
[https://youraccount.visualstudio.com] PS TFS:\> Set-Location youraccount
[https://youraccount.visualstudio.com] PS TFS:\youraccount> Get-ChildItem

Name                 Id                                 Url                                                                                  
----                 --                                 ---                                                                                  
ContosoEnterprise    9e2ecd2c-8e8f-1000-9685-00000001   https://youraccount.visualstudio.com/_apis/projects/9e2ecd2c-1000-4ed6-9685-00000001
Playground           7fca4b34-b9a1-1000-831d-00000001   https://youraccount.visualstudio.com/_apis/projects/7fca4b34-1000-4111-831d-00000001
DynamicsCrmRAV       c380c682-d664-1000-bcbb-00000001   https://youraccount.visualstudio.com/_apis/projects/c380c682-1000-4235-bcbb-00000001
WebControls          21a00171-b6f8-1000-87f9-00000001   https://youraccount.visualstudio.com/_apis/projects/21a00171-1000-49f0-87f9-00000001
MNIS                 c50b0b10-2b8e-1000-9c03-00000001   https://youraccount.visualstudio.com/_apis/projects/c50b0b10-1000-4603-9c03-00000001
DockiOS              2f5d4fe4-e58d-1000-b138-00000001   https://youraccount.visualstudio.com/_apis/projects/2f5d4fe4-1000-4ae2-b138-00000001
CaseStudies          693dc83b-3f20-1000-86cb-00000001   https://youraccount.visualstudio.com/_apis/projects/693dc83b-1000-4202-86cb-00000001
```

Executing ```Get-ChildItem``` under a specific project will list the further navigation options within the current project. The builds node has builds children, the BuildDefintions node will return the build definitions as children.

```powershell
[https://youraccount.visualstudio.com] PS TFS:\youraccount> Set-Location Playground
[https://youraccount.visualstudio.com] PS TFS:\youraccount\Playground> Get-ChildItem

Name             Project Name  Project Url
----             ------------  -----------
Builds           Playground    https://youraccount.visualstudio.com/_apis/projects/7fca4b34-1000-4111-831d-00000001
BuildDefinitions Playground    https://youraccount.visualstudio.com/_apis/projects/7fca4b34-1000-4111-831d-00000001
```
