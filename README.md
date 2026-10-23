# Telemedicine Platform

## Overview

A telemedicine web platform with an ASP.NET Core 10 Web API and a Vue 3 single-page application.
It supports role-based accounts (Administrator, Physician, Patient), hospital and physician
directories, patient profiles, telemedicine service catalogues, simulated payments, appointment
scheduling, messaging, health reports and an admin analytics dashboard.

The repository contains two independent applications:

```
telemedicine/
  telemedicine-webapi/   ASP.NET Core API, EF Core, Identity, SignalR
  telemedicine-spa/      Vue 3 SPA (Vue CLI 5, Vuex, Vue Router, PrimeVue, Tailwind)
```

## Problem statement

Patients in remote or under-served areas often travel long distances for routine consultations.
A telemedicine platform lets them find hospitals and physicians, book remote consultations and
communicate online. This project demonstrates the account, directory, booking and payment
workflows needed for such a system.

## Aim

To provide a role-based telemedicine prototype covering patient registration, physician discovery, appointment booking, simulated payment and role-specific dashboards.

## Objectives

- Provide secure registration/login with Administrator, Physician and Patient roles.
- Manage hospitals, physicians, patients and telemedicine services.
- Let patients create a medical profile, pay for a service and schedule an appointment.
- Provide physician and admin dashboards with appointment and payment data.
- Demonstrate a layered .NET solution (Domain, Application, Infrastructure, API) and a Vue SPA.

## Technology stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core 10, MediatR, AutoMapper, FluentValidation, CsvHelper |
| Identity/Security | ASP.NET Core Identity, JWT bearer, custom `[Permit]` policies |
| Persistence | EF Core 10, PostgreSQL (Npgsql) |
| Realtime | SignalR chat hub |
| Frontend | Vue 3, Vuex 4, Vue Router 4, Vue CLI 5, Tailwind CSS, PrimeVue, ApexCharts |
| Tests | NUnit, FluentAssertions, Moq, Respawn, SpecFlow/Playwright (legacy) |

## Architecture

```
Vue SPA (telemedicine-spa, port 8080 in development)
  -> /api (Vue CLI proxy; same-origin in production)
      -> ASP.NET Core API (telemedicine-webapi, ports 7231/5231)
          -> Controllers -> MediatR commands/queries
              -> Application validators/mappings
              -> Infrastructure (EF Core, Identity, JWT, repositories)
                  -> PostgreSQL `telemedicine`
          -> SignalR `/chat-hub`
```

The API layers are `Domain`, `Application`, `Infrastructure`, `Contract` and `telemedicine.api`.
The SPA is a separate project and is not built or served by the API; run it with Vue CLI.

## Software prerequisites

- .NET 10 SDK (`dotnet --list-sdks`)
- Node.js 18+ and npm 9+
- PostgreSQL 14+ (tested on PostgreSQL 18)
- Optional: Playwright browsers for the legacy acceptance test project

## Database setup

Create the database:

```sql
CREATE DATABASE telemedicine;
```

Default development connection (`src/telemedicine.api/appsettings.Development.json`):

```
Host=localhost;Database=telemedicine;Username=postgres;Password=worisoft@123
```

Migrations are applied automatically at API startup (`ApplicationDbContextInitialiser`). The
seeder creates the roles `Administrator`, `Physician`, `Patient`, the admin account, and a sample
Todo list. Hospital, physician, service and patient demo records are created through the API (or
the UI) as described below.

The integration test project uses a separate database:

```sql
CREATE DATABASE telemedicine_test;
```

and the connection string in `tests/Application.IntegrationTests/appsettings.json`.

## Environment configuration

API environment variables (a `.env` file is not auto-loaded):

```
ASPNETCORE_ENVIRONMENT=Development
DefaultConnection=Host=localhost;Database=telemedicine;Username=postgres;Password=worisoft@123
JwtSettings__Key=replace-with-a-random-base64-secret
JwtSettings__Issuer=Telemedicine
JwtSettings__Audience=https://localhost:44396/
JwtSettings__DurationInMinutes=60
Cors__AllowedOrigins__0=http://localhost:8080
```

See `telemedicine-webapi/.env.example`.

SPA configuration (`telemedicine-spa/.env.development`):

```
VUE_APP_API_BASE_URL=/api
```

`vue.config.js` proxies `/api` to `http://localhost:5231` by default. Override with
`VUE_APP_API_TARGET` if your API runs on a different URL.

## Build

```powershell
# API
dotnet build telemedicine-webapi\telemedicine-webapi.sln

# SPA
cd telemedicine-spa
npm install
npm run build
```

## Run

### 1. Start the API

```powershell
cd telemedicine-webapi\src\telemedicine.api
dotnet run
```

- HTTPS: <https://localhost:7231>
- HTTP: <http://localhost:5231>
- Swagger: <https://localhost:7231/swagger>
- Health: <https://localhost:7231/health>

### 2. Start the SPA

```powershell
cd telemedicine-spa
npm run serve -- --port 8080
```

Open <http://localhost:8080/>.

The SPA dev server proxies `/api` to the API, so both must be running.

## Demo accounts (synthetic local data)

| Role | Email | Password |
|---|---|---|
| Administrator | `administrator@localhost` | `Administrator1!` |
| Patient | `patient@demo.local` | `Patient@12345` |
| Physician | `physician@demo.local` | `Physician@12345` |

The patient and physician accounts are synthetic accounts created during the audit. The patient
account has a completed profile; the physician account has a physician profile. The local database
also contains a synthetic hospital (`Demo Community Hospital`) and service
(`General Consultation`, cost 500).

To recreate a patient from scratch:

1. Register at `/signup` with account type `Patient`.
2. Log in, open **Update Record**, and submit the patient profile with the same email as the
   account.
3. The profile is required because `/api/Patient/me` looks up the patient by the signed-in email.

To recreate the rest of the demo data on a fresh database:

1. Start the API and open <https://localhost:7231/swagger>.
2. Log in as the admin (`administrator@localhost` / `Administrator1!`) with `POST /api/auth/login`
   and paste the token into Swagger's **Authorize** box.
3. Create a hospital with `POST /api/Hospital`:
   `{"name":"Demo Community Hospital","location":"Cape Town, South Africa"}`.
4. Create a service with `POST /api/TelemedicineServices`:
   `{"serviceName":"General Consultation","cost":500,"description":"Synthetic demo service"}`.
5. Register a physician through the SPA, log in as that physician, and create the physician profile
   with `POST /api/Physician` (include `license`, `medicalSpecialization` and `accountType: 1`).
6. As the patient, book through the SPA: **Book Appointment** -> choose service/date/time/doctor
   -> **Continue to Payment** -> **Pay (demo)**. The page performs the equivalent of
   `POST /api/TelemedicinePayment` followed by `POST /api/Appointment` with `serviceId`,
   `paymentToken`, `appointmentDate` and `physicianId`.

All records created this way are synthetic; do not enter real patient information.

## Demonstration procedure

1. Sign in as the patient and book **General Consultation** with a future date/time and Dr. Demo.
2. Continue to the sandbox payment page and complete the simulated payment.
3. Show the appointment confirmation and the appointment on the patient dashboard.
4. Sign in as the physician, open **Unattended**, and accept the request.
5. Show the appointment becomes Active and appears in the physician's appointments/patients.
6. As the patient, cancel an appointment from the dashboard.
7. As the administrator, show hospital/service management and the dashboard.
8. Optional: attempt to open another patient's appointment/profile and show 403.

The full academic presentation script is in `DEMO_GUIDE.md`.

## Main features

Implemented end-to-end:

- Registration, login, JWT authentication, role claims and `/api/auth/me` equivalents.
- Admin hospital CRUD and analytics endpoints.
- Patient registration/profile creation and retrieval.
- Physician directory and physician profiles.
- Telemedicine service catalogue (admin create/update/delete; all roles read).
- Simulated payment API that returns a short-lived, single-use payment token.
- Patient checkout page that calls the payment API and then creates the appointment. It is clearly
  labelled as a sandbox and collects no card data.
- Appointment scheduling with optional requested date/time and physician assignment.
- Appointment responses now include patient and physician display names and the linked service.
- Physician appointment list, unattended requests, and "accept appointment" workflow; accepting an
  unassigned request assigns it to the responding physician.
- Patient appointment dashboard with correctly mapped statuses and cancellation.
- Admin dashboard, patient dashboard, booking page, update-record page, payment page.
- SignalR chat hub and health-analysis endpoints.

Verification highlights (Phase 3):

- Invalid appointment state transitions are rejected (accepting an already-active appointment
  returns 400).
- Sandbox payment rejects insufficient amounts; the payment token is single-use, so a repeated
  schedule request fails.
- Cross-patient appointment/profile access returns 403; assigned physicians and administrators have
  the documented access.
- Appointment responses include patient, physician and service display names.

Security and authorization:

- Patient records and appointments are restricted to the owning patient, the assigned physician or
  an administrator.
- A physician can read a patient record only when they have an appointment with that patient.
- Role-based navigation and protected API policies were verified for all three roles.

Partially implemented or template-derived:

- No real video consultation is implemented; chat is the realtime component.
- Todo CRUD endpoints remain from the Clean Architecture template.
- Some analytics widgets use template/sample data rather than live clinical analytics.

## API overview

| Area | Endpoints |
|---|---|
| Auth | `POST /api/auth/register`, `POST /api/auth/login`, `POST /api/auth/refresh` |
| Hospitals | `GET/POST /api/Hospital`, `GET/PUT/DELETE /api/Hospital/{id}` |
| Patients | `GET /api/Patient`, `GET /api/Patient/me`, `GET/POST/PUT/DELETE /api/Patient/{id}` |
| Physicians | `GET /api/Physician`, `GET /api/Physician/me`, `GET/POST/PUT/DELETE /api/Physician/{id}` |
| Services | `GET/POST /api/TelemedicineServices`, `GET/PUT/DELETE /api/TelemedicineServices/{id}` |
| Payments | `POST /api/TelemedicinePayment` |
| Appointments | `POST /api/Appointment`, `GET /api/Appointment/patient/{id}`, `GET /api/Appointment/physician/{id}`, `GET /api/Appointment/unattended`, `PUT/DELETE /api/Appointment/{id}` |
| Analytics | `GET /api/Analytics/users`, `/services`, `/payments`, `/visitors`, `/transactions`, `/reports` |
| Health checks | `GET /health`, `/health/ready`, `/health/live` |

Authorization uses `[Permit("Administrator")]`-style policies over a JWT `Role` claim. Protected
reads return `401` when no valid bearer token is supplied.

## Testing

```powershell
# Unit tests
dotnet test telemedicine-webapi\tests\Domain.UnitTests\Domain.UnitTests.csproj
dotnet test telemedicine-webapi\tests\Application.UnitTests\Application.UnitTests.csproj

# Integration tests (require PostgreSQL and database telemedicine_test)
dotnet test telemedicine-webapi\tests\Application.IntegrationTests\Application.IntegrationTests.csproj

# SPA production build
cd telemedicine-spa
npm run build
```

Audit result: 23 backend tests passed (5 domain, 8 application, 10 integration). The integration
suite uses PostgreSQL and Respawn to reset the isolated `telemedicine_test` database; it never
touches the development `telemedicine` database.

The `tests/WebUI.AcceptanceTests` SpecFlow project is a legacy template suite that targets
`/authentication/login` pages that are not part of this SPA. It is not part of the current
verification path; use the Vue UI and the Playwright smoke tests described in the audit report.

## Troubleshooting

| Symptom | Fix |
|---|---|
| API exits with an Npgsql connection error | Ensure PostgreSQL is running and the `telemedicine` database exists. |
| `401` on every protected endpoint | Log in again and send `Authorization: Bearer <token>`; the JWT dev key must match between issuer and API config. |
| SPA login says "Invalid email or password" | Start the API on port 5231 and confirm the Vue CLI proxy is active. |
| CORS errors | In development the API allows any origin; in production update `Cors:AllowedOrigins`. |
| Patient dashboard says "Patient record not found" | Complete the profile under **Update Record** using the same email as the account. |
| Integration tests cannot connect | Create `telemedicine_test` and run the test project; it uses Respawn with the PostgreSQL adapter. |
| Ports already in use | Change `--urls` for the API or `--port` for `npm run serve`. |

## Known limitations

- Not a clinical system: no real video, prescriptions, regulatory compliance, audit trail or
  encryption at rest. Never store real patient data in it.
- Payment is simulated with an in-memory, single-use token; no payment gateway or card processing
  is integrated, and the demonstration must not be presented as a real payment system.
- Appointment date/time is stored when supplied by the booking page; the API also accepts a
  request without a date for backwards compatibility.
- Unassigned appointments are visible to physicians; a physician can accept one to claim it.
- Legacy Todo endpoints and some template files remain.
- Some analytics cards use template/sample values, not live clinical measurements.
- Some NuGet dependencies have published advisories (AutoMapper, System.Security.Cryptography.*);
  review and upgrade before production use.
- The checked-in JWT key and local database password are development defaults and must be
  replaced for any deployment. The previously committed Azure Blob connection string was removed
  and should be treated as compromised.
