# Release 1 Test Results

Executed: 2026-10-23
Environment: Windows, .NET SDK 10.0.402, PostgreSQL 18 (local service),
Node 22.19.0, Playwright 1.64.1 Chromium.

## 1. Build

Command:

```powershell
dotnet build telemedicine-webapi/telemedicine-webapi.sln --no-incremental
```

Result:

- Errors: **0**
- Warnings: **15** (nullable/obsolete warnings and existing package advisories)
- API, Domain, Application, Infrastructure and test projects built successfully.
- SPA production build (Vite) completed successfully.

## 2. Backend automated tests (Release 1 scope)

Commands:

```powershell
dotnet test telemedicine-webapi/tests/Domain.UnitTests/Domain.UnitTests.csproj
dotnet test telemedicine-webapi/tests/Application.UnitTests/Application.UnitTests.csproj
dotnet test telemedicine-webapi/tests/Application.IntegrationTests/Application.IntegrationTests.csproj `
  --filter "FullyQualifiedName!~ConsultationIntegrationTests"
```

| Suite | Passed | Failed | Skipped | Total |
|---|---:|---:|---:|---:|
| Domain.UnitTests (original R1 scope) | 5 | 0 | 0 | 5 |
| Application.UnitTests | 8 | 0 | 0 | 8 |
| Application.IntegrationTests (R1 scope, R2 tests excluded) | 31 | 0 | 0 | 31 |
| **R1 backend total** | **44** | **0** | **0** | **44** |

The current Domain.UnitTests assembly also contains 4 concurrent Release 2
consultation-domain tests; they pass but are excluded from the R1 count above.

The original 23 tests remain passing:

- 5 domain (`Colour`) tests.
- 8 application unit tests.
- 10 original integration tests (Todo template CRUD).

Additional R1-scope integration coverage (31 total):

- 13 `R1ReleaseWorkflowsTests`:
  - registration creates linked Patient and Physician profiles; duplicate profile,
    duplicate account and duplicate licence rejected;
  - role authorization on profiles, availability and respond;
  - availability persistence, slot generation and invalid schedule/range rejection;
  - valid booking, pending status, slot removal and persistence;
  - invalid past/outside/overlap/reused-token/role bookings rejected;
  - concurrent API booking allows exactly one success;
  - direct PostgreSQL exclusion constraint allows exactly one concurrent insert;
  - reschedule moves the interval, releases the old slot, rejects conflicts and
    cross-patient access;
  - owner-only cancellation releases the slot and rejects re-cancellation;
  - physician accept/decline, reason requirement, ownership and patient denial;
  - patient/physician record-level access;
  - chat authentication/treating relationship and forbidden broadcast/group;
  - EMR read returns reports/comments without NullReferenceException and denies an
    unrelated physician.
- 4 middleware security tests (health sanitization, 401/no-store, CORS, rate limiting).
- 3 SignalR security tests (anonymous/invalid rejection, authenticated exchange,
  arbitrary group/recipient denial).
- 1 direct PostgreSQL concurrency test
  (`ExclusionConstraint_AllowsExactlyOneConcurrentOverlappingBooking`).

All database-concurrency evidence uses PostgreSQL, not EF Core InMemory.

## 3. Playwright browser tests

Command (R1 specs):

```powershell
cd telemedicine-spa
npx playwright test tests/e2e/release1.spec.js tests/e2e/r1-workflows.spec.js
```

Result: **7 passed, 0 failed** (~60 seconds total, Chromium).

| # | Scenario | Result |
|---|---|---|
| 1 | Patient registers with linked profile, finds Dr. Demo, selects a real slot, pays, sees confirmation, reschedules and cancels | PASS |
| 2 | Physician saves real availability, accepts one request and declines another with a reason | PASS |
| 3 | Invalid credentials rejected | PASS |
| 4 | Protected routes redirect anonymous users; logout protects them again | PASS |
| 5 | Patient logs in, opens directory, sees persisted appointments from the API | PASS |
| 6 | Physician logs in, sees assigned requests and is denied another physician's id | PASS |
| 7 | Chat and EMR endpoints reject unauthenticated access without leaking errors | PASS |

R1 browser flows assert:

- no unexpected browser `console` errors (expected 401/403 credential checks excluded);
- no uncaught page errors;
- no HTTP 500 responses;
- working API calls for every booking, reschedule, cancellation, accept and decline
  action.

Artifacts:

- HTML report: `telemedicine-spa/playwright-report/index.html`.
- Traces/screenshots/video for failures: `telemedicine-spa/test-results/` (no R1
  workflow failure artifacts were produced on the passing run).
- Raw run log retained in the execution environment (`r1_playwright_r1only.log`).

## 4. Migration tests

| Test | Result |
|---|---|
| Apply both migrations to a clean database (`telemedicine_r1_clean`) | PASS |
| Dump/restore the existing synthetic schema into `telemedicine_r1_copy`, then apply R1 migration | PASS |
| Preserve all 4 legacy appointment rows | PASS |
| Link 2 legacy profiles to Identity accounts by email | PASS |
| Preserve and cancel the legacy duplicate appointment | PASS |
| Create the PostgreSQL exclusion and check constraints | PASS |
| Apply to `telemedicine_test` and run all R1-scope integration tests | PASS |
| Apply to the development/demo database and run Playwright workflows | PASS |

See `R1_DATABASE_MIGRATIONS.md` for commands and row counts.

## 5. Full-suite scope note

The working tree contains concurrent, unapproved Release 2 consultation/signalling
work and three `ConsultationIntegrationTests` (not part of Release 1). On the final
working tree, the unfiltered integration command reports 32 passed / 2 failed because
those two R2 tests fail at `JoinConsultation`. They are **excluded** from the Release 1
result above and must be reviewed only under the separate Release 2 approval gate.

No Release 1-scoped test is failing or skipped.

## 6. Residual quality notes

- Build warnings remain (nullable annotations, obsolete FluentValidation call,
  obsolete formatter API, package advisories).
- Dependency advisories remain in the existing dependency graph (for example
  AutoMapper 12.0.1 and `System.Security.Cryptography.Pkcs`); they are tracked as
  security/upgrade work, not R1 acceptance failures.
- The development configuration contains a synthetic local JWT key and local database
  password; these must be replaced before any non-demo deployment.
- The EMR endpoint is fixed and regression-tested, but the clinical-record redesign,
  consent/audit and versioning remain later-release work.
