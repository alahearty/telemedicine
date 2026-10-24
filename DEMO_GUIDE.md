# Telemedicine Platform - Demonstration Guide

## 1. Purpose

This guide demonstrates the approved Release 1 workflow:

- patient and physician registration with linked profiles;
- doctor discovery and professional profiles;
- real physician availability and generated slots;
- appointment booking with a clearly simulated payment;
- rescheduling and cancellation;
- physician acceptance and decline;
- role-based and record-level access control.

All data is synthetic. The application is not a clinical system and must not receive
real patient information.

## 2. Demo accounts

The Development API seeds these accounts when `DemoData:Enabled=true`:

| Role | Email | Password |
|---|---|---|
| Administrator | `administrator@localhost` | `Administrator1!` |
| Patient | `patient@demo.local` | `Patient@12345` |
| Physician | `physician@demo.local` | `Physician@12345` |
| Second physician | `doctor@demo.local` | `Doctor@12345` |

Seed data includes two physicians, a patient, a hospital, three services, weekday
availability (08:00-16:00 UTC, 30-minute slots) and sample pending/active appointments.

## 3. Start the application

From the repository root:

```powershell
dotnet run --project telemedicine-webapi/src/telemedicine.api --urls http://127.0.0.1:5231
```

In a second terminal:

```powershell
npm install --prefix telemedicine-spa
npm run serve --prefix telemedicine-spa
```

Open `http://localhost:8080`.

### Scheduled appointment chat and video

Accepted appointments expose two actions in **My Appointments**, the patient/physician dashboards and **Appointment details**:

- **Join video** opens the existing WebRTC consultation room.
- **Message** opens the secure appointment thread in the new **Messages** inbox.

Pending appointments show **Awaiting confirmation** until the physician accepts; cancelled or declined appointments have no consultation actions. Messages are persisted per appointment and delivered live over SignalR. Text messages are limited to 4,000 characters and attachments are not enabled in this release.

Video is available from 15 minutes before the appointment until 30 minutes after it ends. Local Development configuration enables `Consultations:AllowOutsideWindow` so the demo can join early; production defaults to the scheduled window.

If the database has not been created/migrated, follow `README.md` and
`docs/releases/R1/R1_DATABASE_MIGRATIONS.md`.

## 4. Patient demonstration

1. Optionally open **Sign up** and register a new Patient. The form now collects the
   linked patient profile (name, date of birth, contact, emergency contact) in the same
   operation.
2. Sign in with the demo patient or the newly registered patient.
3. Open **Find a Doctor**. Search by name/speciality and open Dr. Demo Physician.
4. Review the professional profile, weekly availability and next available slots.
5. Select a real slot and choose **Continue booking**.
6. Select **General Consultation**, confirm the doctor and slot, then continue to
   payment.
7. Point out the **Sandbox / simulated payment** notice. No card data is collected.
8. Pay in the sandbox. The API creates a **Pending** appointment and shows its details.
9. Open **My Appointments** or the appointment detail page:
   - **Reschedule** to another available slot;
   - **Cancel** with an optional reason.
10. Confirm the status changes to **Cancelled** and the released slot is bookable again.

## 5. Physician demonstration

1. Sign in as `physician@demo.local`.
2. Open **Availability** and show the persisted weekday schedule editor. Save changes.
3. Open **Appointments**:
   - pending patient requests show **Accept** and **Decline**;
   - open **Details** to see patient/contact/service/notes;
   - accept one request and show the status becoming **Active**;
   - decline another request with a required reason and show **Declined**.
4. Open **Profile** and show licence, speciality, qualifications and bio.
5. Open **Patients** to show patients with an appointment relationship.

## 6. Authorization demonstration

- A patient cannot open physician-only endpoints or replace a physician schedule.
- A different patient cannot read or cancel another patient's appointment.
- A different physician cannot read or respond to an assigned appointment.
- A physician without a treating relationship cannot read the patient's EMR.
- Anonymous chat/EMR requests return `401`; unrelated chat recipients return `403`;
  public broadcast and client-managed groups are forbidden.

## 7. Automated evidence

Release 1 test results:

- 44/44 backend tests (23 original + 21 new R1-scope tests).
- 7/7 Playwright browser workflows.
- PostgreSQL-backed concurrent booking and direct exclusion-constraint tests.
- Clean and existing-schema-copy migration verification.

Commands and evidence: `docs/releases/R1/R1_TEST_RESULTS.md` and
`docs/releases/R1/R1_END_TO_END_EVIDENCE.md`.

## 8. Scope and limitations

- No WebRTC/video consultation in Release 1. Do not demonstrate a video room.
- No prescriptions, structured EMR redesign, invoices, persistent notifications or
  audit/consent trail yet.
- Payment is simulated; no real money or card data.
- The current working tree contains unapproved Release 2 consultation work. Do not
  present it as part of Release 1.
- Demo credentials and local secrets are development-only.

## 9. Reset demo data

For a disposable local demo database, recreate the database and re-apply migrations;
the Development API will seed the synthetic accounts and data again. Never run a reset
command against a database containing real or non-synthetic data.

