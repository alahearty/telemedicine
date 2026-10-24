# Release 2 handover

Status: **Partially Complete**.

Application scope is implemented and locally verified: persistent consultation sessions, participant authorization, dedicated signaling, real browser media/peer connection, controls, cleanup, tests, migration, and deployment configuration contract.

Before production approval:

1. Provision managed or self-hosted TURN and inject short-lived credentials into `WebRtc:Ice:Servers`.
2. Apply migration `20261023034651_Release2Consultations` through the normal reviewed deployment pipeline. It was not applied to production by this work.
3. Run two-device acceptance from different networks and confirm a `relay` candidate under restrictive NAT/firewall conditions.
4. Confirm TLS/WSS termination, reverse-proxy WebSocket support, SignalR scale-out, rate limits, observability, alerting, and retention policy.
5. Complete clinical/privacy governance review for the target jurisdiction.

Rollback is application rollback plus migration rollback only if no consultation data must be retained. Prefer forward remediation after production data exists. Removing ICE configuration disables cross-network traversal without altering persisted appointment data.
