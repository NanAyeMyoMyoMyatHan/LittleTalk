# LittleTalk Repository Constitution
Version: 1.0.0 | Ratified: 2026-11-16

## I. Existing Repository Authority
Follow AGENTS.md and user-provided instructions. Preserve existing development configuration and connection settings. Never commit secrets. Respect each project's architectural responsibilities.

## II. Domain and Persistence
Domain layer is the single authoritative source for CRUD validation. API DTOs are plain transport contracts; UI annotations provide immediate user feedback. Do not define or use field-length validation rules or constants in validation policies. Reuse `DevCode.cs` in the Shared project for shared utilities, page-size choices, and pagination validation. Use EF Core only for straightforward CRUD, and `FirstOrDefaultAsync` for first matching entity. Use stored procedures for complex queries and reports.

## III. Contracts and Security
Every new or modified first-party API endpoint must accept exactly one named request model and return `Result<TResponse>`, where `TResponse` is a named, endpoint-specific response model. Outbound HTTP calls use `IHttpClientFactory`. Keep authentication, HTTPS, and authorization checks secure.

## IV. Architectural Alignment & Project Layout
Align folder and solution structure with enterprise standard (GTB project structure):
- Root-level flat project placement (no `src/` or `tests/` wrappers).
- Layered projects: `<SolutionName>.BackendApi`, `<SolutionName>.App`, `<SolutionName>.Domain`, `<SolutionName>.Models`, `<SolutionName>.Database`, `<SolutionName>.Shared`, `<SolutionName>.Tests`.
- Centralized `Config/` directory for application settings.
- Centralized `docs/` directory for architecture documentation.
- Spec-Kit SDD infrastructure (`.specify/`, `specs/`).

## V. Operations and Logging
Web applications must register health checks at `/health` and Serilog logging with hourly rolling files. All log messages must follow the exact `[ProjectName] MethodName => ...` format. Preserve safe error handling and verify that existing test suites continue to pass with 0 regressions.

## Governance
Changes require explicit rationale and updated gates. Recheck before and after design. User instructions supersede this constitution; AGENTS.md is authoritative.
