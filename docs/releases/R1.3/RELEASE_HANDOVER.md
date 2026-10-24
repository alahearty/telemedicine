# Release 1.3 Handover

## Status

Complete. Vite, Tailwind modernization, dependency remediation, deterministic Playwright lifecycle, and authenticated SignalR security integration coverage meet the release definition of done.

## Reproduce

```powershell
cd telemedicine-spa
npm ci
npm run build
cd ..
dotnet build telemedicine-webapi/telemedicine-webapi.sln --configuration Release --no-restore
dotnet test telemedicine-webapi/telemedicine-webapi.sln --configuration Release --no-restore
cd telemedicine-spa
npm run test:e2e
npm audit --json
```

PostgreSQL must be reachable with the integration-test configuration. Browser binaries for Playwright Chromium must already be installed. The E2E command starts and cleans up its own API and Vite processes and uses synthetic local data.

## Operational notes

- Development frontend: `npm run dev`; preview: `npm run preview`.
- Client API/hub URLs default to Vite same-origin proxies and may be changed with public `VITE_*` settings. Never place secrets in them.
- Release 1.2 backend security controls remain present: explicit CORS origins, trusted proxy opt-in, JWT validation, live/ready health checks, sanitized errors, development/config-controlled Swagger, rate limits, safe seeding and authorized SignalR.
- No database migration and no WebRTC feature were added.

## Remaining non-blocking risks

- Live JavaScript SignalR chat is not yet enabled in the Vue chat page; server protocol/security is covered by real-client integration tests.
- Pixel-level responsive parity was not compared against stored visual baselines.
- The test environment emits development-only ephemeral data-protection and unlicensed-development-package notices; production licensing/key persistence remains an operations concern.
- Dependency audits are time-sensitive and should run in CI on every lockfile change.

## Rollback

Rollback is source-based: restore the prior frontend manifest/lockfile, Vue CLI config/template, environment references, PostCSS/Tailwind setup and Playwright configuration together. No database rollback is necessary because this release adds no migration.
