# Release 1 Database Migrations

Status: R1 migration generated, reviewed and applied to clean and existing-schema test
databases. No R1 `Up` operation drops a table, removes a column, or deletes a row.
Date: 2026-10-23

## 1. Migration

| Item | Value |
|---|---|
| Migration | `20261023010348_Release1Scheduling` |
| Project | `telemedicine-webapi/src/Infrastructure` |
| Provider | PostgreSQL / Npgsql |
| Previous migration | `20260318130134_Initial` |
| R1 schema state | Applied successfully |

The generated migration was manually reviewed and adjusted:

- It no longer contains unrelated ASP.NET Identity token/login column-width churn.
- Legacy appointment rows are backfilled before NOT NULL constraints are enforced.
- Legacy profiles are linked to existing Identity accounts by email.
- A legacy duplicate appointment is preserved but cancelled with a reason before the
  PostgreSQL exclusion constraint is created.
- The database-level overlap constraint is added explicitly.

## 2. Schema changes

### 2.1 `User` (shared TPH profile table)

| Column | Type | Notes |
|---|---|---|
| `IdentityUserId` | `uuid` nullable | Immutable linked Identity account; unique index; legacy rows backfilled by email |
| `Qualifications` | `text` nullable | Physician professional information |
| `Bio` | `text` nullable | Physician professional information |
| `EmergencyContactName` | `text` nullable | Patient profile |
| `EmergencyContactPhone` | `text` nullable | Patient profile |

Indexes:

- `IX_User_IdentityUserId` unique (multiple NULLs remain valid for legacy rows)
- `IX_User_Email` unique
- `IX_User_License` unique

### 2.2 `PhysicianAvailabilities` (new)

| Column | Type |
|---|---|
| `Id` | integer identity PK |
| `PhysicianId` | integer FK -> `User` (cascade) |
| `DayOfWeek` | integer |
| `StartTime` / `EndTime` | interval |
| `SlotDurationMinutes` | integer |
| `IsActive` | boolean |
| audit columns | created/modified timestamps and actors |

Indexes/constraints:

- unique `(PhysicianId, DayOfWeek, StartTime, EndTime)`
- check `StartTime < EndTime`
- check `SlotDurationMinutes BETWEEN 10 AND 240`
- check `DayOfWeek BETWEEN 0 AND 6`

### 2.3 `Appointments`

Added/changed:

- `StatusReason` (`character varying(1000)`, nullable) for decline/cancel/legacy reasons.
- `AppointmentDate` and `ScheduleEndTime` are required UTC instants.
- index `(PhysicianId, AppointmentDate, ScheduleEndTime)`.
- index `(Status, AppointmentDate)`.
- check `ScheduleEndTime > AppointmentDate`.

## 3. Legacy data preservation

### 3.1 Appointment backfill

```sql
UPDATE "Appointments"
SET "AppointmentDate" = COALESCE("AppointmentDate", "Created"),
    "ScheduleEndTime" = COALESCE("ScheduleEndTime",
        COALESCE("AppointmentDate", "Created") + interval '30 minutes');
```

All existing appointment rows are retained. Missing legacy intervals receive a
documented 30-minute default beginning at the record creation instant.

### 3.2 Identity link backfill

```sql
UPDATE "User"
SET "IdentityUserId" = u."Id"
FROM "AspNetUsers" u
WHERE "User"."IdentityUserId" IS NULL
  AND lower("User"."Email") = lower(u."Email");
```

### 3.3 Legacy duplicate repair

The existing demo schema contained two identical active appointments for the same
physician and start time (rows 3 and 4). Creating the exclusion constraint would fail
without repair. The migration preserves both rows: the later duplicate is changed to
`Cancelled` and annotated. No row is deleted.

```sql
UPDATE "Appointments" a
SET "Status" = 3,
    "StatusReason" = COALESCE(NULLIF(a."StatusReason", ''),
        'Legacy duplicate cancelled during Release 1 migration'),
    "LastModified" = now()
WHERE a."Status" IN (0, 1, 4)
  AND EXISTS (
      SELECT 1 FROM "Appointments" b
      WHERE b."PhysicianId" = a."PhysicianId"
        AND b."Id" < a."Id"
        AND b."Status" IN (0, 1, 4)
        AND b."AppointmentDate" < a."ScheduleEndTime"
        AND b."ScheduleEndTime" > a."AppointmentDate"
  );
```

## 4. Database-level overlap protection

```sql
CREATE EXTENSION IF NOT EXISTS btree_gist;

ALTER TABLE "Appointments"
ADD CONSTRAINT "EX_Appointments_Physician_Time"
EXCLUDE USING gist (
    "PhysicianId" WITH =,
    tstzrange("AppointmentDate", "ScheduleEndTime", '[)') WITH &&
)
WHERE ("PhysicianId" IS NOT NULL AND "Status" IN (0, 1, 4));
```

Statuses `0` (Active), `1` (NotAttended) and `4` (Pending) reserve a slot. Cancelled and
declined appointments do not. The half-open range allows a subsequent appointment to
start exactly when another ends.

The application also performs a pre-check for a friendly 400 response, but PostgreSQL
is the final authority under concurrent requests. The integration suite exercises both
the API race and two direct concurrent EF Core inserts; exactly one row commits.

## 5. Applying the migration

```powershell
dotnet ef database update `
  --project telemedicine-webapi/src/Infrastructure `
  --startup-project telemedicine-webapi/src/telemedicine.api `
  --connection "Host=localhost;Database=<database>;Username=postgres;Password=<password>"
```

Recommended order:

1. Disposable clean database.
2. Copy/restore of the existing synthetic database.
3. Test database.
4. Development/demo database.

For a copy, take a backup or schema dump first. Do not point the update command at a
database containing real patient data.

## 6. Verification evidence

Environment: PostgreSQL 18, local Windows service.

| Database | Setup | Result |
|---|---|---|
| `telemedicine_r1_clean` | Empty database; both migrations applied | Success; empty tables; constraints present |
| `telemedicine_r1_copy` | `pg_dump`/restore copy of the pre-R1 `telemedicine` schema and data; R1 migration applied | Success; 3 Identity users, 2 profiles, 4 appointment rows preserved |
| `telemedicine_test` | Integration database | R1 migration applied; 31 R1-scope integration tests pass |
| `telemedicine` | Local synthetic demo database | R1 migration applied; demo seed and Playwright workflows pass |

On the copy:

- `AspNetUsers`: 3 rows.
- `User`: 2 profile rows.
- `Appointments`: 4 rows (no deletions).
- Duplicate appointment `Id=4` became `Cancelled` with reason
  `Legacy duplicate cancelled during Release 1 migration`.
- Both legacy profiles were linked to their Identity ids by email.
- `EX_Appointments_Physician_Time`, `CK_Appointments_TimeRange` and the availability
  check constraints were present.

Generated SQL for review:

```powershell
dotnet ef migrations script 20260318130134_Initial 20261023010348_Release1Scheduling `
  --project telemedicine-webapi/src/Infrastructure `
  --startup-project telemedicine-webapi/src/telemedicine.api `
  --no-build
```

## 7. Rollback

`Down` removes the R1 constraint, table, indexes and columns (and reverses the NOT NULL
changes). It is suitable only for a disposable R1 database. It is **not** the preferred
rollback for a database with retained Release 1 data because it discards R1 columns and
the availability table. Restore from backup for a data-bearing rollback.

## 8. Known scope note

The final working tree contains an additional, unapproved `Release2Consultations`
migration added by concurrent work. It is **not** part of this Release 1 migration
certification. The R1 migration and its clean/copy verification were performed with the
R1 migration alone. The R2 migration must be reviewed and approved separately before it
is applied to any non-synthetic database.
