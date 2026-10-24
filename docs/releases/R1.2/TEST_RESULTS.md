# Release 1.2 Test Results

Date: 2026-10-23

| Verification | Result |
|---|---|
| `dotnet build telemedicine-webapi/telemedicine-webapi.sln --configuration Release --no-restore` | Passed, 0 errors; 4 existing warnings in final run |
| `dotnet test telemedicine-webapi/telemedicine-webapi.sln --configuration Release --no-restore` | Passed: Domain 5, Application 8, PostgreSQL Integration 15; 28 total |
| PostgreSQL exclusion-concurrency test | Included in the 15 integration tests and passed |
| New middleware tests | 4/4 passed: live health, missing/malformed JWT, allowlisted/rejected CORS, 429 throttling |
| `npm.cmd run build` | Passed; 3 size/performance warnings, 1.02 MiB entry point |
| `npm.cmd run test:e2e` | 5/5 tests visibly passed; runner required manual interrupt after its managed servers failed to exit |
| NuGet vulnerability audit | Passed; no vulnerable package entries |
| npm audit after remediation | Full tree: 36 (0 critical, 16 high, 20 moderate); production view: 18 (0 critical, 6 high, 12 moderate) |

The solution's retired `WebUI.AcceptanceTests` assembly contains no discoverable tests. Playwright is the active browser suite. The E2E runner printed all five `ok` results (invalid login, protected routes/logout, patient workflow, physician isolation, unauthorized chat/EMR), then hung during web-server teardown; therefore the observed tests passed but the command exit was interrupted.

Existing nullable, obsolete FluentValidation API, and frontend bundle-size warnings remain. No assertion or security check was weakened.
