# Release 1 End-to-End Evidence

Executed: 2026-10-23
Environment: Windows, .NET 10.0.402 SDK, PostgreSQL 18 local service,
Node 22.19.0, Playwright 1.64.1 Chromium.

All data used for these checks is synthetic. No real patient data was entered.

## 1. Build evidence

```powershell
dotnet build telemedicine-webapi/telemedicine-webapi.sln --no-incremental
```

Result:

```text
15 Warning(s)
0 Error(s)
```

```powershell
npm run build --prefix telemedicine-spa
```

Result: Vite production build completed successfully.

## 2. Database migration evidence

R1 migration: `20261023010348_Release1Scheduling`.

| Check | Result |
|---|---|
| Clean database `telemedicine_r1_clean` | Both migrations applied; constraints/indexes present |
| Existing-schema copy `telemedicine_r1_copy` | R1 migration applied; 3 Identity users, 2 profiles, 4 appointments preserved |
| Legacy duplicate | Row preserved, changed to Cancelled with migration reason |
| Identity linkage | Both legacy profiles linked by email to Identity ids |
| PostgreSQL exclusion constraint | `EX_Appointments_Physician_Time` present |
| Integration database | R1 migration applied; 31 R1-scope integration tests pass |
| Demo database | R1 migration applied; demo seed and Playwright pass |

Exact schema details and commands are in `R1_DATABASE_MIGRATIONS.md`.

## 3. Automated backend evidence

```powershell
dotnet test telemedicine-webapi/tests/Domain.UnitTests/Domain.UnitTests.csproj
dotnet test telemedicine-webapi/tests/Application.UnitTests/Application.UnitTests.csproj
dotnet test telemedicine-webapi/tests/Application.IntegrationTests/Application.IntegrationTests.csproj `
  --filter "FullyQualifiedName!~ConsultationIntegrationTests"
```

| Suite | Result |
|---|---|
| Original 23 tests (5 domain + 8 application unit + 10 integration) | 23/23 pass |
| R1-scoped integration tests (R2 tests excluded) | 31/31 pass |
| **R1 backend total** | **44/44 pass** |

The 23 original tests are included and remain passing. Concurrency evidence is
PostgreSQL-backed and includes both an API race and two direct EF Core inserts; no
InMemory database is used for concurrency evidence.

## 4. API smoke evidence

A running API on `http://localhost:5231` was exercised with real demo accounts and a
new synthetic account. Representative results:

| Probe | Result |
|---|---|
| Demo patient login | `200` role Patient |
| Demo physician login | `200` role Physician |
| `GET /api/Patient/me` | `200`, linked profile |
| `GET /api/Physician/me` | `200`, linked professional profile |
| `GET /api/physicians/{id}/availability` | `200`, 5 weekday rules |
| `GET /api/physicians/{id}/slots` | `200`, 141 future slots in a 14-day window |
| Register new patient | `200`, linked profile id returned |
| Duplicate patient profile | `400` (no second profile) |
| Patient calling physician-only profile | `403` |
| Sandbox payment | `200`, one-time token returned |
| Valid booking | `201`, Pending interval created |
| Same-slot booking attempt | `400` |
| Cross-patient appointment read | `403` |
| Physician decline with reason | `200` |
| Anonymous chat REST publish | `401` |
| EMR read with a record | `200`, no NullReferenceException (integration test) |
| Concurrent bookings | exactly one `201`, one controlled `400` |

The full automated R1 workflow tests also cover rescheduling, owner-only cancellation,
acceptance, physician access control, chat relationship checks and EMR relationship
denial.

## 5. Playwright browser evidence

Command:

```powershell
cd telemedicine-spa
npx playwright test tests/e2e/release1.spec.js tests/e2e/r1-workflows.spec.js
```

Result: **7 passed / 7** (Chromium).

Core patient workflow:

1. Open `/signup`, select Patient, submit full profile.
2. Sign in and see the real patient dashboard.
3. Open **Find a doctor**, select Dr. Demo physician, load real availability slots.
4. Select a real slot, continue to booking, select General Consultation, continue to
   payment.
5. Pay in the sandbox; the API creates a pending appointment.
6. Open appointment details, select a new available slot and reschedule.
7. Cancel with a reason and verify the status becomes Cancelled.
8. Verify the API (not only the UI) reflects `Cancelled`.

Core physician workflow:

1. Create a synthetic patient and two pending bookings through the real API.
2. Sign in as the Demo physician.
3. Load/save the real availability editor.
4. Open the pending appointments list and accept one request; status becomes Active.
5. Open the second pending request, decline with a reason; status becomes Declined.

Additional browser security/regression checks:

- invalid credentials remain on `/login`;
- anonymous protected routes redirect to login and remain protected after logout;
- patient directory and persisted appointments work against the real API;
- physician appointment list works and cross-physician access is denied;
- unauthenticated chat and EMR calls are rejected.

The R1 browser flows fail on unexpected console errors, page errors or HTTP 500
responses. The run completed cleanly.

Artifacts:

- `telemedicine-spa/playwright-report/index.html`
- `telemedicine-spa/test-results/` (traces/videos on failure; no R1 failure artifacts on
  the passing run)

## 6. Reproducible demonstration

1. Ensure PostgreSQL is running and create the demo database from
   `database/create-databases.sql`.
2. Apply migrations with the R1 connection command in
   `R1_DATABASE_MIGRATIONS.md`.
3. Start the API in Development with demo data enabled:
   ```powershell
   dotnet run --project telemedicine-webapi/src/telemedicine.api --urls http://localhost:5231
   ```
   Development `DemoData:Enabled=true` seeds the synthetic accounts and data.
4. Start the SPA:
   ```powershell
   npm install --prefix telemedicine-spa
   npm run serve --prefix telemedicine-spa
   ```
5. Open `http://localhost:8080` and use the credentials in
   `R1_API_REFERENCE.md` / `DEMO_GUIDE.md`.
6. Reset synthetic state by recreating the local demo database; never point reset
   commands at a database containing real data.

## 7. Scope caveat

The working tree also contains concurrent, unapproved Release 2 consultation/video
signalling code and tests. The unfiltered test command includes those tests; two R2
`ConsultationIntegrationTests` currently fail. They are outside Release 1 and were
excluded from the R1-scoped test command above. Release 1 certification does not cover
the R2 implementation, migration or UI.
