# Release 1 Architecture

Status: implemented and verified for the approved Release 1 scope.
Date: 2026-10-23
Project: `C:\SyncGrass Limited\Topics\telemedicine`

## 1. Scope

Release 1 delivers identity onboarding, linked patient and physician profiles,
physician professional information, recurring availability, slot generation, booking,
rescheduling, cancellation, physician acceptance/decline, record-level authorization,
secure chat surfaces, the EMR read regression fix, real dashboards and reproducible
synthetic demonstration data.

Explicitly outside Release 1:

- WebRTC media, consultation rooms, offer/answer/ICE relay, TURN deployment.
- The structured clinical-record and prescription redesign (Release 3).
- Invoices, receipts, persistent notifications and admin operational modules (Release 4).
- Production payment gateway or live clinical use.

## 2. Layered architecture

The existing Clean Architecture split is preserved:

```text
Domain           Patient, Physician, Appointment, PhysicianAvailability,
                 HealthAnalysisReport, Comment, Message, Hospital, Service
Application      MediatR commands/queries, validators, DTOs, slot calculator,
                 appointment access policy, chat access policy
Infrastructure   EF Core 10 / Npgsql, ASP.NET Identity, JWT, seeding,
                 repositories/UnitOfWork, audit interceptor
telemedicine.api Controllers, [Permit] authorization, SignalR hub, middleware
telemedicine-spa Vue 3, Vuex, Vue Router, Vite, Tailwind, PrimeVue
```

No microservices, no parallel replacement stack and no rewrite of the existing
controllers/repositories were introduced. Existing endpoints and legacy behaviour
that do not conflict with Release 1 were retained.

## 3. Identity and onboarding

- `Patient` and `Physician` remain TPH subclasses of `User`.
- `User.IdentityUserId` stores the immutable ASP.NET Identity id. It is nullable for
  legacy rows and protected by a unique index. A migration backfills it by email.
- Registration accepts the account type plus the minimum linked profile data.
- `AuthService.Register` creates the Identity account and the Patient/Physician
  profile in one relational transaction. Failure rolls back both writes; unique
  indexes provide the final duplicate guard under concurrency.
- The profile email must match the authenticated account for profile-completion
  endpoints. Duplicate identities, profile emails and physician licences are rejected.
- Demo accounts are synthetic. The administrator is seeded only when demo data or the
  explicit `SeedAdmin` option is enabled; production defaults do not seed an admin.
- The demo seeder resets only the documented synthetic demo passwords so the
  demonstration is reproducible from a previous local state.

## 4. Authorization

Two independent layers are used:

1. Controller/endpoint role policies through `[Permit("...")]` and a custom policy
   provider/handler. The effective endpoint `Permit` is resolved from endpoint
   metadata, so an action-level grant can intentionally broaden a restrictive
   controller-level policy without relying on fragile display-name parsing.
2. Record-level checks in application handlers for patients, physicians, appointments,
   health reports and chat recipients. Frontend hiding is never treated as security.

Administrators may perform administrative reads/cancellations. A physician may see a
patient only when an appointment relationship exists. A patient may see only their own
profile and appointments. Chat additionally requires an accepted appointment.

## 5. Availability and slot model

- Availability is persisted as recurring weekday intervals
  (`PhysicianAvailability`): day, start, end, slot duration and active flag.
- All Release 1 instants and recurring times are interpreted as **UTC**. The SPA
  converts UTC slot instants to the browser's local time zone for display.
- A slot is a half-open interval `[start, end)`. Generation walks each active rule,
  advances by the rule's slot duration, rejects past slots/out-of-range slots and
  subtracts non-terminal appointments.
- The slot endpoint accepts a maximum 62-day range and returns `start`, `end`,
  `startUtc`, `endUtc` and `durationMinutes`.
- Booking re-runs the availability/slot calculation server-side; the client cannot
  manufacture an arbitrary slot. Payment tokens are single-use and are consumed only
  after a successful booking commit.

## 6. Appointment integrity and lifecycle

- `Appointment` stores a required UTC `AppointmentDate` and `ScheduleEndTime` plus an
  optional `StatusReason`.
- Statuses: `Active`, `NotAttended`, `Attended`, `Cancelled`, `Pending`, `Declined`.
- Booking creates `Pending`; physician acceptance moves it to `Active`; physician
  decline sets `Declined` and requires a reason; cancellation sets `Cancelled` and
  frees the slot; rescheduling validates and moves the interval.
- Application checks reject past, unavailable, overlapping and misaligned slots.
- PostgreSQL is the final concurrency authority. A `btree_gist` exclusion constraint
  prevents overlapping non-terminal intervals for the same physician. A losing
  concurrent request receives a controlled failure rather than a 500.
- Rescheduling uses the same server-side slot rules and excludes the appointment
  itself from the overlap query.

## 7. Chat and realtime security

- The existing SignalR hub and chat REST controller require JWT authentication.
- Client-supplied connection ids and public broadcast are not accepted.
- REST and hub recipients are identified by recipient email and authorized against
  an accepted patient-physician appointment relationship.
- Client-managed group joins, group publishing and arbitrary recipient targeting are
  rejected. The default SignalR user identifier is replaced by a server-side
  `IUserIdProvider` that reads the token's immutable `uid`/name identifier.
- File messages are capped at 2 MB and limited to images, PDF and plain text.
- Full persistent conversation/messaging design remains a later release; Release 1
  only secures the existing surfaces.

## 8. EMR regression fix

- `GET /api/HealthAnalysisReport/{patientId}` now loads reports, comments and comment
  physicians explicitly, projects to DTOs and never dereferences a missing navigation.
- The read query is restricted to an administrator or a physician with an appointment
  relationship to the patient.
- Create/add-comment use the authenticated physician identity and require the treating
  relationship for physicians. The full structured EMR redesign remains Release 3.

## 9. Frontend

- Existing Vue 3, Vuex, Vue Router and component architecture is retained. The local
  frontend toolchain in the final working tree uses Vite, Tailwind and the existing
  PrimeVue/ApexCharts components; the Vue 3 source architecture is unchanged.
- New routes: doctor directory/profile, patient appointment list/details, physician
  availability editor and physician appointment details.
- Signup now collects and submits the linked profile in one operation.
- Booking uses real services, real physicians and server-generated slots; static time
  lists and fake doctor fallbacks were removed.
- Payment uses the real sandbox payment token and creates the appointment through the
  API.
- Patient and physician dashboards read real appointment data. Appointment details
  expose only actions allowed by role/status; reschedule, cancel, accept and decline
  call working endpoints.
- Playwright assertions fail on unexpected browser console errors, page errors or
  HTTP 500 responses during R1 workflows.

## 10. Configuration and demo operation

- Development uses the local PostgreSQL database `telemedicine`; integration tests use
  `telemedicine_test`.
- `DemoData:Enabled=true` seeds the admin, two synthetic doctors, a synthetic patient,
  services, hospital, weekday availability and sample appointments.
- `Database:ApplyMigrationsOnStartup` is explicit. Demo/development may enable it;
  production should apply migrations through the deployment process.
- Demo credentials are documented in `DEMO_GUIDE.md` and `R1_END_TO_END_EVIDENCE.md`.

## 11. Release 2 gate

No WebRTC implementation is approved by this Release 1 certification. The recommended
Release 2 architecture is documented in
`docs/releases/R1.1/RELEASE_2_VIDEO_ARCHITECTURE_PROPOSAL.md` and summarised here:

- Keep SignalR as an authenticated signalling plane only.
- Derive consultation-room membership from accepted appointment participants on the
  server; never accept client-selected groups/sender identities.
- Use two-party WebRTC peer connections with HTTPS/WSS, explicit media consent and no
  recording by default.
- Provide STUN plus a TURN service over TLS/443 with short-lived credentials and
  operational monitoring; STUN-only is not sufficient for restrictive networks.
- Define room creation/expiry, join/leave/end lifecycle, reconnect/ICE restart, TURN
  fallback, participant presence, audit-safe logging and network-failure UX.
- Test with two isolated browser contexts and synthetic media, including unauthorized
  third-party denial, expired-room denial, reconnect and device-permission failures.
- Production deployment requires secrets management, TURN capacity planning, privacy
  review and clinical/regulatory approval.

This Release 2 proposal is submitted for separate approval and is **not** implemented
as part of this Release 1 certification.
