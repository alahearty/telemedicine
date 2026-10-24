# Release 1.3 Baseline Audit

Audit date: 2026-10-23

## Verified starting point

- Frontend: Vue 3, Vue Router 4, Vuex 4, Vue CLI 5/webpack, Tailwind CSS 2.2, PostCSS 8 and Autoprefixer 10.
- Production command: `npm run build`; development command: `npm run serve`.
- Vue CLI supplied the `@` source alias, HTML template, public assets, development server and `/api` proxy.
- Client configuration used `VUE_APP_API_BASE_URL`, `VUE_APP_SIGNALR_HUB_URL` and webpack-specific `require.context`/asset resolution.
- Release 1.2 npm baseline: 36 advisories (0 critical, 16 high, 20 moderate); production-only: 18 (0 critical, 6 high, 12 moderate).
- Release 1.2 backend baseline: Release build passed; 28 tests passed, including 15 PostgreSQL integration tests and the overlapping-booking concurrency test.
- Release 1.2 Playwright baseline: five tests passed, but the managed web-server teardown hung.
- SignalR used the real `/chat-hub` endpoint with bearer authentication and `AppUserIdProvider`; the Vue chat view contained only a placeholder client implementation.

## Risk map

The remaining npm findings originated in the obsolete Vue CLI/webpack build graph and the Tailwind 2/3 scanner graph. The installed Tailwind 3 line still retained six high-severity scanner advisories with no safe patched v3 resolution. Runtime Vue, routing, state management and API behavior did not require replacement.

The repository already contained Release 1.2 startup validation, explicit CORS origins, forwarded-header controls, JWT validation, health endpoints, conditional Swagger, partitioned rate limits, safe seeding and authorized SignalR registration. These controls were treated as regression-sensitive.

No database schema change or WebRTC work was required.
