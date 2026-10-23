# Telemedicine Implementation Plan

Date: 2026-10-23
Based on: `TELEMEDICINE_FULL_SYSTEM_AUDIT.md`
Status: **Proposed; awaiting approval. No production implementation has started.**

## 1. Planning principles

- Preserve the existing ASP.NET Core 10 layered monolith and Vue 3 SPA. No microservices.
- Preserve the verified authentication, booking, payment and role behavior where it remains safe.
- Build missing healthcare workflows as modular feature slices inside the existing layers.
- Every release includes database migrations, backend, frontend, tests, documentation and a demo.
- Existing data is preserved. New columns are nullable/backfilled before constraints are enforced.
- No real payments, real patient data, unverified AI clinical recommendations or recording.
- Clinical access is authorized server-side and audited; frontend hiding is never treated as
  security.
- Sandbox payment remains the default; no live gateway activation without separate approval.

## 2. Target architecture

Keep the existing projects:

```
Domain           entities/enums/domain events
Application      commands, queries, validators, DTOs, access policies
Infrastructure   EF Core/PostgreSQL, Identity/JWT, files, notifications, audit
telemedicine.api controllers, SignalR, health checks, authorization
telemedicine-spa Vue 3 pages, stores, API client
```

Add feature modules inside the same layers:

- `Availability`
- `Consultations`
- `MedicalRecords`
- `Prescriptions`
- `Billing`
- `Notifications`
- `Audit`
- `Specialties`
- `Admin` extensions

New integrations:

- **Signalling:** existing SignalR, authenticated, used for WebRTC offer/answer/ICE exchange.
- **Media:** browser WebRTC peer-to-peer. STUN configurable; TURN optional for restrictive networks.
- **Notifications:** in-app first; email behind an interface/config for later.
- **Payments:** existing sandbox provider; invoice/payment records only.

## 3. Target data model and migration strategy

### 3.1 New/extended tables

| Area | Addition |
|---|---|
| Specialties | `Specialty` lookup; `Physician.SpecialtyId` (retain free-text for legacy) |
| Doctor profile | Qualifications, bio, consultation fee, default duration, time zone, verification status |
| Availability | `DoctorAvailability` (day, start/end local time, slot minutes, timezone); `DoctorAvailabilityException` (blocked date/range) |
| Appointments | `StartUtc`, `EndUtc`, `TimeZoneId`, `ConsultationType`, `Fee`, `PaymentId`, `ConsultationSessionId`, `RescheduleCount`, cancellation/decline reason; keep legacy `AppointmentDate`/`Notes` |
| Consultation | `ConsultationSession` (appointment, room token/hash, status, started/ended, participants); optional participant audit |
| EMR | `MedicalRecord` with chief complaint, history, symptoms, diagnosis, treatment plan, follow-up, visibility; `Allergy`, `PatientMedication`, `MedicalHistoryEntry` |
| Prescriptions | `Prescription` + `PrescriptionItem` linked to appointment/doctor/patient |
| Billing | `Invoice` + extended `Payment` (patient, appointment, invoice, provider reference, status, idempotency key, failure reason) |
| Notifications | `Notification` (recipient identity id, type, safe title/body, entity ref, read state) |
| Audit | `AuditLog` (actor, action, entity, patient ref, timestamp, sanitized metadata) |
| Identity | Account status, refresh-token table, optional email-verification state |

### 3.2 Migration strategy

1. Add nullable columns/tables in a new EF migration.
2. Backfill appointments from existing `AppointmentDate`; default duration 30 minutes; default time zone
   `Africa/Johannesburg` for local demo (configurable), storing all instants in UTC.
3. Leave existing payments/records legacy-tagged; do not delete or rewrite them.
4. Add partial unique index for active physician/start slots after checking/cleaning duplicates.
5. Add indexes for patient/physician/status/time queries and audit queries.
6. Seed reproducible synthetic demo data through an idempotent `DemoDataSeeder`.
7. Require explicit approval before applying migrations to any database with data beyond the local
   synthetic demo. Back up first.

## 4. Release plan

### Release 1 - Core platform, identity, profiles, availability and appointment integrity

**Goal:** A complete non-clinical booking journey for patient/doctor/admin.

Backend:

- Complete registration onboarding: create/link Patient or Physician profile (or guided required
  completion), validate duplicate identity/profile.
- Add specialties lookup and doctor profile fields (qualifications, fee, duration, time zone,
  verification status, bio).
- Add doctor availability CRUD and slot generation.
- Add patient medical-profile fields (emergency contact, history, allergies, current medications)
  as structured data, not just text.
- Extend appointment booking to use a UTC instant/end and selected physician; enforce availability,
  no past dates, slot duration and conflicts in a transaction; add unique/partial unique index.
- Add reschedule and decline commands with rules and ownership checks.
- Add doctor self-availability endpoints and admin doctor verification status (approval is an
  operational status, not a claim of credential verification).
- Add refresh-token rotation/revocation; shorten access tokens; add account status/lockout policy.

Frontend:

- Patient dashboard cards/tabs: upcoming, history, statuses, doctor, payments, records,
  prescriptions, notifications (initially placeholders for later releases).
- Doctor directory with name/specialty search and doctor profile pages.
- Availability-aware booking calendar and slot selector; reschedule UI.
- Doctor dashboard: today/upcoming/pending/completed; availability management page.
- Admin: users, doctor verification/status, appointments filter view.

DB:

- Specialty, availability, extended profile, appointment fields, refresh-token, indexes.

Tests:

- Authentication/profile onboarding; role restrictions.
- Availability slot generation and timezone conversion.
- Double-booking rejected under concurrent requests; reschedule rules; decline/accept.
- Existing 23 tests preserved.
- Playwright: patient/doctor/admin onboarding and booking/accept/reschedule/cancel.

Acceptance criteria:

- New patient can register, complete profile, find a doctor by specialty, see real slots, book,
  reschedule within rules, cancel; doctor can manage availability and accept/decline; admin can see
  users/verification status; no double booking.
- No existing workflow regressions.

Estimated effort: 10-14 person-days. Dependencies: none beyond local PostgreSQL.

### Release 2 - Teleconsultation and secure video room

**Goal:** Real two-participant consultation room with authenticated signalling and lifecycle.

Backend:

- `ConsultationSession` created for accepted/eligible appointments; secure room token/hash;
  only patient and assigned physician (or admin on exceptional support) may join.
- Authenticated SignalR hub with JWT access token; remove anonymous hub/REST publish.
- WebRTC signalling messages (offer/answer/ICE) routed only to the peer in the same session.
- Session lifecycle: scheduled -> waiting -> live -> ended; idempotent start/end; audit entries.
- Consultation notes/commands (start/complete) and follow-up recommendation.
- No recording by default; no server media storage.

Frontend:

- Consultation room route with local/remote video, mic/camera controls, connection status,
  leave/end, permission and error states; responsive layout.
- Patient join button only for eligible appointments; doctor start/join.
- Secure room access handling; no video UI unless authorized.

Configuration/docs:

- STUN default public server for local demo; TURN configuration placeholder/environment-only.
- HTTPS/localhost and browser permission instructions; two-browser test procedure.
- Explicit non-recording statement.

Tests:

- Authorized vs unauthorized room access; token expiry; session transitions.
- SignalR authentication; peer isolation (participant A cannot signal participant C).
- Playwright two-context media test (where headless browser/device permissions permit); documented
  manual fallback if media devices unavailable.
- Existing tests preserved.

Acceptance criteria:

- Two authenticated participants connect, see local/remote video, mute/unmute, toggle camera,
  leave, and the session ends with correct DB state. Unauthorized users are rejected.
- No recording or external media upload.

Estimated effort: 8-12 person-days. Dependencies: Release 1 appointment/authorization;
STUN; optional TURN; HTTPS.

**Approval gate:** approve WebRTC/STUN/TURN approach and local testing method before implementation.

### Release 3 - Medical records and prescriptions

**Goal:** Structured clinical documentation and prescription workflow.

Backend:

- Fix `HealthAnalysisReport`/EMR read path (EF includes, mappings, no NRE) or replace with a new
  structured `MedicalRecord` module while retaining legacy data.
- Add medical record create/update/read with author, consultation link, timestamps, versioning and
  patient-visible flag; no silent destructive overwrite (audit revision).
- Add allergies, medications, history and follow-up structures.
- Add prescription + items; validation; issue status; printable/downloadable document; patient ACL.
- Record-level policies: patient only sees visible records/prescriptions; physician only treating
  relationship (or explicit admin/clinical role); audit every read/write.
- Add doctor consultation completion that creates/links the clinical note and prescription.

Frontend:

- Patient: medical records, consultation history, prescriptions, download/print.
- Doctor: consultation note form, diagnosis/treatment/allergies/medications/follow-up, prescription
  builder, patient clinical timeline.
- Clear demo/synthetic labels; no AI-generated prescriptions.

Tests:

- Record creation/visibility; unrelated patient/physician 403; patient-visible vs internal.
- Prescription field validation; patient retrieval; authorisation.
- EMR read regression test (the current 500 scenario).
- Playwright: doctor completes consultation and issues synthetic prescription; patient views/downloads.

Acceptance criteria:

- A doctor can complete an appointment, write a structured record and issue a synthetic
  prescription; the correct patient can view/download it; unrelated users receive 403; every access
  is audited.

Estimated effort: 10-14 person-days. Dependencies: Release 2 session/consultation; approval of
clinical data model.

### Release 4 - Billing, notifications, administration and reporting

**Goal:** Coherent operational platform.

Backend:

- Invoice generation from consultation fee; payment statuses (pending/paid/failed/cancelled);
  idempotency key; appointment/invoice/patient FKs; receipts; server-side status lookup.
- Keep sandbox provider; no live card data.
- Persistent notification service, in-app endpoints, read/unread; safe payloads without clinical
  detail; optional email adapter disabled in tests.
- Admin: user search/status/roles; doctor verification queue; appointment filters; payment
  records; reporting endpoints; audit log viewer (sanitized).
- Replace hard-coded analytics with real queries or explicitly label sample widgets.
- Demo seeder: admin/patient/doctor, specialties, availability, appointments, completed
  consultation, record, prescription, invoice/payment, notifications.

Frontend:

- Patient payment history, invoice/receipt view, notification center.
- Doctor notifications and payment/consultation status.
- Admin users/doctor verification/appointments/payments/reports/audit pages.
- Remove or clearly label remaining sample analytics.

Tests:

- Payment success/failure/cancel/idempotency/appointment linkage; invoice receipt.
- Notification event creation/read; no sensitive detail in preview.
- Admin role restrictions; doctor verification workflow.
- Reporting correctness; Playwright end-to-end.

Acceptance criteria:

- A booking produces one invoice/payment, is idempotent, appears in patient/admin history, and
  produces a receipt; notifications appear in-app; admin can verify a doctor and inspect
  appointments/payments/reports; no clinical detail leaks into general reports.

Estimated effort: 10-14 person-days. Dependencies: Releases 1-3.

**Approval gate:** confirm payment stays sandbox and email delivery is disabled/optional.

### Release 5 - Production-oriented quality, security, documentation and packaging

**Goal:** Final certification for the approved academic/demo scope.

- Security review: secrets, auth/session, rate limiting, lockout, CORS, upload validation, audit
  redaction, dependency review.
- Full regression: existing 23 + all new unit/integration/Playwright tests.
- Responsive UI review at laptop/desktop/mobile; loading/empty/error states; remove dead controls.
- End-to-end demonstration: patient registration -> discovery -> booking -> sandbox payment ->
  doctor acceptance -> video consultation -> notes -> prescription -> patient follow-up.
- Documentation set under `docs/`; update README/DEMO_GUIDE; synthetic demo reset instructions.
- Rebuild and re-verify student handover packages; checksums.
- Record residual limitations and explicitly separate demo vs production-ready.

Estimated effort: 6-10 person-days.

## 5. Requirements-to-implementation matrix

Statuses: **F** fully implemented, **P** partial, **BO** backend only, **FO** frontend only,
**S** simulated, **M** missing, **B** blocked by external integration.

| ID | Requirement | Current | Target release | Implementation / evidence |
|---|---|---|---|---|
| IAM-01 | Registration | P | R1 | Identity account only; no domain profile onboarding |
| IAM-02 | Login | F | - | JWT login, roles |
| IAM-03 | Refresh/revocation | M (stub) | R1 | `AuthController.RefreshToken` returns 400 |
| IAM-04 | Password reset/email verification | M | R1/R5 | Not present |
| IAM-05 | Account lockout/rate limit | M | R5 | Not present |
| IAM-06 | Roles Patient/Doctor/Admin | F | - | Identity roles + `[Permit]` |
| IAM-07 | Resource-level authorization | P | R1-R3 | Patient/appointment checks exist; health/chat gaps |
| PAT-01 | Patient profile basics | F | - | Names, contact, gender, birth, address |
| PAT-02 | Emergency contact | M | R1 | Not in schema/UI |
| PAT-03 | Medical history/allergies/medications | M | R3 | Not in schema/UI |
| PAT-04 | Dashboard | P | R1/R4 | Appointments only |
| PAT-05 | Doctor discovery/search/profile | P | R1 | Only booking dropdown; no search/profile page |
| PAT-06 | Availability-aware booking | M | R1 | Static times; no slot check |
| PAT-07 | Reschedule/cancel/history | P | R1 | Cancel/history basic; reschedule API only |
| PAT-08 | Join consultation | M | R2 | No room |
| PAT-09 | View records/prescriptions | M | R3 | No patient-visible model/UI |
| PAT-10 | Payment history/receipt | P | R4 | Payment admin data only |
| DOC-01 | Doctor profile | P | R1 | License/specialization; no qualifications/fee/verification |
| DOC-02 | Availability/schedule | M | R1 | No entities/API/UI |
| DOC-03 | Appointment inbox/accept/decline | P | R1 | Accept only; no decline reason |
| DOC-04 | Start/complete consultation | M | R2 | Not present |
| DOC-05 | Clinical notes/follow-up | M | R3 | No structured module |
| APT-01 | Book appointment | F | - | Payment-token scheduling works |
| APT-02 | Double-booking prevention | M | R1 | No slot/unique constraint |
| APT-03 | Duration/time zones | M | R1 | Only date; no duration/end |
| APT-04 | Status lifecycle | P | R1/R2 | Active/NotAttended/Attended/Cancelled; limited transitions |
| CON-01 | Video consultation | M | R2 | No WebRTC; dead button |
| CON-02 | Room access control | M | R2 | No session/room |
| CON-03 | Two-participant media | M | R2 | Not tested/exists |
| CHAT-01 | Authenticated chat | P/S | R2 | UI static; hub/REST unauthenticated |
| CHAT-02 | Persistent messaging | M | R2/R4 | Messages table unused |
| EMR-01 | Structured record | M | R3 | HealthAnalysisReport only patient/comments |
| EMR-02 | Record read | M (500) | R3 | NullReferenceException confirmed |
| EMR-03 | Clinical access control | P | R3 | Broad physician access; no patient visibility/audit |
| EMR-04 | Versioning/audit | M | R3/R4 | Created/Modified fields only |
| RX-01 | Prescription creation | M | R3 | No entity/API/UI |
| RX-02 | Patient prescription view/download | M | R3 | Not present |
| BIL-01 | Consultation pricing | F | - | Service cost |
| BIL-02 | Invoice | M | R4 | No invoice |
| BIL-03 | Payment lifecycle/failure/cancel | P | R4 | Only paid sandbox |
| BIL-04 | Idempotency | M | R4 | No idempotency key |
| BIL-05 | Appointment/payment association | M | R4 | No FK |
| BIL-06 | Receipt | M | R4 | Not present |
| NOT-01 | In-app notifications | FO/S | R4 | Toast only |
| NOT-02 | Email/SMS | M/B | R4/R5 | No provider; disabled tests |
| ADM-01 | Hospital CRUD | F | - | Verified |
| ADM-02 | User management | M | R4 | Analytics user list read-only |
| ADM-03 | Doctor verification | M | R1/R4 | No status/approval |
| ADM-04 | Appointment/payment administration | M | R4 | No management pages |
| ADM-05 | Reporting | P | R4 | Some hard-coded endpoints |
| SEC-01 | Secure password storage | F | - | ASP.NET Identity |
| SEC-02 | JWT/session security | P | R1/R5 | Long-lived token, no refresh/revocation |
| SEC-03 | Clinical audit log | M | R3/R4 | None |
| SEC-04 | Privacy/consent/retention | M | R5 | No policy/implementation |
| SEC-05 | Chat/hub authorization | M | R2 | Unauthenticated 200 confirmed |
| TST-01 | Existing tests | F | - | 23/23 pass (templates only) |
| TST-02 | Healthcare tests | M | R1-R5 | Add auth/booking/slots/records/Rx/billing/video |
| DEMO-01 | Reproducible demo seed | P/M | R4 | Only admin/todo seeded |
| DOC-01 | Architecture/API/security docs | M | R5 | Only README/audit docs exist |

## 6. Testing and acceptance strategy

### Unit tests

- Availability slot generation, time zones, duration, overlap.
- Appointment transition rules and ownership.
- Payment idempotency/amount/status; invoice calculations.
- EMR visibility and prescription validation.
- Notification safe payloads; audit metadata redaction.

### Integration tests

- Concurrent booking (two requests, one slot).
- Reschedule/cancel/accept/decline.
- Consultation session access/start/end.
- Medical record/prescription ACLs and audit.
- Payment/appointment/invoice consistency.
- Admin verification and role restrictions.
- Chat/SignalR authorization.

### Playwright

- Full journey: patient registration → profile → doctor search → slot booking → sandbox payment →
  doctor acceptance → consultation room → notes → prescription → patient follow-up.
- Negative cases: unauthorized patient/physician, double booking, invalid payment, expired slot,
  missing permissions.
- Two browser contexts for WebRTC where supported; otherwise a documented manual two-device test.
- Desktop/laptop/mobile viewport checks.

### Regression gate

- All 23 original tests continue to pass.
- All new tests pass; no skipped tests reported as passed.
- Builds: API + SPA; migrations on a clean database and on a preserved synthetic DB.
- Browser console/network checks for each major journey.

## 7. Demo data and local development plan

Create an idempotent `DemoDataSeeder` that seeds:

- Roles: Administrator, Physician, Patient.
- Admin: `administrator@localhost` / documented synthetic password.
- Doctor: `doctor@demo.local` / documented synthetic password, approved demo status, specialty,
  availability Monday-Friday, fee/duration, synthetic registration details.
- Patient: `patient@demo.local` / documented synthetic password, profile, allergies/medications/
  history (clearly synthetic).
- Hospital, specialties, services.
- One completed consultation with record, prescription, invoice/payment, notification.
- One upcoming accepted appointment and one pending request for demo.
- Clearly labelled `DEMO / SYNTHETIC` in UI/docs.

Reset script: a safe local database reset command that deletes only the demo database data and
re-runs migrations/seeder. Never point it at a database with real data without confirmation.

## 8. Documentation deliverables

Create after implementation:

```
docs/
├── architecture/SYSTEM_OVERVIEW.md
├── architecture/DATABASE_DESIGN.md
├── architecture/API_REFERENCE.md
├── architecture/SECURITY_ARCHITECTURE.md
├── guides/INSTALLATION.md
├── guides/PATIENT_GUIDE.md
├── guides/DOCTOR_GUIDE.md
├── guides/ADMIN_GUIDE.md
├── guides/DEMONSTRATION_GUIDE.md
└── audits/TEST_RESULTS.md, FINAL_READINESS_REPORT.md
```

Update README/DEMO_GUIDE to distinguish implemented, simulated and production-blocked features.

## 9. Risks, dependencies and approval gates

| Gate | What must be approved before work |
|---|---|
| G1 | Audit/plan approval and scope for each release |
| G2 | Data-model/migration plan, especially appointment/payment backfill and unique index |
| G3 | WebRTC/SignalR signalling design, STUN/TURN choice, HTTPS/local two-participant test method |
| G4 | Confirmation that payments remain sandbox-only and no email/SMS is sent in tests |
| G5 | Any database change against a database containing non-synthetic data |
| G6 | Final release certification after all tests and handover package verification |

Key risks:

- Migration of existing appointments without a duration/time zone; backfill defaults must be
  documented and reversible.
- WebRTC may require TURN for networks beyond localhost; demo must document the boundary.
- Adding clinical data increases security/privacy obligations; audit and access control are
  non-negotiable.
- Scope is large; releases must not overlap uncontrolled changes.
- Existing 23 tests do not protect healthcare behavior; new tests must be written before calling a
  feature complete.

## 10. Recommended sequence and approvals

1. Approve this audit and plan.
2. Approve Release 1 data model/migration approach; implement and verify.
3. Approve WebRTC signalling/TURN approach; implement Release 2 and test two participants.
4. Approve clinical record/prescription model; implement Release 3.
5. Approve sandbox-only billing/notification/admin scope; implement Release 4.
6. Execute Release 5 security/QA/docs/packaging and final certification.

No implementation should begin until the audit and this plan are approved. At that point, start
with Release 1 only, with a clean build, 23-test regression run and new Release 1 tests before
moving on.
