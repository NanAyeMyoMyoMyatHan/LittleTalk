# Feature Specification: Align LittleTalk Folder Structure with GTB Project Structure

**Feature Branch**: `001-gtb-folder-structure-alignment`

**Created**: 2026-11-16

**Status**: Planned

**Input**: User description: "i want the little talk folder structure exactly like GTB pj folder structure /speckit-plan"

## Summary

Restructure the LittleTalk repository layout to exactly replicate the enterprise architecture and directory conventions established in the GTB reference project (`ACE_Work\GTB`). This transition eliminates arbitrary `src/` and `tests/` wrappers in favor of root-level flat project placement, renames and realigns layers to GTB standards (`LittleTalk.BackendApi`, `LittleTalk.App`, `LittleTalk.Domain`, `LittleTalk.Models`, `LittleTalk.Database`, `LittleTalk.Shared`, `LittleTalk.Tests`), extracts a dedicated `LittleTalk.Shared` project to host `DevCode.cs` and `Result<T>` as mandated by repository rules, centralizes configuration into `Config/`, standardizes documentation under `docs/`, and organizes the modern `.slnx` solution file with corresponding solution folders.

---

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Developer Navigates and Builds Standard GTB-Aligned Solution (Priority: P1)

As a developer working across ACE projects, I want LittleTalk to adopt the identical project organization and naming conventions as GTB, so that I can immediately navigate, inspect, and build the solution using standard commands without mental overhead or diverging patterns.

**Why this priority**: Eliminates architecture discrepancies between repositories, aligns with AGENTS.md rules, and establishes clear layer responsibilities.

**Independent Test**:
Can be verified by executing `dotnet build LittleTalk.slnx` and `dotnet test LittleTalk.slnx`, ensuring 100% build success with 0 errors and all 53 unit/integration tests passing against the new project paths.

**Acceptance Scenarios**:
1. **Given** the root directory, **When** listing top-level entries, **Then** all .NET projects are located directly at root (`LittleTalk.BackendApi`, `LittleTalk.App`, `LittleTalk.Domain`, `LittleTalk.Models`, `LittleTalk.Database`, `LittleTalk.Shared`, `LittleTalk.Tests`) and no `src/` or `tests/` folders exist.
2. **Given** the solution file `LittleTalk.slnx`, **When** inspected or opened in Visual Studio / VS Code, **Then** projects are logically categorized into `/.Config/`, `/Common/`, `/Domain/`, `/Shared/`, and `/_Documents/` folders matching GTB.

---

### User Story 2 - Authoritative Shared Layer and Validation Policy Adherence (Priority: P2)

As a backend engineer, I want `DevCode.cs`, `Result<T>`, and common base controller types housed within an independent `LittleTalk.Shared` project, so that shared utilities, pagination rules, and API response envelopes are strictly separated from domain models and transport DTOs in full compliance with repository instructions.

**Why this priority**: Directly satisfies the AGENTS.md rule: "Keep shared utility code in the Shared project in a file named DevCode.cs" and enables clean separation between models and transport contracts.

**Independent Test**:
Inspect `LittleTalk.Shared` to confirm `DevCode.cs`, `Result.cs`, and `ResultType.cs` are present and referenced by `LittleTalk.Models`, `LittleTalk.Domain`, and `LittleTalk.BackendApi`.

**Acceptance Scenarios**:
1. **Given** a shared utility method (e.g. pagination validator), **When** invoked by Domain or Web API, **Then** it is consumed from `LittleTalk.Shared` via `DevCode.cs`.
2. **Given** `LittleTalk.Models`, **When** compiled, **Then** it references `LittleTalk.Shared` and only contains request/response models.

---

### User Story 3 - Consolidated Database and Migration Artifacts (Priority: P3)

As a database administrator or developer, I want all database entities, DbContext, and SQL deployment scripts located inside `LittleTalk.Database` alongside a `db.md` documentation file, so that all database-related assets reside in one unified project folder identical to `WalletIbmb.Database`.

**Why this priority**: Resolves the fragmentation between `src/LittleTalk.Infrastructure/Data` and the loose root `database/` folder.

**Independent Test**:
Verify `LittleTalk.Database` compiles cleanly, contains `Entities/`, `LittleTalkDbContext.cs`, `Scripts/LittleTalk_Db_Init.sql`, and `db.md`, and is referenced by `LittleTalk.Domain`.

**Acceptance Scenarios**:
1. **Given** `LittleTalk.Database`, **When** inspected, **Then** it houses `LittleTalkDbContext`, all 20 EF entity classes, and the database creation SQL script in `Scripts/`.

---

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST move all .NET projects from `src/` and `tests/` directly to root-level folders:
  - `src/LittleTalk.Api` → `LittleTalk.BackendApi`
  - `src/LittleTalk.Client` → `LittleTalk.App`
  - `src/LittleTalk.Application` → `LittleTalk.Domain`
  - `src/LittleTalk.Model` → `LittleTalk.Models`
  - `src/LittleTalk.Infrastructure/Data` + `database/` → `LittleTalk.Database`
  - `tests/LittleTalk.Tests` → `LittleTalk.Tests`
- **FR-002**: System MUST extract a new `LittleTalk.Shared` project containing:
  - `DevCode.cs` (centralized pagination, string extensions, common utilities)
  - `Result.cs` & `ResultType.cs` (unified operation envelope)
  - `BaseApiController.cs`
- **FR-003**: System MUST relocate infrastructure authentication and user services (`JwtTokenService`, `PasswordHashService`, `CurrentUserService`) into `LittleTalk.Domain/Security/` or `LittleTalk.Domain/Services/`.
- **FR-004**: System MUST create a `Config/` folder containing environment configuration (`appsettings.json`, `custom-settings-dev.json`) matching GTB's config pattern.
- **FR-005**: System MUST create a `docs/` folder containing `architecture.md` detailing project responsibilities and structure decisions.
- **FR-006**: System MUST update all `.csproj` project references to use direct sibling relative paths (e.g., `..\LittleTalk.Shared\LittleTalk.Shared.csproj`).
- **FR-007**: System MUST update `LittleTalk.slnx` and `LittleTalk.sln` to reflect root paths and solution folder groupings (`/.Config/`, `/Common/`, `/Domain/`, `/Shared/`, `/_Documents/`).
- **FR-008**: System MUST update all namespaces in C# source files to reflect the aligned project names (`LittleTalk.BackendApi`, `LittleTalk.App`, `LittleTalk.Domain`, `LittleTalk.Models`, `LittleTalk.Database`, `LittleTalk.Shared`).
- **FR-009**: System MUST ensure all 53 existing unit and integration tests compile and pass with 0 failures and 0 regressions.
- **FR-010**: System MUST clean up obsolete directories (`src/`, `tests/`, `database/`, redundant `AGENTS.md.md`).

---

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of projects reside at repository root matching GTB directory layout (`Get-ChildItem` shows no `src/` or `tests/` folders).
- **SC-002**: Clean Release build with 0 warnings and 0 errors across the entire solution (`dotnet build LittleTalk.slnx -c Release`).
- **SC-003**: 100% pass rate across the full test suite (`dotnet test LittleTalk.slnx` passes all 53 tests).
- **SC-004**: Full compliance with AGENTS.md rules (`DevCode.cs` in `LittleTalk.Shared`, `[BackendApi]` logging prefixes preserved, domain-authoritative validation intact).

---

## Assumptions

- Target .NET SDK remains .NET 8.0 (current runtime of LittleTalk).
- Database-first schema and existing SQL script (`LittleTalk_Db_Init.sql`) remain unchanged in behavior and data integrity.
- Client Blazor components and MudBlazor UI remain intact, retaining their existing routes, forms, and service connections.
- Existing development connection strings and secrets in `appsettings.json` are preserved without rotation.
