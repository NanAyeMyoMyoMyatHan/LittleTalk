# Research and Technical Decisions: GTB Folder Structure Alignment

## Context
LittleTalk is currently organized using a traditional `src/` and `tests/` folder hierarchy with projects named `LittleTalk.Api`, `LittleTalk.Client`, `LittleTalk.Application`, `LittleTalk.Infrastructure`, and `LittleTalk.Model`. The goal is to align its solution architecture and repository folder structure exactly with the GTB reference project (`ACE_Work\GTB`).

---

## Technical Decisions

### Decision 1: Root-Level Flat Project Hierarchy
- **Decision**: Remove `src/` and `tests/` directories; place all C# project folders directly at the repository root.
- **Rationale**:
  - GTB places all project folders (`WalletIbmb.BackendApi`, `WalletIbmb.App`, `WalletIbmb.Domain`, etc.) directly at the repository root.
  - Sibling relative references (`..\ProjectName\ProjectName.csproj`) simplify project dependency graphs and eliminate nested path discrepancies between Visual Studio solution folders and filesystem paths.
- **Alternatives Considered**:
  - *Keep `src/` and `tests/` wrappers*: Rejected because user explicitly specified exact matching with GTB folder structure.

---

### Decision 2: Project Naming and Layer Mapping
- **Decision**: Map existing LittleTalk projects to GTB standard names:
  1. `LittleTalk.Api` → `LittleTalk.BackendApi`
  2. `LittleTalk.Client` → `LittleTalk.App`
  3. `LittleTalk.Application` → `LittleTalk.Domain`
  4. `LittleTalk.Model` → `LittleTalk.Models` (pluralized)
  5. `LittleTalk.Infrastructure/Data` + `database/` → `LittleTalk.Database`
  6. New Project: `LittleTalk.Shared` (extracted)
  7. `tests/LittleTalk.Tests` → `LittleTalk.Tests`
- **Rationale**:
  - Matches GTB project responsibilities: `BackendApi` (Web API host), `App` (Blazor UI host), `Domain` (business rules and validation policies), `Models` (DTOs), `Database` (EF context, entities, SQL scripts), `Shared` (DevCode utilities, Result envelope), and `Tests`.
  - Conforms to AGENTS.md logging rules which explicitly use `[BackendApi]` as the required prefix.
- **Alternatives Considered**:
  - *Keep `LittleTalk.Client` instead of `LittleTalk.App`*: Rejected to maintain strict symmetry with GTB's `WalletIbmb.App`.
  - *Keep `LittleTalk.Model` singular*: Rejected; GTB uses `WalletIbmb.Models`.

---

### Decision 3: Extraction of `LittleTalk.Shared` Project
- **Decision**: Create an independent `LittleTalk.Shared` project to host:
  - `DevCode.cs`: pagination options, page-size validation, string extensions, helper methods.
  - `Result.cs` & `ResultType.cs`: standardized operation result envelope (`Result<T>`).
  - `BaseApiController.cs` (or `BaseController.cs`): shared controller base handling Result responses.
  - `ResponseDefinition.cs`: common status codes and metadata.
- **Rationale**:
  - Repository instructions (`AGENTS.md`) state: *"Keep shared utility code in the Shared project in a file named DevCode.cs... require callers to reuse them rather than define duplicate behavior."*
  - In GTB, `WalletIbmb.Shared` serves as the foundational library referenced by Models, Domain, BackendApi, and App.
- **Alternatives Considered**:
  - *Leave `DevCode.cs` inside Models*: Rejected because it violates the repository rule and entangles domain transport models with operational utilities.

---

### Decision 4: Consolidation of Database Assets into `LittleTalk.Database`
- **Decision**: Combine the scaffolded EF Core entities and `LittleTalkDbContext` with the SQL initialization scripts into a single `LittleTalk.Database` project:
  - `Entities/`: All 20 EF entity classes.
  - `LittleTalkDbContext.cs`: DbContext configuration and model builder mappings.
  - `Scripts/`: `LittleTalk_Db_Init.sql`.
  - `db.md`: Database documentation and table descriptions.
- **Rationale**:
  - Matches GTB's `WalletIbmb.Database` structure (`AppDbContextModels/`, `Scripts/`, `Rollbacks/`, `db.md`).
  - Co-locates SQL schema scripts with the EF Core representations.
- **Alternatives Considered**:
  - *Keep a loose `database/` root folder*: Rejected; GTB embeds database scripts inside the Database project.

---

### Decision 5: Reallocation of Infrastructure Security & Services
- **Decision**: Relocate `JwtTokenService`, `PasswordHashService`, `CurrentUserService`, and their interfaces into `LittleTalk.Domain/Security/` and `LittleTalk.Domain/Services/`.
- **Rationale**:
  - In GTB, domain security and token services (e.g. `TokenIssuer.cs`) reside in `Domain`.
  - Avoids maintaining a sparse `Infrastructure` project for just three service classes when EF is moved to `LittleTalk.Database`.

---

### Decision 6: Solution and Configuration Organization
- **Decision**:
  - Standardize `LittleTalk.slnx` and `LittleTalk.sln` to utilize GTB's solution folders:
    - `/.Config/`: `AGENTS.md`, `Config/appsettings.json`, `Config/custom-settings-dev.json`
    - `/Common/`: `LittleTalk.Database`
    - `/Domain/`: `LittleTalk.Domain`, `LittleTalk.Tests`
    - `/Shared/`: `LittleTalk.Models`, `LittleTalk.Shared`
    - `/_Documents/`: `README.md`, `docs/architecture.md`
    - Root projects: `LittleTalk.BackendApi`, `LittleTalk.App`
  - Establish `Config/` directory for configuration files and `docs/` directory for documentation.
- **Rationale**:
  - Mirrors GTB's exact Visual Studio solution presentation (`AceWalletIbmb.slnx`).
