# Release 1 regression results

Executed 2026-10-23:

- `dotnet build ...sln --no-restore`: exit 0, 0 errors.
- Domain tests: 5 passed.
- Application unit tests: 8 passed.
- Existing integration tests: 10 passed.
- New PostgreSQL concurrency tests: 1 passed.
- Existing backend total retained: 23/23; new backend: 1/1.
- `npm run build`: exit 0, compiled with two size warnings.
- `npm run test:e2e`: exit 0, 5 passed, 0 skipped.
- NuGet vulnerability recheck: exit 0, no reported vulnerable packages.

The PostgreSQL test creates two overlapping reservations in separate DbContexts concurrently and asserts exactly one commit. The integration host applies the actual migration, including `EX_Appointments_Physician_Time`.
