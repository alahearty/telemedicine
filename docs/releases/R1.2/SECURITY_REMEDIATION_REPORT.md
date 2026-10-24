# Release 1.2 Security Remediation Report

## Implemented

- Replaced wildcard development CORS with one explicit `Cors:AllowedOrigins` policy; credentials are not enabled because the client uses bearer tokens.
- Forwarded headers are disabled by default and enabled only with configured known proxy IPs and a bounded forward limit.
- Kept JWT as the sole authentication scheme and added fail-fast validation requiring a 32-byte signing key from a secure provider.
- Preserved permission policies, ownership checks, authenticated SignalR, server-derived sender identity, and chat membership checks.
- Added fixed-window limits to authentication, appointment mutation, and payment initiation endpoints. Rejections use 429 Problem Details and `Retry-After` when available.
- Added `nosniff`, frame denial, and no-referrer headers.
- Removed global response caching and apply `no-store, no-cache` to authentication, appointment, patient, payment, chat, and all authenticated responses, including 401 short-circuits.
- Swagger is development-only unless explicitly enabled.
- Removed committed runtime PostgreSQL/JWT secrets, fixed administrator creation, and existing demo-password resets.
- Production exception responses are sanitized and include only a trace identifier; details remain server-side.

## Dependency evidence

- NuGet audit (`--vulnerable --include-transitive --no-restore --format json`): all eight projects listed, no vulnerability entries.
- Initial npm audit: 109 total / 7 critical / 38 high.
- Updated Axios 0.27 to 1.20, Vue to 3.5.44, Vue CLI packages to 5.0.9, CoreJS and PostCSS, then applied non-breaking audit fixes.
- Final full npm audit: 36 total / 0 critical / 16 high / 20 moderate. The production-dependency view is 18 total / 0 critical / 6 high / 12 moderate. Remaining findings are transitive in the legacy Vue CLI 5, webpack-dev-server, Tailwind 3, scrollbar/CSS build toolchain. npm proposes breaking or incorrect major changes (including Vue CLI 3 or Tailwind 4); these were not forced.

## Residual risks

- The Vue CLI stack is end-of-life in practice and retains build/development-only advisories. Migrate to Vite in a separately tested release.
- Rate-limit counters are process-local and do not coordinate across replicas.
- Refresh tokens remain explicitly unimplemented by the existing design.
- Demo and integration credentials are synthetic test fixtures; production secrets must come from environment variables, user-secrets, or a secret manager.
