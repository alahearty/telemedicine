# Session lifecycle

States are `Scheduled`, `Ready`, `InProgress`, `Completed`, `Cancelled`, and `Failed`.

An accepted appointment creates one `Scheduled` session. The first authorized participant join records that participant's first-join UTC timestamp and advances the session to `Ready`. When both participants have joined, the session advances to `InProgress` and records `StartedAtUtc`. Repeated joins are safe and do not replace first timestamps.

An authorized participant may end an open session. End is idempotent: the first end stores `Completed`, `EndedAtUtc`, and the reason; later calls preserve those values. Terminal sessions reject new joins and signaling. All persisted lifecycle timestamps are UTC, and the database enforces end-at-or-after-start.

Leaving the page closes local browser resources and notifies presence, but it does not automatically complete the clinical session. Explicit **End consultation** completes it for both participants.
