# WebRTC implementation

The consultation view requests real microphone and camera tracks with `getUserMedia`, attaches them to a real `RTCPeerConnection`, renders local and remote streams, and exchanges SDP and ICE through the consultation hub. The patient is the deterministic offerer; participant-joined notifications also recover when participants arrive in either order.

ICE candidates received before a remote description are queued and then applied. SignalR automatic reconnect reauthorizes and rejoins the session. UI state is driven by peer/signaling events and distinguishes waiting, connecting, connected, reconnecting, ended, and error states.

Controls enable/disable the actual audio and video tracks. End notifies the other participant and persists completion. Route change, unmount, leave, failure, and end handlers stop every media track, close the peer connection, detach video streams, and stop the SignalR connection.

Permission denial is shown explicitly. Audio uses the browser-selected output device; explicit `setSinkId` selection is not exposed in this release.
