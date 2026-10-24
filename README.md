# Telemedicine Platform

A synthetic telemedicine demonstration platform built with ASP.NET Core 10 and Vue 3.
Release 1 covers identity onboarding, patient/physician profiles, physician
availability, appointment booking/rescheduling/cancellation, physician
acceptance/decline, role and record-level authorization, secure chat surfaces and the
EMR read regression fix.

This is a demonstration system. It contains no real patient data and must not be used
for clinical care.

## Technology stack

| Concern | Technology |
|---|---|
| API | ASP.NET Core 10, MediatR, FluentValidation, AutoMapper |
| Identity | ASP.NET Core Identity, JWT bearer, custom permission policies |
| Persistence | EF Core 10, PostgreSQL (Npgsql) |
| Realtime | SignalR (authenticated chat; WebRTC is not part of Release 1) |
| Frontend | Vue 3, Vuex, Vue Router, Vite, Tailwind, PrimeVue, ApexCharts |
| Tests | NUnit, FluentAssertions, Respawn, Playwright |

## Architecture

```text
Vue 3 SPA (telemedicine-spa, Vite)
  -> /api and /chat-hub proxy (http://127.0.0.1:5231)
      -> ASP.NET Core API
          -> Controllers -> MediatR commands/queries
              -> Domain + Application rules
              -> Infrastructure: EF Core, Identity, JWT, seeding
                  -> PostgreSQL telemedicine
```

Release 1 architecture, API, migration, security and test evidence:
`docs/releases/R1/`.

## Prerequisites

- .NET SDK 10
- Node.js 20+ and npm
- PostgreSQL 15+ (local service used for verification: PostgreSQL 18)
- Playwright Chromium for browser tests (`npx playwright install chromium`)

## Database setup

1. Create the databases:

   ```powershell
   psql -h localhost -U postgres -f database/create-databases.sql
   ```

   This creates `telemedicine` (demo) and `telemedicine_test` (integration tests).
2. Apply migrations:

   ```powershell
   dotnet ef database update `
     --project telemedicine-webapi/src/Infrastructure `
     --startup-project telemedicine-webapi/src/telemedicine.api `
     --connection "Host=localhost;Database=telemedicine;Username=postgres;Password=worisoft@123"
   ```

   In Development, the API can also apply migrations and seed demo data on startup
   (`Database:ApplyMigrationsOnStartup` and `DemoData:Enabled`).

## Configuration

`telemedicine-webapi/src/telemedicine.api/appsettings.Development.json` contains local
synthetic demo settings, including a local database password and JWT signing key. These
are development-only values and must be replaced before any non-demo deployment.
Production `appsettings.json` intentionally has no working database or JWT secret by
default.

The SPA uses `VITE_API_TARGET` for the Vite proxy (default
`http://127.0.0.1:5231`).

## Build

```powershell
dotnet build telemedicine-webapi/telemedicine-webapi.sln
npm install --prefix telemedicine-spa
npm run build --prefix telemedicine-spa
```

## Run

Start the API:

```powershell
dotnet run --project telemedicine-webapi/src/telemedicine.api --urls http://127.0.0.1:5231
```

Start the SPA in another terminal:

```powershell
npm run serve --prefix telemedicine-spa
```

Open `http://localhost:8080`.

Swagger: `http://localhost:5231/swagger`
Health: `http://localhost:5231/health`

## Demo accounts (synthetic)

Demo data is seeded only when `DemoData:Enabled=true` (Development default) or when the
administrator seed is explicitly enabled.

| Role | Email | Password |
|---|---|---|
| Administrator | `administrator@localhost` | `Administrator1!` |
| Patient | `patient@demo.local` | `Patient@12345` |
| Physician | `physician@demo.local` | `Physician@12345` |
| Second physician | `doctor@demo.local` | `Doctor@12345` |

The seed includes two physicians, a patient, a hospital, three services, weekday
availability (08:00-16:00 UTC, 30-minute slots) and synthetic appointments. All data is
synthetic; do not enter real patient information.

## Release 1 features

- Patient and physician registration with linked profiles in one transaction.
- Duplicate account/profile/licence prevention.
- Patient profile with emergency contact; physician professional profile with licence,
  speciality, qualifications and bio.
- Doctor directory and profile pages.
- Physician weekly availability editor.
- Server-generated, UTC, half-open appointment slots.
- Booking with service, physician, slot and sandbox payment token.
- Rescheduling and cancellation.
- Physician acceptance and decline with reason.
- PostgreSQL exclusion constraint preventing overlapping active/pending appointments.
- Patient and physician dashboards with live data.
- Role and record-level backend authorization.
- Authenticated chat REST/SignalR surfaces; arbitrary groups/broadcast are forbidden.
- Fixed, authorized EMR read endpoint.
- Reproducible synthetic demo data.

WebRTC/video consultation is deliberately excluded from Release 1. Do not implement or
deploy it without the separate Release 2 approval described in
`R1_COMPLETION_REPORT.md`.

## Testing

Backend:

```powershell
dotnet test telemedicine-webapi/tests/Domain.UnitTests/Domain.UnitTests.csproj
dotnet test telemedicine-webapi/tests/Application.UnitTests/Application.UnitTests.csproj
dotnet test telemedicine-webapi/tests/Application.IntegrationTests/Application.IntegrationTests.csproj `
  --filter "FullyQualifiedName!~ConsultationIntegrationTests"
```

The filter excludes unapproved Release 2 consultation tests present in the current
working tree. Release 1 result: 44/44 backend tests pass.

Browser workflows:

```powershell
cd telemedicine-spa
npx playwright test tests/e2e/release1.spec.js tests/e2e/r1-workflows.spec.js
```

Release 1 result: 7/7 Playwright tests pass. See `docs/releases/R1/R1_TEST_RESULTS.md`.

## API overview

- `POST /api/auth/register`, `POST /api/auth/login`
- Patient and physician profile endpoints (`/api/Patient`, `/api/Physician`)
- `PUT /api/physicians/me/availability`
- `GET /api/physicians/{id}/availability`, `/slots`
- `POST /api/Appointment`, `PUT/DELETE /api/Appointment/{id}`
- `POST /api/Appointment/respond`
- Patient/physician appointment queries
- `POST /api/TelemedicinePayment` (sandbox)
- Authenticated `/chat-hub` REST/SignalR
- `GET /api/HealthAnalysisReport/{patientId}` (physician/administrator)

Full request/response details: `docs/releases/R1/R1_API_REFERENCE.md`.

## Documentation

- `docs/releases/R1/R1_ARCHITECTURE.md`
- `docs/releases/R1/R1_DATABASE_MIGRATIONS.md`
- `docs/releases/R1/R1_IMPLEMENTATION_CHANGELOG.md`
- `docs/releases/R1/R1_API_REFERENCE.md`
- `docs/releases/R1/R1_TEST_RESULTS.md`
- `docs/releases/R1/R1_SECURITY_VERIFICATION.md`
- `docs/releases/R1/R1_END_TO_END_EVIDENCE.md`
- `docs/releases/R1/R1_COMPLETION_REPORT.md`
- `DEMO_GUIDE.md`

## Known limitations

- No WebRTC/video consultation, TURN infrastructure or recording.
- No structured clinical-record redesign, prescriptions or audit/consent trail.
- No invoices, receipts, persistent notifications or full admin operations.
- Refresh-token endpoint is a stub; no password reset/email verification/account
  lockout beyond rate limiting.
- Chat history/conversation persistence is not implemented.
- Build warnings and dependency advisories remain.
- Development secrets must be replaced for deployment.
- The final working tree contains unapproved Release 2 consultation code/tests added by
  concurrent work; they are not part of Release 1 certification.

## Troubleshooting

| Symptom | Check |
|---|---|
| API cannot connect | PostgreSQL is running; `DefaultConnection` is set |
| Demo login fails | `DemoData:Enabled=true` and the API has restarted to seed |
| SPA API calls fail | Vite proxy target `VITE_API_TARGET` and API port 5231 |
| Integration tests fail to start | `telemedicine_test` exists; PostgreSQL credentials match |
| Playwright cannot start | ports 5231/8080 are free; Chromium is installed |
| 401/403 from an endpoint | Check the role, ownership and treating relationship |
