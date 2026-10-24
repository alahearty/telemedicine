# Release 1.1 acceptance report

## Status

**Conditionally ready, not fully accepted.** Builds, the original 23 backend tests, one new PostgreSQL concurrency test, five current Vue Playwright checks, and a clean NuGet advisory scan pass. No critical runtime regression was observed.

## Completed

- Replaced obsolete Razor Playwright coverage with the current Vue/JWT application.
- Verified patient/physician login, protected routes, logout, directory/dashboard data, record restriction, and anonymous chat/EMR rejection.
- Verified the migration and database exclusion constraint against PostgreSQL concurrency.
- Resolved all currently reported critical/high NuGet advisories without suppression.
- Reduced initial application JavaScript from 193.47 KiB to 22.70 KiB through route splitting.

## Outstanding acceptance risks

- Playwright still needs a repeatable isolated workflow that creates a fresh paid appointment, reschedules/cancels it, and accepts/declines it through the UI while verifying both users.
- Explicit integration coverage is still needed for failed reschedule rollback, onboarding compensation across Identity/domain persistence, authenticated conversation membership/impersonation, and all EMR valid/not-found/authorised cases.

Release 2 implementation must not start until these remaining acceptance assertions pass or are explicitly waived.
