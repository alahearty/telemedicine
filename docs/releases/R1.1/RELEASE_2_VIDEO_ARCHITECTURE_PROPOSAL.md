# Release 2 video consultation architecture proposal

Use the existing JWT-authenticated SignalR connection only as a signalling plane. A consultation-room service creates a random room identifier for an accepted appointment shortly before its start, expires it after a bounded grace period, and authorises exactly the linked patient and physician on every join, offer, answer and ICE message. Clients cannot choose groups or sender identities.

Media uses a two-party WebRTC `RTCPeerConnection`. Configure a managed TURN service where operational simplicity and global reliability outweigh cost, or redundant coturn instances with TLS/443, short-lived credentials, monitoring and controlled egress. STUN-only is inadequate for healthcare networks. Require HTTPS/WSS, explicit camera/microphone consent, clear mute/video controls, no recording by default, minimal signalling logs, and documented retention/privacy policy.

Connection handling should include preflight device checks, ICE restart, bounded reconnect, TURN fallback, participant presence, clean teardown, and actionable network errors. Automated acceptance needs two isolated browser contexts with synthetic media, permission grants, offer/answer/ICE assertions, media-track verification, disconnect/reconnect, unauthorised third-party denial, and room expiry.

Estimated complexity is medium-high: room/auth service and UI 2–3 weeks, TURN/deployment/observability 1–2 weeks, resilience/security/testing 2 weeks, subject to hosting and compliance review. Acceptance requires two-party media on representative networks, strict appointment-bound access, expired-room denial, TURN fallback, no identity spoofing, consent UX, supported-browser evidence, monitoring, and documented operating cost.

No WebRTC code is included. Explicit approval is required before implementation.
