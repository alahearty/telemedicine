# Release 1.3 Test Results

Executed on 2026-10-23.

| Check | Result |
|---|---|
| `npm ci` | Pass; 143 installed, 144 audited, 0 vulnerabilities |
| `npm run build` | Pass; 413 modules, 26.60 s |
| Backend Release build (`--no-restore`) | Pass; 0 warnings, 0 errors, 9.08 s |
| Domain unit tests | 5 passed |
| Application unit tests | 8 passed |
| PostgreSQL integration tests | 18 passed |
| Total backend tests | 31 passed, 0 failed |
| WebUI acceptance assembly | No discoverable tests (existing condition) |
| SignalR security subset | 3 passed, included above |
| Playwright Chromium | 7 passed in 35.4 s; exit 0 |
| npm audit | 0 advisories; exit 0 |
| NuGet vulnerable audit | 0 vulnerable packages; exit 0 |
| Git whitespace check | Pass; line-ending notices only |

The PostgreSQL suite includes the existing concurrent overlapping-booking protection. The browser suite covers registration/authentication, patient and physician dashboards, doctor discovery/availability, booking/payment/reschedule/cancel, appointment acceptance/decline, persistence/ownership checks, and unauthenticated chat/EMR rejection.

One earlier build attempt overlapped a test process and encountered a test-DLL file lock; the required sequential Release build was rerun cleanly and is the result reported above.
