# Telemedicine Full System Audit

Date: 2026-10-23
Project: `C:\SyncGrass Limited\Topics\telemedicine`
Audit type: read-only source, API, database and test audit
Implementation status: **not started; awaiting plan approval**

## 1. Executive summary

The repository is a working **booking, role and sandbox-payment prototype**, not a complete
telemedicine platform. Its core authentication, patient/doctor profiles, doctor directory,
appointment booking/acceptance/cancellation, hospital management, admin analytics and sandbox
payment flow are operational and verified by previous audits. The existing 23 automated tests pass
and the API/SPA build cleanly.

However, the application does **not** currently provide the majority of the capabilities in the
Phase 0-16 brief:

- No video consultation or WebRTC implementation. The chat UI has a dead video-call button and is
  populated with hard-coded contacts/messages.
- No prescriptions, invoices/receipts, persistent notifications, doctor availability/slots,
  structured medical records, consultation-session lifecycle or audit trail.
- The existing medical-record read endpoint returns HTTP 500 as soon as a record exists.
- Chat REST endpoints and the SignalR hub are reachable without authentication.
- Existing tests are all **template tests** (Todo, Colour, generic mapping/logger); none test a
  healthcare workflow.
- Demo data is not reproducible from a clean database: the seeder creates only roles, the admin
  identity and a Todo list. The current patient/doctor/hospital/service/payment/appointment data
  was seeded manually during previous audits.
- Registration creates an Identity account and role only; it does not create a Patient or Physician
  domain profile. Profile completion is a separate manual step and doctor signup currently reaches
  a dashboard that fails until a profile exists.

This is a credible final-year **demonstration prototype**, but it cannot honestly be described as a
complete, secure, enterprise-grade healthcare platform until the plan in `IMPLEMENTATION_PLAN.md`
is approved and executed.

## 2. Scope and method

Inspected:

- All solution projects, `.csproj` files and package references.
- All domain entities and the EF Core context/migration.
- All controllers and application command/query handlers.
- The Vue router, layouts, pages, stores and API wrapper.
- The live PostgreSQL schema, constraints, indexes and current synthetic data.
- Phase 1, Phase 2 and Phase 3 audit reports and handover guides.
- Existing automated test sources and their actual assertions.

Executed:

- Existing domain/application/integration tests: **23/23 pass**.
- API build: **PASS** (0 errors, 62 warnings).
- SPA production build: **PASS**.
- Read-only/diagnostic API probes against the local synthetic database.
- The temporary synthetic health reports created for the probe were deleted after the test.

No production code was modified during this audit.

## 3. Verified technology and architecture

| Layer | Actual implementation |
|---|---|
| Backend | ASP.NET Core 10 Web API, C# 13-era SDK |
| Architecture | Layered Clean Architecture: Domain, Application, Infrastructure, API |
| Patterns | MediatR commands/queries, AutoMapper, FluentValidation, repository + UnitOfWork |
| Identity | ASP.NET Core Identity + JWT bearer, custom `[Permit]` policy handler |
| Database | PostgreSQL 10.0.1 Npgsql, EF Core migrations |
| Realtime | SignalR `ChatHub` mapped at `/chat-hub` |
| Frontend | Vue 3, Vuex 4, Vue Router 4, Vue CLI 5, Tailwind, PrimeVue, ApexCharts |
| Frontend API | Hand-written `fetch` wrapper in `src/api/index.js` |
| Tests | NUnit/FluentAssertions/Moq, Respawn integration fixture, legacy SpecFlow/Playwright |
| Runtime ports | API `7231`/`5231`, SPA `8080` |
| Database | `telemedicine` development; `telemedicine_test` integration test |

### Backend projects

```
telemedicine-webapi/src/
├── Domain/         entities, enums, domain events, value objects
├── Application/    commands, queries, DTOs, validation, access helpers
├── Infrastructure/ Identity, JWT, EF Core, repositories, CSV, migrations
├── Contract/       auth contracts
├── telemedicine.api/ controllers, middleware, authorization, health checks
└── tests/          domain/application/integration/acceptance projects
```

### Frontend structure

```
telemedicine-spa/src/
├── api/index.js                  active API wrapper
├── api-services/index.js         alternate unused wrapper
├── router/index.js               routes and role guard
├── Layout/                       Admin/User/Physician layouts
├── components/account-dashboard/ role-specific sidebars
├── components/common/            toast notifications
└── views/                        General, Patient, Physician, Admin, Doctors
```

## 4. Current database/domain inventory

Live tables: **19** (`Appointments`, `AspNet*`, `Comments`, `HealthAnalysisReport`, `Hospitals`,
`Messages`, `PhysianPatientTransactions`, `TelemedicinePayments`, `TelemedicineServices`,
`TodoItems`, `TodoLists`, `User`, `__EFMigrationsHistory`).

Current synthetic data:

| Entity | Rows / state |
|---|---|
| Identity users | 3 (`administrator@localhost`, `patient@demo.local`, `physician@demo.local`) |
| Patient profiles | 1 (synthetic) |
| Physician profiles | 1 (synthetic, `General Practice`, license `DEMO-MP-001`) |
| Hospitals | 1 (`Demo Community Hospital`) |
| Telemedicine services | 1 (`General Consultation`, 500) |
| Appointments | 4 (1 cancelled, 3 active/not-attended) |
| Payments | 4 (all paid sandbox records, amount 500) |
| Health analysis reports | 0 after audit cleanup |
| Comments | 0 |
| Messages | 0 |
| Physician-patient transactions | 0 |

Existing relationships:

- `Appointment` -> `Patient`, `Physician`, `TelemedicineService`.
- `TelemedicinePayment` -> `TelemedicineService` only.
- `HealthAnalysisReport` -> `Patient`; `Comment` -> `Physician`, `HealthAnalysisReport`.
- `Message` -> sender `User` via `SenderUserId`; receiver is only a username string.
- `Physician`/`Patient` are TPH subclasses of `User`; `User` contains License,
  MedicalSpecialization and HospitalId columns.
- Hospital -> Doctors collection exists, but no command/UI assigns a doctor to a hospital.

Missing domain concepts/tables:

- Specialities (free-text `MedicalSpecialization` only).
- Doctor availability, working hours, breaks, exceptions, slot reservations.
- Consultation session/room, encounter, clinical note versioning.
- Structured EMR (history, symptoms, diagnosis, treatment plan, allergies, medications,
  follow-up, documents, patient visibility).
- Prescription and prescription items.
- Invoice, receipt, payment transaction status/idempotency key, patient/payment/appointment link.
- Persistent notification.
- Audit log/access log.
- Doctor verification/approval status.
- Account status/lockout/email verification/password reset.

Schema risks:

- No unique constraint prevents two appointments for the same physician/time.
- `ScheduleStartTime` is a computed `DateTime.UtcNow` property, not persisted. Only
  `AppointmentDate` is stored; duration/end is not enforced.
- No `RowVersion`/concurrency token on appointments or slot reservations.
- `Message.SenderUserId` is `int`, while Identity uses `Guid`; receiver is a username string.
- `HealthAnalysisReport.CreatedBy` is populated by the audit interceptor with the Identity GUID,
  not a clinician identity/display name.
- No indexes beyond foreign-key indexes; no time-range or status query indexes.
- `TelemedicinePayment` has no appointment or patient foreign key and no invoice identity.

## 5. API inventory and actual behaviour

### Authentication

| Method | Route | Auth | Status |
|---|---|---|---|
| POST | `/api/auth/register` | Anonymous | Creates Identity account + role only; no domain profile |
| POST | `/api/auth/login` | Anonymous | Returns JWT + role |
| POST | `/api/auth/refresh` | Anonymous | **Stub: always 400 "not yet implemented"** |

JWT expiry: 3600 minutes (`appsettings.Development.json`). Frontend has no refresh implementation.
Token is stored in `localStorage`. No account lockout, email verification or password reset.

### Patients

| Method | Route | Policy | Status |
|---|---|---|---|
| GET | `/api/Patient` | Administrator | Returns profiles; no identity/account status |
| GET | `/api/Patient/me` | Patient | Current profile |
| GET | `/api/Patient/{id}` | Patient/Admin/Physician | Phase 3 ownership/relationship check |
| POST | `/api/Patient` | Patient | Creates profile; no identity/email uniqueness link |
| PUT | `/api/Patient/{id}` | Patient | Owner check; only first/last/email/phone/avatar fields |
| DELETE | `/api/Patient/{id}` | Administrator | Deletes domain profile |
| DELETE | `/api/Patient/purge` | Administrator | Purges profiles |

The Patient domain does not contain emergency contact, medical history, allergies, current
medications, diagnoses, treatment plans, or patient-visible record flags.

### Physicians

| Method | Route | Policy | Status |
|---|---|---|---|
| GET | `/api/Physician` | Admin/Physician/Patient | Directory list |
| GET | `/api/Physician/{id}` | Admin/Physician/Patient | Profile |
| GET | `/api/Physician/me` | Physician | Current profile |
| POST | `/api/Physician` | Admin/Physician | Creates profile; no identity-link/verification check |
| PUT | `/api/Physician/{id}` | Physician | Self check; does not accept specialization/address |
| DELETE | `/api/Physician/{id}` | Administrator | Delete |
| DELETE | `/api/Physician/purge` | Administrator | Purge |

No availability, consultation fee (service pricing only), qualifications, registration
verification/status, profile approval, consultation duration or slot management.

### Appointments

| Method | Route | Policy | Status |
|---|---|---|---|
| POST | `/api/Appointment` | Patient | Requires sandbox payment token; creates appointment |
| POST | `/api/Appointment/respond` | Physician | Accept/assign; invalid transitions rejected |
| GET | `/api/Appointment/unattended` | Physician/Admin | Unattended list |
| GET | `/api/Appointment/{id}` | Patient/Physician/Admin | Record-level check |
| GET | `/api/Appointment/patient/{patientId}` | Patient/Admin | Own-patient check |
| GET | `/api/Appointment/physician/{physicianId}` | Physician/Admin | Own-physician check |
| DELETE | `/api/Appointment/{id}` | Patient/Physician/Admin | Cancels; ownership check |
| PUT | `/api/Appointment/{id}` | Patient/Physician/Admin | Changes date/notes/status; no slot validation |

Not implemented: availability lookup, double-booking prevention, reschedule rules, duration/end
time, consultation type, join/start/complete commands, decline with reason, follow-up session,
payment/appointment foreign key.

### Payments

| Method | Route | Policy | Status |
|---|---|---|---|
| POST | `/api/TelemedicinePayment` | Patient | Sandbox payment; returns 16-char memory token |

The token is generated with `System.Random`, stored only in `IMemoryCache` for 15 minutes, and
consumed by scheduling. There is no invoice, receipt, failed/cancelled payment state, idempotency
key, patient FK, appointment FK or server-side payment-status lookup. The SPA disables the button
while processing, but the API itself is not idempotent.

### Hospitals / services / admin analytics

| Area | Endpoints | Status |
|---|---|---|
| Hospitals | `GET` list/single public; `POST/PUT/DELETE` Admin | Functional CRUD |
| Services | `GET` all roles; `POST/PUT/DELETE` Admin | Functional CRUD |
| Analytics | `/api/Analytics/users`, `services`, `payments`, `visitors`, `transactions`, `reports` | Mixed: users/services/payments real; visitors/transactions/reports **hard-coded sample data** |
| Active users | `GetActiveUsersQuery` | Commented out/not implemented |
| User management | No account status/role assignment endpoint | Missing |
| Doctor verification | No approval/rejection endpoint | Missing |
| Appointment/payment admin management | No filtered operational endpoints | Missing |

### Clinical / communication / template endpoints

| Area | Endpoint | Status |
|---|---|---|
| Health reports | `POST /api/HealthAnalysisReport/create` | Works for create; no clinician-patient relationship check |
| Health reports | `POST /api/HealthAnalysisReport/add-comment` | Works; no check that caller is the physician |
| Health reports | `GET /api/HealthAnalysisReport/{patientId}` | **HTTP 500** with existing records (NullReferenceException) |
| Chat REST | `/chat-hub/*` | **No authorization**; unauthenticated publish returned 200 |
| SignalR | `/chat-hub` | **No authorization**; any client can connect/publish |
| Chat persistence | none | Messages are not stored by the hub/controller |
| Physician-patient transactions | `PhysianPatientTransactions` table/controller | Controller is an empty placeholder |
| Todo | `/api/TodoLists`, `/api/TodoItems` | Template CRUD; unrelated to healthcare |
| Weather | `/WeatherForecast`, `/SpatialAnalysis` | Template/demo placeholder |
| OIDC configuration | `/_configuration/{clientId}` | Template stub |
| Health checks | `/health`, `/health/ready`, `/health/live` | Self/database checks; ready/live tags absent |

## 6. Frontend page inventory

| Route | Component | Status / evidence |
|---|---|---|
| `/` | Home | Static marketing landing page; departments/promos are static |
| `/login`, `/signup` | General | Functional API login/registration |
| `/dashboard` | Admin `Dashboard` or patient `DashboardPatient` by localStorage role | Admin analytics partly static; patient shows only appointments |
| `/dashboard/account` | Admin `Account` | Admin message; patient basic profile only |
| `/dashboard/update-record` | Patient `Patient` | Basic profile; no history/allergies/emergency/medications |
| `/dashboard/book-service` | `BookAppointment` | Functional service/doctor/date selection; static time list; fallback hard-coded services if API fails; no availability/overlap |
| `/dashboard/payment` | `Payment` | Sandbox checkout + schedule; functional, but no invoice/receipt |
| `/dashboard/chat` | `Chat` | **Simulated**: hard-coded contacts/messages; SignalR dynamic import is commented out; `@microsoft/signalr` not installed; video/emoji/attachment/new-chat buttons are dead |
| `/dashboard/feedback` | `DoctorFeedback` | **Simulated**: `TODO: Wire to feedback API`; no backend feedback entity; status filter wrong |
| `/dashboard/hospital` | `HospitalManagement` | Functional hospital CRUD for admin; same route is exposed in patient sidebar and fails server-side for patients |
| `/physician` | `DashboardPhysician` | Real stats/appointments; no consultation start/complete |
| `/physician/appointments` | `AppointmentsPhysician` | Real list/accept; no notes, join, complete, decline reason |
| `/physician/patients` | `PatientsPhysician` | Loads patients from appointments |
| `/physician/profile` | `ProfilePhysician` | Partial; specialization/address fields are not accepted by the update command |
| `/physician/unattended` | `UnattendedPhysician` | Real accept; no doctor availability context |
| `/dashboard/hospital` for admin | `HospitalManagement` | Functional CRUD |
| `views/Doctors/*` | Doctor, DashboardDoctor, Appointment | Unrouted/dead legacy pages |
| `views/Admin/Hospital.vue` | Hospital | Unrouted duplicate/legacy page |
| Header notification icon | `Header.vue` | Decorative; no notification center |
| Sidebar | Role-aware but coarse | Patient sidebar includes hospital admin page; admin/patient share `/dashboard`; no route-level doctor-page protection beyond server |

The design system is consistent enough to preserve (Tailwind, card-modern, primary color, sidebar
layouts). The phase-1 UI should be repaired rather than redesigned.

## 7. Module inventory with classifications

### Fully implemented

- Basic registration/login and role assignment.
- JWT bearer authentication and custom permission policies (for implemented endpoints).
- Patient basic profile create/read/update.
- Physician directory/profile read/create/self-update.
- Hospital CRUD.
- Telemedicine service CRUD.
- Appointment booking, list, retrieve, cancel, accept, status display.
- Record-level patient/appointment authorization added in Phase 3.
- Sandbox payment initiation and success path.
- Admin analytics endpoints for users/services/payments (plus hard-coded demo series).
- API health checks and Swagger.
- SPA build, login, patient/doctor/admin dashboard shells.

### Partially implemented

- Patient dashboard (appointments only; no prescriptions, records, payments, notifications).
- Doctor dashboard (appointments/stats; no consultation or clinical actions).
- Appointment management (no availability, slots, reschedule rules, duration, overlap checks).
- Payments (sandbox only; no invoice/receipt/status/idempotency/links).
- Medical records (create/comment backend, read broken, no structured model or UI).
- Admin analytics (real user/service/payment queries mixed with sample reports).
- Demo data (currently manual/synthetic; only admin/todo seeded).
- Authorization (good record checks on patient/appointments; gaps in health reports/chat).
- Frontend feedback/chat/notification shells (visual only).

### Backend only

- `HealthAnalysisReport`/`Comment` endpoints (no UI; read endpoint broken).
- `ChatHub`/`ChatHubController` (no working frontend; REST/hub unauthenticated; no persistence).
- `Messages` entity (no API/UI conversation workflow).
- `PhysianPatientTransactions` (placeholder; no controller).
- `TodoLists`/`TodoItems` template CRUD (not used by telemedicine UI).
- `WeatherForecast`/`SpatialAnalysis` template endpoints.
- CSV export commands (hospitals/patients/physicians) without controller/UI exposure.
- `GetActiveUsersQuery` (commented/stub).

### Frontend only

- Chat page static data and dead video/emoji/attachment/new-chat controls.
- Doctor feedback page (fake submit).
- Landing-page static departments/promos.
- Header notification icon (decorative).
- Unrouted `Doctors/*` and `Admin/Hospital.vue` legacy pages.
- Static analytics chart fallbacks in the admin dashboard.
- `api-services/index.js` duplicate unused wrapper.

### Simulated

- Sandbox payment (explicit simulation; legitimate for development).
- Chat messages/contacts (hard-coded, not real-time).
- Doctor feedback submission (setTimeout, not persisted).
- Some admin analytics (`visitors`, `transactions`, `reports`, partial chart fallbacks).
- Demo patient/doctor/hospital/service/payment/appointment records (synthetic, manually created).
- Profile/qualification details are synthetic, not verified credentials.

### Missing

- Video consultation, WebRTC, room lifecycle, participant media, mute/camera/leave.
- Consultation session/encounter entity and lifecycle.
- Doctor availability/working hours/breaks/blocked dates/duration.
- Slot reservation and double-booking prevention.
- Structured EMR, clinical observations, diagnoses, treatment plans, allergies, current medications,
  follow-up recommendations, document upload, patient-visible flags.
- Prescriptions and printable/downloadable prescription documents.
- Invoices, receipts, payment status lifecycle, idempotency, appointment/payment linkage.
- Persistent in-app notifications; email/SMS delivery; templates.
- Admin user management/account status/role assignment; doctor verification/approval; appointment
  and payment administration; cancellation/clinical reporting.
- Audit logs for clinical access/changes; data retention/deletion policy.
- Password reset, email verification, refresh tokens (stub), rate limiting/lockout.
- Real video signalling/STUN/TURN documentation and two-participant test.

### Blocked by external integration

- Production payment gateway (not desired/approved; sandbox stays default).
- Email/SMS provider (none configured).
- TURN server for reliable WebRTC across restrictive networks (STUN may suffice on localhost/LAN;
  TURN needs infrastructure/credentials).
- Live video cannot be represented as working until signalling and media connectivity are tested.
- Legal/regulatory compliance cannot be claimed without a dedicated assessment.

## 8. Security and privacy findings

### Strengths

- ASP.NET Core Identity password hashing (not plaintext).
- JWT bearer with role claims and custom `[Permit]` policies on implemented controllers.
- Phase 3 per-record patient/appointment checks; cross-patient access returns 403.
- Sandbox checkout does not collect/store card data.
- CORS is restricted by configured origins in production; development allows any origin.
- No real patient data in the repository; current records are synthetic.

### High-risk findings

1. **Unauthenticated chat surfaces.** `ChatHubController` has no `[Authorize]`; `MapHub("/chat-hub")`
   has no authorization. `POST /chat-hub/publish-to-all` returned **200 without a token**. The hub
   can broadcast to all clients and send files with no validation or room access control.
2. **Medical-record read failure.** `GET /api/HealthAnalysisReport/{patientId}` returned **HTTP 500**
   (NullReferenceException) when a record exists. It does not include required navigations and has
   no patient/physician relationship check.
3. **Broader clinical access than intended.** Health report create/add-comment take
   `Patientid`/`PhysicianId` from the request; there is no check that the current physician is the
   treating physician or that the patient is in their care. All physicians can read any patient's
   reports by id.
4. **No audit trail/consent.** Clinical read/write access is not audited; no patient visibility flag,
   legal-hold, retention or deletion policy.
5. **Token/refresh weakness.** Refresh endpoint is a stub; access token lifetime is 3600 minutes;
   token is stored in localStorage; no revocation/rotation or logout invalidation.
6. **Development secrets/defaults.** JWT signing key and local PostgreSQL password are committed;
   documented but must be replaced before deployment. `BlobStorageSettings` is empty (good).
7. **Appointment/payment integrity.** No DB uniqueness/concurrency control for slots; payment token
   is `System.Random`, in-memory, not idempotent, and not linked to an appointment/invoice.
8. **Demo onboarding.** Registration creates no Patient/Physician profile; a fresh physician
   registration cannot use the dashboard until a profile is created through a separate endpoint.

### Medium/low findings

- Admin dashboard mixes real and hard-coded analytics; could be mistaken for live metrics.
- Patient sidebar exposes hospital CRUD route; server rejects non-admin but UX is confusing.
- `BookAppointment` falls back to hard-coded services if the API fails; this can create fake
  service choices.
- `DoctorFeedback` accepts active appointments as completed and never persists feedback.
- `Patient/Account` and `Physician/Profile` update commands silently ignore fields not present in
  their commands (patient address/gender/birth; physician specialization/address).
- No rate limiting, account lockout, email verification or password reset.
- Dependency advisories remain (AutoMapper, ImageSharp, cryptography packages, npm tree).
- Legacy SpecFlow acceptance suite targets `/authentication/login`, which does not exist in this SPA.

## 9. Testing and QA findings

Existing suites:

| Suite | Count | What it actually covers |
|---|---|---|
| Domain.UnitTests | 5 | `Colour` value object only |
| Application.UnitTests | 8 | validation exception, mapping config, request logger |
| Application.IntegrationTests | 10 | TodoLists/TodoItems template CRUD |
| **Total** | **23** | **No authentication, appointment, payment, EMR, prescription or video tests** |
| WebUI.AcceptanceTests | not applicable | Legacy SpecFlow login against missing Identity UI path |

The 23 tests pass, but they do not verify any telemedicine business workflow. The Phase 2/3
browser/API probes were manual/Playwright smoke tests external to the project.

## 10. Maintainability and deployment findings

- Two parallel API wrappers (`api/index.js` active and `api-services/index.js` unused).
- Unrouted legacy pages (`views/Doctors/*`, `views/Admin/Hospital.vue`, unused layout variants).
- Template CSS/asset bundle and many unused icon/image assets.
- `Todo`, `WeatherForecast`, `SpatialAnalysis`, OIDC, CSV exports and placeholder transaction
  controller are not part of telemedicine functionality.
- No CI pipeline, no frontend unit tests, no healthcare test data factory.
- Two-process local run (API + SPA), PostgreSQL and manually created demo rows.
- 62 build warnings (nullable/deprecations/advisories) reported by the solution build.
- No production deployment configuration for video/TURN, email, payment provider or audit storage.

## 11. Risk register (highest first)

| ID | Risk | Severity | Impact | Recommended action |
|---|---|---|---|---|
| R1 | No video consultation | Critical for brief | Cannot demonstrate remote consultation | Implement WebRTC + authenticated SignalR signaling + room authorization |
| R2 | EMR read 500 / no structured records | Critical | Clinical records unusable | Fix EF includes; add structured record model/API/UI/visibility/audit |
| R3 | Unauthenticated chat/hub | High | Message/file broadcast abuse | Add `[Authorize]`, JWT SignalR auth, conversation membership checks, persistence |
| R4 | No slot/availability/double-booking control | High | Duplicate appointments, unreliable schedule | Add availability tables + transactional slot reservation + unique constraint |
| R5 | No prescriptions | High | Incomplete consultation outcome | Add prescription entities/API/UI/printable document |
| R6 | Payment not invoice/idempotent/linked | High | Billing cannot be demonstrated reliably | Add invoice/payment lifecycle/idempotency/appointment FK |
| R7 | No notifications | Medium | Poor workflow feedback | Add persistent in-app notification service/API/UI |
| R8 | Admin missing operational modules | Medium | No user/doctor verification | Add user status/role, doctor approval, appointment/payment/reporting views |
| R9 | No audit/consent/retention | High for healthcare | Privacy non-compliance | Add audit log, patient visibility, retention policy; document legal boundary |
| R10 | Registration/profile gap | Medium | New doctor cannot use app | Create domain profile during onboarding or guided completion |
| R11 | Tests cover only templates | High for QA | Regressions unnoticed | Add healthcare unit/integration/Playwright tests |
| R12 | Hard-coded/static analytics and fake UI | Medium | Misleading demo | Label sample data; replace dead controls; add real data or remove |
| R13 | Dev secrets/long-lived token/no refresh | High for deployment | Token theft, insecure ops | Secret management, short tokens, refresh/revocation, rate limiting |
| R14 | Legacy/unused code | Low | Maintenance burden | Remove or isolate after approval |

## 12. Requirements-to-implementation snapshot

See the detailed matrix in `IMPLEMENTATION_PLAN.md`. High-level status:

| Functional area | Current status |
|---|---|
| Identity and access | Partially implemented; refresh stub; no reset/verification/account status |
| Patient management | Partially implemented (basic profile only) |
| Doctor management | Partially implemented (profile/directory; no verification/availability) |
| Appointment management | Partially implemented (booking/accept/cancel; no slots/rules) |
| Teleconsultation | Missing video; chat simulated/unsecured |
| Electronic medical records | Backend partial/broken; no structured model/UI; privacy gaps |
| Prescription management | Missing |
| Payments and billing | Sandbox payment only; no invoice/receipt/idempotency |
| Notifications | Frontend toast only; no persistent backend |
| Administration | Hospital CRUD; missing users/verification/appointments/payments/reporting |
| Reporting/analytics | Partial; some static sample data |
| Configuration/audit logging | Missing |
| Testing | 23 template tests; no healthcare coverage |
| Demo data | Partial/manual; not reproducibly seeded |

## 13. Audit conclusion

The application is a good starting point and its existing architecture can support the target
platform without a rewrite. The booking, role and sandbox payment workflows should be preserved.
The platform is **not** currently a complete telemedicine system: video, availability, clinical
records, prescriptions, notifications, billing integrity, administration, audit logging and
healthcare testing are missing or incomplete.

Implementation must proceed incrementally, with migrations and tests at each release, and with
explicit approval before any destructive database change or production integration. The proposed
sequence, dependencies, file impact, risks and acceptance criteria are in
`docs/audits/IMPLEMENTATION_PLAN.md`.
