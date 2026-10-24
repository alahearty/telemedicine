# SignalR signaling security

The consultation hub requires JWT authentication. Browser WebSocket/SSE access tokens are accepted from the query string only for the exact `/consultation-hub` and existing `/chat-hub` paths.

Every hub operation re-resolves identity from the authenticated hub claims and checks persisted session membership and appointment eligibility. Clients cannot choose a group name or recipient: the server derives `consultation:{sessionId:N}`. The sender identifier is derived from the authenticated connection, not message input.

The hub validates SDP type and bounds SDP to 128 KiB and ICE candidates to 4 KiB. SignalR receive size is capped at 160 KiB and parallel client invocations at one per connection. Terminal/ineligible sessions cannot signal. Sensitive hub and consultation responses receive no-store caching headers.

Authorization failures do not disclose the other participant or session state. Detailed hub errors are enabled only in the test host. Access tokens, SDP, ICE, and media are not logged by application code.
