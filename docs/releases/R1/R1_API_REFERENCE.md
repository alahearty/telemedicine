# Release 1 API Reference

Base URL (development): `http://localhost:5231/api`
Swagger: `http://localhost:5231/swagger`
All protected endpoints require `Authorization: Bearer <JWT>`.

Scheduling instants are UTC ISO-8601 values. The SPA converts them to local time for
display.

## 1. Authentication and onboarding

### `POST /api/auth/register`

```json
{
  "email": "new.patient@example.test",
  "password": "Patient1!x",
  "accountType": "Patient",
  "firstName": "Jane",
  "lastName": "Doe",
  "gender": "Female",
  "birth": "1992-05-06",
  "phone": "+27 11 700 1000",
  "address": "1 Example Street",
  "emergencyContactName": "John Doe",
  "emergencyContactPhone": "+27 11 700 1001"
}
```

Physician registration additionally requires `license` and
`medicalSpecialization`; `qualifications` and `bio` are optional.

The request creates the Identity account, role, and linked Patient/Physician profile
transactionally. Duplicate identity, profile email or licence returns `400`.

Response:

```json
{
  "data": { "id": 12, "role": "Patient", "email": "new.patient@example.test" },
  "errors": [],
  "wasSuccesful": true,
  "notSuccesful": false
}
```

### `POST /api/auth/login`

```json
{ "email": "patient@demo.local", "password": "Patient@12345" }
```

Returns `{ data: { token, role } }`. Invalid credentials return `401`.

### `POST /api/auth/refresh`

Reserved; currently returns `400` because refresh-token rotation is not part of
Release 1. The access token remains a single JWT.

## 2. Patient profiles

| Method | Route | Access | Notes |
|---|---|---|---|
| GET | `/api/Patient/me` | Patient | Current linked profile |
| GET | `/api/Patient/{id}` | Patient/Physician/Administrator | Patient owner; physician only with an appointment relationship |
| POST | `/api/Patient` | Patient | Completes a missing profile; email must match the JWT; duplicate rejected |
| PUT | `/api/Patient/{id}` | Patient | Owner only; accepts names, email, phone, address, gender, birth, emergency contacts |
| GET | `/api/Patient` | Administrator | Directory |
| DELETE | `/api/Patient/{id}` | Administrator | Administrative removal |

## 3. Physician profiles and directory

| Method | Route | Access | Notes |
|---|---|---|---|
| GET | `/api/Physician` | Patient/Physician/Administrator | Directory |
| GET | `/api/Physician/{id}` | Patient/Physician/Administrator | Professional profile |
| GET | `/api/Physician/me` | Physician | Current linked profile |
| POST | `/api/Physician` | Physician/Administrator | Completes a missing profile; email must match; licence unique |
| PUT | `/api/Physician/{id}` | Physician | Owner only; accepts names/contact/address/speciality/qualifications/bio |

## 4. Availability and slots

### `PUT /api/physicians/me/availability`

Physician only. Replaces the complete weekly schedule atomically. An empty array
clears the schedule.

```json
[
  { "dayOfWeek": 1, "startTime": "08:00:00", "endTime": "16:00:00", "slotDurationMinutes": 30 },
  { "dayOfWeek": 2, "startTime": "08:00:00", "endTime": "16:00:00", "slotDurationMinutes": 30 }
]
```

`dayOfWeek` uses .NET values (`0` Sunday ... `6` Saturday). Times are UTC. Overlapping
intervals, invalid ranges and durations outside 10-240 minutes return `400`.

### `GET /api/physicians/{physicianId}/availability`

Returns active rules ordered by day/start.

### `GET /api/physicians/{physicianId}/slots?from=<utc>&to=<utc>`

- Maximum range: 62 days.
- Only future slots in the requested half-open range are returned.
- Existing Pending/Active/NotAttended appointments are removed.
- Response items: `{ start, end, startUtc, endUtc, durationMinutes }`.

## 5. Appointments

### `POST /api/Appointment` (Patient)

```json
{
  "serviceId": 1,
  "paymentToken": "<one-time token from /api/TelemedicinePayment>",
  "appointmentDate": "2026-10-26T08:00:00Z",
  "physicianId": 2,
  "durationMinutes": 30
}
```

Creates a `Pending` appointment in a server-validated available slot. The payment token
is single-use and is consumed only after a successful commit. Returns `201` with
`{ data: { id, start, end, durationMinutes, status } }`.

Invalid/past/unavailable/overlapping slots return `400`; a non-patient caller gets
`403`.

### `PUT /api/Appointment/{id}`

Patient owner, assigned physician or administrator. Updates notes and/or reschedules a
Pending/Active/NotAttended appointment. Status is not changed through this endpoint.

```json
{ "appointmentDate": "2026-10-27T09:00:00Z", "durationMinutes": 30, "notes": "Optional" }
```

The new slot is validated with the same server-side availability/overlap rules. A
conflict returns `400`; the previous slot is released only when the new slot commits.

### `DELETE /api/Appointment/{id}?reason=<optional>`

Cancels an accessible appointment. Completed appointments cannot be cancelled; already
cancelled appointments return `400`. The slot becomes available again.

### `POST /api/Appointment/respond` (Physician)

```json
{ "appointmentId": 42, "accept": true }
```

Decline requires a reason:

```json
{ "appointmentId": 42, "accept": false, "reason": "Emergency theatre list" }
```

Only pending appointments can be accepted/declined. The responding physician must be
the assigned physician; otherwise `403`.

### Queries

| Method | Route | Access |
|---|---|---|
| GET | `/api/Appointment/{id}` | Patient owner / assigned physician / administrator |
| GET | `/api/Appointment/patient/{patientId}` | Patient owner / administrator |
| GET | `/api/Appointment/physician/{physicianId}` | Assigned physician / administrator |
| GET | `/api/Appointment/unattended` | Physician (own/unassigned Pending or NotAttended) / administrator |

Status values are serialized as strings: `Active`, `NotAttended`, `Attended`,
`Cancelled`, `Pending`, `Declined`.

## 6. Payment (sandbox)

### `POST /api/TelemedicinePayment` (Patient)

```json
{ "telemedicineServiceId": 1, "amountPaid": 500 }
```

Records a synthetic payment and returns a short-lived, single-use payment token:

```json
{ "data": { "paymentToken": "...", "id": 7 } }
```

No card data is accepted. Rate-limited to 10 requests/minute by default.

## 7. Chat REST and SignalR

All `/chat-hub` REST routes and the SignalR hub require a valid JWT.

| Method | Route | Notes |
|---|---|---|
| POST | `/chat-hub/send-to-user` | Body `{ recipientEmail, message }`; recipient must be the caller's patient/physician with an accepted appointment relationship |
| POST | `/chat-hub/send-file` | Multipart `recipientEmail` + `file`; maximum 2 MB; images/PDF/plain text only |
| POST | `/chat-hub/publish-to-all` | Always `403` |
| POST | `/chat-hub/join-group` | Always `403` |
| PUT | `/chat-hub/exit-group` | Always `403` |
| POST | `/chat-hub/publish-to-group` | Always `403` |
| GET | `/chat-hub/connectionId` | Returns the authenticated identity context, not a client-selected recipient |

The SignalR hub uses server identity, rejects client-managed groups, and isolates
messages to the authorized peer. Persistent conversation history and the consultation
room are later-release work.

## 8. EMR regression endpoint

| Method | Route | Access | Notes |
|---|---|---|---|
| POST | `/api/HealthAnalysisReport/create` | Physician/Administrator | Physician identity is taken from the JWT; treating relationship enforced |
| POST | `/api/HealthAnalysisReport/add-comment` | Physician/Administrator | Treating relationship enforced for physicians |
| GET | `/api/HealthAnalysisReport/{patientId}` | Physician/Administrator | Returns `200` with an array (possibly empty); comments/physicians are projected safely; no NullReferenceException |

The structured EMR redesign, versioning and patient-visible clinical records remain
Release 3.

## 9. Errors, status codes and limits

Application failures use:

```json
{ "data": null, "errors": ["Reason"], "wasSuccesful": false, "notSuccesful": true }
```

Middleware failures use `ProblemDetails` (`application/problem+json`).

| Status | Meaning |
|---|---|
| 200/201/204 | Success |
| 400 | Validation, duplicate, invalid/occupied slot, invalid lifecycle action |
| 401 | Missing/invalid JWT |
| 403 | Role, ownership or treating-relationship denial |
| 404 | Missing resource |
| 429 | Rate limit exceeded (`Retry-After` and problem+json) |
| 500 | Unexpected server error (sanitized in non-development) |

Default rate limits (configurable): authentication 10/min, payment 10/min,
scheduling 30/min.

## 10. Demo credentials and seed

| Role | Email | Password |
|---|---|---|
| Administrator | `administrator@localhost` | `Administrator1!` |
| Patient | `patient@demo.local` | `Patient@12345` |
| Physician | `physician@demo.local` | `Physician@12345` |
| Second physician | `doctor@demo.local` | `Doctor@12345` |

Demo mode seeds two physicians, a patient, a hospital, three services, weekday
availability (08:00-16:00 UTC, 30-minute slots) and synthetic appointments. All data is
synthetic; never enter real patient data.
