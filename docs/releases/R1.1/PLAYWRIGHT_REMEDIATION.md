# Playwright remediation

The obsolete SpecFlow suite targeting `/authentication/login` was removed. The active suite is `telemedicine-spa/tests/e2e/release1.spec.js`, driven by `playwright.config.js` and `npm run test:e2e`.

The harness starts/reuses the real ASP.NET API on port 5231 and Vue dev server on port 8080, enables idempotent synthetic data, uses the login form and JWT middleware, and retains screenshots, video, traces, console failures, and HTTP 5xx evidence on failure.

Executed 2026-10-23: 5 passed, 0 failed, 0 skipped (16.1 seconds). Covered invalid login; anonymous route redirect; logout protection; patient login, doctor directory, dashboard and API-persisted appointments; physician login, assigned appointments and cross-physician rejection; anonymous chat/SignalR and EMR rejection.

The current suite does not yet automate creating a new paid booking or changing a physician response through the UI. Those remain acceptance gaps rather than being reported as passed.
