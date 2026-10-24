# Release 1.2 Health and Middleware Report

## Effective order

1. Trusted forwarded headers (only when enabled)
2. Production exception handler, HSTS, HTTPS redirect
3. Environment-gated Swagger
4. Security headers
5. Explicit CORS
6. no-store callback registration
7. authentication
8. authorization
9. rate limiting
10. health endpoints, status-code Problem Details, controllers, SignalR

## Health semantics

- `/health/live`: tagged `self` only; database failure does not make liveness fail.
- `/health/ready`: tagged EF Core `ApplicationDbContext` PostgreSQL connectivity check; it cannot be healthy with an empty selection.
- `/health`: all registered checks.
- Responses contain stable check name/status/duration fields and never exception messages, stack traces, paths, connection strings, or credentials. Failed reports retain the health middleware's non-2xx status.

## Boundaries

CORS has no wildcard or credential mode. Forwarded values are ignored unless forwarding is enabled, and then only known proxies are trusted. `KnownNetworks.Clear()` and `KnownProxies.Clear()` are no longer used. Swagger defaults off outside Development. Status pages do not write when a response body/content type is already present.

The general health endpoints are intentionally anonymous for orchestrators. Network-level ingress policy should restrict them if operational policy requires internal-only probes.
