---
description: "Task list for aligning LittleTalk folder structure with GTB project structure"
---

# Tasks: Align LittleTalk Folder Structure with GTB Project Structure

**Input**: Design documents from `/specs/001-gtb-folder-structure-alignment/`
**Prerequisites**: [plan.md](plan.md), [spec.md](spec.md), [research.md](research.md), [data-model.md](data-model.md), [contracts/](contracts/), [quickstart.md](quickstart.md)

---

## Phase 1: Setup (Shared Directories & Configuration)

**Purpose**: Initialize directory structure, configuration, and documentation per GTB standards.

- [x] T001 Create `Config/` directory, migrate root `appsettings.json` to `Config/appsettings.json`, and create `Config/custom-settings-dev.json`
- [x] T002 [P] Create `docs/` directory and author `docs/architecture.md` detailing the GTB-aligned layer responsibilities
- [x] T003 [P] Remove obsolete/redundant files (`AGENTS.md.md`, `client_response.html`) from repository root

---

## Phase 2: Foundational (Shared Project Extraction)

**Purpose**: Establish `LittleTalk.Shared` containing `DevCode.cs`, `Result<T>`, and `BaseApiController` as required by AGENTS.md.

**⚠️ CRITICAL**: Foundational phase blocks domain, models, web API, and test migration.

- [x] T004 Create `LittleTalk.Shared` project in `LittleTalk.Shared/LittleTalk.Shared.csproj`
- [x] T005 [P] Relocate and update `DevCode.cs` into `LittleTalk.Shared/DevCode.cs` with namespace `LittleTalk.Shared`
- [x] T006 [P] Relocate and update `Result.cs` and `ResultType.cs` into `LittleTalk.Shared/Result.cs` and `LittleTalk.Shared/ResultType.cs` with namespace `LittleTalk.Shared`
- [x] T007 [P] Create `ResponseDefinition.cs` in `LittleTalk.Shared/ResponseDefinition.cs`
- [x] T008 Relocate `BaseApiController.cs` to `LittleTalk.Shared/Common/BaseApiController.cs` with namespace `LittleTalk.Shared`

---

## Phase 3: User Story 3 - Consolidated Database and Migration Artifacts (Priority: P3)

**Goal**: Consolidate EF Core entities, DbContext, and SQL scripts into unified `LittleTalk.Database` project matching `WalletIbmb.Database`.

**Independent Test**: `dotnet build LittleTalk.Database/LittleTalk.Database.csproj` compiles cleanly and contains `LittleTalkDbContext`, 20 entity classes, and `Scripts/LittleTalk_Db_Init.sql`.

- [x] T009 [US3] Create `LittleTalk.Database` project file in `LittleTalk.Database/LittleTalk.Database.csproj` referencing `LittleTalk.Shared`
- [x] T010 [P] [US3] Move `LittleTalkDbContext.cs` and `Entities/` from `src/LittleTalk.Infrastructure/Data/` to `LittleTalk.Database/` and adjust namespaces
- [x] T011 [P] [US3] Move `database/LittleTalk_Db_Init.sql` to `LittleTalk.Database/Scripts/LittleTalk_Db_Init.sql`
- [x] T012 [P] [US3] Create `LittleTalk.Database/db.md` documenting database tables and relations

---

## Phase 4: User Story 2 - Authoritative Domain & Transport Models (Priority: P2)

**Goal**: Transition Models and Domain projects to root-level, update namespaces, wire dependencies to `LittleTalk.Shared` and `LittleTalk.Database`.

**Independent Test**: `dotnet build LittleTalk.Models/LittleTalk.Models.csproj` and `dotnet build LittleTalk.Domain/LittleTalk.Domain.csproj` build with 0 errors.

- [x] T013 [US2] Move `src/LittleTalk.Model/` to `LittleTalk.Models/`, rename to `LittleTalk.Models.csproj`, reference `LittleTalk.Shared`, and update namespaces
- [x] T014 [US2] Move `src/LittleTalk.Application/` to `LittleTalk.Domain/`, rename to `LittleTalk.Domain.csproj`, and update project references to `LittleTalk.Database`, `LittleTalk.Models`, and `LittleTalk.Shared`
- [x] T015 [P] [US2] Relocate authentication services from `src/LittleTalk.Infrastructure/Authentication/` and `Services/` to `LittleTalk.Domain/Security/` and `LittleTalk.Domain/Services/`
- [x] T016 [US2] Update domain feature services, validation policies, and `DependencyInjection.cs` in `LittleTalk.Domain/` to align with `LittleTalk.Shared`, `LittleTalk.Models`, and `LittleTalk.Database`

---

## Phase 5: User Story 1 - Developer Navigates and Builds Standard GTB-Aligned Solution (Priority: P1) 🎯 MVP

**Goal**: Flatten Web API host (`LittleTalk.BackendApi`), Blazor UI (`LittleTalk.App`), tests (`LittleTalk.Tests`), and solution files (`LittleTalk.slnx`, `LittleTalk.sln`) to repository root.

**Independent Test**: Full solution builds via `dotnet build LittleTalk.slnx` with 0 warnings/errors, and all projects open under GTB solution folders.

- [x] T017 [US1] Move `src/LittleTalk.Api/` to `LittleTalk.BackendApi/`, rename to `LittleTalk.BackendApi.csproj`, update project references and namespaces to `LittleTalk.BackendApi`
- [x] T018 [US1] Move `src/LittleTalk.Client/` to `LittleTalk.App/`, rename to `LittleTalk.App.csproj`, update project references and namespaces to `LittleTalk.App`
- [x] T019 [US1] Move `tests/LittleTalk.Tests/` to `LittleTalk.Tests/`, update project references in `LittleTalk.Tests/LittleTalk.Tests.csproj` to sibling project paths, and update namespaces
- [x] T020 [US1] Update `LittleTalk.slnx` with GTB solution folder layout (`/.Config/`, `/Common/`, `/Domain/`, `/Shared/`, `/_Documents/`)
- [x] T021 [US1] Update `LittleTalk.sln` and `LittleTalk.slnLaunch.user` to reflect root project paths
- [x] T022 [US1] Clean up obsolete legacy directories (`src/`, `tests/`, `database/`)

---

## Phase 6: Polish & Verification

**Purpose**: Solution-wide build, test verification, and documentation updates.

- [x] T023 Execute `dotnet clean LittleTalk.slnx` and `dotnet build LittleTalk.slnx -c Release -p:NuGetAudit=false`
- [x] T024 Execute `dotnet test LittleTalk.Tests/LittleTalk.Tests.csproj` and verify all 53 unit and integration tests pass
- [x] T025 Update `README.md` to reflect the GTB-aligned architecture and project layout

---

## Dependencies & Execution Order

- **Phase 1 (Setup)** → Complete
- **Phase 2 (Foundational: Shared)** → Complete
- **Phase 3 (User Story 3: Database)** → Complete
- **Phase 4 (User Story 2: Models & Domain)** → Complete
- **Phase 5 (User Story 1: Hosts & Solutions)** → Complete
- **Phase 6 (Polish & Verification)** → Complete
