# Release 1.2 Baseline Audit

Date: 2026-10-23

## Observed baseline

- Repository was already a dirty working tree containing Release 1/1.1 work; unrelated changes were preserved.
- Initial Vue production build passed with two size warnings (1,020 KiB entry point; 757 KiB vendor chunk).
- The first backend command used an incorrect solution name (`telemedicine.sln`) and failed before execution. The discovered solution is `telemedicine-webapi.sln`; all final commands use it.
- Previously verified Release 1.1 baseline was 5 domain, 8 application, and 11 PostgreSQL integration tests (24 total). The final Release 1.2 suite contains four additional middleware tests.

## Verified findings before remediation

| Finding | Evidence | Severity |
|---|---|---|
| Unrestricted development CORS | `DevelopmentPolicy` used `AllowAnyOrigin()` | High |
| Forwarded-header spoofing boundary | known networks/proxies were cleared | High |
| False-positive readiness | `/health/ready` selected tag `ready`, but no check had that tag | High |
| Diagnostic disclosure | `/health` serialized exception messages | High |
| Shared-cache exposure risk | response caching was global across API traffic | High |
| Public production Swagger | Swagger middleware ran in every environment | Medium |
| No abuse throttling | no rate limiter was registered | High |
| Unsafe startup mutation | every start migrated and seeded; a fixed administrator was created | Critical |
| Existing-user mutation | demo seeding reset passwords | Critical |
| Secrets in application settings | PostgreSQL password and JWT key were committed in runtime settings | Critical |
| Frontend dependency exposure | initial npm audit: 109 (7 critical, 38 high, 59 moderate, 5 low) | Critical |

Authentication already used JWT as the default scheme, middleware ordering was authentication before authorization, SignalR was `[Authorize]`, immutable identity IDs were used by `AppUserIdProvider`, and Release 1 chat methods already rejected arbitrary groups/connection IDs. These working controls were retained.

## Implementation plan executed

Harden middleware/configuration, add dependency-aware health checks, sanitize errors, apply endpoint-specific throttling and no-store rules, make migrations/seeding opt-in, remove runtime secrets/default admin behavior, add integration tests, update safely compatible dependencies, then run builds/tests/E2E and audits.
