# Solution Contract: Visual Studio Solution Architecture

## Solution File Specification (`LittleTalk.slnx`)

The solution file MUST conform to the modern XML-based `.slnx` format utilizing logical solution folders that match the GTB project organization:

```xml
<Solution>
  <Folder Name="/.Config/">
    <File Path="AGENTS.md" />
    <File Path="Config/appsettings.json" />
    <File Path="Config/custom-settings-dev.json" />
  </Folder>
  <Folder Name="/Common/">
    <Project Path="LittleTalk.Database/LittleTalk.Database.csproj" />
  </Folder>
  <Folder Name="/Domain/">
    <Project Path="LittleTalk.Domain/LittleTalk.Domain.csproj" />
    <Project Path="LittleTalk.Tests/LittleTalk.Tests.csproj" />
  </Folder>
  <Folder Name="/Shared/">
    <Project Path="LittleTalk.Models/LittleTalk.Models.csproj" />
    <Project Path="LittleTalk.Shared/LittleTalk.Shared.csproj" />
  </Folder>
  <Folder Name="/_Documents/">
    <File Path="README.md" />
    <File Path="docs/architecture.md" />
  </Folder>
  <Project Path="LittleTalk.App/LittleTalk.App.csproj" />
  <Project Path="LittleTalk.BackendApi/LittleTalk.BackendApi.csproj" />
</Solution>
```

---

## Solution Launch Specification (`LittleTalk.slnLaunch`)

Defines multi-project startup profile for concurrent execution of the Backend API and Blazor App:

```json
{
  "Profiles": [
    {
      "Name": "BackendApi + App",
      "Projects": [
        {
          "Path": "LittleTalk.BackendApi\\LittleTalk.BackendApi.csproj",
          "Action": "Start",
          "DebugTarget": "http"
        },
        {
          "Path": "LittleTalk.App\\LittleTalk.App.csproj",
          "Action": "Start",
          "DebugTarget": "http"
        }
      ]
    }
  ]
}
```
