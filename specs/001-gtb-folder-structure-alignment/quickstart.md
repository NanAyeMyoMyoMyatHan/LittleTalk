# Quickstart Validation Guide: GTB Folder Structure Alignment

## Overview
This guide provides runnable commands and validation scenarios to verify that LittleTalk has been successfully reorganized to match the GTB project layout.

---

## 1. Directory Structure Verification

Confirm that all projects live at the repository root and no legacy `src/` or `tests/` directories remain:

```powershell
# Verify root-level project directories exist
Test-Path "LittleTalk.BackendApi", "LittleTalk.App", "LittleTalk.Domain", "LittleTalk.Models", "LittleTalk.Database", "LittleTalk.Shared", "LittleTalk.Tests", "Config", "docs"

# Verify legacy wrapper folders are gone
-not (Test-Path "src", "tests")
```

**Expected Outcome**:
All root-level project directories return `True`, and legacy wrapper directories return `True` for being absent.

---

## 2. Solution Build Validation

Execute a clean Release build across the solution:

```powershell
dotnet clean LittleTalk.slnx
dotnet build LittleTalk.slnx -c Release -p:NuGetAudit=false
```

**Expected Outcome**:
Build completes with `0 Warning(s)` and `0 Error(s)` across all 7 projects:
- `LittleTalk.Shared`
- `LittleTalk.Models`
- `LittleTalk.Database`
- `LittleTalk.Domain`
- `LittleTalk.BackendApi`
- `LittleTalk.App`
- `LittleTalk.Tests`

---

## 3. Test Suite Execution & Regression Check

Run the comprehensive unit and integration test suite:

```powershell
dotnet test LittleTalk.Tests\LittleTalk.Tests.csproj --verbosity normal
```

**Expected Outcome**:
All 53 unit and integration tests pass with 0 failures and 0 skipped tests:
- `AiLessonPlanGeneratorTests`
- `AssessmentScoringTests`
- `AssessmentValidationPolicyTests`
- `AttendanceValidationPolicyTests`
- `CourseValidationPolicyTests`
- `JwtTokenServiceTests`
- `LessonPlanValidationPolicyTests`
- `PasswordHasherTests`
- `ValidationPolicyTests`
- `ValidationTests`
- `ApiIntegrationTests`

---

## 4. Architectural Rules Compliance Check

Verify compliance with key AGENTS.md rules:

```powershell
# 1. DevCode.cs must exist in LittleTalk.Shared
Test-Path "LittleTalk.Shared\DevCode.cs"

# 2. DbContext and Entities must exist in LittleTalk.Database
Test-Path "LittleTalk.Database\LittleTalkDbContext.cs"
Test-Path "LittleTalk.Database\Entities"
Test-Path "LittleTalk.Database\Scripts\LittleTalk_Db_Init.sql"

# 3. Centralized config exists
Test-Path "Config\appsettings.json"

# 4. Solution files correctly reference root paths
Select-String -Path "LittleTalk.slnx" -Pattern "src/"
```

**Expected Outcome**:
- First 3 checks return `True`.
- Last command returns no matches (verifying no legacy `src/` paths in the solution file).
