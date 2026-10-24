# Release 1.2 Configuration Guide

Production must supply secrets through environment variables or a secret manager; do not add them to `appsettings*.json`.

| Setting / environment variable | Required | Purpose |
|---|---|---|
| `DefaultConnection` | Yes | PostgreSQL connection string |
| `JwtSettings__Key` | Yes | At least 32-byte JWT signing secret; startup validation fails when absent/short |
| `JwtSettings__Issuer`, `JwtSettings__Audience` | Deployment policy | JWT metadata retained by the existing token design |
| `Cors__AllowedOrigins__0` (and higher indexes) | Yes | Exact Vue origins; no wildcard fallback |
| `Database__ApplyMigrationsOnStartup` | No; default false | Opt-in only for controlled single-instance/dev startup |
| `DemoData__Enabled` | No; default false | Synthetic local data only; never production |
| `Swagger__Enabled` | No; default false outside Development | Explicit production OpenAPI exposure switch |
| `ForwardedHeaders__Enabled` | No; default false | Enable only behind a known reverse proxy |
| `ForwardedHeaders__KnownProxies__0` | When forwarding enabled | Literal proxy/ingress IP; configure the address observed by the app |
| `ForwardedHeaders__ForwardLimit` | No; default 1 | Maximum trusted proxy hops |
| `RateLimiting__{Authentication|Scheduling|Payment}__PermitLimit` | No | Per-window request limit |
| `RateLimiting__{...}__WindowSeconds` | No | Window duration |

For Nginx, IIS, a load balancer, or container ingress, set `Enabled=true` and list only the immediate proxy IP(s). If addresses are dynamic, establish a controlled stable network boundary before enabling forwarding; do not clear known-proxy collections. Direct deployments leave forwarding disabled.

Apply production migrations as a deployment job using the existing EF migration assembly before routing traffic. Keep startup migration disabled for multi-replica deployments. Probe `/health/live` for process restarts and `/health/ready` for traffic admission.

In-memory rate limits are per instance. Use gateway/distributed enforcement if a global multi-instance quota is required.
