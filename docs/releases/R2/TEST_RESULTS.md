# Release 2 test results

Executed on 2026-10-23 against PostgreSQL database `telemedicine_test`.

- .NET solution build, Release: passed; zero errors (three pre-existing warnings in the final API build).
- .NET tests: 52 passed, 0 failed — Domain 9, Application 8, Integration 35. WebUI acceptance project contains no discoverable tests.
- Consultation coverage includes lifecycle rules, idempotent end, concurrent idempotent session creation, participant/non-participant authorization, ICE authorization, terminal signaling rejection, and authenticated SignalR exchange.
- Existing PostgreSQL appointment concurrency coverage passed.
- Vue/Vitest consultation tests: 2 passed — permission failure and real track control/cleanup behavior.
- Release 2 Playwright: passed with two isolated authenticated browser contexts, synthetic camera/microphone tracks, real `RTCPeerConnection`, real SignalR signaling, bidirectional remote streams (`readyState === 4`), controls, and remote end notification.
- Full Playwright regression: 8 passed, 0 failed in 1.7 minutes. The Release 2 call scenario completed in 22.4 seconds.
- Vite production build: passed, 442 modules transformed in 24.21 seconds.
- `npm audit --audit-level=low`: passed with 0 vulnerabilities.

The browser test does not simulate TURN or independent external networks. See `STUN_TURN_DEPLOYMENT.md`.
