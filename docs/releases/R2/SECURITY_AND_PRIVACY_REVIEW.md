# Security and privacy review

The design minimizes protected data: the database records participant identifiers and lifecycle timestamps, while media, SDP, ICE candidates, and device details remain ephemeral. No recording, transcription, or signaling persistence is implemented.

Controls reviewed:

- authenticated REST and SignalR endpoints;
- patient/physician role restriction plus per-session participant checks;
- server-derived groups and sender identity;
- message and invocation bounds;
- terminal-state enforcement and idempotent completion;
- no-store headers on sensitive paths;
- no committed ICE credentials;
- cleanup of camera/microphone tracks and connections;
- no runtime third-party font or icon API dependency.

Deployment owners must provide TLS, a production secret provider, short-lived TURN credentials, centralized redaction-aware logs, retention policy for lifecycle metadata, monitoring/alerting, and abuse controls at the reverse proxy. Horizontal SignalR scale requires a supported backplane or managed SignalR service and sticky/compatible transport routing.
