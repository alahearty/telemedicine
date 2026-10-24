# Telemedicine Test Results — Baseline Audit

Date: 2026-10-23
Status: historical baseline before Phase 0-16 implementation.

Current Release 1 results are in `docs/releases/R1/R1_TEST_RESULTS.md`
(44/44 R1-scope backend tests and 7/7 Playwright workflows pass).

## Automated tests

| Suite | Executed | Passed | Failed | Skipped | Exit |
|---|---|---|---|---|---|
| Domain.UnitTests | 5 | 5 | 0 | 0 | 0 |
| Application.UnitTests | 8 | 8 | 0 | 0 | 0 |
| Application.IntegrationTests | 10 | 10 | 0 | 0 | 0 |
| **Total** | **23** | **23** | **0** | **0** | **0** |

Commands:

```powershell
dotnet test telemedicine-webapi\tests\Domain.UnitTests\Domain.UnitTests.csproj
dotnet test telemedicine-webapi\tests\Application.UnitTests\Application.UnitTests.csproj
dotnet test telemedicine-webapi\tests\Application.IntegrationTests\Application.IntegrationTests.csproj
```

Important coverage limitation: the 23 tests cover Colour/value objects, validation/mapping/logger
infrastructure, and TodoList/TodoItem template CRUD. They do **not** test authentication beyond the
application service, appointments, availability, payments, EMR, prescriptions, chat, video, or
role-specific UI journeys.

The legacy `WebUI.AcceptanceTests` SpecFlow suite targets `/authentication/login`, which is not a
route in the current Vue SPA; it is not part of the applicable baseline.

## Build results

```powershell
dotnet build telemedicine-webapi\telemedicine-webapi.sln
npm run build --prefix telemedicine-spa
```

- API solution: PASS, 0 errors, 62 warnings.
- SPA: PASS, production build generated.

## Diagnostic API probes

| Probe | Result |
|---|---|
| Login as seeded administrator | 200 |
| Create synthetic health report as admin | 201 |
| Read synthetic health report by patient id | **500 NullReferenceException** |
| Unauthenticated `POST /chat-hub/publish-to-all` | **200** |

The synthetic health reports and comments created for the probe were deleted from the local
database afterwards.

## Browser/manual verification from prior phases

The previous handover validation established working login, booking, sandbox payment, physician
acceptance, patient cancellation and role dashboards. Those results are preserved, but they are not
a substitute for the missing healthcare tests required by the implementation plan.

## Result

Baseline is stable and buildable. The platform must not be declared complete until the missing
healthcare workflows and their tests are implemented and verified.
