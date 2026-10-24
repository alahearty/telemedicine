# Consultation architecture

`ConsultationSession` is a persisted domain entity linked one-to-one to an accepted appointment and to its assigned patient and physician. Its public UUID is separate from the database key. Unique constraints on appointment and session identifiers make creation idempotent under concurrency.

The REST controller exposes create/read/join/end/history and ICE configuration operations. `ConsultationService` resolves the current authenticated principal to the patient or physician record and verifies that the caller is one of the two appointment participants. Only active/accepted appointments can create or use a consultation.

The dedicated `/consultation-hub` SignalR endpoint transports offer, answer, and ICE messages. It never transports audio or video; media flows peer-to-peer through WebRTC, or through the configured TURN relay. The Vue consultation view owns media capture, the `RTCPeerConnection`, signaling connection, reconnection, controls, and teardown.

The server stores lifecycle metadata only. SDP, ICE candidates, media, and access tokens are not persisted.
