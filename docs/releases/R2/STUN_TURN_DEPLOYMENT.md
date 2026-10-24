# STUN/TURN deployment

ICE servers are supplied by server configuration under `WebRtc:Ice:Servers` and returned only to an authenticated, authorized consultation participant. The repository default is an empty server list: it contains no public STUN dependency and no embedded TURN secret.

Example production configuration (use a secret provider, not committed settings):

```json
{
  "WebRtc": {
    "Ice": {
      "Servers": [
        { "Urls": ["stun:stun.example.org:3478"] },
        { "Urls": ["turns:turn.example.org:5349"], "Username": "runtime-user", "Credential": "runtime-secret" }
      ]
    }
  }
}
```

Production must use TLS for the SPA/API and preferably `turns:` on 5349, restrict relay allocations, rotate short-lived credentials, monitor allocation/egress volume, and validate UDP plus TCP/TLS fallback from separate networks. TURN URLs and credentials are operational secrets and should be injected through environment configuration or a secret store.

The automated acceptance test proves host candidates and real media tracks between two isolated Chromium contexts on one machine. TURN relay and cross-network traversal remain an environment acceptance item; absence of that infrastructure is why the release is not classified fully complete.
