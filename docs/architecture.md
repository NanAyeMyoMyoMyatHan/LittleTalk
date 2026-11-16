# Project Architecture & Responsibility Matrix

The solution architecture follows the enterprise GTB pattern, utilizing flat root-level projects, strict separation between transport contracts and domain validation, and a centralized Shared project.

## Responsibilities Retained

| Project | Responsibility | Target Framework |
|---|---|---|
| **LittleTalk.App** | Blazor Interactive Server UI host, MudBlazor components, client API clients, auth state provider | .NET 8 (Web SDK) |
| **LittleTalk.BackendApi** | Authorized Web API HTTP entry points, controllers, Serilog logging, Swagger/OpenAPI | .NET 8 (Web SDK) |
| **LittleTalk.Domain** | Domain business rules, authoritative CRUD validation policies, security/token issuance | .NET 8 |
| **LittleTalk.Models** | Shared HTTP contracts, feature request and response DTO models | .NET 8 |
| **LittleTalk.Database** | EF Core DbContext, normalized database entities, SQL scripts, `db.md` | .NET 8 |
| **LittleTalk.Shared** | `DevCode.cs`, `Result<T>` operation envelope, `BaseApiController`, shared utilities | .NET 8 (Razor SDK) |
| **LittleTalk.Tests** | Unit and integration test suites covering domain validation, security, and API endpoints | .NET 8 |

## Visual Studio Solution Layout (`LittleTalk.slnx`)

Projects are organized into logical solution folders matching GTB conventions:
- `/.Config/`: `AGENTS.md`, `Config/appsettings.json`, `Config/custom-settings-dev.json`
- `/Common/`: `LittleTalk.Database`
- `/Domain/`: `LittleTalk.Domain`, `LittleTalk.Tests`
- `/Shared/`: `LittleTalk.Models`, `LittleTalk.Shared`
- `/_Documents/`: `README.md`, `docs/architecture.md`
- Root projects: `LittleTalk.App`, `LittleTalk.BackendApi`
