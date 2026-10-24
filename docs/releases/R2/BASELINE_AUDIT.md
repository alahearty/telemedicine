# Release 2 baseline audit

Date: 2026-10-23

Release 2 extends the accepted Release 1.3 baseline. The existing Clean Architecture split, JWT authentication, role policies, PostgreSQL persistence, Vue 3/Vite application, and the appointment workflow were retained.

Before this release the repository had no persisted consultation aggregate, session-scoped authorization, dedicated signaling channel, browser media capture, peer connection, ICE configuration contract, or end-to-end two-party call test. The legacy chat hub was deliberately not reused because chat membership and consultation eligibility are different trust boundaries.

The implementation adds these capabilities without changing appointment ownership rules or enabling automatic production migrations. The migration was applied only to the isolated `telemedicine_test` database for verification.

Known baseline warnings retained: MediatR and AutoMapper emit development/test license warnings; PostCSS emits its existing missing `from` warning. They did not fail compilation or tests.
