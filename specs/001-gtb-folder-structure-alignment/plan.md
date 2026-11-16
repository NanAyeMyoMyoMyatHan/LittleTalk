# Implementation Plan: Align LittleTalk Folder Structure with GTB Project Structure

**Branch**: `001-gtb-folder-structure-alignment` | **Date**: 2026-11-16 | **Spec**: [spec.md](spec.md)

**Feature identifier**: `001-gtb-folder-structure-alignment`

---

## Summary

Migrate the LittleTalk repository layout to exactly replicate the enterprise architecture and project structure established in the GTB reference project (`ACE_Work\GTB`). The plan flattens the project directory layout by eliminating `src/` and `tests/` wrappers, aligns project names with GTB standards (`LittleTalk.BackendApi`, `LittleTalk.App`, `LittleTalk.Domain`, `LittleTalk.Models`, `LittleTalk.Database`, `LittleTalk.Shared`, `LittleTalk.Tests`), extracts a dedicated `LittleTalk.Shared` project hosting `DevCode.cs` and `Result<T>` to fulfill AGENTS.md requirements, centralizes configuration into `Config/`, standardizes documentation into `docs/`, and reconfigures `LittleTalk.slnx` and `LittleTalk.sln` with GTB-style solution folders.

---

## Technical Context

- **Language/Platform**: C# 12 / .NET 8.0, ASP.NET Core Web API, Blazor Interactive Server.
- **Primary Dependencies**: MudBlazor 9.9.0, Serilog.AspNetCore 10.0.0, FluentValidation 11.9.2, Microsoft.EntityFrameworkCore.SqlServer 8.0.10.
- **Storage**: Microsoft SQL Server 2022+ (`LittleTalkDb`), EF Core (Database-First lifecycle), SQL scripts in `LittleTalk.Database/Scripts/`.
- **Testing**: xUnit v2, Microsoft.AspNetCore.Mvc.Testing, Microsoft.EntityFrameworkCore.InMemory.
- **Target Platform**: Windows / Cross-platform .NET 8 runtime.
- **Project Type**: Multi-tier enterprise solution (Web API backend, Blazor interactive server frontend, domain libraries, shared utilities).
- **Constraints**: 
  - Zero regression: all 53 existing tests must pass after restructuring.
  - Development configuration values and connection strings must be preserved.
  - Strict compliance with AGENTS.md rules (`DevCode.cs` in Shared, `[BackendApi]` logging prefixes, domain validation).
- **Scope**: Reorganization of all project directories, csproj files, namespaces, solution files, and documentation.

---

## Constitution Check

| Gate | Before Research | After Design | Status |
|---|---|---|---|
| **I. Existing Repository Authority** | Pass | Pass: Follows AGENTS.md; preserves development configuration; no secrets committed. | PASS |
| **II. Domain and Persistence** | Pass | Pass: Domain layer retains authoritative CRUD validation; `DevCode.cs` centralized in Shared project; EF Core and SQL scripts unified in `LittleTalk.Database`. | PASS |
| **III. Contracts and Security** | Pass | Pass: `Result<T>` unified in `LittleTalk.Shared`; named request/response contracts in `LittleTalk.Models`; JWT and password hashing relocated to `LittleTalk.Domain/Security`. | PASS |
| **IV. Architectural Alignment & Project Layout** | Pass | Pass: Exact 1-to-1 match with GTB project layout: flat root projects, `Config/`, `docs/`, `.specify/`, `specs/`, and `.slnx` solution folders. | PASS |
| **V. Operations and Logging** | Pass | Pass: `/health` endpoint retained; Serilog configuration and `[BackendApi]` log prefixes strictly maintained; test suite baseline (53 tests) preserved. | PASS |

---

## Project Structure and Required Reach

### Target Repository Layout (GTB Aligned)

```text
D:\LittleTalk\
├── .specify/                         # Spec-Kit workflow, constitution, and scripts
├── specs/                            # Feature specifications and implementation plans
│   └── 001-gtb-folder-structure-alignment/
├── Config/                           # Centralized configuration files
│   ├── appsettings.json
│   └── custom-settings-dev.json
├── docs/                             # Architecture and system documentation
│   └── architecture.md
├── LittleTalk.BackendApi/            # ASP.NET Core Web API entry points (was src/LittleTalk.Api)
├── LittleTalk.App/                   # MudBlazor UI host application (was src/LittleTalk.Client)
├── LittleTalk.Domain/                # Business logic, services, validation (was src/LittleTalk.Application)
├── LittleTalk.Models/                # Transport contracts and DTOs (was src/LittleTalk.Model)
├── LittleTalk.Database/              # EF DbContext, entities, and SQL scripts (was src/Infrastructure/Data + database/)
├── LittleTalk.Shared/                # Shared utilities, DevCode.cs, Result<T>, base controller (NEW)
├── LittleTalk.Tests/                 # Unit and integration test suite (was tests/LittleTalk.Tests)
├── AGENTS.md                         # Universal repository instructions
├── README.md                         # Project documentation
├── Directory.Build.props             # Central MSBuild properties
├── LittleTalk.sln                    # Visual Studio classic solution
└── LittleTalk.slnx                   # Modern XML solution file with GTB folders
```

### Detailed Project Mapping & File Moves

| Source Path | Destination Path | Action & Description |
|---|---|---|
| `src/LittleTalk.Api/` | `LittleTalk.BackendApi/` | Move directory; rename project to `LittleTalk.BackendApi.csproj`; update namespace to `LittleTalk.BackendApi`. |
| `src/LittleTalk.Client/` | `LittleTalk.App/` | Move directory; rename project to `LittleTalk.App.csproj`; update namespace to `LittleTalk.App`. |
| `src/LittleTalk.Application/` | `LittleTalk.Domain/` | Move directory; rename project to `LittleTalk.Domain.csproj`; update namespace to `LittleTalk.Domain`. |
| `src/LittleTalk.Model/` | `LittleTalk.Models/` | Move directory; rename project to `LittleTalk.Models.csproj`; update namespace to `LittleTalk.Models`. Move `DevCode.cs` and `Result.cs` to `LittleTalk.Shared`. |
| `src/LittleTalk.Infrastructure/Data/` | `LittleTalk.Database/` | Move `LittleTalkDbContext.cs` and `Entities/` into new `LittleTalk.Database/`. |
| `database/LittleTalk_Db_Init.sql` | `LittleTalk.Database/Scripts/LittleTalk_Db_Init.sql` | Relocate SQL init script into `LittleTalk.Database/Scripts/`. Create `db.md`. |
| `src/LittleTalk.Infrastructure/Authentication/` & `Services/` | `LittleTalk.Domain/Security/` & `LittleTalk.Domain/Services/` | Move JWT and password hash services into Domain. |
| *New Extraction* | `LittleTalk.Shared/` | Create project with `DevCode.cs`, `Result.cs`, `ResultType.cs`, `ResponseDefinition.cs`, and `BaseApiController.cs`. |
| `tests/LittleTalk.Tests/` | `LittleTalk.Tests/` | Move directory to root; update project references to sibling paths `..\ProjectName\`. |
| `appsettings.json` (root) | `Config/appsettings.json` | Move to `Config/` directory; create `Config/custom-settings-dev.json`. |
| `AGENTS.md.md` | *(Deleted)* | Remove duplicate accidental file. |
| `client_response.html` | *(Deleted or moved to docs/)* | Remove loose HTML test file. |
| `src/`, `tests/`, `database/` | *(Deleted)* | Remove empty legacy parent folders once migrated. |

---

## Complexity Tracking

| Component | Why Needed | Rejected Alternative |
|---|---|---|
| Extraction of `LittleTalk.Shared` | Mandated by AGENTS.md rule to centralize `DevCode.cs` and eliminate coupling between DTO models and shared utility extensions. | Leaving `DevCode.cs` in `LittleTalk.Models` violates repository guidelines. |
| Merging EF Data & SQL into `LittleTalk.Database` | Unifies database concerns (entities, context, scripts) in one project matching GTB's `WalletIbmb.Database`. | Retaining a fragmented `LittleTalk.Infrastructure` project with a separate loose `database/` directory causes architectural friction. |

---

## Execution Phases & Implementation Sequence

### Phase 1: Shared and Foundation Extraction
1. Create `LittleTalk.Shared/` and `LittleTalk.Shared.csproj`.
2. Move `DevCode.cs`, `Result.cs`, `ResultType.cs` from `LittleTalk.Model/Common` to `LittleTalk.Shared/`.
3. Add `ResponseDefinition.cs` and move `BaseApiController.cs` to `LittleTalk.Shared/Common/`.

### Phase 2: Database Layer Creation
1. Create `LittleTalk.Database/` and `LittleTalk.Database.csproj`.
2. Move `LittleTalkDbContext.cs` and `Entities/` from `src/LittleTalk.Infrastructure/Data/` to `LittleTalk.Database/`.
3. Create `LittleTalk.Database/Scripts/` and move `database/LittleTalk_Db_Init.sql` into it.
4. Add `LittleTalk.Database/db.md`.

### Phase 3: Models Project Migration
1. Move `src/LittleTalk.Model/` to `./LittleTalk.Models/`.
2. Rename project file to `LittleTalk.Models.csproj`.
3. Add reference to `..\LittleTalk.Shared\LittleTalk.Shared.csproj`.
4. Update namespace to `LittleTalk.Models`.

### Phase 4: Domain Layer Migration
1. Move `src/LittleTalk.Application/` to `./LittleTalk.Domain/`.
2. Rename project file to `LittleTalk.Domain.csproj`.
3. Move `Authentication/` (`JwtTokenService`, `PasswordHashService`) and `Services/CurrentUserService` into `LittleTalk.Domain/Security/` and `LittleTalk.Domain/Services/`.
4. Update project references to point to `..\LittleTalk.Database\`, `..\LittleTalk.Models\`, and `..\LittleTalk.Shared\`.
5. Update namespaces and dependency registrations.

### Phase 5: BackendApi and App Migration
1. Move `src/LittleTalk.Api/` to `./LittleTalk.BackendApi/` and rename project to `LittleTalk.BackendApi.csproj`.
2. Move `src/LittleTalk.Client/` to `./LittleTalk.App/` and rename project to `LittleTalk.App.csproj`.
3. Update project references to sibling paths (`..\LittleTalk.Domain\`, `..\LittleTalk.Models\`, `..\LittleTalk.Shared\`).
4. Update `Program.cs` and namespaces.

### Phase 6: Tests and Solution Update
1. Move `tests/LittleTalk.Tests/` to `./LittleTalk.Tests/`.
2. Update `LittleTalk.Tests.csproj` references to sibling paths.
3. Update `LittleTalk.slnx` with GTB folder layout (`/.Config/`, `/Common/`, `/Domain/`, `/Shared/`, `/_Documents/`).
4. Update `LittleTalk.sln` and `LittleTalk.slnLaunch.user`.
5. Remove obsolete folders (`src/`, `tests/`, `database/`, `src/LittleTalk.Infrastructure`).

### Phase 7: Validation
1. Run `dotnet clean LittleTalk.slnx`.
2. Run `dotnet build LittleTalk.slnx -c Release`.
3. Run `dotnet test LittleTalk.slnx` and verify all 53 tests pass.


---

## Implementation Outcome (2026-11-16)

- Completed full directory flattening and project alignment with the GTB reference architecture:
  - Extracted and built `LittleTalk.Shared` containing `DevCode.cs`, `Result.cs`, `ResultType.cs`, `ResponseDefinition.cs`, and `BaseApiController.cs`.
  - Consolidated EF entities, DbContext, SQL init scripts, and `db.md` into `LittleTalk.Database`.
  - Migrated `LittleTalk.Models` and `LittleTalk.Domain` to root with updated namespaces and dependencies.
  - Flattened `LittleTalk.BackendApi` and `LittleTalk.App` (Blazor UI) to root.
  - Flattened `LittleTalk.Tests` to root with updated project references.
  - Centralized configuration into `Config/appsettings.json` and `Config/custom-settings-dev.json`.
  - Created `docs/architecture.md` detailing project responsibilities.
  - Modernized `LittleTalk.slnx` with GTB solution folders (`/.Config/`, `/Common/`, `/Domain/`, `/Shared/`, `/_Documents/`).
  - Cleaned up obsolete directories (`src/`, `tests/`, `database/`, redundant `AGENTS.md.md`).
- **Validation**:
  - Release build across all 7 projects: 0 warnings, 0 errors (`dotnet build LittleTalk.slnx -c Release -p:NuGetAudit=false`).
  - Test suite: 53 passed, 0 failed, 0 skipped (`dotnet test LittleTalk.slnx -c Release`).
