# Release 1 Security Verification

Date: 2026-10-23
Scope: approved Release 1 identity, scheduling, appointment, chat and EMR surfaces.

## 1. Verification summary

| Control | Implementation | Evidence |
|---|---|---|
| Password storage | ASP.NET Core Identity hashing | Login/register integration tests |
| Identity/profile linkage | Unique `IdentityUserId`; email fallback for legacy rows | Registration tests; migration copy |
| Duplicate prevention | Pre-checks + unique database indexes | Duplicate registration/profile/licence tests |
| Transactional onboarding | Relational transaction shared by Identity and profile writes | Registration tests; rollback path reviewed |
| Role authorization | `[Permit]` policies with endpoint metadata resolution | 401/403 role tests; middleware tests |
| Record-level authorization | Patient owner, assigned physician, administrator checks | Ownership/physician access tests |
| Booking concurrency | Application pre-check + PostgreSQL exclusion constraint | API race test + direct concurrent insert test |
| Chat authentication | JWT required on REST and hub | SignalR tests; anonymous REST test |
| Chat authorization | Treating relationship; server identity; no client-selected groups | SignalR and R1 chat tests |
| EMR authorization | Administrator or treating physician | EMR regression test |
| Rate limiting | Fixed-window authentication/payment/scheduling policies | Middleware 429 test |
| CORS | Explicit allowlist; no wildcard in normal configuration | Middleware CORS test |
| Security headers | nosniff, frame deny, no-referrer, no-store on sensitive routes | Middleware test |
| Health output | Sanitized check names/status/duration | Middleware test |
| Demo credential safety | Admin seeded only in demo/admin-seed mode; synthetic passwords only | Configuration review |

## 2. Onboarding and duplicate protection

- Registration requires the profile email to match the authenticated account.
- Patient duplicates are rejected by identity, email and database unique indexes.
- Physician duplicates are rejected by identity, email or licence.
- The account + profile creation is transactional. A failure rolls back both sides;
  duplicate constraint violations return a controlled application error.
- Legacy profiles are linked by email in the migration; new profiles always receive
  the immutable Identity id.

Verified by:
`Registration_creates_linked_patient_and_physician_profiles_and_rejects_duplicates`.

## 3. Authentication and authorization

- JWTs contain the Identity `sub`/`uid`, `email` and `Role` claims.
- SignalR user targeting uses `IUserIdProvider` reading `uid` or the name identifier,
  never a client-supplied connection id.
- `PermissionAuthorizationHandler` now uses the effective endpoint `Permit` from
  endpoint metadata instead of parsing display names. The most specific endpoint-level
  permission is authoritative, while the policy requirement remains the fallback.
- Role restrictions verified:
  - patient cannot read physician `/me`, respond to appointments or replace
    physician availability;
  - physician cannot read patient `/me` or create a booking;
  - anonymous appointment and chat calls return `401`;
  - cross-patient and cross-physician appointment reads return `403`;
  - cross-patient cancellation/reschedule returns `403`.
- Record-level checks are repeated in application handlers, not only in controllers.

Verified by:
`Role_authorization_is_enforced_on_profile_and_scheduling_operations`,
`Patient_and_physician_record_level_access_is_enforced`,
`Cancellation_releases_the_slot_and_is_owner_only`,
`Physician_can_accept_and_decline_only_their_own_pending_requests`,
`Physician_availability_persists_and_generates_real_slots`,
`Invalid_bookings_are_rejected`.

## 4. Appointment integrity

- Server-side slot validation checks future time, availability, slot alignment,
  half-open interval overlap and physician assignment.
- The client cannot choose an arbitrary status; status transitions have dedicated
  commands.
- Decline requires a reason; cancellation records an optional reason.
- Payment tokens are single-use and consumed only after commit.
- PostgreSQL exclusion constraint `EX_Appointments_Physician_Time` is the final
  concurrency authority. Statuses Active, NotAttended and Pending reserve the
  half-open `[start, end)` interval.
- The concurrent API test asserts exactly one `201` and one controlled `400`.
- The direct EF Core test asserts exactly one commit when two overlapping rows race.

Verified by:
`Concurrent_booking_attempts_allow_exactly_one_appointment`,
`Database_exclusion_constraint_rejects_overlapping_rows_under_concurrency`,
`ExclusionConstraint_AllowsExactlyOneConcurrentOverlappingBooking`.

## 5. Chat and SignalR

- REST and hub surfaces require authentication.
- Public broadcast and client-managed groups return `403`/`HubException`.
- Recipients must be the caller's patient or physician with an accepted appointment
  relationship; unrelated recipients are denied.
- Hub `SendMessageToUser` and REST `send-to-user` use server-resolved recipient
  identity, not a connection id.
- File uploads are limited to 2 MB and images/PDF/plain text.
- SignalR JWT is accepted from the query string only for the `/chat-hub` path.
- Persistent chat history/conversation membership remains Release 2 scope.

Verified by:
`Chat_requires_authentication_and_a_treating_relationship`,
`Missing_and_invalid_tokens_are_rejected`,
`Authenticated_participants_connect_exchange_and_reconnect_with_server_identity`,
`Arbitrary_groups_recipients_and_broadcasts_are_rejected`,
Playwright `chat and EMR endpoints reject unauthorised access...`.

## 6. EMR regression and authorization

- The prior `GET /api/HealthAnalysisReport/{id}` NullReferenceException is fixed by:
  - explicit `Include` of Patient, Comments and Comment.Physician;
  - null-safe projection into DTOs;
  - returning `200` with an empty array when no reports exist.
- Physician reads require an appointment relationship with the patient; unrelated
  physicians receive `403`.
- Create/add-comment use the authenticated physician identity.
- Full clinical-record ACL/audit/versioning remains Release 3.

Verified by:
`Emr_read_returns_reports_and_comments_without_null_reference`.

## 7. Platform controls

- Fixed-window rate limiting: authentication 10/min, payment 10/min, scheduling
  30/min by default; 429 returns `Retry-After` and problem+json.
- CORS allowlist is explicit; arbitrary origins are not reflected.
- Security headers: `X-Content-Type-Options: nosniff`,
  `X-Frame-Options: DENY`, `Referrer-Policy: no-referrer`.
- Sensitive routes receive `Cache-Control: no-store`.
- Health endpoints return sanitized status/check names, not exception or connection
  details.
- Secrets: development JWT/database credentials remain in local configuration and
  must be replaced in deployment. No real patient data is present.

Verified by:
`MiddlewareSecurityTests` (4 passing).

## 8. Residual security limitations

- `/api/auth/refresh` is still a stub; no refresh-token rotation/revocation.
- No user-facing password reset, email verification or account lockout beyond rate
  limiting.
- No clinical audit log, consent record, retention policy or encryption-at-rest
  implementation.
- Chat persistence and full conversation membership are not implemented; Release 1
  secures the existing surfaces only.
- Dependency advisories remain (AutoMapper 12.0.1, cryptography packages); planned
  upgrade work.
- Development secrets must not be reused in production.
- The final working tree contains unapproved R2 signalling/consultation code and an R2
  migration. It must not be treated as Release 1 security approval.

## 9. Release 2 security gate

No WebRTC/TURN implementation is approved here. The recommended architecture and
security requirements are documented in the Release 2 proposal and summarised in
`R1_ARCHITECTURE.md`. Separate approval, threat modelling, TURN deployment review,
consent UX, retention policy and two-participant browser testing are required before
Release 2 implementation is accepted.
