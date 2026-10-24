# SignalR Security Test Report

## Implementation

The integration project now references the .NET SignalR client and creates connections against the actual TestServer `/chat-hub` endpoint using long polling and real JWT generation. Tests use the existing PostgreSQL integration database and real application authorization/membership services.

## Covered behavior

- Missing, malformed and expired bearer tokens cannot establish an authorized connection.
- Authenticated patient and physician identities connect under product rules.
- Arbitrary `JoinGroup`, arbitrary group send, and unauthorized recipient operations raise `HubException`.
- Private message membership is backed by an active patient/physician appointment.
- The received sender identifier is derived from the authenticated principal, not supplied by the caller.
- Authorized patient-to-physician messaging succeeds.
- Disconnect followed by authenticated reconnect succeeds.

No production policy was relaxed, no public broadcast was added, and no test authentication bypass was introduced.

## Command and result

```powershell
dotnet test telemedicine-webapi/tests/Application.IntegrationTests/Application.IntegrationTests.csproj --configuration Release --no-restore --filter FullyQualifiedName~SignalRSecurityTests
```

SignalR tests: 3 passed, 0 failed. They are also included in the full 18-test PostgreSQL integration result.

The browser chat view's live JavaScript SignalR transport remains a pre-existing placeholder; protocol and authorization are verified at the server boundary, but a browser-level live-chat workflow remains future product work.
