# Release 1 Implementation Changelog

Date: 2026-10-23
Scope: approved Release 1 only (identity, profiles, availability, scheduling,
appointment lifecycle, authorization, chat/EMR security fixes, frontend workflows,
tests and documentation).

## 1. Domain and database

- Added `User.IdentityUserId` (nullable, unique) to link application profiles to
  ASP.NET Identity accounts.
- Added physician `Qualifications` and `Bio`.
- Added patient emergency contact fields.
- Added appointment `StatusReason`.
- Added `PhysicianAvailability` recurrence entity and configuration.
- Required UTC appointment start/end intervals and added `StatusReason` length limit.
- Added check constraints for availability time ranges, slot duration and day values,
  and appointment interval ordering.
- Added appointment lookup indexes and unique profile email/licence indexes.
- Added PostgreSQL `btree_gist` exclusion constraint
  `EX_Appointments_Physician_Time` for non-terminal overlapping physician bookings.
- Added `Release1Scheduling` additive migration, legacy backfills and a
  preserve-and-cancel repair for one duplicate legacy appointment.
- Corrected the EF design-time model factory so the runtime and design-time Identity
  model do not diverge, eliminating the `PendingModelChangesWarning` startup failure.

## 2. Identity and onboarding

- Extended registration to accept patient/physician profile data and to create the
  linked Patient or Physician profile in the same relational transaction as the
  Identity account.
- Added rollback/compensation on failure and duplicate detection by identity, email
  and licence.
- Added safe duplicate handling to profile-completion commands.
- Added linked-account lookups to current-patient/current-physician and appointment
  access checks with email fallback for legacy rows.
- Added `Gender.Other` and JSON string enum handling.
- Fixed signup to send the account type and profile fields expected by the API.
- Added accepted profile updates for patient address/gender/birth/emergency contacts
  and physician speciality/qualifications/bio/address.
- Demo accounts are seeded idempotently; demo passwords are documented and reset only
  in demo mode. The administrator is seeded only in demo/admin-seed mode.

## 3. Availability and scheduling

- Added `AvailabilityController`:
  - replace the authenticated physician's weekly schedule;
  - read a physician's schedule;
  - generate available slots for a bounded UTC range.
- Added `SlotCalculator` with shared generation and server-side resolution rules.
- Added input validation for overlapping rules, invalid intervals and slot durations.
- Booking now requires a future, aligned, in-availability, non-overlapping slot.
- Booking consumes its sandbox payment token only after a successful commit.
- Added PostgreSQL-backed overlap enforcement and controlled handling of the losing
  concurrent request.
- Added rescheduling with the same slot/availability/overlap checks.
- Added cancellation reasons and physician decline reasons.
- Restricted accept/decline to pending appointments and validated physician ownership.
- Added the `Pending` and `Declined` lifecycle states and client displays.

## 4. Authorization and security defects

- Fixed the custom permission handler to use the policy requirement and effective
  endpoint permit metadata instead of fragile endpoint display-name parsing.
- Verified role restrictions on profiles, availability, booking, payment and
  accept/decline operations.
- Enforced patient ownership, physician assignment and administrator access on
  appointments and patient profiles.
- Secured chat REST endpoints and the SignalR hub with JWT authentication.
- Removed arbitrary connection-id targeting and public broadcast; recipients are
  authorized by treating relationship.
- Forbade client-managed group membership.
- Bound SignalR user identity to the server-side immutable identity id.
- Added rate limiting, CORS allowlisting, security headers, sanitized health output
  and no-store responses for sensitive endpoints.
- Fixed `GET /api/HealthAnalysisReport/{patientId}` by loading comments/physicians and
  projecting safely; added treating-relationship checks for physician EMR access.

## 5. Frontend

- Completed the signup flow with patient and physician onboarding data.
- Rebuilt booking around real services, physicians and server-generated slots.
- Added doctor directory, doctor profile with weekly availability and slot selection.
- Added patient appointment list and appointment detail/reschedule/cancel actions.
- Added physician appointment detail, accept/decline with reason, and availability
  management.
- Connected patient and physician dashboards to live profile/appointment APIs and
  removed dead feedback/static booking behaviour from the patient flow.
- Added reusable status helpers and slot picker.
- Kept Vue 3/Vuex/Router/component structure; the local toolchain in the final working
  tree is Vite/Tailwind with the existing PrimeVue/ApexCharts components.
- Added browser-runtime assertions to the Playwright flows so unexpected console
  errors, page errors or HTTP 500 responses fail the workflow.

## 6. Testing

- Preserved and re-ran the original 23 tests.
- Added `R1ReleaseWorkflowsTests` covering:
  - linked patient/physician registration and duplicate rejection;
  - role authorization;
  - availability persistence, slot generation and invalid schedule rejection;
  - valid/invalid booking, single-use payment tokens;
  - concurrent API booking and direct PostgreSQL exclusion concurrency;
  - rescheduling and conflict rejection;
  - owner-only cancellation;
  - physician accept/decline and ownership;
  - patient/physician record-level access;
  - chat authentication/treating relationship;
  - EMR read regression and relationship denial.
- Added middleware/SignalR security tests and a direct PostgreSQL concurrency test.
- Added Playwright patient and physician workflows.
- All R1-scoped tests were executed against local PostgreSQL; no EF Core InMemory
  test is used as evidence of database concurrency safety.

## 7. Demonstration data and documentation

- Idempotent demo seed: admin, two synthetic physicians, synthetic patient, hospital,
  services, weekday availability and sample pending/active appointments.
- Added Release 1 architecture, migration, API, test, security and end-to-end
  evidence documents.
- Documented clean/copy migration verification, Playwright evidence and reproducible
  demo commands.

## 8. Out-of-scope concurrent changes

The working tree also contains concurrent, unapproved Release 2 consultation/signalling
work, an R2 database migration and additional release notes. Those changes are not part
of this Release 1 changelog or certification. See the completion report for the
governance note.
